using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

	#region Manhattan

	/// <summary>
	/// 四方向用A*経路探索ノード
	/// </summary>
	private class DistanceNodeManhattan : DistanceNode {
		// 親ノードからの移動方向
		public eDirectionFour prevDir { get; private set; } = eDirectionFour.Invalid;
		// 親ノード
		public DistanceNodeManhattan rootNode { get; private set; } = null;
		public DistanceNodeManhattan(int distance, int squareID, eDirectionFour dir, DistanceNodeManhattan rootNode) : base(distance, squareID) {
			prevDir = dir;
			this.rootNode = rootNode;
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
		public List<DistanceNodeManhattan> openList = null;
		// クローズ済みのノード
		public List<DistanceNodeManhattan> closeList = null;

		public DistanceNodeManhattanTable() {
			openList = new List<DistanceNodeManhattan>();
			closeList = new List<DistanceNodeManhattan>();
		}

		// 初期化
		public void Clear() {
			goalNode = null;
			openList.Clear();
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
		return CreateRouteManhattan();
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
		_manhattanTable.openList.Add(new DistanceNodeManhattan(0, startSquareID, eDirectionFour.Invalid, null));
		// ゴール座標の取得
		SquareObject goalSquare = MapSquareManager.instance.GetSquare(goalSquareID);
		if (goalSquare == null || goalSquare.squareData == null) return;

		int goalX = goalSquare.squareData.posX, goalY = goalSquare.squareData.posY;
		// ゴールをオープンするまでループ
		while (_manhattanTable.goalNode == null) {
			// スコア最小のノードを探す
			DistanceNodeManhattan minScoreNode = GetMinScoreNodeManhattan(goalX, goalY);
			// スコア最小のノードが見つからなければ終わり
			if (minScoreNode == null) break;
			// 周りをオープン
			OpenNodeAroundManhattan(minScoreNode, goalSquareID, CanPass);
			// クローズする
			_manhattanTable.openList.Remove(minScoreNode);
			_manhattanTable.closeList.Add(minScoreNode);
		}
	}

	// 最少スコアのノードを返す
	private DistanceNodeManhattan GetMinScoreNodeManhattan(int goalX, int goalY) {
		if (CommonModule.IsEmpty(_manhattanTable.openList)) return null;

		int minScore = -1;
		DistanceNodeManhattan result = null;
		List<DistanceNodeManhattan> openList = _manhattanTable.openList;
		for (int i = 0; i < openList.Count; i++) {
			DistanceNodeManhattan node = openList[i];
			if (node == null) continue;
			// 最少スコアチェック
			int nodeScore = node.GetScore(goalX, goalY);
			if (result != null && minScore <= nodeScore) continue;

			result = node;
			minScore = nodeScore;
		}
		return result;
	}

	/// <summary>
	/// 指定ノードの周囲4マスをオープンする
	/// </summary>
	private void OpenNodeAroundManhattan(DistanceNodeManhattan baseNode, int goalSquareID, System.Func<MapSquare, bool> CanPass) {
		if (baseNode == null) return;
		// オープンするノードの実コスト決定
		int distance = baseNode.distance + 1;

		SquareObject square = MapSquareManager.instance.GetSquare(baseNode.squareID);
		int baseX = square.squareData.posX, baseY = square.squareData.posY;
		// 周囲4マスをオープンする
		for (int i = 0; i < (int)eDirectionFour.Max; i++) {
			eDirectionFour dir = (eDirectionFour)i;
			// 指定ノードから指定方向に隣接するマスを取得
			SquareObject openSquare = MapSquareManager.instance.GetToDirSquare(baseX, baseY, dir);
			if (openSquare == null) continue;
			// 既にクローズしたマスなら処理しない	ラムダ式
			if (_manhattanTable.closeList.Exists(node => node.squareID == openSquare.squareData.ID)) continue;
			// 既にオープンしているマスも処理しない
			if (_manhattanTable.openList.Exists(node => node.squareID == openSquare.squareData.ID)) continue;
			// 通行不可のマスなら処理しない
			if (!CanPass(openSquare.squareData)) continue;
			// ノードのオープン
			DistanceNodeManhattan addNode = new DistanceNodeManhattan(distance, openSquare.squareData.ID, dir, baseNode);
			_manhattanTable.openList.Add(addNode);
			// ゴール判定
			if (openSquare.squareData.ID != goalSquareID) continue;
			// ゴールをオープンしたら終わり
			_manhattanTable.goalNode = addNode;
			return;
		}
	}

	/// <summary>
	/// 経路生成
	/// </summary>
	/// <returns></returns>
	private List<ManhattanMoveData> CreateRouteManhattan() {
		// ゴールに辿り着けていないか判定
		if (_manhattanTable == null || _manhattanTable.goalNode == null) return null;
		// 経路用のリストを生成
		int routeCount = _manhattanTable.goalNode.distance;
		List<ManhattanMoveData> route = new List<ManhattanMoveData>();
		for (int i = 0; i < routeCount; i++) {
			route.Add(null);
		}
		// ゴールから遡って経路を生成
		DistanceNodeManhattan currentNode = _manhattanTable.goalNode;
		for (int i = routeCount - 1; i >= 0; i--) {
			ManhattanMoveData moveData = new ManhattanMoveData(currentNode.rootNode.squareID, currentNode.squareID, currentNode.prevDir);
			route[i] = moveData;
			// 親ノードをを現在のノードにする
			currentNode = currentNode.rootNode;
		}
		return route;
	}

	#endregion

	#region Chebyshev

	private class DistanceNodeChebyshev : DistanceNode {
		public eDirectionEight dir { get; private set; } = eDirectionEight.Invalid;
		/// 親ノード
		public DistanceNodeChebyshev rootNode { get; private set; } = null;

		public DistanceNodeChebyshev(int distance, int squareID, eDirectionEight dir, DistanceNodeChebyshev rootNode) : base(distance, squareID) {
			this.dir = dir;
			this.rootNode = rootNode;
		}

		public override int GetScore(int goalX, int goalY) {
			SquareObject square = MapSquareManager.instance.GetSquare(squareID);
			if (square == null || square.squareData == null) return int.MaxValue;
			// ゴールからの差
			int dx = Mathf.Abs(goalX - square.squareData.posX);
			int dy = Mathf.Abs(goalY - square.squareData.posY);
			return Mathf.Max(dx, dy) + distance;
		}
	}

	private class DistanceNodeTableChebyshev {
		// ゴールのノード
		public DistanceNodeChebyshev goalNode = null;
		// オープンしたノードのリスト
		public List<DistanceNodeChebyshev> openList = null;
		// クローズしたノードのリスト
		public List<DistanceNodeChebyshev> closeList = null;

		public DistanceNodeTableChebyshev() {
			openList = new List<DistanceNodeChebyshev>();
			closeList = new List<DistanceNodeChebyshev>();
		}

		public void Clear() {
			goalNode = null;
			openList.Clear();
			closeList.Clear();
		}
	}
	// 8方向経路探索用のノードテーブル
	private DistanceNodeTableChebyshev _nodeTableChebyshev = null;

	/// <summary>
	/// 8方向用経路探索
	/// </summary>
	/// <param name="startSquareID"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	/// <returns></returns>
	public List<ChebyshevMoveData> RouteSearchChebyshev(int startSquareID, int goalSquareID, System.Func<SquareObject, SquareObject, eDirectionEight, bool> CanPass) {
		// ゴールノードに辿り着くまでノードをオープンする
		OpenNodeToGoalChebyshev(startSquareID, goalSquareID, CanPass);
		// ゴールノードからスタートまで遡って経路を生成
		return CreateRouteChebyshev();
	}

	private void OpenNodeToGoalChebyshev(int startSquareID, int goalSquareID, System.Func<SquareObject, SquareObject, eDirectionEight, bool> CanPass) {
		if (_nodeTableChebyshev == null) {
			_nodeTableChebyshev = new DistanceNodeTableChebyshev();
		}
		else {
			_nodeTableChebyshev.Clear();
		}
		// スタートマスのノードを生成してオープンリストに追加
		_nodeTableChebyshev.openList.Add(new DistanceNodeChebyshev(0, startSquareID, eDirectionEight.Invalid, null));
		// ゴールマスの座標をキャッシュしておく
		SquareObject goalSquare = MapSquareManager.instance.GetSquare(goalSquareID);
		int goalX = goalSquare.squareData.posX, goalY = goalSquare.squareData.posY;
		// ゴールマスをオープンするまでループ
		while (_nodeTableChebyshev.goalNode == null) {
			// スコア最小のノードを取得
			DistanceNodeChebyshev minScoreNode = GetMinScoreNodeChebyshev(goalX, goalY);
			if (minScoreNode == null) break;
			// 周囲8マスをオープン
			OpenNodeAroundChebyshev(minScoreNode, goalSquareID, CanPass);
			// 周囲をオープンしたノードをオープンから除きクローズに追加
			_nodeTableChebyshev.openList.Remove(minScoreNode);
			_nodeTableChebyshev.closeList.Add(minScoreNode);
		}
	}

	private DistanceNodeChebyshev GetMinScoreNodeChebyshev(int goalX, int goalY) {
		List<DistanceNodeChebyshev> openList = _nodeTableChebyshev.openList;
		if (CommonModule.IsEmpty(openList)) return null;

		DistanceNodeChebyshev result = null;
		int minScore = -1;
		for (int i = 0; i < openList.Count; i++) {
			DistanceNodeChebyshev node = openList[i];
			if (node == null) continue;

			int score = node.GetScore(goalX, goalY);
			if (result != null && minScore <= score) continue;
			// 最少スコアノードの更新
			result = node;
			minScore = score;
		}
		return result;
	}

	private void OpenNodeAroundChebyshev(DistanceNodeChebyshev baseNode, int goalSquareID, System.Func<SquareObject, SquareObject, eDirectionEight, bool> CanPass) {
		// 実コストの決定
		int distance = baseNode.distance + 1;
		SquareObject baseSquare = MapSquareManager.instance.GetSquare(baseNode.squareID);
		int baseX = baseSquare.squareData.posX, baseY = baseSquare.squareData.posY;
		// 周囲8マスでループ
		for (int i = 0; i < (int)eDirectionEight.Max; i++) {
			// インデクスを方向にキャスト
			eDirectionEight dir = (eDirectionEight)i;
			SquareObject openSquare = MapSquareManager.instance.GetToDirSquare(baseX, baseY, dir);
			// 既にオープン、クローズされたノードなら処理しない
			if (_nodeTableChebyshev.openList.Exists(element => element.squareID == openSquare.squareData.ID) ||
				_nodeTableChebyshev.closeList.Exists(element => element.squareID == openSquare.squareData.ID)) continue;
			// 通行不可のマスなら処理しない
			if (!CanPass(baseSquare, openSquare, dir)) continue;
			// ノードのオープン
			DistanceNodeChebyshev openNode = new DistanceNodeChebyshev(distance, openSquare.squareData.ID, dir, baseNode);
			_nodeTableChebyshev.openList.Add(openNode);
			// ゴール判定
			if (openSquare.squareData.ID != goalSquareID) continue;
			// ゴールをオープンしたので終わり
			_nodeTableChebyshev.goalNode = openNode;
			break;
		}
	}

	/// <summary>
	/// 8方向用経路生成
	/// </summary>
	/// <returns></returns>
	private List<ChebyshevMoveData> CreateRouteChebyshev() {
		// ゴールにたどり着いていないならnullを返す
		if (_nodeTableChebyshev.goalNode == null) return null;
		// あらかじめ経路のリストをキャッシュする
		int routeCount = _nodeTableChebyshev.goalNode.distance;
		List<ChebyshevMoveData> result = new List<ChebyshevMoveData>(routeCount);
		for (int i = 0; i < routeCount; i++) {
			result.Add(null);
		}
		// ゴールから遡って経路生成
		DistanceNodeChebyshev currentNode = _nodeTableChebyshev.goalNode;
		for (int i = routeCount - 1; i >= 0; i--) {
			ChebyshevMoveData moveData = new ChebyshevMoveData(currentNode.rootNode.squareID, currentNode.squareID, currentNode.dir);
			result[i] = moveData;
			// 親ノードを現在ノードにする
			currentNode = currentNode.rootNode;
		}
		return result;
	}

	#endregion

}
