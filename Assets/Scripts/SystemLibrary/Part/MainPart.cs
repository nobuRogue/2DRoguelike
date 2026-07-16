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
	// アイテム管理クラス
	[SerializeField]
	private ItemManager _itemManager = null;
	// 罠管理クラス
	[SerializeField]
	private TrapManager _trapManager = null;

	// ダンジョン実行
	private DungeonProcessor _dungeonProcessor = null;
	// ダンジョンBGMのID
	private const int _BGM_ID = 0;

	public override async UniTask Initialize() {
		await base.Initialize();
		// マップ初期化
		_mapManager?.Initialize();
		// キャラクター初期化
		_characterManager?.Initialize();
		// アイテム初期化
		_itemManager?.Initialize();
		// 罠初期化
		_trapManager.Initialize();
		// ダンジョン実行処理初期化
		_dungeonProcessor = new DungeonProcessor();
		_dungeonProcessor.Initialize();
		// メニュー初期化
		MenuManager.instance.Get<RogueMainMenu>("RogueMainCanvas");
		MenuManager.instance.Get<RogueLogMenu>("RogueLogCanvas");
		MenuManager.instance.Get<ItemList>("ItemListCanvas");
		MenuManager.instance.Get<ItemCommandList>("ItemCommandListCanvas");
	}

	public override async UniTask Setup() {
		await base.Setup();
		// プレイヤーの生成
		CharacterManager.instance.CreatePlayer(0, 0);
		// 階数を1にリセット
		UserDataHolder.instance.currentData.SetFloorCount(1);
	}

	public override async UniTask Execute() {
		// メインUI表示
		RogueMainMenu mainMenu = MenuManager.instance.Get<RogueMainMenu>();
		await mainMenu.Open();
		// ログメニュー表示
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		await logMenu.Open();
		// BGM再生
		SoundManager.instance.PlayBGM(_BGM_ID);
		// ダンジョン実行
		eDungeonEndReason endReason = await _dungeonProcessor.Execute();
		// BGM停止
		SoundManager.instance.StopBGM();
		// メインUI非表示
		await mainMenu.Close();
		// ログメニュー非表示
		await logMenu.Close();
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
