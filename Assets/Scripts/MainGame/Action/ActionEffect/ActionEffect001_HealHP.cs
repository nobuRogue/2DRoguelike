using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HP‰ñ•œŒø‰Ê
/// </summary>
public class ActionEffect001_HealHP : ActionEffectBase {
	// HP‰ñ•œ‚ÌƒƒO
	private const int _HEAL_HP_LOG_ID = 3006;
	// HP‰ñ•œ‚ÌSE‚ÌID
	private const int _HEAL_SE_ID = 9;

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// ‰ñ•œ—Êæ“¾
		int healValue = param[0];
		// ‘ÎÛ‚²‚Æ‚É‰ñ•œˆ—
		List<int> targetList = range.targetCharacterList;
		int targetCount = targetList.Count;
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		for (int i = 0; i < targetCount; i++) {
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// SEÄ¶
			UniTask task = SoundManager.instance.PlaySE(_HEAL_SE_ID);
			// ƒƒO‚Ì•\¦
			logMenu.AddLog(string.Format(_HEAL_HP_LOG_ID.ToMessage(), target.characterData.GetName(), healValue));
			// ‘ÎÛ‚ÌHP‰ñ•œ
			target.characterData.AddHP(healValue);
		}
		await UniTask.DelayFrame(5);
	}

}
