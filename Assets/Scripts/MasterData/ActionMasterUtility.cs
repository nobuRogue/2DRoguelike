/**
 * @file ActionMasterUtility.cs
 * @brief 
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMasterUtility {
	public static Entity_ActionData.Param GetActionMaster( int actionID ) {
		var actionMasterList = MasterDataManager.actionData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			var actionMaster = actionMasterList[i];
			if (actionMaster.ID != actionID) continue;

			return actionMaster;
		}
		return null;
	}
}
