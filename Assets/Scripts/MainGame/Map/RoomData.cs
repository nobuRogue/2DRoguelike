using System.Collections.Generic;
using UnityEngine;

public class RoomData {
	// 識別用のID
	public int ID { get; private set; } = -1;

	// 部屋のマスのIDリスト
	public List<int> squareIDList { get; private set; } = null;

	/// <summary>
	/// 使用前の準備
	/// </summary>
	public void Setup(int ID, List<int> squareIDList) {
		this.ID = ID;
		this.squareIDList = squareIDList;
		if (CommonModule.IsEmpty(this.squareIDList)) return;
		// 全ての部屋マスに部屋ID設定
		for (int i = 0; i < this.squareIDList.Count; i++) {
			SquareObject square = MapSquareManager.instance.GetSquare(this.squareIDList[i]);
			if (square == null) continue;

			square.squareData.SetRoomID(ID);
		}
	}

	/// <summary>
	/// 使用後の片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
		if (CommonModule.IsEmpty(this.squareIDList)) return;
		// 全ての部屋マスの部屋IDをクリア
		for (int i = 0; i < this.squareIDList.Count; i++) {
			SquareObject square = MapSquareManager.instance.GetSquare(this.squareIDList[i]);
			if (square == null) continue;

			square.squareData.SetRoomID(-1);
		}
		squareIDList = null;
	}

}
