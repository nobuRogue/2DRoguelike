/**
 * @file MessageMasterUtility.cs
 * @brief 
 * @author yaonobu
 * @date 2025/1/4
 */

public class MessageMasterUtility {
	public static string GetMasterMessage( int messageID ) {
		var actionMasterList = MasterDataManager.messageData[0];
		for (int i = 0, max = actionMasterList.Count; i < max; i++) {
			var messageMaster = actionMasterList[i];
			if (messageMaster.ID != messageID) continue;

			return messageMaster.Message;
		}
		return string.Empty;
	}
}
