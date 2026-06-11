using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 1ターン実行
/// </summary>
public class TurnProcessor {

	/// <summary>
	/// 1ターンの実行処理
	/// </summary>
	/// <returns></returns>
	public async UniTask Execute() {
		// プレイヤーの入力受付、行動実行
		await UniTask.DelayFrame(1);
		// 全エネミー思考

		// 全キャラクターの見た目の移動

		// 全エネミーの行動

		// ターン終了時処理

	}

}
