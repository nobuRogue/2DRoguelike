using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 対象の所持アイテムを全て焼く
/// </summary>
public class ActionEffect004_BurnItem : ActionEffectBase {

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 対象毎に効果の適用
		List<int> targetList = range.targetCharacterList;
		for (int i = 0; i < targetList.Count; i++) {
			// キャラクターデータの取得
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// 所持アイテムを全て焼く
			BurnItemList(target.characterData.possessItemList);
		}

	}

	private void BurnItemList(List<int> itemIDList) {
		if (CommonModule.IsEmpty(itemIDList)) return;

		for (int i = 0; i < itemIDList.Count; i++) {
			// アイテムを焼く
			// アイテムデータ取得

			// 変化先のマスターデータ取得

			// 変化先がなければ終了

			// ログ表示

			// アイテムマスターデータの変更

		}
	}

}
