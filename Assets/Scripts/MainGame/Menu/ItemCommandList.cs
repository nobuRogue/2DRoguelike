using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// アイテムのコマンドリスト
/// </summary>
public class ItemCommandList : ListMenu {
	[SerializeField]
	private Transform _listRoot = null;

	public async UniTask Setup(eItemCategory itemCategory, ListMenuCallbackFormat format, Vector3 position) {
		// 表示位置設定
		_listRoot.position = position;
		// コールバックの設定
		SetCallbackFormat(format);
		// 全ての項目を削除
		RemoveAllItem();
		// 項目の追加
		AddItemCommand(itemCategory);
		// 0番目の項目を選択
		await SetIndex(0);
	}

	/// <summary>
	/// カテゴリに紐づいた項目の追加
	/// </summary>
	/// <param name="itemCategory"></param>
	private void AddItemCommand(eItemCategory itemCategory) {
		// アイテムカテゴリから表示するコマンド項目を追加
		ItemCommandItem addItem;
		switch (itemCategory) {
			case eItemCategory.Potion:
			case eItemCategory.Food:
			case eItemCategory.Wand:
			case eItemCategory.Scroll:
				// 使う項目の追加
				addItem = AddListItem() as ItemCommandItem;
				addItem.Setup(eItemCommand.Use);
				break;
			case eItemCategory.Bag:
				break;
			case eItemCategory.Throwing:
				break;
			case eItemCategory.Weapon:
			case eItemCategory.Armor:
				// プレイヤーの装備なら外す項目の追加

				// プレイヤーの装備でなければ装備項目の追加
				addItem = AddListItem() as ItemCommandItem;
				addItem.Setup(eItemCommand.Equip);
				break;
		}
		// 投げる項目の追加
		addItem = AddListItem() as ItemCommandItem;
		addItem.Setup(eItemCommand.Throw);
		// 置く項目の追加
		addItem = AddListItem() as ItemCommandItem;
		addItem.Setup(eItemCommand.Puton);

	}

}
