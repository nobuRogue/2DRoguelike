using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 1ターン実行
/// </summary>
public class TurnProcessor {
	// 移動アクションのリスト
	private List<MoveAction> _moveList = null;
	// プレイヤーの入力受付
	private AcceptPlayerInput _acceptPlayer = null;
	// ターン継続フラグ
	private bool _isContinueTurn = true;
	// フロア終了処理
	private System.Action<eFloorEndReason> _EndFloor = null;
	// ダンジョン終了処理
	private System.Action<eDungeonEndReason> _EndDungeon = null;

	public void Initialize(
		System.Action<eFloorEndReason> EndFloor,
		System.Action<eDungeonEndReason> EndDungeon
		) {
		_acceptPlayer = new AcceptPlayerInput();
		_acceptPlayer.Initialize(AddMoveAction);

		_moveList = new List<MoveAction>(GameConst.ENEMY_MAX_COUNT + 1);

		_EndFloor = EndFloor;
		_EndDungeon = EndDungeon;
		// 移動アクションにフロア終了処理設定
		MoveAction.EndFloor = EndFloorAndTurn;
		// 移動アクションにダンジョン終了処理設定
		MoveAction.EndDungeon = EndDungeonAndTurn;
		// キャラクターにダンジョン終了処理設定
		CharacterObject.EndDungeon = EndDungeonAndTurn;
		// エネミーAIに移動アクション追加処理を譲渡
		EnemyAIBase.addMove = AddMoveAction;
	}

	/// <summary>
	/// 1ターンの実行処理
	/// </summary>
	/// <returns></returns>
	public async UniTask Execute() {
		_isContinueTurn = true;
		// プレイヤーの入力受付
		await AcceptPlayerInput();
		// TODO:全エネミー思考
		CharacterManager.instance.ExecuteAllCharacter(ThinkCharacter);
		//CharacterManager.instance.ExecuteAllCharacter(character => character.characterData.Think());
		// 全キャラクターの見た目の移動
		List<UniTask> moveTaskList = new List<UniTask>(_moveList.Count);
		for (int i = 0; i < _moveList.Count; i++) {
			moveTaskList.Add(_moveList[i].ExecuteObject(0.2f));
		}
		// リストのすべてのタスクの終了待ち
		await UniTask.WhenAll(moveTaskList);
		_moveList.Clear();
		// ターン終了の判定
		if (!_isContinueTurn) return;
		// 全エネミーの行動
		await CharacterManager.instance.ExecuteTaskAllCharacter(ExecuteScheduleAction);
		// ターン終了時処理
		CharacterManager.instance.ExecuteAllCharacter(OnEndTurnCharacter);
	}

	/// <summary>
	/// キャラクターの行動思考
	/// </summary>
	/// <param name="character"></param>
	private void ThinkCharacter(CharacterObject character) {
		character?.characterData.Think();
	}

	/// <summary>
	/// キャラクターの予定行動実行
	/// </summary>
	/// <param name="character"></param>
	/// <returns></returns>
	private async UniTask ExecuteScheduleAction(CharacterObject character) {
		await character.characterData.ExecuteScheduleAction();
	}

	/// <summary>
	/// キャラクターのターン終了時処理の実行
	/// </summary>
	/// <param name="character"></param>
	private void OnEndTurnCharacter(CharacterObject character) {
		character.OnEndTurn();
	}

	/// <summary>
	/// プレイヤーの入力受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptPlayerInput() {
		// 継続移動の判定
		if (_acceptPlayer.AcceptMove()) return;
		// 全キャラクターを待機アニメーションに戻す
		CharacterManager.instance.ExecuteAllCharacter(SetWaitAnimation);
		// プレイヤーの入力受付、行動実行
		await _acceptPlayer.AcceptInput();
	}

	/// <summary>
	/// キャラクターを待機アニメーションに設定
	/// </summary>
	/// <param name="character"></param>
	private void SetWaitAnimation(CharacterObject character) {
		character.SetAnimation(eCharacterAnimation.Wait);
	}

	/// <summary>
	/// 移動アクションの追加
	/// </summary>
	/// <param name="moveAction"></param>
	private void AddMoveAction(MoveAction moveAction) {
		_moveList.Add(moveAction);
	}

	/// <summary>
	/// ターンを終了させる
	/// </summary>
	private void EndTurn() {
		_isContinueTurn = false;
	}

	/// <summary>
	/// フロア終了処理
	/// </summary>
	private void EndFloorAndTurn(eFloorEndReason endReason) {
		// フロア終了
		_EndFloor?.Invoke(endReason);
		// ターン終了
		EndTurn();
	}

	/// <summary>
	/// ダンジョン終了処理
	/// </summary>
	/// <param name="endReason"></param>
	private void EndDungeonAndTurn(eDungeonEndReason endReason) {
		_EndDungeon?.Invoke(endReason);
		EndFloorAndTurn(endReason.GetFloorEndReason());
	}

}
