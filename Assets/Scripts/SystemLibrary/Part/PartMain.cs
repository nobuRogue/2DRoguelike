/**
 * @file PartMain.cs
 * @brief メインゲームパート
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PartMain : PartBase {
	[SerializeField]
	private MapSquareManager _squareManager = null;

	[SerializeField]
	private CharacterManager _characterManager = null;

	private DungeonProcessor _dungeonProcessor = null;

	public override async UniTask Initialize() {
		await MenuManager.instance.Get<MenuPlayerStatus>( "Prefabs/Menu/MenuPlayerStatus" ).Initialize();

		TerrainSpriteAssignor.Initialize();
		_dungeonProcessor = new DungeonProcessor();
		_dungeonProcessor.Initialize();
	}

	public override async UniTask Setup() {
		PlayerCharacter player;
		UserData userData = UserDataHolder.currentData;
		if (userData.isNewGame) {
			_squareManager.Initialize();
			_characterManager.Initialize();
			// プレイヤー生成
			var squareData = MapSquareManager.instance.Get( 1, 1 );
			player = CharacterManager.instance.UsePlayer( 0, squareData );
			userData.SetIsNewGame( false );
		} else {
			// プレイヤー取得
			player = CharacterManager.instance.GetPlayer();
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
		// ダンジョン処理実行
		var menuPlayerStatus = MenuManager.instance.Get<MenuPlayerStatus>();
		await menuPlayerStatus.Open();
		await _dungeonProcessor.Execute();
		await menuPlayerStatus.Close();
		UserDataHolder.currentData.SetFloorCount( 1 );
		UniTask unitask = PartManager.instance.TransitionPart( eGamePart.Ending );
	}

	public override async UniTask Cleannup() {

	}

}
