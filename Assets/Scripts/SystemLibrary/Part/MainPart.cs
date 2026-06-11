using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// メインのゲームパート
/// </summary>
public class MainPart : PartBase {
	// マップマス管理クラス
	[SerializeField]
	private MapSquareManager mapManager = null;

	// ダンジョン実行
	private DungeonProcessor _dungeonProcessor = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		// マップ初期化
		mapManager?.Initialize();
		// ダンジョン実行処理初期化
		_dungeonProcessor = new DungeonProcessor();
		_dungeonProcessor.Initialize();
	}

	public override async UniTask Execute() {
		// ダンジョン実行
		eDungeonEndReason endReason = await _dungeonProcessor.Execute();
		// ダンジョン結果処理
		switch (endReason) {
			case eDungeonEndReason.GameOver:
				// ゲームオーバーならタイトルパートに遷移
				UniTask transT = PartManager.instance.TransitionPart(eGamePart.Title);
				break;
			case eDungeonEndReason.Clear:
				// クリアしたらエンディングパートへ遷移
				UniTask transE = PartManager.instance.TransitionPart(eGamePart.Ending);
				break;
		}
	}
}
