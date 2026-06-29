using System.Collections.Generic;
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
		if (moveSquare.existCharacter) return false;
		// 地形の移動可否判定
		return CanMoveTerrain(startX, startY, moveSquare, moveDir);
	}

	/// <summary>
	/// 地形の移動可否判定
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="moveSquare"></param>
	/// <param name="moveDir"></param>
	/// <returns></returns>
	public bool CanMoveTerrain(int startX, int startY, SquareObject moveSquare, eDirectionEight moveDir) {
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

	/// <summary>
	/// 指定マスを起点にした視界の取得
	/// </summary>
	/// <param name="baseSquare"></param>
	/// <returns></returns>
	public List<SquareObject> GetVisibleArea(SquareObject baseSquare) {
		List<SquareObject> result = new List<SquareObject>();
		int dirMax = (int)eDirectionEight.Max;
		List<int> roomIDList = new List<int>(dirMax);
		// 周囲8マスを追加
		int baseX = baseSquare.squareData.posX, baseY = baseSquare.squareData.posY;
		for (int i = 0; i < dirMax; i++) {
			eDirectionEight dir = (eDirectionEight)i;
			SquareObject square = MapSquareManager.instance.GetToDirSquare(baseX, baseY, dir);
			if (square == null) continue;
			// 視界に追加
			result.Add(square);
			// 部屋マスならキャッシュしておく
			int roomID = square.squareData.roomID;
			if (roomID < 0) continue;
			// 既に追加済みの部屋なら処理しない
			if (roomIDList.Exists(element => element == roomID)) continue;
			// 部屋リストに追加
			roomIDList.Add(roomID);
		}
		// 部屋マスがあればその部屋内のマスを全て追加
		for (int i = 0; i < roomIDList.Count; i++) {
			RoomData room = MapSquareManager.instance.GetRoom(roomIDList[i]);
			if (room == null) continue;

			List<int> roomSquareList = room.squareIDList;
			for (int j = 0; j < roomSquareList.Count; j++) {
				SquareObject square = MapSquareManager.instance.GetSquare(roomSquareList[j]);
				if (square == null) continue;
				// 既に追加済みのマスなら追加しない
				if (result.Exists(element => element == square)) continue;
				// 視界に追加
				result.Add(square);
			}
		}
		return result;
	}

}
