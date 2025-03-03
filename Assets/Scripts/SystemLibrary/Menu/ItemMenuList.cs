/**
 * @file ItemMenuList.cs
 * @brief リスト項目の基底クラス
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;
using UnityEngine.UI;

public abstract class ItemMenuList : MonoBehaviour {
	[SerializeField]
	private Image _selectImage = null;

	public virtual void Select() {
		_selectImage.enabled = true;
	}

	public virtual void Deselect() {
		_selectImage.enabled = false;
	}

}
