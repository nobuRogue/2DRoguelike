/**
 * @file UserDataHolder.cs
 * @brief ユーザデータ保持クラス
 * @author yaonobu
 * @date 2025/1/4
 */

public class UserDataHolder {
	public static UserData currentData { get; private set; } = null;

	public static void SetCurrentData( UserData setData ) {
		currentData = setData;
		MapSquareManager.SetGetSquareListProcess( currentData.GetSquareList );
		CharacterManager.SetCharacterProcess( currentData.GetPlayer, currentData.SetPlayer, currentData.GetEnemyList );
		ItemManager.SetItemProcess( currentData.GetitemList );
	}

}
