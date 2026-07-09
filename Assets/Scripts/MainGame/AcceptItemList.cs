using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using System.Threading;

/// <summary>
/// アイテムリストメニュー処理
/// </summary>
public class AcceptItemList {
	// アイテムリストの入力に対する処理のフォーマット
	ListMenu.ListMenuCallbackFormat _itemListCallbackFormat = null;
	// アイテムコマンドリストの入力に対する処理のフォーマット
	ListMenu.ListMenuCallbackFormat _commandListCallbackFormat = null;
	// 決定したアイテムID
	private int _decideItemID = -1;
	// 決定したコマンド
	private eItemCommand _decideCommand = eItemCommand.Invalid;
	// 決定SEのID
	private const int _DECIDE_SE_ID = 12;
	// キャンセルSEのID
	private const int _CANCEL_SE_ID = 13;

	// アイテムをマスに置くアクション
	private PutonAction _putonAction = null;
	public AcceptItemList() {
		_itemListCallbackFormat = new ListMenu.ListMenuCallbackFormat();
		_itemListCallbackFormat.OnDecide = DecideItemList;
		_itemListCallbackFormat.OnCancel = CancelItemList;
		_itemListCallbackFormat.FreeAccept = AcceptSortItem;

		_commandListCallbackFormat = new ListMenu.ListMenuCallbackFormat();
		_commandListCallbackFormat.OnDecide = DecideCommandList;
		_commandListCallbackFormat.OnCancel = CancelCommandList;

		_putonAction = new PutonAction();
	}

	/// <summary>
	/// アイテムリストメニュー受付
	/// </summary>
	/// <returns></returns>
	public async UniTask<bool> Accept() {
		// アイテムリスト入力受付
		ItemList itemList = MenuManager.instance.Get<ItemList>();
		CharacterObject player = CharacterManager.instance.GetPlayer();
		await itemList.Setup(player.characterData.possessItemList, _itemListCallbackFormat);
		await itemList.Open();
		await itemList.AcceptInput();
		await itemList.Close();
		// アイテムリストメニューの結果に応じた処理の実行
		return await ProcessItemListResult(player);
	}

	/// <summary>
	/// アイテムリストの結果処理
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> ProcessItemListResult(CharacterObject player) {
		if (_decideCommand == eItemCommand.Invalid ||
			_decideItemID < 0) return false;

		ItemObject item = ItemManager.instance.GetItem(_decideItemID);
		Debug.Log(item.itemData.GetName() + "に" + _decideCommand + "を実行");
		switch (_decideCommand) {
			case eItemCommand.Use:
				// アイテムを使う処理実行
				await ActionManager.instance.UseItem(player, item);
				break;
			case eItemCommand.Puton:
				// アイテムを置く処理実行
				SquareObject playerSquare = MapSquareManager.instance.GetSquare(player.characterData.posX, player.characterData.posY);
				await _putonAction.ExecutePuton(playerSquare, _decideItemID);
				break;
			case eItemCommand.Equip:
				// アイテムの装備処理実行
				break;
			case eItemCommand.Remove:
				// アイテムを外す処理実行
				break;
		}
		// 決定内容をクリア
		_decideCommand = eItemCommand.Invalid;
		_decideItemID = -1;
		return true;
	}

	/// <summary>
	/// ソート入力の受付、処理
	/// </summary>
	/// <param name="currentItem"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> AcceptSortItem(ListItem currentItem, CancellationToken ct) {
		// Vキー入力判定
		if (!Input.GetKeyDown(KeyCode.V)) return true;
		// プレイヤーの所持アイテムソート
		CharacterObject player = CharacterManager.instance.GetPlayer();
		player.characterData.possessItemList.Sort(ItemSortMethod);
		// アイテムリストのセットアップ
		ItemList itemList = MenuManager.instance.Get<ItemList>();
		await itemList.Setup(player.characterData.possessItemList, _itemListCallbackFormat);
		return true;
	}

	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="itemID1"></param>
	/// <param name="itemID2"></param>
	/// <returns></returns>
	private int ItemSortMethod(int itemID1, int itemID2) {
		ItemObject item1 = ItemManager.instance.GetItem(itemID1);
		ItemObject item2 = ItemManager.instance.GetItem(itemID2);
		return item1.itemData.itemMaster.ID - item2.itemData.itemMaster.ID;
	}

	/// <summary>
	/// キャンセルSEを再生
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> CancelItemList(ListItem item, CancellationToken ct) {
		await UniTask.CompletedTask;
		UniTask task = SoundManager.instance.PlaySE(_CANCEL_SE_ID);
		return false;
	}

	/// <summary>
	/// アイテムコマンドリストの入力の受付
	/// </summary>
	/// <param name="item"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> DecideItemList(ListItem item, CancellationToken ct) {
		var itemListItem = item as ItemListItem;
		if (itemListItem == null) return false;
		// 決定SEの再生
		UniTask task = SoundManager.instance.PlaySE(_DECIDE_SE_ID);
		// 選択アイテムIDの設定
		_decideItemID = itemListItem.itemID;
		ItemObject itemObject = ItemManager.instance.GetItem(_decideItemID);
		// アイテムコマンドリストの入力受付
		ItemCommandList commandList = MenuManager.instance.Get<ItemCommandList>();
		await commandList.Setup(itemObject.itemData.GetCategory(), _commandListCallbackFormat, itemListItem.GetCommandRoot());
		// 直後のリスト選択で決定入力が行われないように1フレーム待機
		await UniTask.DelayFrame(1, cancellationToken: ct);
		await commandList.Open();
		await commandList.AcceptInput();
		await commandList.Close();
		// 直後のリスト選択でキャンセル入力が行われないように1フレーム待機
		await UniTask.DelayFrame(1, cancellationToken: ct);
		// アイテムコマンドリストでキャンセルされたら継続、決定されたら終了
		return _decideCommand == eItemCommand.Invalid;
	}

	/// <summary>
	/// コマンドリスト決定時処理
	/// </summary>
	/// <param name="item"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> DecideCommandList(ListItem item, CancellationToken ct) {
		// 決定SE再生
		UniTask task = SoundManager.instance.PlaySE(_DECIDE_SE_ID);
		// 選択コマンドを現在の項目のコマンドに設定
		var commandItem = item as ItemCommandItem;
		if (commandItem == null) return false;

		_decideCommand = commandItem.command;
		await UniTask.CompletedTask;
		return false;
	}

	/// <summary>
	/// コマンドリストキャンセル時処理
	/// </summary>
	/// <param name="item"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	private async UniTask<bool> CancelCommandList(ListItem item, CancellationToken ct) {
		// キャンセルSE再生
		UniTask task = SoundManager.instance.PlaySE(_CANCEL_SE_ID);
		// 選択アイテムIDをクリア
		_decideItemID = -1;
		await UniTask.CompletedTask;
		return false;
	}

}
