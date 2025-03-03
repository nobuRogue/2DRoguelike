/**
 * @file MenuList.cs
 * @brief リストメニュー
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public abstract class MenuList : MenuBase {
	[SerializeField]
	private ItemMenuList _itemOrigin = null;

	[SerializeField]
	private Transform _contentRoot = null;

	[SerializeField]
	private Transform _unuseRoot = null;

	[SerializeField]
	private bool _isVertical = true;

	private int _currentIndex = -1;

	List<ItemMenuList> _itemList = null;
	List<ItemMenuList> _unuseItemList = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		_itemList = new List<ItemMenuList>();
		_unuseItemList = new List<ItemMenuList>();
	}

	/// <summary>
	/// 項目の追加
	/// </summary>
	/// <returns></returns>
	protected ItemMenuList AddItem() {
		ItemMenuList addItem;
		if (IsEmpty( _unuseItemList )) {
			addItem = Instantiate( _itemOrigin, _contentRoot );
		} else {
			addItem = _unuseItemList[0];
			_unuseItemList.RemoveAt( 0 );
			addItem.transform.SetParent( _contentRoot );
		}
		addItem.Deselect();
		_itemList.Add( addItem );
		return addItem;
	}

	/// <summary>
	/// 全ての項目を取り除く
	/// </summary>
	protected void RemoveAllItem() {
		while (!IsEmpty( _itemList )) {
			ItemMenuList removeItem = _itemList[0];
			removeItem.Deselect();
			removeItem.transform.SetParent( _unuseRoot );
			_unuseItemList.Add( removeItem );
			_itemList.RemoveAt( 0 );
		}
	}

	private void SelectItem() {

	}

	public async UniTask AcceptInput() {
		while (true) {

			await UniTask.DelayFrame( 1 );
		}
	}

	private void AcceptMoveCursor() {
		eDirectionFour inputDir = GetDirInput();
		if (inputDir == eDirectionFour.Invalid) return;

		switch (inputDir) {
			case eDirectionFour.Up:
				if (_isVertical) _currentIndex--;

				break;
			case eDirectionFour.Right:
				if (!_isVertical) _currentIndex++;

				break;
			case eDirectionFour.Down:
				if (_isVertical) _currentIndex++;

				break;
			case eDirectionFour.Left:
				if (!_isVertical) _currentIndex--;

				break;
		}
		//if (_currentIndex >= _)


	}

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

}
