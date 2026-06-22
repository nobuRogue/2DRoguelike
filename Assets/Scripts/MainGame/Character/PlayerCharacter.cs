using UnityEngine;

public class PlayerCharacter : CharacterBase {
	// 最大満腹度
	private const int _MAX_STAMINA = 10000;
	// 満腹度のターン減少値
	private const int _TURN_DECREASE_STAMINA = 10;
	// 満腹度表示用変換係数
	private const int _SHOW_STAMINA_RATIO = 100;

	/// <summary>
	/// 満腹度
	/// </summary>
	public int stamina { get; private set; } = -1;

	// プレイヤーか否か
	public override bool IsPlayer() {
		return true;
	}

	public override void Setup(int ID, Entity_CharacterData.Param characterMaster) {
		// 基底のセットアップ
		base.Setup(ID, characterMaster);
		// 満腹度を最大値にする
		SetStamina(_MAX_STAMINA);
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
		MenuManager.instance.Get<RogueMainMenu>().SetHP(HP, maxHP);
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
	public void SetStamina(int stamina) {
		this.stamina = Mathf.Clamp(stamina, 0, _MAX_STAMINA);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetStamina(GetShowStamina());
	}

	/// <summary>
	/// ターン終了時処理
	/// </summary>
	public override void OnEndTurn() {
		base.OnEndTurn();
		if (stamina >= 1) {
			// 満腹度が1以上なら満腹度を減らす
			SetStamina(stamina - _TURN_DECREASE_STAMINA);
		}
		else {
			// 満腹度が0以下ならHPを減らす
			SetHP(HP - 1);
		}
	}

	/// <summary>
	/// 満腹度を表示用の数値に変換
	/// </summary>
	/// <returns></returns>
	private int GetShowStamina() {
		return (stamina + _SHOW_STAMINA_RATIO - 1) / _SHOW_STAMINA_RATIO;
	}

}
