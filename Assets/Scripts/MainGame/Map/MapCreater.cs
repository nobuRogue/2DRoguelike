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
	private List<int> _divideLineList = null;
	// エリアの分割回数
	private const int _AREA_DEVIDE_COUNT = 8;
	// 最小部屋サイズ
	private const int _MIN_ROOM_SIZE = 3;

	public void CreateMap() {
		// 最初のエリアを生成
		CreateFirstArea();
		// エリアを分割
		DivideAreaFixCount();
		// 部屋の配置
		CreateAllRoom();
		// 全部屋を連結

		// 階段を置く

	}

	/// <summary>
	/// 最初のエリア生成
	/// </summary>
	private void CreateFirstArea() {
		_areaList = new List<AreaData>();
		_divideLineList = new List<int>();
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
		AddDivideLine(square);
	}

	/// <summary>
	/// 分割線マスへの追加
	/// </summary>
	/// <param name="square"></param>
	private void AddDivideLine(SquareObject square) {
		_divideLineList.Add(square.squareData.ID);
		square.SetTerrain(eTerrain.Wall, 2);
	}

	/// <summary>
	/// エリアの分割
	/// </summary>
	private void DivideAreaFixCount() {
		for (int i = 0; i < _AREA_DEVIDE_COUNT; i++) {
			// 幅最大のエリアを取得
			AreaData divideArea = GetMaxSizeArea();
			bool isVertical = divideArea.width < divideArea.height;
			// 三項演算子：条件 ? trueの際の処理 : falseの際の処理 ;
			int maxSize = isVertical ? divideArea.height : divideArea.width;
			// 取得したエリアが分割可能か判定
			if (maxSize < (_MIN_ROOM_SIZE + 2) * 2 + 1) break;
			// 取得したエリアを分割
			DivideArea(divideArea, isVertical);
		}
	}

	private void DivideArea(AreaData divideArea, bool isVertical) {
		if (isVertical) {
			// 水平に線を引いて縦に分割
			DivideAreaVertical(divideArea);
		}
		else {
			// 垂直に線を引いて横に分割
			DivideAreaHorizontal(divideArea);
		}
	}

	/// <summary>
	/// 水平に線を引いてエリアを縦に分割
	/// </summary>
	/// <param name="divideArea"></param>
	private void DivideAreaVertical(AreaData divideArea) {
		// 分割位置の決定
		int randomMax = divideArea.height - (_MIN_ROOM_SIZE + 2) * 2;
		int dividePos = Random.Range(0, randomMax);
		dividePos += _MIN_ROOM_SIZE + 2 + divideArea.startY;
		// 新しいエリアを生成
		int newAreaHeight = divideArea.startY + divideArea.height - dividePos - 1;
		int newAreaY = dividePos + 1;
		AreaData newArea = new AreaData(divideArea.startX, newAreaY, divideArea.width, newAreaHeight);
		_areaList.Add(newArea);
		// 既存エリアの修正
		divideArea.height = dividePos - divideArea.startY;
		// 分割線マスの追加
		for (int x = 0; x < divideArea.width; x++) {
			SquareObject square = MapSquareManager.instance.GetSquare(divideArea.startX + x, dividePos);
			AddDivideLine(square);
		}
	}

	/// <summary>
	/// 垂直に線を引いてエリアを横に分割
	/// </summary>
	/// <param name="divideArea"></param>
	private void DivideAreaHorizontal(AreaData divideArea) {
		// 分割位置の決定
		int randomMax = divideArea.width - (_MIN_ROOM_SIZE + 2) * 2;
		int dividePos = Random.Range(0, randomMax);
		dividePos += _MIN_ROOM_SIZE + 2 + divideArea.startX;
		// 新しいエリアを生成
		int newAreaWidth = divideArea.startX + divideArea.width - dividePos - 1;
		int newAreaX = dividePos + 1;
		AreaData newArea = new AreaData(newAreaX, divideArea.startY, newAreaWidth, divideArea.height);
		_areaList.Add(newArea);
		// 既存エリアの修正
		divideArea.width = dividePos - divideArea.startX;
		// 分割線マスの追加
		for (int y = 0; y < divideArea.height; y++) {
			SquareObject square = MapSquareManager.instance.GetSquare(dividePos, divideArea.startY + y);
			AddDivideLine(square);
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

	/// <summary>
	/// 全てのエリアに部屋を生成
	/// </summary>
	private void CreateAllRoom() {
		for (int i = 0; i < _areaList.Count; i++) {
			CreateRoom(_areaList[i]);
		}
	}

	/// <summary>
	/// エリアへの部屋配置
	/// </summary>
	/// <param name="area"></param>
	private void CreateRoom(AreaData area) {
		if (area == null) return;
		// 部屋のサイズ決定
		int roomWidth = Random.Range(_MIN_ROOM_SIZE, area.width - 1);
		int roomHeight = Random.Range(_MIN_ROOM_SIZE, area.height - 1);
		// 部屋の位置決定
		int xRandomRange = area.width - roomWidth;
		int yRandomRange = area.height - roomHeight;
		int startX = area.startX + Random.Range(1, xRandomRange);
		int startY = area.startY + Random.Range(1, yRandomRange);
		// 部屋の生成
		List<int> roomIDList = new List<int>(roomWidth * roomHeight);
		for (int y = 0; y < roomHeight; y++) {
			for (int x = 0; x < roomWidth; x++) {
				SquareObject square = MapSquareManager.instance.GetSquare(startX + x, startY + y);
				if (square == null) continue;
				// マスを部屋地形に変更
				square.SetTerrain(eTerrain.Room);
				roomIDList.Add(square.squareData.ID);
			}
		}
		MapSquareManager.instance.AddRoom(roomIDList);
	}

	/// <summary>
	/// すべての部屋を繋げる
	/// </summary>
	private void ConnectAllRoom() {
		// 掘削方向の決定
		eDirectionFour digDir = (eDirectionFour)Random.Range(0, (int)eDirectionFour.Max);
		for (int i = 0; i < _areaList.Count - 1; i++) {
			// エリア1を分割線まで掘る
			AreaData area1 = _areaList[i];

			// 掘削方向の決定

			// エリア2を分割線まで掘る
			AreaData area2 = _areaList[i + 1];

			// 分割線内で通路を繋げる

			// 掘削方向の決定

		}
	}

	/// <summary>
	/// 指定エリアを指定方向に分割線まで掘る
	/// </summary>
	/// <param name="area"></param>
	/// <param name="dir"></param>
	private void DigToDivideLine(AreaData area, eDirectionFour dir) {
		// 掘削開始マスの決定
		// 掘削方向の逆方向を取得
		eDirectionFour reverseDir = dir.ReverseDir();
		// エリアの全てのマスから壁かつ、掘削と逆方向の隣接マスが部屋マスのマスを集約
		int startX = area.startX;
		int startY = area.startY;
		for (int y = 0; y < area.height; y++) {
			for (int x = 0; x < area.width; x++) {
				SquareObject square = MapSquareManager.instance.GetSquare(startX + x, startY + y);
				// 壁でなければ処理しない
				if (square == null || square.squareData.terrain != eTerrain.Wall) continue;
				// 掘削方向の逆の隣接マスを取得

			}
		}

		// ↑からランダムに1マス抽選
		// 分割線までの掘削

	}

}
