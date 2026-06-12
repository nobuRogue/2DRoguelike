using UnityEngine;

/// <summary>
/// 8方向用1マス分移動データ
/// </summary>
public class ChebyshevMoveData {
	// 移動元のマスID
	public int sourceSquareID = -1;
	// 移動先のマスID
	public int targetSquareID = -1;
	// 移動方向
	public eDirectionEight dir = eDirectionEight.Invalid;

	public ChebyshevMoveData(int sourceSquareID, int targetSquareID, eDirectionEight dir) {
		this.sourceSquareID = sourceSquareID;
		this.targetSquareID = targetSquareID;
		this.dir = dir;
	}
}
