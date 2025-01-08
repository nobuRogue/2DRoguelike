/**
 * @file TurnProcessor.cs
 * @brief ターン実行
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class TurnProcessor {

	private System.Action<eDungeonEndReason> _EndDungeon = null;
	private System.Action _EndFloor = null;

	private bool _continueTurn = true;

	private List<MoveAction> _moveActionList = null;

	private AcceptPlayerInput _acceptPlayerInput = null;

	public void Initialize() {
		_acceptPlayerInput = new AcceptPlayerInput();
		_acceptPlayerInput.SetActionCallback( AddMove );
		MoveAction.SetEndProcess( EndFloor, EndDungeon );
	}

	public void SetEndProcess( System.Action<eDungeonEndReason> endDungeonProcess, System.Action endFloorProcess ) {
		_EndDungeon = endDungeonProcess;
		_EndFloor = endFloorProcess;
	}

	public async UniTask Execute() {
		_continueTurn = true;
		while (_continueTurn) {
			InitializeList( ref _moveActionList, GameConst.FLOOR_ENEMY_MAX + 1 );
			// プレイヤーの入力を受け付ける
			await _acceptPlayerInput.AcceptInput();
			// 全てのエネミーに行動を思考させる
			ThinkAllEnemyAction();
			// キャラクターオブジェクトの移動
			await MoveCharacterObject();

			// 行動を再思考させ、実行させる
			break;
		}
		// ターン終了時処理
		await OnEndTurn();
	}

	private void ThinkAllEnemyAction() {
		CharacterManager.instance.ExecuteAllCharacter( ThinkEnemyAction );
	}

	private void ThinkEnemyAction( CharacterBase character ) {
		var enemy = character as EnemyCharacter;
		if (enemy == null) return;

		MoveAction thinkMove = enemy.ThinkAction();
		if (thinkMove == null) return;

		_moveActionList.Add( thinkMove );
	}

	private async UniTask MoveCharacterObject() {
		int moveActionCount = _moveActionList.Count;
		List<UniTask> moveTaskList = new List<UniTask>( moveActionCount );
		for (int i = 0; i < moveActionCount; i++) {
			moveTaskList.Add( _moveActionList[i].ProcessObject( 0.2f ) );
		}
		await WaitTask( moveTaskList );
		_moveActionList.Clear();
	}

	private void EndTurn() {
		_continueTurn = false;
	}

	private void EndFloor() {
		EndTurn();
		_EndFloor();
	}

	private void EndDungeon( eDungeonEndReason endReason ) {
		EndFloor();
		_EndDungeon( endReason );
	}

	private void AddMove( MoveAction moveAction ) {
		_moveActionList?.Add( moveAction );
	}

	private async UniTask OnEndTurn() {
		await CharacterManager.instance.ExecuteTaskAllCharacter( OnEndTurnCharacter );
	}

	private async UniTask OnEndTurnCharacter( CharacterBase character ) {
		if (character == null) return;

		await character.OnEndTurnProcess();
	}

}
