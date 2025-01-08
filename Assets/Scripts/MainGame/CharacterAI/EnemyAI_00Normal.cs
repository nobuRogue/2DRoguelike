/**
 * @file EnemyAI_00Normal.cs
 * @brief �v���C���[�ɋ߂Â��ĉ���AI
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
		// ���E�ƃv���C���[���擾����
		List<int> visibleArea = null;
		EnemyCharacter sourceEnemy = _GetSourceEnemy();
		MapSquareData sourceSquare = GetSquareData( sourceEnemy.squarePosition );
		PlayerCharacter player = GetVisibleAreaAndPlayer( ref visibleArea, sourceSquare );
		if (player == null) {
			// �ړ��\�ȕ������擾
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
			// �ړ��\�ȃ����_���ȕ����Ɉړ�
			eDirectionEight moveDir = canMoveList[Random.Range( 0, canMoveList.Count )];
			MapSquareData randomMoveSquare = GetSquareData( sourcePosition.ToVectorPos( moveDir ) );
			MoveAction randomMove = new MoveAction();
			randomMove.ProcessData( sourceEnemy, new ChebyshevMoveData( sourceSquare.ID, randomMoveSquare.ID, moveDir ) );
			return randomMove;
		} else {
			// �v���C���[�������Ă���̂ŉ\�ȍs�������邩�m�F
			var attackActionMaster = ActionMasterUtility.GetActionMaster( GameConst.ATTACK_ACTION_ID );
			eDirectionEight dir = eDirectionEight.Invalid;
			if (ActionRangeManager.GetRange( attackActionMaster.rangeID ).CanUse( sourceSquare.ID, ref dir )) {
				// �\�ȍs��������̂ŗ\��ɐݒ肵�ďI���
				_scheduleActionID = GameConst.ATTACK_ACTION_ID;
				return null;
			}
			// �v���C���[�ɋ߂Â��ړ����s��
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
