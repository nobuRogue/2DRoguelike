/**
 * @file ItemMenuList.cs
 * @brief ���X�g���ڂ̊��N���X
 * @author yaonobu
 * @date 2025/1/4
 */

using UnityEngine;
using UnityEngine.UI;

public abstract class MenuListItem : MonoBehaviour {
	[SerializeField]
	private Image _selectImage = null;

	/// <summary>
	/// �J�[�\�������Ă�ꂽ�Ƃ��̏���
	/// </summary>
	public virtual void Select() {
		_selectImage.enabled = true;
	}

	/// <summary>
	/// �J�[�\�����O�ꂽ�Ƃ�
	/// </summary>
	public virtual void Deselect() {
		_selectImage.enabled = false;
	}

}
