/**
 * @file RouteSearch.cs
 * @brief �o�H�T���N���X
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
	/// �}���n�b�^�������ł̌o�H�T��
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
		// �ŏ��̃m�[�h�𐶐��I�[�v�����X�g�ɉ�����
		_manhattanOpenList.Add( new DistanceNodeManhattan( eDirectionFour.Invalid, null, 0, startSquareID ) );
		Vector2Int goalPosition = MapSquareManager.instance.Get( goalSquareID ).squarePosition;
		while (_distanceTableManhattan.goalNode == null) {
			var minScoreNode = GetMinManhattanScoreNode( goalPosition );
			if (minScoreNode == null) return;

			SearchDistanceDirectionFour( minScoreNode, goalSquareID, CanPass );
			_manhattanOpenList.Remove( minScoreNode );
		}
	}

	/// <summary>
	/// 4������T��
	/// �X�^�[�g�}�X���܂�
	/// �C���f�N�X0���X�^�[�g
	/// </summary>
	/// <param name="distanceTable"></param>
	/// <param name="baseNode"></param>
	/// <param name="range"></param>
	/// <param name="CanPass"></param>
	/// <param name="maxRange"></param>
	private static void SearchDistanceDirectionFour( DistanceNodeManhattan baseNode, int goalSquareID,
		System.Func<MapSquareData, eDirectionFour, int, bool> CanPass ) {
		if (baseNode == null) return;

		Vector2Int basePosition = MapSquareManager.instance.Get( baseNode.squareID ).squarePosition;
		for (int i = (int)eDirectionFour.Up; i < (int)eDirectionFour.Max; i++) {
			eDirectionFour dir = (eDirectionFour)i;
			Vector2Int nextPosition = basePosition.ToVectorPos( dir );
			MapSquareData nextSquare = MapSquareManager.instance.Get( nextPosition );
			if (nextSquare == null) continue;

			if (_distanceTableManhattan.distanceNodeList.Exists( element => element.squareID == nextSquare.ID )) continue;
			// ���g�̒ǉ�
			int range = baseNode.distance + 1;
			if (!CanPass( nextSquare, dir, range )) continue;
			// �������Ēǉ�
			DistanceNodeManhattan addNode = new DistanceNodeManhattan( dir, baseNode, range, nextSquare.ID );
			_distanceTableManhattan.distanceNodeList.Add( addNode );
			_manhattanOpenList.Add( addNode );
			if (nextSquare.ID != goalSquareID) continue;
			// �S�[���ɂ��ǂ蒅�����̂ŏI���
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
			} else {
				if (minScore < currenScore) {
					minScoreNode = currentNode;
					minScore = currenScore;
				}
			}
		}
		return minScoreNode;
	}

}
