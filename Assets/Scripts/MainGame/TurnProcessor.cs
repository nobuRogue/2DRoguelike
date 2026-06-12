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

	public void Initialize() {
		_acceptPlayer = new AcceptPlayerInput();
		_acceptPlayer.Initialize(AddMoveAction);

		_moveList = new List<MoveAction>(GameConst.ENEMY_MAX_COUNT + 1);
	}

	/// <summary>
	/// 1ターンの実行処理
	/// </summary>
	/// <returns></returns>
	public async UniTask Execute() {
		// プレイヤーの入力受付、行動実行
		await _acceptPlayer.AcceptInput();
		// 全エネミー思考

		// 全キャラクターの見た目の移動
		List<UniTask> moveTaskList = new List<UniTask>(_moveList.Count);
		for (int i = 0; i < _moveList.Count; i++) {
			moveTaskList.Add(_moveList[i].ExecuteObject(0.5f));
		}
		// リストのすべてのタスクの終了待ち
		await UniTask.WhenAll(moveTaskList);
		_moveList.Clear();
		// 全エネミーの行動

		// ターン終了時処理

	}

	/// <summary>
	/// 移動アクションの追加
	/// </summary>
	/// <param name="moveAction"></param>
	private void AddMoveAction(MoveAction moveAction) {
		_moveList.Add(moveAction);
	}

}
