using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムリストメニュー
/// </summary>
public class ItemList : ListMenu {

	/// <summary>
	/// アイテムリスト項目の生成
	/// </summary>
	/// <returns></returns>
	public async UniTask Setup(List<int> itemIDList, ListMenuCallbackFormat format) {
		// コールバックの設定
		SetCallbackFormat(format);

		// すべての項目削除
		RemoveAllItem();
		// アイテムリストが空なら終了
		if (CommonModule.IsEmpty(itemIDList)) return;
		// 項目の生成
		for (int i = 0; i < itemIDList.Count; i++) {
			ItemListItem item = AddListItem() as ItemListItem;
			if (item == null) continue;

			item.Setup(itemIDList[i]);
		}
		// 0番目の項目を選択
		await SetIndex(0);
	}

}
