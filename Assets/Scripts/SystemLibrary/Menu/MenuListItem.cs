/**
 * @file ItemMenuList.cs
 * @brief リスト項目の基底クラス
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;
using UnityEngine.UI;

public abstract class MenuListItem : MonoBehaviour {
	[SerializeField]
	private Image _selectImage = null;

	/// <summary>
	/// カーソルが当てられたときの処理
	/// </summary>
	public virtual void Select() {
		_selectImage.enabled = true;
	}

	/// <summary>
	/// カーソルが外れたとき
	/// </summary>
	public virtual void Deselect() {
		_selectImage.enabled = false;
	}

}
