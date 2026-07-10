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
	// 床落ちアイテムか否か
	public bool isFloorItem {
		get {
			if (itemData == null) return false;

			return itemData.posX >= 0 && itemData.posY >= 0;
		}
	}

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

	/// <summary>
	/// マスに置く
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare(SquareObject square) {
		// 見た目の処理
		SetPosition(square.GetObjectRoot().position);
		gameObject.SetActive(true);
		// 内部情報の処理
		itemData.SetSquare(square);
	}

	/// <summary>
	/// キャラの手持ちに加える
	/// </summary>
	public void SetCharacter(CharacterObject character) {
		// 見た目の処理
		gameObject.SetActive(false);
		// 内部情報の処理
		itemData.SetCharacter(character);
	}

	private void SetPosition(Vector3 position) {
		transform.position = position;
	}

	/// <summary>
	/// マスターデータ変更
	/// </summary>
	/// <param name="itemMaster"></param>
	public void SetMasterData(Entity_ItemData.Param itemMaster) {
		// 情報の反映
		itemData.SetMasterData(itemMaster);
	}

}
