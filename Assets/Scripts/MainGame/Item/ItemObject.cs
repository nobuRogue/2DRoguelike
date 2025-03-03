/**
 * @file ItemObject.cs
 * @brief �A�C�e���I�u�W�F�N�g
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;

public class ItemObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _itemImage = null;

	private static readonly string ITEM_ICON_SPRITE = "Design/Sprites/Item/itemIcons";

	public void Setup( Entity_ItemData.Param masterData ) {
		_itemImage.sprite = Resources.LoadAll<Sprite>( ITEM_ICON_SPRITE )[masterData.category];
	}

	public void Teardown() {

	}

	public void SetPostion( Vector3 setPosition ) {
		// �I�u�W�F�N�g�̍��W��ύX���鏈��
		transform.position = setPosition;
	}
}
