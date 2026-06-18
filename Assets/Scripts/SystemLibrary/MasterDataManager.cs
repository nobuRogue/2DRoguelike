using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// マスターデータ管理
/// </summary>
public class MasterDataManager {
	private static MasterDataManager _instance;

	public static MasterDataManager instance {
		get {
			if (_instance == null) _instance = new MasterDataManager();

			return _instance;
		}
	}
	// マスターデーターのファイルパス
	private const string _DATA_PATH = "MasterData/";

	// フロア情報
	private List<List<Entity_FloorData.Param>> _floorData = null;

	/// <summary>
	/// 全マスターデータ読み込み
	/// </summary>
	private MasterDataManager() {
		// フロア情報の読み込み
		_floorData = Load<Entity_FloorData, Entity_FloorData.Sheet, Entity_FloorData.Param>("FloorData");
	}

	/// <summary>
	/// マスターデータ読み込み
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <param name="dataName"></param>
	/// <returns></returns>
	private List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject {
		// ファイルを読み込む
		T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
		// 名称指定でシートを取得
		FieldInfo sheetField = typeof(T1).GetField("sheets");
		List<T2> sheetListData = sheetField.GetValue(sourceData) as List<T2>;

		// 名称指定で変数を取得
		FieldInfo listField = typeof(T2).GetField("list");
		List<List<T3>> paramList = new List<List<T3>>();
		foreach (object element in sheetListData) {
			List<T3> param = listField.GetValue(element) as List<T3>;
			paramList.Add(param);
		}
		return paramList;
	}

	/// <summary>
	/// フロアマスターデータ取得
	/// </summary>
	/// <param name="floorCount"></param>
	/// <returns></returns>
	public Entity_FloorData.Param GetFloorData(int floorCount) {
		List<Entity_FloorData.Param> dataList = _floorData[0];
		for (int i = 0; i < dataList.Count; i++) {
			if (dataList[i].floorCount != floorCount) continue;
			// データが一致するものを返す
			return dataList[i];
		}
		return null;
	}

}
