using System.Collections.Generic;
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

	// フロア情報
	List<List<Entity_FloorData.Param>> _floorData = null;

	/// <summary>
	/// 全マスターデータ読み込み
	/// </summary>
	private MasterDataManager() {
		// フロア情報の読み込み
		


	}



}
