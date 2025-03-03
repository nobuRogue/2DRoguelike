/**
 * @file PartMain.cs
 * @brief メインゲームパート
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using UnityEngine;

using static CharacterUtility;

public class PartMain : PartBase {
	[SerializeField]
	private MapSquareManager _squareManager = null;

	[SerializeField]
	private CharacterManager _characterManager = null;

	[SerializeField]
	private ItemManager _itemManager = null;

	private DungeonProcessor _dungeonProcessor = null;

	public override async UniTask Initialize() {
		await MenuManager.instance.Get<MenuPlayerStatus>( "Prefabs/Menu/MenuPlayerStatus" ).Initialize();
		await MenuManager.instance.Get<MenuGameOver>( "Prefabs/Menu/MenuGameOver" ).Initialize();

		TerrainSpriteAssignor.Initialize();
		_dungeonProcessor = new DungeonProcessor();
		_dungeonProcessor.Initialize();

		ActionRangeManager.Initialize();
		ActionEffectManager.Initialize();
	}

	public override async UniTask Setup() {
		PlayerCharacter player;
		UserData userData = UserDataHolder.currentData;
		if (userData.isNewGame) {
			_squareManager.Initialize();
			_characterManager.Initialize();
			_itemManager.Initialize();
			// プレイヤー生成
			var squareData = MapSquareManager.instance.Get( 1, 1 );
			player = CharacterManager.instance.UsePlayer( 0, squareData );
			userData.SetIsNewGame( false );
		} else {
			// プレイヤー取得
			player = GetPlayer();
		}
		//
		var menuPlayerStatus = MenuManager.instance.Get<MenuPlayerStatus>();
		menuPlayerStatus.SetFloorCount( userData.floorCount );
		menuPlayerStatus.SetPlayerHP( player.maxHP, player.HP );
		menuPlayerStatus.SetPlayerHP( player.maxHP, player.HP );

		// カメラを追従させるためのオブザーバ設定
		player.SetMoveObserver( CameraManager.instance );
	}

	public override async UniTask Execute() {
		SoundManager.instance.PlayBgm( 0 );
		// ダンジョン処理実行
		var menuPlayerStatus = MenuManager.instance.Get<MenuPlayerStatus>();
		await menuPlayerStatus.Open();
		eDungeonEndReason endReason = await _dungeonProcessor.Execute();
		await menuPlayerStatus.Close();
		UserDataHolder.currentData.SetFloorCount( 1 );
		GetPlayer().ResetStatus();
		SoundManager.instance.StopBgm();
		UniTask task;
		switch (endReason) {
			case eDungeonEndReason.Dead:
				var gameOverMenu = MenuManager.instance.Get<MenuGameOver>();
				await gameOverMenu.Open();
				await gameOverMenu.Close();
				task = PartManager.instance.TransitionPart( eGamePart.Title );
				break;
			case eDungeonEndReason.Clear:
				task = PartManager.instance.TransitionPart( eGamePart.Ending );
				break;
			default:
				task = PartManager.instance.TransitionPart( eGamePart.Title );
				break;
		}

	}

	public override async UniTask Cleannup() {

	}

}
