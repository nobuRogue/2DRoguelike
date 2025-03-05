/**
 * @file ItemListItem.cs
 * @brief アイテムリストの項目
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemListItem : MenuListItem {
	[SerializeField]
	private Image _itemIconImage = null;

	[SerializeField]
	private TextMeshProUGUI _itemNameText = null;

	private static readonly string _ITEM_ICON_SPRITE = "Design/Sprites/Item/itemIcons";

	public void Setup( int itemID ) {
		var itemData = ItemManager.instance.GetItem( itemID );
		var itemMaster = ItemMasterUtility.GetItemMaster( itemData.masterID );

		_itemIconImage.sprite = Resources.LoadAll<Sprite>( _ITEM_ICON_SPRITE )[itemMaster.category];
		_itemNameText.text = itemMaster.nameID.ToMessage();
	}
}
