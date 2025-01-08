/**
 * @file FloorMasterUtility.cs
 * @brief 
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMasterUtility {
	public static Entity_FloorData.Param GetFloorMaster( int floorCount ) {
		var floorMasterList = MasterDataManager.floorData[0];
		for (int i = 0, max = floorMasterList.Count; i < max; i++) {
			var floorMaster = floorMasterList[i];
			if (floorMaster.floorCount != floorCount) continue;

			return floorMaster;
		}
		return null;
	}
}
