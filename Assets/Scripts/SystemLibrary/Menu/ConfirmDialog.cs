using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 確認ダイアログメニュー
/// </summary>
public class ConfirmDialog : ListMenu {
	[SerializeField]
	private TextMeshProUGUI _descriptionText = null;

	// 確認ダイアログの選択結果
	public eConfirmResult result { get; private set; } = eConfirmResult.Invalid;

	public override void Initialize() {
		base.Initialize();
		// はい、いいえ項目の生成
		ConfirmListItem yesItem = AddListItem() as ConfirmListItem;
		yesItem.Setup(eConfirmResult.Yes);
		ConfirmListItem noItem = AddListItem() as ConfirmListItem;
		noItem.Setup(eConfirmResult.No);

		// リスト決定時、キャンセル時のコールバック設定
		ListMenuCallbackFormat format = new ListMenuCallbackFormat();
		format.OnDecide = SetConfirmResult; // 決定時処理
		format.OnCancel = SetCancelResult;  // キャンセル時処理
		SetCallbackFormat(format);
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	/// <param name="descriptionText"></param>
	public async UniTask Setup(string descriptionText) {
		// 任意テキスト設定
		_descriptionText.text = descriptionText;
		// 選択結果のクリア
		result = eConfirmResult.Invalid;
		// 0番目の項目を選択
		await SetIndex(0);
	}

	/// <summary>
	/// 選択された項目で結果を確定する
	/// </summary>
	/// <param name="currentItem"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> SetConfirmResult(ListItem currentItem, CancellationToken ct) {
		// アップキャスト
		ConfirmListItem confirmItem = currentItem as ConfirmListItem;
		if (confirmItem == null) return false;
		// 結果を選択された項目にする
		result = confirmItem.result;
		await UniTask.CompletedTask;
		return false;
	}

	/// <summary>
	/// キャンセルで結果を確定する
	/// </summary>
	/// <param name="currentItem"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> SetCancelResult(ListItem currentItem, CancellationToken ct) {
		// 結果をキャンセルにする
		result = eConfirmResult.Cancel;
		await UniTask.CompletedTask;
		return false;
	}

}
