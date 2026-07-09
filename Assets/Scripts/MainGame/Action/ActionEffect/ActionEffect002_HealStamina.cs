using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 満腹度回復効果
/// </summary>
public class ActionEffect002_HealStamina : ActionEffectBase {
	// 満腹度回復時のログ
	private const int _HEAL_STAMINA_LOG_ID = 3007;
	// HP回復時のSEのID
	private const int _HEAL_SE_ID = 8;

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 回復量取得
		int healValue = param[0];
		// 対象ごとに回復処理
		List<int> targetList = range.targetCharacterList;
		int targetCount = targetList.Count;
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		for (int i = 0; i < targetCount; i++) {
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// SE再生
			UniTask task = SoundManager.instance.PlaySE(_HEAL_SE_ID);
			// ログの表示
			logMenu.AddLog(string.Format(_HEAL_STAMINA_LOG_ID.ToMessage(), target.characterData.GetName(), healValue));
			// 対象の満腹度回復
			target.characterData.AddStamina(healValue * 100);
		}
		await UniTask.DelayFrame(5);

	}
}
