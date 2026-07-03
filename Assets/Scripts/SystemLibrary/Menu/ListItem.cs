using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リストの1項目
/// </summary>
public abstract class ListItem : MonoBehaviour {
	// 選択中にアクティブになる画像
	[SerializeField]
	private Image _selectImage = null;

	/// <summary>
	/// 項目選択時の処理
	/// </summary>
	public virtual void Select() {
		if (_selectImage == null) return;

		_selectImage.enabled = true;
	}

	/// <summary>
	/// 項目選択が外れた際の処理
	/// </summary>
	public virtual void Deselect() {
		if (_selectImage == null) return;

		_selectImage.enabled = false;
	}

}
