/**
 * @file MenuBase.cs
 * @brief メニューの基底
 * @author yaonobu
 * @date 2020/11/11
 */
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using static CommonModule;

/// <summary>
/// メニューの基底
/// </summary>
public class MenuBase : MonoBehaviour {
	[SerializeField]
	private GameObject _menuRoot = null;

	public bool IsOpen() {
		return _menuRoot.activeInHierarchy;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Initialize() {

	}

	/// <summary>
	/// 開く
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Open() {
		_menuRoot.SetActive( true );
	}

	/// <summary>
	/// 閉じる
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Close() {
		_menuRoot.SetActive( false );
	}
}
