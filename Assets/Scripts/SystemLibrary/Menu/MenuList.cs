/**
 * @file MenuList.cs
 * @brief リストメニュー
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public abstract class MenuList : MenuBase {
	[SerializeField]
	private MenuListItem _itemOrigin = null;

	[SerializeField]
	private Transform _contentRoot = null;

	[SerializeField]
	private Transform _unuseRoot = null;

	[SerializeField]
	private bool _isVertical = true;

	private static readonly int _MOVECURSOR_SEID = 11;
	private static readonly int _DECIDE_SEID = 12;
	private static readonly int _CANCEL_SEID = 13;

	/// <summary>
	/// リストメニューのコールバック集クラス
	/// </summary>
	public class MenuListCallbackFormat {
		public System.Func<MenuListItem, UniTask<bool>> OnDecide = null;
		public System.Func<MenuListItem, UniTask<bool>> OnCancel = null;
		public System.Func<MenuListItem, MenuListItem, UniTask<bool>> OnSetCursor = null;
	}
	private MenuListCallbackFormat _currentFormat = null;

	private int _currentIndex = -1;
	private bool _isContinue = false;

	List<MenuListItem> _useList = null;
	List<MenuListItem> _unuseItemList = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		_useList = new List<MenuListItem>();
		_unuseItemList = new List<MenuListItem>();
	}

	/// <summary>
	/// コールバック用クラス設定
	/// </summary>
	/// <param name="setFormat"></param>
	public void SetCallbackFormat( MenuListCallbackFormat setFormat ) {
		_currentFormat = setFormat;
	}

	/// <summary>
	/// 項目の追加
	/// </summary>
	/// <returns></returns>
	protected MenuListItem AddItem() {
		MenuListItem addItem;
		if (IsEmpty( _unuseItemList )) {
			addItem = Instantiate( _itemOrigin, _contentRoot );
		} else {
			addItem = _unuseItemList[0];
			_unuseItemList.RemoveAt( 0 );
			addItem.transform.SetParent( _contentRoot );
		}
		addItem.Deselect();
		_useList.Add( addItem );
		return addItem;
	}

	/// <summary>
	/// 全ての項目を取り除く
	/// </summary>
	protected void RemoveAllItem() {
		while (!IsEmpty( _useList )) {
			MenuListItem removeItem = _useList[0];
			removeItem.Deselect();
			removeItem.transform.SetParent( _unuseRoot );
			_unuseItemList.Add( removeItem );
			_useList.RemoveAt( 0 );
		}
	}

	/// <summary>
	/// 入力受付ループ
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		_isContinue = true;
		while (_isContinue) {
			// カーソル移動受付
			await AcceptMoveCursor();
			if (!_isContinue) break;
			// 決定受付
			await AcceptDecide();
			if (!_isContinue) break;
			// キャンセル受付
			await AcceptCancel();
			if (!_isContinue) break;

			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// カーソル移動受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptMoveCursor() {
		eDirectionFour inputDir = GetDirInput();
		if (inputDir == eDirectionFour.Invalid) return;

		int moveIndex = _currentIndex;
		// インデクス変更
		switch (inputDir) {
			case eDirectionFour.Up:
				if (_isVertical) moveIndex--;

				break;
			case eDirectionFour.Right:
				if (!_isVertical) moveIndex++;

				break;
			case eDirectionFour.Down:
				if (_isVertical) moveIndex++;

				break;
			case eDirectionFour.Left:
				if (!_isVertical) moveIndex--;

				break;
		}
		// インデックス修正
		if (moveIndex >= _useList.Count) moveIndex -= _useList.Count;

		if (moveIndex < 0) moveIndex += _useList.Count;

		await SetIndex( moveIndex );
	}

	/// <summary>
	/// 項目設定
	/// </summary>
	/// <param name="setIndex"></param>
	/// <returns></returns>
	protected async UniTask SetIndex( int setIndex ) {
		if (_currentIndex == setIndex) return;
		// 現在項目を未選択にする
		MenuListItem prevItem = null;
		if (IsEnableIndex( _useList, _currentIndex )) {
			_useList[_currentIndex].Deselect();
			prevItem = _useList[_currentIndex];
		}
		_currentIndex = setIndex;
		if (!IsEnableIndex( _useList, _currentIndex )) return;

		_useList[_currentIndex].Select();
		if (_currentFormat == null || _currentFormat.OnSetCursor == null) return;

		if (prevItem != null) {
			UniTask seTask = SoundManager.instance.PlaySE( _MOVECURSOR_SEID );
		}
		_isContinue = await _currentFormat.OnSetCursor( _useList[_currentIndex], prevItem );
	}

	/// <summary>
	/// 四方向の入力受付
	/// </summary>
	/// <returns></returns>
	private eDirectionFour GetDirInput() {
		if (Input.GetKeyDown( KeyCode.UpArrow )) {
			return eDirectionFour.Up;
		} else if (Input.GetKeyDown( KeyCode.RightArrow )) {
			return eDirectionFour.Right;
		} else if (Input.GetKeyDown( KeyCode.DownArrow )) {
			return eDirectionFour.Down;
		} else if (Input.GetKeyDown( KeyCode.LeftArrow )) {
			return eDirectionFour.Left;
		}
		return eDirectionFour.Invalid;
	}

	/// <summary>
	/// 決定入力受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptDecide() {
		if (!Input.GetKeyDown( KeyCode.Z )) return;

		if (_currentFormat == null ||
			_currentFormat.OnDecide == null) return;

		UniTask seTask = SoundManager.instance.PlaySE( _DECIDE_SEID );
		MenuListItem currentItem = IsEnableIndex( _useList, _currentIndex ) ? _useList[_currentIndex] : null;
		_isContinue = await _currentFormat.OnDecide( currentItem );
	}

	/// <summary>
	/// キャンセル入力受付
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptCancel() {
		if (!Input.GetKeyDown( KeyCode.X )) return;

		if (_currentFormat == null ||
			_currentFormat.OnCancel == null) return;

		UniTask seTask = SoundManager.instance.PlaySE( _CANCEL_SEID );
		MenuListItem currentItem = IsEnableIndex( _useList, _currentIndex ) ? _useList[_currentIndex] : null;
		_isContinue = await _currentFormat.OnCancel( currentItem );
	}

}
