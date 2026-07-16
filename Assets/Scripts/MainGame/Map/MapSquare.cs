using UnityEngine;

/// <summary>
/// マップの1マス情報
/// </summary>
public class MapSquare {
	// 識別用のID
	public int ID { get; private set; } = -1;
	public int posX { get; private set; } = -1;
	public int posY { get; private set; } = -1;
	// 地形情報
	public eTerrain terrain { get; private set; } = eTerrain.Invalid;
	// 相互参照はダメ
	// 参照カウンタ式のGC（ガベージコレクション）が呼ばれなくなるから
	// ガベージコレクション：特定のアルゴリズムに則った自動的な参照の破棄
	// 参照カウンタ式のGC：参照されている個所をカウントしておく、参照されている数が0になったら解放される
	public int roomID { get; private set; } = -1;
	// マスに居るキャラクターのID
	public int characterID { get; private set; } = -1;
	// マスに置かれているオブジェクトの種類
	public eSqaureObjectType objectType { get; private set; } = eSqaureObjectType.Invalid;
	// マスにあるオブジェクトのID
	public int objectID { get; private set; } = -1;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="ID"></param>
	/// <param name="posX"></param>
	/// <param name="posY"></param>
	public MapSquare(int ID, int posX, int posY) {
		this.ID = ID;
		this.posX = posX;
		this.posY = posY;
	}

	/// <summary>
	/// 地形の変更
	/// </summary>
	/// <param name="setTerrain"></param>
	public void SetTerrain(eTerrain setTerrain) {
		terrain = setTerrain;
	}

	/// <summary>
	/// ルームIDのセッタ
	/// </summary>
	/// <param name="roomID"></param>
	public void SetRoomID(int roomID) {
		this.roomID = roomID;
	}

	/// <summary>
	/// マスに居るキャラクターの設定
	/// </summary>
	public void SetCharacter(int characterID) {
		this.characterID = characterID;
	}

	/// <summary>
	/// マスに居るキャラクターの削除
	/// </summary>
	public void RemoveCharacter() {
		characterID = -1;
	}

	/// <summary>
	/// マスにアイテムを設定
	/// </summary>
	/// <param name="itemID"></param>
	public void SetItem(int itemID) {
		objectType = eSqaureObjectType.Item;
		this.objectID = itemID;
	}

	/// <summary>
	/// マスにあるアイテムの削除
	/// </summary>
	public void RemoveObject() {
		objectType = eSqaureObjectType.Invalid;
		objectID = -1;
	}

	public void SetTrap(int trapID) {
		objectType = eSqaureObjectType.Trap;
		objectID = trapID;
	}

}
