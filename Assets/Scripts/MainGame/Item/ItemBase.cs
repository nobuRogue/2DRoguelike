using UnityEngine;

/// <summary>
/// アイテムデータの基底
/// </summary>
public abstract class ItemBase {
	// 識別用のID
	public int ID { get; private set; } = -1;
	// アイテムのマス基準の位置
	public int posX { get; private set; } = -1;
	public int posY { get; private set; } = -1;
	// 所持キャラクターID
	public int possessCharacterID { get; private set; } = -1;
	// マスターデータ
	protected Entity_ItemData.Param _itemMaster = null;
	// カテゴリ取得
	public abstract eItemCategory GetCategory();
	/// <summary>
	/// 使用前準備
	/// </summary>
	/// <param name="ID"></param>
	public void Setup(int ID, Entity_ItemData.Param itemMaster) {
		this.ID = ID;
		_itemMaster = itemMaster;
	}
	/// <summary>
	/// 使用後片付け
	/// </summary>
	public void Teardown() {
		this.ID = -1;
	}

	/// <summary>
	/// 名前取得
	/// </summary>
	/// <returns></returns>
	public string GetName() {
		if (_itemMaster == null) return string.Empty;

		return _itemMaster.nameID.ToMessage();
	}

	/// <summary>
	/// マスにアイテムを配置
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare(SquareObject square) {
		// 現在の場所から取り除く
		RemoveCurrentPlace();
		// マスに設定する
		square.squareData.SetItem(ID);
		posX = square.squareData.posX;
		posY = square.squareData.posY;
	}

	/// <summary>
	/// 現在の場所から取り除く
	/// </summary>
	public void RemoveCurrentPlace() {
		SquareObject currentSquare = MapSquareManager.instance.GetSquare(posX, posY);
		if (currentSquare != null) {
			// マスから取り除く
			currentSquare.squareData.RemoveItem();
			posX = -1;
			posY = -1;
		}
		CharacterObject possessCharacter = CharacterManager.instance.GetCharacter(possessCharacterID);
		if (possessCharacter != null) {
			// キャラクターの手持ちから取り除く
			possessCharacter.characterData.RemoveItem(ID);
			possessCharacterID = -1;
		}
	}

}
