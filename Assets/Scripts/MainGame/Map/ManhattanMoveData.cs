using UnityEngine;

/// <summary>
/// 4方向用1マス分移動データ
/// </summary>
public class ManhattanMoveData {
	// 移動元のマスID
	public int sourceSquareID = -1;
	// 移動先のマスID
	public int targetSquareID = -1;
	// 移動方向
	eDirectionFour dir = eDirectionFour.Invalid;

	public ManhattanMoveData(int sourceSquareID, int targetSquareID, eDirectionFour dir) {
		this.sourceSquareID = sourceSquareID;
		this.targetSquareID = targetSquareID;
		this.dir = dir;
	}

}
