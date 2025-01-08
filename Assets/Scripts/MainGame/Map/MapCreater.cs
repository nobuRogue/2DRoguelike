/**
 * @file MapCreater.cs
 * @brief マップ生成クラス
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using static CommonModule;

public class MapCreater {



	private class AreaData {
		public Vector2Int startPos = Vector2Int.zero;
		public int width = 0;
		public int height = 0;
		public AreaData( Vector2Int setPos, int setWidth, int setHeight ) {
			startPos = setPos;
			width = setWidth;
			height = setHeight;
		}
	}

	private static List<AreaData> _areaList = null;
	private static List<int> _devideLineSquare = null;
	private static List<int> _roomSquareList = null;

	public static async UniTask CreateMap() {
		// 壁で埋める
		int squareMaxCount = GameConst.MAP_SQUARE_MAX_WIDTH * GameConst.MAP_SQUARE_MAX_HEIGHT;
		InitializeList( ref _devideLineSquare, squareMaxCount );
		InitializeList( ref _roomSquareList, squareMaxCount );
		MapSquareManager.instance.ExecuteAllSquare( SetWallAndChecDevideLine );
		MapSquareUtility.ClearRoom();
		// 最初のエリアを作る
		_areaList = new List<AreaData>();
		_areaList.Add( new AreaData( new Vector2Int( 2, 2 ), GameConst.MAP_SQUARE_MAX_WIDTH - 4, GameConst.MAP_SQUARE_MAX_HEIGHT - 4 ) );
		// エリアを分割する
		await DevideArea( GameConst.AREA_DEVIDE_COUNT );
		// 部屋を置く
		CreateRoom();
		// 部屋を繋げる
		ConnectRoom();
		// 階段を置く
		CreateStair();
	}

	private static void SetWallAndChecDevideLine( MapSquareData square ) {
		square.SetTerrain( eTerrain.Wall );
		var position = square.squarePosition;
		if (position.x == 0 || position.y == 0 ||
			position.x == GameConst.MAP_SQUARE_MAX_WIDTH - 1 ||
			position.y == GameConst.MAP_SQUARE_MAX_HEIGHT - 1) return;

		if (position.x == 1 || position.y == 1 ||
			position.x == GameConst.MAP_SQUARE_MAX_WIDTH - 2 ||
			position.y == GameConst.MAP_SQUARE_MAX_HEIGHT - 2) _devideLineSquare.Add( square.ID );

	}

	private static async UniTask DevideArea( int devideCount ) {
		for (int i = 0; i < devideCount; i++) {
			// 幅最大のエリアと縦か横か取得
			var maxSizeArea = GetMaxSizeArea( out int maxSize, out bool isVertical );
			if (maxSizeArea == null || maxSize < (GameConst.MIN_ROOM_SIZE + 2) * 2 + 1) break;

			await DevideArea( maxSizeArea, isVertical );
		}
	}

	private static AreaData GetMaxSizeArea( out int maxSize, out bool isVertical ) {
		maxSize = -1;
		isVertical = false;
		AreaData result = null;
		for (int i = 0, max = _areaList.Count; i < max; i++) {
			AreaData areaData = _areaList[i];
			if (areaData.width > maxSize) {
				maxSize = areaData.width;
				result = areaData;
				isVertical = false;
			}

			if (areaData.height > maxSize) {
				maxSize = areaData.height;
				result = areaData;
				isVertical = true;
			}
		}
		return result;
	}

	private static async UniTask DevideArea( AreaData devideArea, bool isVertical ) {
		if (isVertical) {
			await DevideAreaVertical( devideArea );
		} else {
			await DevideAreaHorizontal( devideArea );
		}
	}

	private static async UniTask DevideAreaVertical( AreaData devideArea ) {
		int randomMax = devideArea.height - ((GameConst.MIN_ROOM_SIZE + 2) * 2);
		int devidePos = Random.Range( 0, randomMax );
		devidePos += GameConst.MIN_ROOM_SIZE + 2 + devideArea.startPos.y;
		// 新しいエリアの生成
		_areaList.Add( new AreaData( new Vector2Int( devideArea.startPos.x, devidePos + 1 ), devideArea.width, devideArea.startPos.y + devideArea.height - devidePos - 1 ) );
		// 既存エリアの修正
		devideArea.height = devidePos - devideArea.startPos.y;

		for (int x = 0, max = devideArea.width; x < max; x++) {
			_devideLineSquare.Add( MapSquareManager.instance.GetID( devideArea.startPos.x + x, devidePos ) );
		}
	}
	private static async UniTask DevideAreaHorizontal( AreaData devideArea ) {
		int randomMax = devideArea.width - ((GameConst.MIN_ROOM_SIZE + 2) * 2);
		int devidePos = Random.Range( 0, randomMax );
		devidePos += GameConst.MIN_ROOM_SIZE + 2 + devideArea.startPos.x;
		// 新しいエリアの生成
		_areaList.Add( new AreaData( new Vector2Int( devidePos + 1, devideArea.startPos.y ), devideArea.startPos.x + devideArea.width - devidePos - 1, devideArea.height ) );
		// 既存エリアの修正
		devideArea.width = devidePos - devideArea.startPos.x;

		for (int y = 0, max = devideArea.height; y < max; y++) {
			_devideLineSquare.Add( MapSquareManager.instance.GetID( devidePos, devideArea.startPos.y + y ) );
		}
	}

	private static void CreateRoom() {
		for (int i = 0, max = _areaList.Count; i < max; i++) {
			CreateAreaRoom( _areaList[i] );
		}
	}

	private static void CreateAreaRoom( AreaData areaData ) {
		int roomWidth = Random.Range( GameConst.MIN_ROOM_SIZE, areaData.width - 1 );
		int roomHeight = Random.Range( GameConst.MIN_ROOM_SIZE, areaData.height - 1 );

		int widthRandomRange = areaData.width - 1 - roomWidth;
		int heightRandomRange = areaData.height - 1 - roomHeight;

		int startX = areaData.startPos.x + 1 + Random.Range( 0, widthRandomRange );
		int startY = areaData.startPos.y + 1 + Random.Range( 0, heightRandomRange );

		RoomData createRoom = new RoomData();
		for (int y = 0, yMax = roomHeight; y < yMax; y++) {
			for (int x = 0, xMax = roomWidth; x < xMax; x++) {
				MapSquareData roomSquare = MapSquareManager.instance.Get( startX + x, startY + y );
				roomSquare.SetTerrain( eTerrain.Room );
				createRoom.AddSquare( roomSquare.ID );
			}
		}
		_roomSquareList.AddRange( createRoom.squareIDList );
		MapSquareUtility.AddRoom( createRoom );
	}

	private static void ConnectRoom() {
		List<ManhattanMoveData> routeSearchResult = new List<ManhattanMoveData>( 1024 );
		var dir = (eDirectionFour)Random.Range( 0, 4 );
		for (int i = 0, max = _areaList.Count - 1; i < max; i++) {
			AreaData area1 = _areaList[i];
			var startSquare = DigToDevideLine( area1, dir );

			dir = (eDirectionFour)Random.Range( 0, 4 );
			AreaData area2 = _areaList[i + 1];
			var goalSquare = DigToDevideLine( area2, dir );
			RouteSearcher.RouteSearchManhattan( ref routeSearchResult, startSquare.ID, goalSquare.ID, CanPassRoute );
			DigMoveRouteSquare( routeSearchResult );
			int dirIndex = (int)dir + Random.Range( 1, 4 );
			if (dirIndex >= (int)eDirectionFour.Max) dirIndex -= (int)eDirectionFour.Max;

			dir = (eDirectionFour)dirIndex;
		}
	}

	private static void DigMoveRouteSquare( List<ManhattanMoveData> route ) {
		for (int i = 0, max = route.Count; i < max; i++) {
			MapSquareData square = MapSquareManager.instance.Get( route[i].moveSquareID );
			if (square.terrain == eTerrain.Passage) continue;

			square.SetTerrain( eTerrain.Passage );
		}
	}

	private static bool CanPassRoute( MapSquareData square, eDirectionFour dir, int distance ) {
		return _devideLineSquare.Exists( devideSquareID => devideSquareID == square.ID );
	}

	/// <summary>
	/// 部屋からエリア分割線まで掘る
	/// </summary>
	private static MapSquareData DigToDevideLine( AreaData areaData, eDirectionFour dir ) {
		eDirectionFour reverseDir = dir.ReverseDir();
		List<MapSquareData> targetList = new List<MapSquareData>();

		int startX = areaData.startPos.x;
		int startY = areaData.startPos.y;
		for (int y = 0, yMax = areaData.height; y < yMax; y++) {
			for (int x = 0, xMax = areaData.width; x < xMax; x++) {
				MapSquareData squareData = MapSquareManager.instance.Get( startX + x, startY + y );
				if (squareData.terrain != eTerrain.Wall) continue;

				MapSquareData adjucentSquare = MapSquareManager.instance.Get( squareData.squarePosition.ToVectorPos( reverseDir ) );
				if (adjucentSquare == null || adjucentSquare.terrain != eTerrain.Room) continue;

				targetList.Add( squareData );
			}
		}
		if (IsEmpty( targetList )) return null;

		var startSquare = targetList[Random.Range( 0, targetList.Count )];
		while (true) {
			startSquare.SetTerrain( eTerrain.Passage );
			if (_devideLineSquare.Exists( square => square == startSquare.ID )) break;

			startSquare = MapSquareManager.instance.Get( startSquare.squarePosition.ToVectorPos( dir ) );
		}
		return startSquare;
	}

	private static void CreateStair() {
		if (IsEmpty( _roomSquareList )) return;

		MapSquareData stairSquare = MapSquareManager.instance.Get( _roomSquareList[Random.Range( 0, _roomSquareList.Count )] );
		stairSquare.SetTerrain( eTerrain.Stair );
	}

}
