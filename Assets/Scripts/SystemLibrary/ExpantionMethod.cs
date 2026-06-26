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
	/// 地形の射程対象可否判定
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public static bool CanRangeTarget(this eTerrain terrain) {
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

	/// <summary>
	/// ループアニメーションか判定
	/// </summary>
	/// <param name="animation"></param>
	/// <returns></returns>
	public static bool IsLoopAnimation(this eCharacterAnimation animation) {
		return animation != eCharacterAnimation.Attack && animation != eCharacterAnimation.Damage;
	}

	/// <summary>
	/// ダンジョン終了要因からフロア終了要因の取得
	/// </summary>
	/// <param name="reason"></param>
	/// <returns></returns>
	public static eFloorEndReason GetFloorEndReason(this eDungeonEndReason reason) {
		switch (reason) {
			case eDungeonEndReason.GameOver:
				return eFloorEndReason.GameOver;
			case eDungeonEndReason.Clear:
				return eFloorEndReason.Stair;
		}
		return eFloorEndReason.Invalid;
	}

	/// <summary>
	/// メッセージ取得
	/// </summary>
	/// <param name="messageID"></param>
	/// <returns></returns>
	public static string ToMessage(this int messageID) {
		return MasterDataManager.instance.GetMessage(messageID);
	}

	/// <summary>
	/// 整数を8方向列挙子に変換
	/// </summary>
	/// <param name="dirIndex"></param>
	/// <returns></returns>
	public static eDirectionEight ToDir8(this int dirIndex) {
		int maxIndex = (int)eDirectionEight.Max;
		// 最小値チェック
		while (dirIndex < 0) dirIndex += maxIndex;
		// 最大値チェック
		while (dirIndex >= maxIndex) dirIndex -= maxIndex;

		return (eDirectionEight)dirIndex;
	}

}
