/**
 * @file MenuManager.cs
 * @brief メニュー管理
 * @author yaonobu
 * @date 2020/11/11
 */
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using static CommonModule;

/// <summary>
/// メニュー管理
/// </summary>
public class MenuManager : SystemObject {
	/// <summary>
	/// メニューの親
	/// </summary>
	[SerializeField]
	private Transform _menuRoot = null;
	private List<GameObject> _menuObjectList = null;

	/// <summary>
	/// インスタンスの参照
	/// </summary>
	private static MenuManager _instance = null;
	public static MenuManager instance {
		get {
			return _instance;
		}
	}

	public override async UniTask Initialize() {
		await base.Initialize();
		_instance = this;
	}

	/// <summary>
	/// メニューの取得
	/// なければ生成する
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <returns></returns>
	public T Get<T>( string name = null ) where T : MenuBase {
		if (IsEmpty( _menuObjectList )) _menuObjectList = new List<GameObject>();

		for (int i = 0, max = _menuObjectList.Count; i < max; i++) {
			T menu = _menuObjectList[i].GetComponent<T>();
			if (menu == null) continue;

			return menu;
		}
		// 見つからないので読み込む
		return Load<T>( name );
	}

	/// <summary>
	/// 読み込み
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <returns></returns>
	public T Load<T>( string name ) where T : MenuBase {
		var menuObject = Resources.Load( name ) as GameObject;
		if (menuObject == null) return null;

		var createObject = Instantiate( menuObject, _menuRoot );
		if (createObject == null) return null;

		T menu = createObject.GetComponent<T>();
		if (menu == null) return null;

		createObject.SetActive( false );
		if (_menuObjectList == null) _menuObjectList = new List<GameObject>();

		_menuObjectList.Add( createObject );
		return menu;
	}

	/// <summary>
	/// 読み込んだメニューオブジェクトの全消去
	/// </summary>
	public void UnloadAll() {
		if (IsEmpty( _menuObjectList )) return;

		for (int i = 0, max = _menuObjectList.Count; i < max; i++) {
			if (_menuObjectList[i] == null) continue;

			Destroy( _menuObjectList[i] );
		}
		_menuObjectList.Clear();
	}
}
