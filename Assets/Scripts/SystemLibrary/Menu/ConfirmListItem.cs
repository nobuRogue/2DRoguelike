using TMPro;
using UnityEngine;

public class ConfirmListItem : ListItem {
	// 項目テキストIDのオフセット
	private const int _ITEM_TEXT_ID_OFFSET = 10;

	// 項目テキスト
	[SerializeField]
	private TextMeshProUGUI _itemText = null;

	public eConfirmResult result { get; private set; } = eConfirmResult.Invalid;

	/// <summary>
	/// 使用前の準備
	/// </summary>
	/// <param name="result"></param>
	public void Setup(eConfirmResult result) {
		this.result = result;
		// テキストの設定
		_itemText.text = (_ITEM_TEXT_ID_OFFSET + (int)result).ToMessage();
	}


}
