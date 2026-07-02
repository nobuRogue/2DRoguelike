using UnityEngine;

/// <summary>
/// アイテムオブジェクトの見た目情報
/// </summary>
public class ItemObject : MonoBehaviour {
	// アイテムの見た目スプライト
	[SerializeField]
	private SpriteRenderer _itemSprite = null;
	// アイテム情報
	public ItemBase itemData { get; private set; } = null;

	public void Initialize(ItemBase itemData, Sprite sprite) {
		this.itemData = itemData;
		// アイテムのスプライト設定
		_itemSprite.sprite = sprite;
	}

	public void Setup(int ID, Entity_ItemData.Param itemMaster) {
		itemData.Setup(ID, itemMaster);
	}

	public void Teardown() {
		itemData.Teardown();
		gameObject.SetActive(false);
	}

	public void SetSquare(SquareObject square) {
		// 見た目の処理
		SetPosition(square.GetObjectRoot().position);
		gameObject.SetActive(true);
		// 内部情報の処理
		itemData.SetSquare(square);
	}

	private void SetPosition(Vector3 position) {
		transform.position = position;
	}

}
