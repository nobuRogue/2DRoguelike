using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyCharacter : CharacterBase {
	// AIクラスの保持
	private EnemyAIBase _actionAI = null;

	public override void Setup(int ID, Entity_CharacterData.Param characterMaster) {
		base.Setup(ID, characterMaster);

		_actionAI = new EnemyAI00_ChasePlayer(ID, characterMaster.actionID);
	}

	// プレイヤーか否か
	public override bool IsPlayer() {
		return false;
	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public override void Think() {
		_actionAI?.ThinkAction();
	}

	/// <summary>
	/// AI予定行動の実行
	/// </summary>
	/// <returns></returns>
	public override async UniTask ExecuteScheduleAction() {
		if (_actionAI == null) return;

		await _actionAI.ExecuteScheduleAction();
	}

}
