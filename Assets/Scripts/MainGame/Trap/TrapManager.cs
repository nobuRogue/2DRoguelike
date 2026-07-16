using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering;

public class TrapManager : MonoBehaviour {

	public static TrapManager instance { get; private set; } = null;
	// 罠プレハブへの参照
	[SerializeField]
	private TrapObject _originObject = null;
	// 使用リスト
	private List<TrapObject> _useList = null;
	// 未使用リスト
	private List<TrapObject> _unuseList = null;
	// 初期化時に確保される罠オブジェクトキャッシュ
	private const int _TRAP_MAX = 64;
	// 罠スプライト配列
	private Sprite[] _trapSpriteList = null;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		instance = this;
		// 罠スプライトの読み込み
		_trapSpriteList = Resources.LoadAll<Sprite>("Design/Sprites/Trap/trap");
		// リストの初期化
		_useList = new List<TrapObject>(_TRAP_MAX);
		_unuseList = new List<TrapObject>(_TRAP_MAX);
		// いくつか罠オブジェクトを生成し未使用状態にしておく
		for (int i = 0; i < _TRAP_MAX; i++) {
			// オブジェクト生成
			TrapObject trap = Instantiate(_originObject, transform);
			// 初期化
			trap.Initilize();
			// 未使用リストに追加
			_unuseList.Add(trap);
		}
	}

	/// <summary>
	/// ID指定の罠取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public TrapObject GetTrap(int ID) {
		if (!CommonModule.IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// 罠の生成
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="square"></param>
	public void CreateTrap(int masterID, SquareObject square) {
		// 使用可能なインスタンスを取得
		TrapObject trap;
		if (CommonModule.IsEmpty(_unuseList)) {
			// 未使用リストが空なので生成する
			trap = Instantiate(_originObject, transform);
			trap.Initilize();
		}
		else {
			// 未使用リストにオブジェクトがあるのでそれを使う
			trap = _unuseList[0];
			_unuseList.RemoveAt(0);
		}
		// 使用リストに追加
		int useID = -1;
		for (int i = 0; i < _useList.Count; i++) {
			if (_useList[i] != null) continue;

			_useList[i] = trap;
			useID = i;
			break;
		}
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(trap);
		}
		trap.Setup(useID, square, masterID);
	}

	/// <summary>
	/// 罠の削除
	/// </summary>
	/// <param name="trap"></param>
	public void RemoveTrap(TrapObject trap) {
		// 使用リストから取り除く
		_useList[trap.trapData.ID] = null;
		// 片付け処理
		trap.Teardown();
		// 未使用リストに加える
		_unuseList.Add(trap);
	}

	/// <summary>
	/// 罠スプライト取得
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public Sprite GetTrapSprite(int index) {
		if (!CommonModule.IsEnableIndex(_trapSpriteList, index)) return null;

		return _trapSpriteList[index];
	}

	/// <summary>
	/// 全ての罠に対し、指定処理実行
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllTrap(System.Action<TrapObject> action) {
		if (CommonModule.IsEmpty(_useList) || action == null) return;

		for (int i = 0; i < _useList.Count; i++) {
			if (_useList[i] == null) continue;

			action(_useList[i]);
		}

	}

}
