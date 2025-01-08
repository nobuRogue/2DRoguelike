/**
 * @file EnemyAI_00Normal.cs
 * @brief プレイヤーに近づいて殴るAI
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;
using static MapSquareUtility;

public class EnemyAI_00Normal : EnemyAIBase {
	public EnemyAI_00Normal( System.Func<EnemyCharacter> GetSourceEnemy ) : base( GetSourceEnemy ) {

	}

	public override MoveAction ThinkAction() {
		// 視界とプレイヤーを取得する
		List<int> visibleArea = null;
		EnemyCharacter sourceEnemy = _GetSourceEnemy();
		MapSquareData sourceSquare = GetSquareData( sourceEnemy.squarePosition );
		PlayerCharacter player = GetVisibleAreaAndPlayer( ref visibleArea, sourceSquare );
		if (player == null) {
			// 移動可能な方向を取得
			int maxDirIndex = (int)eDirectionEight.Max;
			List<eDirectionEight> canMoveList = new List<eDirectionEight>( maxDirIndex );
			Vector2Int sourcePosition = sourceSquare.squarePosition;
			for (int i = 0; i < maxDirIndex; i++) {
				var checkDir = (eDirectionEight)i;
				MapSquareData checkSquare = GetSquareData( sourcePosition.ToVectorPos( checkDir ) );
				if (checkSquare == null) continue;

				if (!CanMove( sourcePosition, checkSquare, checkDir )) continue;

				canMoveList.Add( checkDir );
			}
			if (IsEmpty( canMoveList )) return null;
			// 移動可能なランダムな方向に移動
			eDirectionEight moveDir = canMoveList[Random.Range( 0, canMoveList.Count )];
			MapSquareData randomMoveSquare = GetSquareData( sourcePosition.ToVectorPos( moveDir ) );
			MoveAction randomMove = new MoveAction();
			randomMove.ProcessData( sourceEnemy, new ChebyshevMoveData( sourceSquare.ID, randomMoveSquare.ID, moveDir ) );
			return randomMove;
		} else {
			// プレイヤーを見つけているので可能な行動があるか確認
			var attackActionMaster = ActionMasterUtility.GetActionMaster( GameConst.ATTACK_ACTION_ID );
			eDirectionEight dir = eDirectionEight.Invalid;
			if (ActionRangeManager.GetRange( attackActionMaster.rangeID ).CanUse( sourceSquare.ID, ref dir )) {
				// 可能な行動があるので予定に設定して終わり
				_scheduleActionID = GameConst.ATTACK_ACTION_ID;
				return null;
			}
			// プレイヤーに近づく移動を行う
			MapSquareData playerSquare = GetSquareData( player.squarePosition );
			List<ChebyshevMoveData> routeResult = null;
			RouteSearcher.RouteSearchChebyshev( ref routeResult, sourceSquare.ID, playerSquare.ID, CanPassToPlayer );
			if (IsEnableIndex( routeResult, 1 )) {
				ChebyshevMoveData toPlayerMoveData = routeResult[0];
				MoveAction toPlayerMove = new MoveAction();
				toPlayerMove.ProcessData( sourceEnemy, toPlayerMoveData );
				return toPlayerMove;
			}
		}
		return null;
	}

	private bool CanPassToPlayer( MapSquareData square, eDirectionEight dir, int distance ) {
		MapSquareData prevSquare = GetSquareData( square.squarePosition.ToVectorPos( dir.ReverseDir() ) );
		if (prevSquare == null) return false;

		if (!CanMoveTerrain( prevSquare.squarePosition, square, dir )) return false;

		return square.enemyID < 0;
	}

}
