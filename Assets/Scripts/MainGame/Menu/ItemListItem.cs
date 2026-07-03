using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテムリストの1項目
/// </summary>
public class ItemListItem : ListItem {
	// アイテムのアイコン画像
	[SerializeField]
	private Image _itemIconImage = null;
	// アイテム名テキスト
	[SerializeField]
	private TextMeshProUGUI _itemNameText = null;
	// 対応するアイテムのID
	public int itemID { get; private set; } = -1;

	public void Setup(int itemID) {
		this.itemID = itemID;
		// アイテムアイコンの設定
		ItemObject item = ItemManager.instance.GetItem(this.itemID);
		eItemCategory itemCategory = item.itemData.GetCategory();
		_itemIconImage.sprite = ItemManager.instance.GetItemCategorySprite(itemCategory);
		// アイテム名の設定
		_itemNameText.text = item.itemData.GetName();
	}
}
