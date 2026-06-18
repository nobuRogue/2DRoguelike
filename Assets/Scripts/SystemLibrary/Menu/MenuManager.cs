using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class MenuManager : SystemObject {
	public static MenuManager instance { get; private set; } = null;
	// 管理されているメニューのリスト
	private List<MenuBase> _menuList = null;
	// メニュープレハブのパス
	private const string _PREFAB_PATH = "Prefabs/Menu/";
	// ファイルパス指定用
	private StringBuilder _filePathBuilder = null;

	public override async UniTask Initialize() {
		instance = this;
		_menuList = new List<MenuBase>();
		_filePathBuilder = new StringBuilder();
		await UniTask.CompletedTask;
	}

	// メニューを取得
	// なければ生成する
	public T Get<T>(string name = null) where T : MenuBase {
		// キャッシュされているメニューリストから探す
		for (int i = 0; i < _menuList.Count; i++) {
			T menu = _menuList[i] as T;
			if (menu == null) continue;
			// メニューが見つかった
			return menu;
		}
		// なければ生成する
		return Load<T>(name);
	}

	/// <summary>
	/// メニュープレハブを読み込み生成
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="name"></param>
	/// <returns></returns>
	private T Load<T>(string name) where T : MenuBase {
		// メニュープレハブの読み込み
		_filePathBuilder.Clear();
		_filePathBuilder.Append(_PREFAB_PATH);
		_filePathBuilder.Append(name);
		T menu = Resources.Load<T>(_filePathBuilder.ToString());
		// プレハブの生成
		T createMenu = Instantiate(menu, transform);
		if (createMenu == null) return null;
		// 初期化してリストに追加
		createMenu.Initialize();
		_menuList.Add(createMenu);
		return createMenu;
	}

}
