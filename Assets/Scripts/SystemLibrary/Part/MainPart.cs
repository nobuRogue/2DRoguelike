using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// メインのゲームパート
/// </summary>
public class MainPart : PartBase {
	// マップマス管理クラス
	[SerializeField]
	private MapSquareManager _mapManager = null;
	// キャラクター管理クラス
	[SerializeField]
	private CharacterManager _characterManager = null;

	// ダンジョン実行
	private DungeonProcessor _dungeonProcessor = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		// マップ初期化
		_mapManager?.Initialize();
		// キャラクター初期化
		_characterManager?.Initialize();
		// ダンジョン実行処理初期化
		_dungeonProcessor = new DungeonProcessor();
		_dungeonProcessor.Initialize();
	}

	public override async UniTask Setup() {
		await base.Setup();
		// プレイヤーの生成
		CharacterManager.instance.CreatePlayer(0, 0);
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

	public override async UniTask Teardown() {
		await base.Teardown();
		// プレイヤー破棄
		CharacterManager.instance.DeleteCharacter(CharacterManager.instance.GetPlayer());
	}

}
