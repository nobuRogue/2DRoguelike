/**
 * @file ActionEffect000_Attack.cs
 * @brief í èÌçUåÇå¯â 
 * @author yaonobu
 * @date 2025/1/4
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class ActionEffect000_Attack : ActionEffectBase {

	public override async UniTask Execute( CharacterBase sourceCharacter, ActionRangeBase rangeData ) {
		sourceCharacter.SetAnimation( eCharacterAnimation.Attack );

		int sourceAttack = sourceCharacter.attack;
		if (rangeData.isPlayerTarget) {
			PlayerCharacter player = GetPlayer();
			await ExecuteAttack( sourceAttack, player );
		}
		List<int> enemyList = rangeData.targetEnemyIDList;
		for (int i = 0, max = enemyList.Count; i < max; i++) {
			EnemyCharacter targetEnemy = GetEnemy( enemyList[i] );
			if (targetEnemy == null) continue;

			await ExecuteAttack( sourceAttack, targetEnemy );
		}
		while (sourceCharacter.GetCurrentAnimation() == eCharacterAnimation.Attack) await UniTask.DelayFrame( 1 );

	}

	private async UniTask ExecuteAttack( int sourceAttack, CharacterBase targetCharacter ) {
		targetCharacter.SetAnimation( eCharacterAnimation.Damage );
		int targetDefense = targetCharacter.defense;
		int damage = (int)(sourceAttack * Mathf.Pow( 15.0f / 16.0f, targetDefense ));
		targetCharacter.Damage( damage );
		await UniTask.Delay( 150 );
		UniTask task = SoundManager.instance.PlaySE( 1 );
		while (targetCharacter.GetCurrentAnimation() == eCharacterAnimation.Damage) await UniTask.DelayFrame( 1 );

		if (!targetCharacter.IsDead()) return;

		task = SoundManager.instance.PlaySE( 2 );
		await DeadCharacter( targetCharacter );
	}

}
