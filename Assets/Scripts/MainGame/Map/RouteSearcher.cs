/**
 * @file RouteSearch.cs
 * @brief 経路探索クラス
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class RouteSearcher {
	private class DistanceNode {
		public int distance { get; private set; } = -1;
		public int squareID { get; private set; } = -1;
		public DistanceNode( int setDistance, int setSquareID ) {
			distance = setDistance;
			squareID = setSquareID;
		}
	}

	#region Manhattan

	private class DistanceNodeManhattan : DistanceNode {
		public eDirectionFour dir;
		public DistanceNodeManhattan prevNode = null;

		public DistanceNodeManhattan( eDirectionFour setDir, DistanceNodeManhattan setPrevNode, int setDistance, int setSquareID ) : base( setDistance, setSquareID ) {
			dir = setDir;
			prevNode = setPrevNode;
		}

		public int GetScore( Vector2Int goalPosition ) {
			MapSquareData square = MapSquareManager.instance.Get( squareID );
			int xdiff = Mathf.Abs( square.squarePosition.x - goalPosition.x );
			int ydiff = Mathf.Abs( square.squarePosition.y - goalPosition.y );
			return xdiff + ydiff;
		}
	}

	private class DistanceNodeTableManhattan {
		public DistanceNodeManhattan goalNode = null;
		public List<DistanceNodeManhattan> distanceNodeList = null;

		public static void Initialize( ref DistanceNodeTableManhattan distanceNodeTable ) {
			if (distanceNodeTable == null) {
				distanceNodeTable = new DistanceNodeTableManhattan();
				distanceNodeTable.distanceNodeList = new List<DistanceNodeManhattan>( 1024 );
			} else {
				InitializeList( ref distanceNodeTable.distanceNodeList );
				distanceNodeTable.goalNode = null;
			}
		}

		public DistanceNodeTableManhattan() {
			distanceNodeList = new List<DistanceNodeManhattan>();
		}
	}

	private static DistanceNodeTableManhattan _distanceTableManhattan = null;
	private static List<DistanceNodeManhattan> _manhattanOpenList = new List<DistanceNodeManhattan>();
	/// <summary>
	/// マンハッタン距離での経路探索
	/// </summary>
	/// <param name="result"></param>
	/// <param name="startSquareID"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	public static void RouteSearchManhattan(
		ref List<ManhattanMoveData> result, int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass ) {
		CreateDistanceTableManhattan( startSquareID, goalSquareID, CanPass );
		CreateRouteManhattan( ref result );
	}

	private static void CreateDistanceTableManhattan( int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass ) {
		DistanceNodeTableManhattan.Initialize( ref _distanceTableManhattan );
		InitializeList( ref _manhattanOpenList, 1024 );
		// 最初のノードを生成オープンリストに加える
		_manhattanOpenList.Add( new DistanceNodeManhattan( eDirectionFour.Invalid, null, 0, startSquareID ) );
		Vector2Int goalPosition = MapSquareManager.instance.Get( goalSquareID ).squarePosition;
		while (_distanceTableManhattan.goalNode == null) {
			var minScoreNode = GetMinManhattanScoreNode( goalPosition );
			if (minScoreNode == null) return;

			SearchDistanceDirectionManhattan( minScoreNode, goalSquareID, CanPass );
			_manhattanOpenList.Remove( minScoreNode );
		}
	}

	/// <summary>
	/// 4方向を探索
	/// スタートマスを含む
	/// インデクス0がスタート
	/// </summary>
	/// <param name="baseNode"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	private static void SearchDistanceDirectionManhattan( DistanceNodeManhattan baseNode, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass ) {
		if (baseNode == null) return;

		Vector2Int basePosition = MapSquareManager.instance.Get( baseNode.squareID ).squarePosition;
		for (int i = (int)eDirectionFour.Up; i < (int)eDirectionFour.Max; i++) {
			eDirectionFour dir = (eDirectionFour)i;
			Vector2Int nextPosition = basePosition.ToVectorPos( dir );
			MapSquareData nextSquare = MapSquareManager.instance.Get( nextPosition );
			if (nextSquare == null) continue;

			if (_distanceTableManhattan.distanceNodeList.Exists( element => element.squareID == nextSquare.ID )) continue;
			// 自身の追加
			int distance = baseNode.distance + 1;
			if (!CanPass( nextSquare, dir, distance )) continue;
			// 生成して追加
			DistanceNodeManhattan addNode = new DistanceNodeManhattan( dir, baseNode, distance, nextSquare.ID );
			_distanceTableManhattan.distanceNodeList.Add( addNode );
			_manhattanOpenList.Add( addNode );
			if (nextSquare.ID != goalSquareID) continue;
			// ゴールにたどり着いたので終わり
			_distanceTableManhattan.goalNode = addNode;
			return;
		}
	}

	private static void CreateRouteManhattan( ref List<ManhattanMoveData> result ) {
		InitializeList( ref result );
		if (_distanceTableManhattan == null || _distanceTableManhattan.goalNode == null) return;

		var currentNode = _distanceTableManhattan.goalNode;
		int routeCount = currentNode.distance;
		InitializeList( ref result, routeCount );
		for (int i = 0; i < routeCount; i++) {
			result.Add( null );
		}

		for (int i = routeCount - 1; i >= 0; i--) {
			var moveData = new ManhattanMoveData( currentNode.prevNode.squareID, currentNode.squareID, currentNode.dir );
			result[i] = moveData;
			currentNode = currentNode.prevNode;
		}
	}

	private static DistanceNodeManhattan GetMinManhattanScoreNode( Vector2Int goalPosition ) {
		if (IsEmpty( _manhattanOpenList )) return null;

		DistanceNodeManhattan minScoreNode = null;
		int minScore = -1;
		for (int i = 0, max = _manhattanOpenList.Count; i < max; i++) {
			var currentNode = _manhattanOpenList[i];
			if (currentNode == null) continue;

			int currenScore = currentNode.GetScore( goalPosition );
			if (minScoreNode == null) {
				minScoreNode = currentNode;
				minScore = currenScore;
			} else if (minScore > currenScore) {
				minScoreNode = currentNode;
				minScore = currenScore;
			}
		}
		return minScoreNode;
	}

	#endregion

	#region Chebyshev

	private class DistanceNodeChebyshev : DistanceNode {
		public eDirectionEight dir;
		public DistanceNodeChebyshev prevNode = null;

		public DistanceNodeChebyshev( eDirectionEight setDir, DistanceNodeChebyshev setPrevNode, int setDistance, int setSquareID ) : base( setDistance, setSquareID ) {
			dir = setDir;
			prevNode = setPrevNode;
		}

		public int GetScore( Vector2Int goalPosition ) {
			MapSquareData square = MapSquareManager.instance.Get( squareID );
			int xdiff = Mathf.Abs( square.squarePosition.x - goalPosition.x );
			int ydiff = Mathf.Abs( square.squarePosition.y - goalPosition.y );
			int score = Mathf.Max( xdiff, ydiff ) * 50;
			score += Mathf.Min( xdiff, ydiff );
			return score;
		}
	}

	private class DistanceNodeTableChebyshev {
		public DistanceNodeChebyshev goalNode = null;
		public List<DistanceNodeChebyshev> distanceNodeList = null;

		public static void Initialize( ref DistanceNodeTableChebyshev distanceNodeTable ) {
			if (distanceNodeTable == null) {
				distanceNodeTable = new DistanceNodeTableChebyshev();
				distanceNodeTable.distanceNodeList = new List<DistanceNodeChebyshev>( 1024 );
			} else {
				InitializeList( ref distanceNodeTable.distanceNodeList );
				distanceNodeTable.goalNode = null;
			}
		}

		public DistanceNodeTableChebyshev() {
			distanceNodeList = new List<DistanceNodeChebyshev>();
		}
	}

	private static DistanceNodeTableChebyshev _distanceTableChebyshev = null;
	private static List<DistanceNodeChebyshev> _chebyshevOpenList = new List<DistanceNodeChebyshev>();
	/// <summary>
	/// マンハッタン距離での経路探索
	/// </summary>
	/// <param name="result"></param>
	/// <param name="startSquareID"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	public static void RouteSearchChebyshev(
		ref List<ChebyshevMoveData> result, int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionEight, int, bool> CanPass ) {
		CreateDistanceTableChebyshev( startSquareID, goalSquareID, CanPass );
		CreateRouteChebyshev( ref result );
	}

	private static void CreateDistanceTableChebyshev( int startSquareID, int goalSquareID,
		System.Func<MapSquareData, eDirectionEight, int, bool> CanPass ) {
		DistanceNodeTableChebyshev.Initialize( ref _distanceTableChebyshev );
		InitializeList( ref _chebyshevOpenList, 1024 );
		// 最初のノードを生成オープンリストに加える
		_chebyshevOpenList.Add( new DistanceNodeChebyshev( eDirectionEight.Invalid, null, 0, startSquareID ) );
		Vector2Int goalPosition = MapSquareManager.instance.Get( goalSquareID ).squarePosition;
		while (_distanceTableChebyshev.goalNode == null) {
			var minScoreNode = GetMinChebyshevScoreNode( goalPosition );
			if (minScoreNode == null) return;

			SearchDistanceDirectionChebyshev( minScoreNode, goalSquareID, CanPass );
			_chebyshevOpenList.Remove( minScoreNode );
		}
	}

	/// <summary>
	/// 8方向を探索
	/// スタートマスを含む
	/// インデクス0がスタート
	/// </summary>
	/// <param name="baseNode"></param>
	/// <param name="goalSquareID"></param>
	/// <param name="CanPass"></param>
	private static void SearchDistanceDirectionChebyshev( DistanceNodeChebyshev baseNode, int goalSquareID,
		System.Func<MapSquareData, eDirectionEight, int, bool> CanPass ) {
		if (baseNode == null) return;

		Vector2Int basePosition = MapSquareManager.instance.Get( baseNode.squareID ).squarePosition;
		for (int i = (int)eDirectionEight.Up; i < (int)eDirectionEight.Max; i++) {
			eDirectionEight dir = (eDirectionEight)i;
			Vector2Int nextPosition = basePosition.ToVectorPos( dir );
			MapSquareData nextSquare = MapSquareManager.instance.Get( nextPosition );
			if (nextSquare == null) continue;

			if (_distanceTableChebyshev.distanceNodeList.Exists( element => element.squareID == nextSquare.ID )) continue;
			// 自身の追加
			int distance = baseNode.distance + 1;
			if (!CanPass( nextSquare, dir, distance )) continue;
			// 生成して追加
			DistanceNodeChebyshev addNode = new DistanceNodeChebyshev( dir, baseNode, distance, nextSquare.ID );
			_distanceTableChebyshev.distanceNodeList.Add( addNode );
			_chebyshevOpenList.Add( addNode );
			if (nextSquare.ID != goalSquareID) continue;
			// ゴールにたどり着いたので終わり
			_distanceTableChebyshev.goalNode = addNode;
			return;
		}
	}

	private static void CreateRouteChebyshev( ref List<ChebyshevMoveData> result ) {
		InitializeList( ref result );
		if (_distanceTableChebyshev == null || _distanceTableChebyshev.goalNode == null) return;

		var currentNode = _distanceTableChebyshev.goalNode;
		int routeCount = currentNode.distance;
		InitializeList( ref result, routeCount );
		for (int i = 0; i < routeCount; i++) {
			result.Add( null );
		}

		for (int i = routeCount - 1; i >= 0; i--) {
			var moveData = new ChebyshevMoveData( currentNode.prevNode.squareID, currentNode.squareID, currentNode.dir );
			result[i] = moveData;
			currentNode = currentNode.prevNode;
		}
	}

	private static DistanceNodeChebyshev GetMinChebyshevScoreNode( Vector2Int goalPosition ) {
		if (IsEmpty( _chebyshevOpenList )) return null;

		DistanceNodeChebyshev minScoreNode = null;
		int minScore = -1;
		for (int i = 0, max = _chebyshevOpenList.Count; i < max; i++) {
			var currentNode = _chebyshevOpenList[i];
			if (currentNode == null) continue;

			int currenScore = currentNode.GetScore( goalPosition );
			if (minScoreNode == null) {
				minScoreNode = currentNode;
				minScore = currenScore;
			} else if (minScore > currenScore) {
				minScoreNode = currentNode;
				minScore = currenScore;
			}
		}
		return minScoreNode;
	}

	#endregion

}
