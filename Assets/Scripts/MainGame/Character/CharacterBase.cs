using UnityEngine;

/// <summary>
/// ゲームキャラクターの基底
/// </summary>
public abstract class CharacterBase {
	// 識別用のID
	public int ID { get; private set; } = -1;
	// キャラのマス基準の位置
	public int posX { get; private set; } = -1;
	public int posY { get; private set; } = -1;
	// 向き
	public eDirectionEight direction { get; private set; } = eDirectionEight.Invalid;

	/// <summary>
	/// 使用間準備
	/// </summary>
	/// <param name="ID"></param>
	public void Setup(int ID) {
		this.ID = ID;
	}

	/// <summary>
	/// 使用後片付け
	/// </summary>
	public void Teardown() {
		this.ID = -1;
	}

	/// <summary>
	/// マスにキャラを置く
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare(SquareObject square) {
		if (square == null) return;
		// 現在のマスから取り除く
		SquareObject current = MapSquareManager.instance.GetSquare(posX, posY);
		if (current != null) current.squareData.RemoveCharacter();
		// 座標の変更
		posX = square.squareData.posX;
		posY = square.squareData.posY;
		// マスにキャラクターIDを設定
		square.squareData.SetCharacter(ID);
	}

}
