using System.Collections.Generic;
using UnityEngine;

public class RouteSearcher {
	// シングルトン
	private static RouteSearcher _instance = null;
	public static RouteSearcher instance {
		get {
			if (_instance == null) _instance = new RouteSearcher();

			return _instance;
		}
	}
	private RouteSearcher() {

	}

	/// <summary>
	/// A*経路探索ノード
	/// </summary>
	private abstract class DistanceNode {
		// 実コスト
		public int distance { get; private set; } = -1;
		// マスID
		public int squareID { get; private set; } = -1;

		public DistanceNode(int distance, int squareID) {
			this.distance = distance;
			this.squareID = squareID;
		}

		// スコア取得処理
		public abstract int GetScore(int goalX, int goalY);
	}
	/// <summary>
	/// 四方向用A*経路探索ノード
	/// </summary>
	private class DistanceNodeManhattan : DistanceNode {
		// 親ノードからの移動方向
		public eDirectionFour prevDir { get; private set; } = eDirectionFour.Invalid;
		// 親ノード
		public DistanceNodeManhattan rootNode { get; private set; } = null;
		public DistanceNodeManhattan(int distance, int squareID) : base(distance, squareID) {

		}

		// スコア取得処理
		public override int GetScore(int goalX, int goalY) {
			SquareObject square = MapSquareManager.instance.GetSquare(squareID);
			if (square == null || square.squareData == null) return int.MaxValue;
			// ゴールからの差
			int dx = Mathf.Abs(goalX - square.squareData.posX);
			int dy = Mathf.Abs(goalY - square.squareData.posY);
			// 推定コスト+実コストをスコアとして返す
			return dx + dy + distance;
		}
	}
	/// <summary>
	/// 四方向用A*経路探索ノードテーブル
	/// </summary>
	private class DistanceNodeManhattanTable {
		// ゴールノード
		public DistanceNodeManhattan goalNode = null;
		// オープン済みのノード
		public List<DistanceNodeManhattan> openNodeList = null;
		// クローズ済みのノード
		public List<DistanceNodeManhattan> closeList = null;

		public DistanceNodeManhattanTable() {
			openNodeList = new List<DistanceNodeManhattan>();
			closeList = new List<DistanceNodeManhattan>();
		}

		// 初期化
		public void Clear() {
			goalNode = null;
			openNodeList.Clear();
			closeList.Clear();
		}
	}

	// 4方向用のノードテーブル
	private DistanceNodeManhattanTable _manhattanTable = null;

	/// <summary>
	/// 4方向経路探索
	/// </summary>
	/// <param name="startSquareID">開始マスID</param>
	/// <param name="goalSquareID">終了マスID</param>
	/// <param name="CanPass">通行可否判定</param>
	/// <returns>経路</returns>
	public List<ManhattanMoveData> RouteSearchManhattan(int startSquareID, int goalSquareID, System.Func<MapSquare, bool> CanPass) {
		// ゴールノードにたどり着くまでノードをオープンする
		OpenNodeToGoalManhattan(startSquareID, goalSquareID, CanPass);
		// ゴールノードからスタートまで親ノードを辿って経路を生成

		return null;
	}

	/// <summary>
	/// スタートからゴールノードにたどり着くまでノードをオープンする
	/// </summary>
	private void OpenNodeToGoalManhattan(int startSquareID, int goalSquareID, System.Func<MapSquare, bool> CanPass) {
		// テーブル初期化
		if (_manhattanTable == null) {
			_manhattanTable = new DistanceNodeManhattanTable();
		}
		else {
			_manhattanTable.Clear();
		}
		// スタートノードの生成
		_manhattanTable.openNodeList.Add(new DistanceNodeManhattan(0, startSquareID));
		// ゴール座標の取得
		SquareObject goalSquare = MapSquareManager.instance.GetSquare(goalSquareID);
		if (goalSquare == null || goalSquare.squareData == null) return;

		int goalX = goalSquare.squareData.posX, goalY = goalSquare.squareData.posY;
		// ゴールをオープンするまでループ
		while (_manhattanTable.goalNode == null) {
			// スコア最小のノードを探す

			// スコア最小のノードが見つからなければ終わり

			// 周りをオープン

			// クローズする

		}
	}

	private DistanceNodeManhattan GetMinScoreNode(int goalX, int goalY) {

		return null;
	}

}
