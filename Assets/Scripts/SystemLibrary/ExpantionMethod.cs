using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 拡張メソッド集
/// </summary>
public static class ExpantionMethod {
	/// <summary>
	/// 逆方向を取得
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static eDirectionFour ReverseDir(this eDirectionFour dir) {
		int result = (int)dir + 2;
		if (result >= (int)eDirectionFour.Max) result -= (int)eDirectionFour.Max;

		return (eDirectionFour)result;
	}

	/// <summary>
	/// 地形の移動可否判定
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public static bool CanMove(this eTerrain terrain) {
		return terrain != eTerrain.Wall;
	}

	/// <summary>
	/// 斜め方向か否か
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool IsSlant(this eDirectionEight dir) {
		switch (dir) {
			case eDirectionEight.UpRight:
			case eDirectionEight.DownRight:
			case eDirectionEight.DownLeft:
			case eDirectionEight.UpLeft:
				return true;
		}
		return false;
	}

	/// <summary>
	/// 斜め方向の分割
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static eDirectionFour[] Separate(this eDirectionEight dir) {
		eDirectionFour[] result = new eDirectionFour[2];
		switch (dir) {
			case eDirectionEight.UpRight:
				result[0] = eDirectionFour.Up;
				result[1] = eDirectionFour.Right;
				break;
			case eDirectionEight.DownRight:
				result[0] = eDirectionFour.Down;
				result[1] = eDirectionFour.Right;
				break;
			case eDirectionEight.DownLeft:
				result[0] = eDirectionFour.Down;
				result[1] = eDirectionFour.Left;
				break;
			case eDirectionEight.UpLeft:
				result[0] = eDirectionFour.Up;
				result[1] = eDirectionFour.Left;
				break;
		}
		return result;
	}

}
