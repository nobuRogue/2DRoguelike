using UnityEngine;

public class UserDataHolder {
	private static UserDataHolder _instance = null;

	public static UserDataHolder instance {
		get {
			if (_instance == null) _instance = new UserDataHolder();

			return _instance;
		}
	}
	/// <summary>
	/// 現在のユーザデータ
	/// </summary>
	public UserData currentData { get; private set; }

	private UserDataHolder() {
		currentData = new UserData();
	}
}
