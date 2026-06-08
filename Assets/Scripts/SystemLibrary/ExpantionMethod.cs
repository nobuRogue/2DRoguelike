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
}
