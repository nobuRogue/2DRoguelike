/**
 * @file EnemyAIBase.cs
 * @brief エネミーのAIの基底
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAIBase {
	protected System.Func<EnemyCharacter> _GetSourceEnemy = null;

	public EnemyAIBase( System.Func<EnemyCharacter> GetSourceEnemy ) {
		_GetSourceEnemy = GetSourceEnemy;
	}

	public abstract MoveAction ThinkAction();
}
