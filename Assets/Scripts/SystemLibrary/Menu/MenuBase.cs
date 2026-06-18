using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// メニューの基底
/// </summary>
public abstract class MenuBase : MonoBehaviour {
	[SerializeField]
	private GameObject _menuRoot = null;

	public virtual void Initialize() {
		// メニューを消す
		_menuRoot.SetActive(false);
	}

	/// <summary>
	/// メニュー表示
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Open() {
		// メニューを表示する
		_menuRoot.SetActive(true);
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// メニュー非表示
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Close() {
		// メニューを消す
		_menuRoot.SetActive(false);
		await UniTask.CompletedTask;
	}

}
