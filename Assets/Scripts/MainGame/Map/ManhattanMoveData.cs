/**
 * @file ManhattanMoveData.cs
 * @brief �}�b�v�����N���X
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManhattanMoveData {
	public int sourceSquareID = -1;
	public int moveSquareID = -1;
	public eDirectionFour dir = eDirectionFour.Invalid;

	public ManhattanMoveData( int setSourceSquareID, int setMoveSquareID, eDirectionFour setDir ) {
		sourceSquareID = setSourceSquareID;
		moveSquareID = setMoveSquareID;
		dir = setDir;
	}
}
