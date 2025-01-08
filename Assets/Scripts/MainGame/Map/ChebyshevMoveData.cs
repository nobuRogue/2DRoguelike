/**
 * @file ChebyshevMoveData.cs
 * @brief 8•ûŒü‚ÌˆÚ“®î•ñ
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChebyshevMoveData {
	public int sourceSquareID = -1;
	public int moveSquareID = -1;
	public eDirectionEight dir = eDirectionEight.Invalid;

	public ChebyshevMoveData( int setSourceSquareID, int setMoveSquareID, eDirectionEight setDir ) {
		sourceSquareID = setSourceSquareID;
		moveSquareID = setMoveSquareID;
		dir = setDir;
	}
}
