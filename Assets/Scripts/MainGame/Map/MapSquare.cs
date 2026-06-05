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
}
