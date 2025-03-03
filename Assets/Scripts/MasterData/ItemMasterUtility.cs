/**
 * @file ItemMasterUtility.cs
 * @brief 
 * @author yaonobu
 * @date 2025/1/4
 */

public class ItemMasterUtility {
	public static Entity_ItemData.Param GetItemMaster( int itemID ) {
		var actionMasterList = MasterDataManager.itemData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			var itemMaster = actionMasterList[i];
			if (itemMaster.ID != itemID) continue;

			return itemMaster;
		}
		return null;
	}
}
