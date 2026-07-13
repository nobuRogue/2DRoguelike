using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 対象の所持アイテムを腐らせる
/// </summary>
public class ActionEffect005_LotItem : ActionEffectBase {

	// アイテムが腐った時のログメッセージID
	private const int _LOT_ITEM_LOG_ID = 3009;

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 対象毎に効果の適用
		List<int> targetList = range.targetCharacterList;
		for (int i = 0; i < targetList.Count; i++) {
			// キャラクターデータの取得
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// 所持アイテムを全て腐らせる
			LotItemList(target.characterData.possessItemList);
		}
		await UniTask.DelayFrame(5);
	}

	/// <summary>
	/// 引数のIDのアイテムを腐らせる
	/// </summary>
	/// <param name="itemIDList"></param>
	private void LotItemList(List<int> itemIDList) {
		if (CommonModule.IsEmpty(itemIDList)) return;

		for (int i = 0; i < itemIDList.Count; i++) {
			// アイテムデータ取得
			ItemObject item = ItemManager.instance.GetItem(itemIDList[i]);
			if (item == null) continue;
			// 変化前の名前をキャッシュ
			string beforeItemName = item.itemData.GetName();
			// 変化先のマスターデータ取得
			Entity_ItemData.Param lotItemMaster = MasterDataManager.instance.GetItemData(item.itemData.itemMaster.lotID);
			// 変化先がなければ終了
			if (lotItemMaster == null) continue;
			// アイテムマスターデータの変更
			item.SetMasterData(lotItemMaster);
			// ログ表示
			RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
			logMenu.AddLog(string.Format(_LOT_ITEM_LOG_ID.ToMessage(), beforeItemName, item.itemData.GetName()));
		}
	}
}
