/**
 * @file FloorProcessor.cs
 * @brief フロア実行
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class FloorProcessor {
	private TurnProcessor _turnProcessor = null;
	private bool _continueFloor = true;

	private System.Action<eDungeonEndReason> _EndDungeon = null;

	public void Initialize() {
		_turnProcessor = new TurnProcessor();
		_turnProcessor.Initialize();
		_turnProcessor.SetEndProcess( EndDungeon, EndFloor );
	}

	public void SetEndProcess( System.Action<eDungeonEndReason> endDungeonProcess ) {
		_EndDungeon = endDungeonProcess;
	}

	public async UniTask Execute() {
		await SetupFloor();
		await UniTask.WaitForSeconds( 0.3f );
		await FadeManager.instance.FadeIn();
		_continueFloor = true;
		while (_continueFloor) await _turnProcessor.Execute();

		await FadeManager.instance.FadeOut( Color.black );
		OnEndFloor();
	}

	private async UniTask SetupFloor() {
		// マップのスプライト設定
		UserData userData = UserDataHolder.currentData;
		var floorMaster = FloorMasterUtility.GetFloorMaster( userData.floorCount );
		TerrainSpriteAssignor.SetFloorSpriteIndex( floorMaster.spriteIndex );
		// フロア生成
		MapSquareManager.instance.SetoutBGSprite( TerrainSpriteAssignor.GetTerrainSprite( eTerrain.Wall ) );
		await MapCreater.CreateMap();
		MenuManager.instance.Get<MenuPlayerStatus>().SetFloorCount( userData.floorCount );
		// プレイヤー設置
		PlayerCharacter player = CharacterUtility.GetPlayer();
		List<MapSquareData> candidateSquareList = new List<MapSquareData>( GameConst.MAP_SQUARE_MAX_WIDTH * GameConst.MAP_SQUARE_MAX_HEIGHT );
		MapSquareManager.instance.ExecuteAllSquare( square => {
			if (square.terrain != eTerrain.Room) return;

			candidateSquareList.Add( square );
		} );

		int playerSquareIndex = Random.Range( 0, candidateSquareList.Count );
		var useSquare = candidateSquareList[playerSquareIndex];
		player.SetSquare( useSquare );
		candidateSquareList.RemoveAt( playerSquareIndex );

		// エネミーのスポーン
		for (int i = 0; i < 5; i++) {
			if (IsEmpty( candidateSquareList )) break;

			int enemySquareIndex = Random.Range( 0, candidateSquareList.Count );
			CharacterManager.instance.UseEnemy( Random.Range( 0, 2 ) == 0 ? 1 : 2, candidateSquareList[enemySquareIndex] );
			candidateSquareList.RemoveAt( enemySquareIndex );
		}
	}

	private void EndFloor() {
		_continueFloor = false;
	}

	private void EndDungeon( eDungeonEndReason endReason ) {
		EndFloor();
		_EndDungeon( endReason );
	}

	private void OnEndFloor() {
		CharacterUtility.UnuseAllEnemy();
	}


}
