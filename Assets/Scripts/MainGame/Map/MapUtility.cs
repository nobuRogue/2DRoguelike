using System.Data.SqlTypes;
using UnityEngine;

/// <summary>
/// マップおよびマスの関連処理
/// </summary>
public class MapUtility {

	private static MapUtility _instance = null;
	public static MapUtility instance {
		get {
			if (_instance == null) _instance = new MapUtility();

			return _instance;
		}
	}

	private MapUtility() {

	}

	/// <summary>
	/// 移動可否判定
	/// </summary>
	/// <returns></returns>
	public bool CanMove(int startX, int startY, SquareObject moveSquare, eDirectionEight moveDir) {
		// 移動先のマスにキャラが居たら移動不可
		if (moveSquare.squareData.characterID >= 0) return false;
		// 移動先のマスが壁地形なら移動不可
		if (!moveSquare.squareData.terrain.CanMove()) return false;
		// 斜め移動でなければ移動可能
		if (!moveDir.IsSlant()) return true;
		// 斜め移動なら、方向を分割し各方向の地形判定
		eDirectionFour[] separateDir = moveDir.Separate();
		for (int i = 0; i < separateDir.Length; i++) {
			SquareObject square = MapSquareManager.instance.GetToDirSquare(startX, startY, separateDir[i]);
			if (square == null) continue;
			// 移動可能なら継続
			if (square.squareData.terrain.CanMove()) continue;
			// 移動不可
			return false;
		}
		return true;
	}

}
