/**
 * @file AutoCreateManager.cs
 * @brief システムオブジェクト管理
 * @author yaonobu
 * @date 2021/4/5
 */
using UnityEngine;
using Cysharp.Threading.Tasks;

using static CommonModule;


public class SystemManager : MonoBehaviour {
	[SerializeField]
	private SystemObject[] _systemObjectList = null;

	[SerializeField]
	private SystemObject[] _debugObjectList = null;

	void Start() {
		UniTask task = Initialize();
	}

	private async UniTask Initialize() {
		await InitializeSystemObject();
#if DEBUG
		await InitializeDebugObject();
#endif
		PartManager.instance.Execute();
	}

	private async UniTask InitializeSystemObject() {
		if (IsEmpty( _systemObjectList )) return;

		for (int i = 0, max = _systemObjectList.Length; i < max; i++) {
			var objOrigin = _systemObjectList[i];
			if (objOrigin == null) continue;

			var createObj = Instantiate( objOrigin, transform );
			await createObj.Initialize();
		}
	}

	private async UniTask InitializeDebugObject() {
		if (IsEmpty( _debugObjectList )) return;

		for (int i = 0, max = _debugObjectList.Length; i < max; i++) {
			var baseObj = _debugObjectList[i];
			if (baseObj == null) continue;

			var createObj = Instantiate( baseObj, transform );
			await createObj.Initialize();
		}
	}

}
