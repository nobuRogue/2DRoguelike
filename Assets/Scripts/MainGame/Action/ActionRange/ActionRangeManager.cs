/**
 * @file ActionRangeManager.cs
 * @brief ÉvÉåÉCÉÑÅ[Ç…ãﬂÇ√Ç¢Çƒâ£ÇÈAI
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ActionRangeManager {
	private static List<ActionRangeBase> _actionRangeList = null;

	public static void Initialize() {
		_actionRangeList = new List<ActionRangeBase>();
		_actionRangeList.Add( new ActionRange00_DirForward() );
	}

	public static ActionRangeBase GetRange( int rangeType ) {
		if (!IsEnableIndex( _actionRangeList, rangeType )) return null;

		return _actionRangeList[rangeType].GetInstance();
	}
}
