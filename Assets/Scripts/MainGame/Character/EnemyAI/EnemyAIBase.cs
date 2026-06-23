using UnityEngine;

/// <summary>
/// エネミーAIの基底
/// </summary>
public abstract class EnemyAIBase {
	// 移動アクションの追加時コールバック
	public static System.Action<MoveAction> addMove = null;
	// 持ち主のID
	protected int _sourceCharacterID = -1;

	public EnemyAIBase(int characterID) {
		_sourceCharacterID = characterID;
	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public abstract void ThinkAction();

}
