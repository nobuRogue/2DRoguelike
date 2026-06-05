using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランダムマップ生成クラス
/// </summary>
public class MapCreater {
	private static MapCreater _instance = null;
	public static MapCreater instance {
		get {
			if (_instance == null) _instance = new MapCreater();

			return _instance;
		}
	}

	private MapCreater() { }

	/// <summary>
	/// マップ生成に用いるエリア情報
	/// </summary>
	private class AreaData {
		// 始点
		public int startX = -1;
		public int startY = -1;
		// サイズ
		public int width = -1;
		public int height = -1;

		public AreaData(int x, int y, int w, int h) {
			startX = x;
			startY = y;
			width = w;
			height = h;
		}
	}

	// エリアのリスト
	private List<AreaData> _areaList = null;
	// 分割線のマスのIDリスト
	private List<int> _devideLineList = null;
	// エリアの分割回数
	private const int _AREA_DEVIDE_COUNT = 8;
	// 最小部屋サイズ
	private const int _MIN_ROOM_SIZE = 3;

	public void CreateMap() {
		// 最初のエリアを生成
		CreateFirstArea();
		// エリアを分割
		DevideAreaFixCount();
		// 部屋の配置

		// 全部屋を連結

		// 階段を置く

	}

	/// <summary>
	/// 最初のエリア生成
	/// </summary>
	private void CreateFirstArea() {
		_areaList = new List<AreaData>();
		_devideLineList = new List<int>();
		// 全てのマスを壁にする
		MapSquareManager.instance.ExecuteAllSquare(SetFirstWall);

		AreaData firstArea = new AreaData(2, 2, GameConst.MAP_SQUARE_WIDTH_COUNT - 4, GameConst.MAP_SQUARE_HEIGHT_COUNT - 4);
		_areaList.Add(firstArea);
	}

	/// <summary>
	/// マスの地形を壁にする
	/// </summary>
	/// <param name="square"></param>
	private void SetFirstWall(SquareObject square) {
		square.SetTerrain(eTerrain.Wall, 0);
		// 最初の分割線マスを追加
		int x = square.squareData.posX;
		int y = square.squareData.posY;
		// 外周マスの排除
		if (x == 0 || x == GameConst.MAP_SQUARE_WIDTH_COUNT - 1 ||
			y == 0 || y == GameConst.MAP_SQUARE_HEIGHT_COUNT - 1) return;
		// 外周から1マス離れたマス以外の排除
		if (x != 1 && x != GameConst.MAP_SQUARE_WIDTH_COUNT - 2 &&
			y != 1 && y != GameConst.MAP_SQUARE_HEIGHT_COUNT - 2) return;
		// 分割線マスの追加
		AddDevideLine(square);
	}

	/// <summary>
	/// 分割線マスへの追加
	/// </summary>
	/// <param name="square"></param>
	private void AddDevideLine(SquareObject square) {
		_devideLineList.Add(square.squareData.ID);
		square.SetTerrain(eTerrain.Wall, 2);
	}

	/// <summary>
	/// エリアの分割
	/// </summary>
	private void DevideAreaFixCount() {
		for (int i = 0; i < _AREA_DEVIDE_COUNT; i++) {
			// 幅最大のエリアを取得
			AreaData devideArea = GetMaxSizeArea();
			bool isVertical = devideArea.width < devideArea.height;
			// 三項演算子：条件 ? trueの際の処理 : falseの際の処理 ;
			int maxSize = isVertical ? devideArea.height : devideArea.width;
			// 取得したエリアが分割可能か判定
			if (maxSize < (_MIN_ROOM_SIZE + 2) * 2 + 1) break;
			// 取得したエリアを分割
			DevideArea(devideArea, isVertical);
		}
	}

	private void DevideArea(AreaData devideArea, bool isVertical) {
		if (isVertical) {
			// 水平に線を引いて縦に分割
			DevideAreaVertical(devideArea);
		}
		else {
			// 垂直に線を引いて横に分割
			DevideAreaHorizontal(devideArea);
		}
	}

	/// <summary>
	/// 水平に線を引いてエリアを縦に分割
	/// </summary>
	/// <param name="devideArea"></param>
	private void DevideAreaVertical(AreaData devideArea) {
		// 分割位置の決定
		int randomMax = devideArea.height - (_MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += _MIN_ROOM_SIZE + 2 + devideArea.startY;
		// 新しいエリアを生成
		int newAreaHeight = devideArea.startY + devideArea.height - devidePos - 1;
		int newAreaY = devidePos + 1;
		AreaData newArea = new AreaData(devideArea.startX, newAreaY, devideArea.width, newAreaHeight);
		_areaList.Add(newArea);
		// 既存エリアの修正
		devideArea.height = devidePos - devideArea.startY;
		// 分割線マスの追加
		for (int x = 0; x < devideArea.width; x++) {
			SquareObject square = MapSquareManager.instance.GetSquare(devideArea.startX + x, devidePos);
			AddDevideLine(square);
		}
	}

	/// <summary>
	/// 垂直に線を引いてエリアを横に分割
	/// </summary>
	/// <param name="devideArea"></param>
	private void DevideAreaHorizontal(AreaData devideArea) {
		// 分割位置の決定
		int randomMax = devideArea.width - (_MIN_ROOM_SIZE + 2) * 2;
		int devidePos = Random.Range(0, randomMax);
		devidePos += _MIN_ROOM_SIZE + 2 + devideArea.startX;
		// 新しいエリアを生成
		int newAreaWidth = devideArea.startX + devideArea.width - devidePos - 1;
		int newAreaX = devidePos + 1;
		AreaData newArea = new AreaData(newAreaX, devideArea.startY, newAreaWidth, devideArea.height);
		_areaList.Add(newArea);
		// 既存エリアの修正
		devideArea.width = devidePos - devideArea.startX;
		// 分割線マスの追加
		for (int y = 0; y < devideArea.height; y++) {
			SquareObject square = MapSquareManager.instance.GetSquare(devidePos, devideArea.startY + y);
			AddDevideLine(square);
		}
	}

	/// <summary>
	/// 横幅か立幅が最大のエリアを取得
	/// </summary>
	/// <returns></returns>
	private AreaData GetMaxSizeArea() {
		int maxSize = -1;
		AreaData result = null;
		for (int i = 0; i < _areaList.Count; i++) {
			AreaData area = _areaList[i];
			// 横幅の確認
			if (area.width > maxSize) {
				maxSize = area.width;
				result = area;
			}
			// 縦幅の確認
			if (area.height > maxSize) {
				maxSize = area.height;
				result = area;
			}
		}
		return result;
	}

}
