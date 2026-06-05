using Cysharp.Threading.Tasks;
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
		Initialize();
	}

	private async UniTask Initialize() {
		// 各システムオブジェクトの生成
		for (int i = 0; i < _systemObjectList.Length; i++) {
			SystemObject origin = _systemObjectList[i];
			if (origin == null) continue;
			// オブジェクトの生成
			SystemObject createObj = Instantiate(origin, transform);
			// 初期化処理
			await createObj.Initialize();
		}
		// タイトルパートの実行
		UniTask task = PartManager.instance.TransitionPart(eGamePart.Title);
	}

}
