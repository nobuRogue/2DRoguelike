using UnityEngine;

/// <summary>
/// 罠の内部情報
/// </summary>
public class TrapData {
	// 識別用のID
	public int ID { get; private set; } = -1;
	// マス基準の位置
	public int posX { get; private set; } = -1;
	public int posY { get; private set; } = -1;
	// マスターデータへの参照
	public Entity_TrapData.Param trapMaster { get; private set; } = null;

	public void Setup(int ID, SquareObject square, Entity_TrapData.Param masterData) {
		this.ID = ID;
		trapMaster = masterData;
		// マスに設置する
		square.squareData.SetTrap(ID);
		posX = square.squareData.posX;
		posY = square.squareData.posY;
	}

	public void Teardown() {
		ID = -1;
		SquareObject square = MapSquareManager.instance.GetSquare(posX, posY);
		// マスから取り除く
		square.squareData.RemoveObject();
		posX = -1;
		posY = -1;
	}

}
