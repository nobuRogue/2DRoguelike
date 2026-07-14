using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase {
	// 最大満腹度
	private const int _MAX_STAMINA = 10000;
	// 満腹度のターン減少値
	private const int _TURN_DECREASE_STAMINA = 10;
	// 満腹度表示用変換係数
	private const int _SHOW_STAMINA_RATIO = 100;

	// 移動軌跡のマスIDリスト
	private List<int> _moveTrailList = null;
	private const int _MOVE_TRAIL_COUNT = 3;

	/// <summary>
	/// 満腹度
	/// </summary>
	public int stamina { get; private set; } = -1;

	public PlayerCharacter() {
		_moveTrailList = new List<int>(_MOVE_TRAIL_COUNT);
	}

	// プレイヤーか否か
	public override bool IsPlayer() {
		return true;
	}

	public override void Setup(int ID, Entity_CharacterData.Param characterMaster) {
		// 基底のセットアップ
		base.Setup(ID, characterMaster);
		// 満腹度を最大値にする
		SetStamina(_MAX_STAMINA);

		_moveTrailList.Clear();
	}

	/// <summary>
	/// 最大HP設定
	/// </summary>
	/// <param name="maxHP"></param>
	public override void SetMaxHP(int maxHP) {
		base.SetMaxHP(maxHP);
		// UI更新
		RogueMainMenu mainMenu = MenuManager.instance.Get<RogueMainMenu>();
		mainMenu.SetHP(HP, maxHP);
	}

	/// <summary>
	/// 現在HP設定
	/// </summary>
	/// <param name="HP"></param>
	public override void SetHP(int HP) {
		base.SetHP(HP);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetHP(base.HP, maxHP);
	}
	/// <summary>
	/// 攻撃力設定
	/// </summary>
	/// <param name="attack"></param>
	public override void SetAttack(int attack) {
		base.SetAttack(attack);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetAttack(attack);
	}
	/// <summary>
	/// 防御力設定
	/// </summary>
	/// <param name="defense"></param>
	public override void SetDefense(int defense) {
		base.SetDefense(defense);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetDefense(defense);
	}

	/// <summary>
	/// 満腹度設定
	/// </summary>
	/// <param name="stamina"></param>
	public override void SetStamina(int stamina) {
		this.stamina = Mathf.Clamp(stamina, 0, _MAX_STAMINA);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetStamina(GetShowStamina());
	}

	/// <summary>
	/// 満腹度回復
	/// </summary>
	/// <param name="addValue"></param>
	public override void AddStamina(int addValue) {
		SetStamina(stamina + addValue);
	}

	/// <summary>
	/// 満腹度減少
	/// </summary>
	/// <param name="removeValue"></param>
	public override void RemoveStamina(int removeValue) {
		SetStamina(stamina - removeValue);
	}

	/// <summary>
	/// ターン終了時処理
	/// </summary>
	public override void OnEndTurn() {
		base.OnEndTurn();
		if (stamina >= 1) {
			// 満腹度が1以上なら満腹度を減らす
			SetStamina(stamina - _TURN_DECREASE_STAMINA);
			// HP1回復
			AddHP(1);
		}
		else {
			// 満腹度が0以下ならHPを減らす
			RemoveHP(1);
		}
	}

	/// <summary>
	/// 満腹度を表示用の数値に変換
	/// </summary>
	/// <returns></returns>
	private int GetShowStamina() {
		return (stamina + _SHOW_STAMINA_RATIO - 1) / _SHOW_STAMINA_RATIO;
	}

	/// <summary>
	/// 移動の軌跡に追加
	/// </summary>
	/// <param name="square"></param>
	public void AddMoveTrail(SquareObject square) {
		// 既に軌跡に存在するマスは追加しない
		if (_moveTrailList.Exists(element => element == square.squareData.ID)) return;
		// 軌跡が3マス以上あったら最も古い要素を取り除く
		if (_moveTrailList.Count >= _MOVE_TRAIL_COUNT) _moveTrailList.RemoveAt(0);
		// 軌跡に追加
		_moveTrailList.Add(square.squareData.ID);
	}

	/// <summary>
	/// 移動の軌跡をクリア
	/// </summary>
	public override void ClearMoveTrail() {
		_moveTrailList.Clear();
	}

	/// <summary>
	/// キャラクターをマスに設置
	/// </summary>
	/// <param name="square"></param>
	public override void SetSquare(SquareObject square) {
		base.SetSquare(square);
		// 軌跡に追加
		AddMoveTrail(square);
	}

	/// <summary>
	/// 移動の軌跡に含まれているか判定
	/// </summary>
	/// <param name="square"></param>
	/// <returns></returns>
	public override bool ExistMoveTrail(SquareObject square) {
		return _moveTrailList.Exists(element => element == square.squareData.ID);
	}
}
