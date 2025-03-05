/**
 * @file MenuList.cs
 * @brief ���X�g���j���[
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
	/// ���X�g���j���[�̃R�[���o�b�N�W�N���X
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
	/// �R�[���o�b�N�p�N���X�ݒ�
	/// </summary>
	/// <param name="setFormat"></param>
	public void SetCallbackFormat( MenuListCallbackFormat setFormat ) {
		_currentFormat = setFormat;
	}

	/// <summary>
	/// ���ڂ̒ǉ�
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
	/// �S�Ă̍��ڂ���菜��
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
	/// ���͎�t���[�v
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		_isContinue = true;
		while (_isContinue) {
			// �J�[�\���ړ���t
			await AcceptMoveCursor();
			if (!_isContinue) break;
			// �����t
			await AcceptDecide();
			if (!_isContinue) break;
			// �L�����Z����t
			await AcceptCancel();
			if (!_isContinue) break;

			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// �J�[�\���ړ���t
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptMoveCursor() {
		eDirectionFour inputDir = GetDirInput();
		if (inputDir == eDirectionFour.Invalid) return;

		int moveIndex = _currentIndex;
		// �C���f�N�X�ύX
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
		// �C���f�b�N�X�C��
		if (moveIndex >= _useList.Count) moveIndex -= _useList.Count;

		if (moveIndex < 0) moveIndex += _useList.Count;

		await SetIndex( moveIndex );
	}

	/// <summary>
	/// ���ڐݒ�
	/// </summary>
	/// <param name="setIndex"></param>
	/// <returns></returns>
	protected async UniTask SetIndex( int setIndex ) {
		if (_currentIndex == setIndex) return;
		// ���ݍ��ڂ𖢑I���ɂ���
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
	/// �l�����̓��͎�t
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
	/// ������͎�t
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
	/// �L�����Z�����͎�t
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
