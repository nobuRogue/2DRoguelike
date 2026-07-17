using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆発ダメージ効果
/// プレイヤーなら割合ダメージ
/// エネミーなら死亡
/// </summary>
public class ActionEffect010_ExplosionDamage : ActionEffectBase {

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 対象の取得
		List<int> targetList = range.targetCharacterList;
		int targetCount = targetList.Count;
		List<UniTask> taskList = new List<UniTask>(targetCount);
		//すべての対象に対して爆発ダメージ効果
		int damageRatio = param[0];
		for (int i = 0; i < targetCount; i++) {
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;

			taskList.Add(ExecuteExDamage(target, damageRatio));
		}
		// 全ての与ダメージタスクの終了待ち
		await UniTask.WhenAll(taskList);
	}

	/// <summary>
	/// 対象単体への爆発ダメージ効果発動
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	private async UniTask ExecuteExDamage(CharacterObject target, int damageRatio) {
		// 対象の被ダメージモーション
		target.SetAnimation(eCharacterAnimation.Damage);
		// 被ダメージモーションの終了待ち
		while (target.currentAnim == eCharacterAnimation.Damage) await UniTask.DelayFrame(1);
		// 爆発効果発動
		target.characterData.ExplosionDamage(damageRatio);
		// 死亡判定
		if (target.characterData.isDead) target.Dead();
	}

}
