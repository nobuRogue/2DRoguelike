using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲームキャラクターの基底
/// </summary>
public abstract class CharacterBase {
	// 識別用のID
	public int ID { get; private set; } = -1;
	// キャラのマス基準の位置
	public int posX { get; private set; } = -1;
	public int posY { get; private set; } = -1;
	// 向き
	public eDirectionEight direction { get; private set; } = eDirectionEight.Invalid;

	public int nameID { get; private set; } = -1;
	public int maxHP { get; private set; } = -1;
	public int HP { get; private set; } = -1;
	public int attack { get; private set; } = -1;
	public int defense { get; private set; } = -1;
	/// <summary>
	/// 死亡判定
	/// </summary>
	public bool isDead { get { return HP <= 0; } }

	// プレイヤーか否か
	public abstract bool IsPlayer();

	/// <summary>
	/// 使用間準備
	/// </summary>
	/// <param name="ID"></param>
	public virtual void Setup(int ID, Entity_CharacterData.Param characterMaster) {
		this.ID = ID;

		nameID = characterMaster.nameID;
		SetMaxHP(characterMaster.HP);
		SetHP(characterMaster.HP);
		SetAttack(characterMaster.Attack);
		SetDefense(characterMaster.Defense);
	}

	public virtual void SetMaxHP(int maxHP) {
		this.maxHP = maxHP;
	}

	public virtual void SetHP(int HP) {
		this.HP = HP;
	}

	/// <summary>
	/// HP減少処理
	/// </summary>
	/// <param name="removeValue"></param>
	public void RemoveHP(int removeValue) {
		SetHP(HP - removeValue);
	}

	public virtual void SetAttack(int attack) {
		this.attack = attack;
	}

	public virtual void SetDefense(int defense) {
		this.defense = defense;
	}

	/// <summary>
	/// 使用後片付け
	/// </summary>
	public void Teardown() {
		this.ID = -1;
		// 今いるマスから自身を取り除く
		MapSquareManager.instance.GetSquare(posX, posY)?.squareData.RemoveCharacter();
	}

	/// <summary>
	/// マスにキャラを置く
	/// </summary>
	/// <param name="square"></param>
	public virtual void SetSquare(SquareObject square) {
		if (square == null) return;
		// 現在のマスから取り除く
		SquareObject current = MapSquareManager.instance.GetSquare(posX, posY);
		if (current != null) current.squareData.RemoveCharacter();
		// 座標の変更
		posX = square.squareData.posX;
		posY = square.squareData.posY;
		// マスにキャラクターIDを設定
		square.squareData.SetCharacter(ID);
	}

	/// <summary>
	/// 向き変更
	/// </summary>
	/// <param name="dir"></param>
	public void SetDirection(eDirectionEight dir) {
		this.direction = dir;
	}

	/// <summary>
	/// ターン終了時処理
	/// </summary>
	public virtual void OnEndTurn() {

	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public virtual void Think() {

	}

	/// <summary>
	/// 移動の軌跡クリア
	/// </summary>
	public virtual void ClearMoveTrail() {

	}

	/// <summary>
	/// 移動の軌跡に含まれているか判定
	/// </summary>
	/// <param name="square"></param>
	/// <returns></returns>
	public virtual bool ExistMoveTrail(SquareObject square) {
		return false;
	}

	public virtual async UniTask ExecuteScheduleAction() {
		await UniTask.CompletedTask;
	}

}
