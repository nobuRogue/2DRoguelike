/**
 * @file EnemyAIBase.cs
 * @brief エネミーのAIの基底
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MapSquareUtility;

public abstract class EnemyAIBase {
	protected System.Func<EnemyCharacter> _GetSourceEnemy = null;

	protected int _scheduleActionID = -1;

	public EnemyAIBase( System.Func<EnemyCharacter> GetSourceEnemy ) {
		_GetSourceEnemy = GetSourceEnemy;
	}

	public abstract MoveAction ThinkAction();

	public async UniTask ExecuteScheduleAction() {
		if (_scheduleActionID < 0) return;

		EnemyCharacter sourceEnemy = _GetSourceEnemy();
		MapSquareData sourceSquare = GetSquareData( sourceEnemy.squarePosition );
		var attackActionMaster = ActionMasterUtility.GetActionMaster( _scheduleActionID );
		eDirectionEight dir = eDirectionEight.Invalid;
		var rangeData = ActionRangeManager.GetRange( attackActionMaster.rangeID );
		if (rangeData.CanUse( sourceSquare.ID, ref dir )) {
			if (dir != eDirectionEight.Invalid) sourceEnemy.SetDirection( dir );

			rangeData.Setup( sourceEnemy );
			await ActionEffectManager.ExecuteEffect( attackActionMaster.effectID, sourceEnemy, rangeData );
		}
		_scheduleActionID = -1;
	}
}
