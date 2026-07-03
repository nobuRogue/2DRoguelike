using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/// <summary>
/// リストメニューの基底
/// </summary>
public abstract class ListMenu : MenuBase {
	// リスト項目のオリジナル
	[SerializeField]
	private ListItem _itemOrigin = null;
	// 表示項目の親オブジェクト
	[SerializeField]
	private Transform _contentRoot = null;
	// 未使用項目の親オブジェクト
	[SerializeField]
	private Transform _unuseRoot = null;

	// プール用リスト
	private List<ListItem> _useList = null;
	private List<ListItem> _unuseList = null;

	// 現在の選択項目インデクス
	private int _currentIndex = -1;

	/// <summary>
	/// リストメニュー用コールバック集クラス
	/// </summary>
	public class ListMenuCallbackFormat {
		// 決定時処理
		public System.Func<ListItem, CancellationToken, UniTask<bool>> OnDecide = null;
		// キャンセル時処理
		public System.Func<ListItem, CancellationToken, UniTask<bool>> OnCancel = null;
		// カーソル移動時処理
		public System.Func<ListItem, ListItem, CancellationToken, UniTask> OnMoveCursor = null;
		// 自由な受付処理
		public System.Func<ListItem, CancellationToken, UniTask<bool>> FreeAccept = null;
	}
	// 現在のコールバックフォーマット
	private ListMenuCallbackFormat _currentFormat = null;
	// 入力受付タスク中断用トークン
	private CancellationToken _ct;

	public override void Initialize() {
		base.Initialize();
		_useList = new List<ListItem>();
		_unuseList = new List<ListItem>();
		// オブジェクト破棄時の中断用トークン取得
		_ct = gameObject.GetCancellationTokenOnDestroy();
	}

	/// <summary>
	/// コールバックフォーマットの設定
	/// </summary>
	/// <param name="format"></param>
	public void SetCallbackFormat(ListMenuCallbackFormat format) {
		_currentFormat = format;
	}

	/// <summary>
	/// リスト項目追加
	/// </summary>
	/// <returns></returns>
	public ListItem AddListItem() {
		ListItem addItem;
		if (CommonModule.IsEmpty(_unuseList)) {
			// 未使用リストが空なので生成
			addItem = Instantiate(_itemOrigin, _contentRoot);
		}
		else {
			// 未使用リストから使う
			addItem = _unuseList[0];
			_unuseList.RemoveAt(0);
			addItem.transform.SetParent(_contentRoot);
		}
		// 使用リストに追加
		_useList.Add(addItem);
		addItem.Deselect();
		return addItem;
	}

	/// <summary>
	/// リスト項目削除
	/// </summary>
	/// <param name="removeIndex"></param>
	public void RemoveListItem(int removeIndex) {
		if (!CommonModule.IsEnableIndex(_useList, removeIndex)) return;
		// 使用リストから取り除く
		ListItem removeItem = _useList[removeIndex];
		_useList.RemoveAt(removeIndex);
		// 未使用リストへ追加
		_unuseList.Add(removeItem);
		removeItem.transform.SetParent(_unuseRoot);
		removeItem.Deselect();
	}
	/// <summary>
	/// すべてのリスト項目の削除
	/// </summary>
	public void RemoveAllItem() {
		// 使用リストが空になるまで0番目の要素を削除
		while (!CommonModule.IsEmpty(_useList)) RemoveListItem(0);
	}

	/// <summary>
	/// リスト入力の受付タスク
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		while (true) {
			// カーソル移動の受付
			await AcceptMoveCursor();
			// 決定入力の受付
			if (!await AcceptDecide()) break;
			// キャンセル入力の受付
			if (!await AcceptCancel()) break;
			// 自由な入力の受付
			if (!await AcceptFree()) break;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _ct);
		}
	}

	private async UniTask AcceptMoveCursor() {
		// 四方向の入力受付
		eDirectionFour dirInput = GetDirInput();
		if (dirInput == eDirectionFour.Invalid) return;
		// 入力に応じたインデクスの決定
		int moveIndex = _currentIndex;
		switch (dirInput) {
			case eDirectionFour.Up:
				moveIndex--;
				break;
			case eDirectionFour.Down:
				moveIndex++;
				break;
		}
		// 移動後のインデクスがリスト項目に収まるように修正
		if (moveIndex < 0) moveIndex = _useList.Count - 1;
		if (moveIndex >= _useList.Count) moveIndex = 0;
		// カーソル移動
		await SetIndex(moveIndex);
	}

	/// <summary>
	/// インデクス変更、カーソル移動
	/// </summary>
	/// <returns></returns>
	public async UniTask SetIndex(int nextIndex) {
		// 現在の項目を未選択状態にする
		ListItem prevItem = null;
		if (CommonModule.IsEnableIndex(_useList, _currentIndex)) {
			prevItem = _useList[_currentIndex];
			prevItem.Deselect();
		}
		_currentIndex = nextIndex;
		// 移動後の項目を選択状態にする
		ListItem currentItem = null;
		if (CommonModule.IsEnableIndex(_useList, _currentIndex)) {
			currentItem = _useList[_currentIndex];
			currentItem.Select();
		}
		// カーソル移動コールバックの実行
		if (_currentFormat == null ||
			_currentFormat.OnMoveCursor == null) return;

		await _currentFormat.OnMoveCursor(prevItem, currentItem, _ct);
	}

	/// <summary>
	/// 四方向入力の受付
	/// </summary>
	/// <returns></returns>
	private eDirectionFour GetDirInput() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) return eDirectionFour.Up;
		if (Input.GetKeyDown(KeyCode.RightArrow)) return eDirectionFour.Right;
		if (Input.GetKeyDown(KeyCode.DownArrow)) return eDirectionFour.Down;
		if (Input.GetKeyDown(KeyCode.LeftArrow)) return eDirectionFour.Left;

		return eDirectionFour.Invalid;
	}

	/// <summary>
	/// 決定入力の受付
	/// </summary>
	/// <returns>入力受付の継続可否</returns>
	private async UniTask<bool> AcceptDecide() {
		// 決定入力の受付
		if (!Input.GetKeyDown(KeyCode.Z)) return true;
		// 決定時のコールバック実行
		if (_currentFormat == null ||
			_currentFormat.OnDecide == null) return true;

		return await _currentFormat.OnDecide(GetCurrentItem(), _ct);
	}
	/// <summary>
	/// キャンセル入力の受付
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptCancel() {
		// キャンセル入力の受付
		if (!Input.GetKeyDown(KeyCode.X)) return true;
		// キャンセル時のコールバック実行
		if (_currentFormat == null ||
			_currentFormat.OnCancel == null) return false;

		return await _currentFormat.OnCancel(GetCurrentItem(), _ct);
	}
	/// <summary>
	/// 自由な入力受付の処理
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptFree() {
		// 自由受付のコールバック実行
		if (_currentFormat == null ||
			_currentFormat.FreeAccept == null) return true;

		return await _currentFormat.FreeAccept(GetCurrentItem(), _ct);
	}

	/// <summary>
	/// 現在の項目取得
	/// </summary>
	/// <returns></returns>
	private ListItem GetCurrentItem() {
		if (!CommonModule.IsEnableIndex(_useList, _currentIndex)) return null;

		return _useList[_currentIndex];
	}

}
