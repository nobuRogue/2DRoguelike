/**
 * @file ActionRange00_DirForward.cs
 * @brief �����Ă������1�}�X
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MapSquareUtility;
using static CommonModule;

public class ActionRange00_DirForward : ActionRangeBase {

	/// <summary>
	/// �Ώۂ̌���
	/// </summary>
	/// <param name="sourceCharacter"></param>
	public override void Setup( CharacterBase sourceCharacter ) {
		isPlayerTarget = false;
		InitializeList( ref targetEnemyIDList );
		eDirectionEight dir = sourceCharacter.direction;
		MapSquareData targetSquare = GetSquareData( sourceCharacter.squarePosition.ToVectorPos( dir ) );

		if (targetSquare.existPlayer) {
			isPlayerTarget = true;
		} else if (targetSquare.enemyID >= 0) {
			targetEnemyIDList.Add( targetSquare.enemyID );
		}
	}

	/// <summary>
	/// AI�̎g�p�۔���
	/// </summary>
	/// <param name="sourceSquareID"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public override bool CanUse( int sourceSquareID, ref eDirectionEight dir ) {
		isPlayerTarget = false;
		InitializeList( ref targetEnemyIDList );
		MapSquareData sourceSquare = GetSquareData( sourceSquareID );
		Vector2Int sourcePos = sourceSquare.squarePosition;
		// ����8�����Ō���
		for (int i = 0, max = (int)eDirectionEight.Max; i < max; i++) {
			eDirectionEight checkDir = (eDirectionEight)i;
			MapSquareData square = GetSquareData( sourceSquare.squarePosition.ToVectorPos( checkDir ) );
			if (!CanAttackSquare( sourcePos, square, checkDir ) || !square.existPlayer) continue;

			dir = checkDir;
			return true;
		}
		dir = eDirectionEight.Invalid;
		return false;
	}

	public override ActionRangeBase GetInstance() {
		return new ActionRange00_DirForward();
	}
}
