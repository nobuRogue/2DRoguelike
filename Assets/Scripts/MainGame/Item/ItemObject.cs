/**
 * @file ItemObject.cs
 * @brief アイテムオブジェクト
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;

public class ItemObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _itemImage = null;

	private static readonly string _ITEM_ICON_SPRITE = "Design/Sprites/Item/itemIcons";

	public void Setup( Entity_ItemData.Param masterData ) {
		_itemImage.sprite = Resources.LoadAll<Sprite>( _ITEM_ICON_SPRITE )[masterData.category];
	}

	public void Teardown() {

	}

	public void SetPostion( Vector3 setPosition ) {
		// オブジェクトの座標を変更する処理
		transform.position = setPosition;
	}

	public void UnuseSelf( int ID ) {
		ItemManager.instance.UnuseObject( ID );
	}

}
