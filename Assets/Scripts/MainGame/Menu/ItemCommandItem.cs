using TMPro;
using UnityEngine;

/// <summary>
/// アイテムコマンドリストの1項目
/// </summary>
public class ItemCommandItem : ListItem {
	// 表示テキスト
	[SerializeField]
	private TextMeshProUGUI _commandName = null;
	// この項目のコマンド
	public eItemCommand command { get; private set; } = eItemCommand.Invalid;
	// コマンド名メッセージID用オフセット
	private const int _COMMAND_NAME_OFFSET_ID = 20;

	public void Setup(eItemCommand command) {
		this.command = command;
		_commandName.text = (_COMMAND_NAME_OFFSET_ID + (int)this.command).ToMessage();
	}

}
