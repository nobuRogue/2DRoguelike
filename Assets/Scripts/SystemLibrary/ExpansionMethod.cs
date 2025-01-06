/**
 * @file ExpansionMethod.cs
 * @brief 拡張メソッド用のクラス
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpansionMethod {
	public static Vector2Int ToVectorPos( this Vector2Int vector2, eDirectionEight dir ) {
		switch (dir) {
			case eDirectionEight.Up:
				vector2.y++;
				break;
			case eDirectionEight.UpRight:
				vector2.x++;
				vector2.y++;
				break;
			case eDirectionEight.Right:
				vector2.x++;
				break;
			case eDirectionEight.DownRight:
				vector2.x++;
				vector2.y--;
				break;
			case eDirectionEight.Down:
				vector2.y--;
				break;
			case eDirectionEight.DownLeft:
				vector2.x--;
				vector2.y--;
				break;
			case eDirectionEight.Left:
				vector2.x--;
				break;
			case eDirectionEight.UpLeft:
				vector2.x--;
				vector2.y++;
				break;
		}
		return vector2;
	}

	public static Vector2Int ToVectorPos( this Vector2Int vector2, eDirectionFour dir ) {
		switch (dir) {
			case eDirectionFour.Up:
				vector2.y++;
				break;
			case eDirectionFour.Right:
				vector2.x++;
				break;
			case eDirectionFour.Down:
				vector2.y--;
				break;
			case eDirectionFour.Left:
				vector2.x--;
				break;
		}
		return vector2;
	}

	public static eDirectionEight ReverseDir( this eDirectionEight dir ) {
		int result = (int)dir + 4;
		if (result >= (int)eDirectionEight.Max) result -= (int)eDirectionEight.Max;

		return (eDirectionEight)result;
	}

	public static eDirectionFour ReverseDir( this eDirectionFour dir ) {
		int result = (int)dir + 2;
		if (result >= (int)eDirectionFour.Max) result -= (int)eDirectionFour.Max;

		return (eDirectionFour)result;
	}
}
