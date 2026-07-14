using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 射程の管理クラス
/// </summary>
public class ActionRangeManager {
	private static ActionRangeManager _instance;

	public static ActionRangeManager instance {
		get {
			if (_instance == null) _instance = new ActionRangeManager();

			return _instance;
		}
	}

	// 射程リスト
	private List<ActionRangeBase> _rangeList = null;

	private ActionRangeManager() {
		// 使用する射程をすべてキャッシュしておく
		_rangeList = new List<ActionRangeBase>();
		_rangeList.Add(new ActionRange00_DirForward());
		_rangeList.Add(new ActionRange01_Self());
		_rangeList.Add(new ActionRange02_Shoot());
		_rangeList.Add(new ActionRange03_RoomAll());
	}

	/// <summary>
	/// 射程取得
	/// </summary>
	/// <param name="rangeType"></param>
	/// <returns></returns>
	public ActionRangeBase GetRange(int rangeType) {
		if (!CommonModule.IsEnableIndex(_rangeList, rangeType)) return null;

		return _rangeList[rangeType];
	}

}
