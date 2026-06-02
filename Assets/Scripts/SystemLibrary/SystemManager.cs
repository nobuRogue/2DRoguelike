using UnityEngine;

/// <summary>
/// ゲーム全体で使用される機能の管理
/// </summary>
public class SystemManager : MonoBehaviour {
	/// <summary>
	/// シリアライズ：直列化（何かを何かに変換する処理）
	/// </summary>
	[SerializeField]
	private SystemObject[] _systemObjectList = null;

	void Start() {

	}
}
