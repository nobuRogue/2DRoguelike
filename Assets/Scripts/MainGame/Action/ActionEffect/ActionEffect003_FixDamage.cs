using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 固定ダメージ付与
/// </summary>
public class ActionEffect003_FixDamage : ActionEffectBase {
	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 付与ダメージ取得
		int damage = param[0];
		// 対象ごとに処理
		List<int> targetList = range.targetCharacterList;
		int targetCount = targetList.Count;
		List<UniTask> taskList = new List<UniTask>(targetCount);
		for (int i = 0; i < targetCount; i++) {
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// ダメージ付与
			taskList.Add(ExecuteDamage(damage, target));
		}
		// 全ての被ダメージタスクの終了待ち
		await UniTask.WhenAll(taskList);
	}


}
