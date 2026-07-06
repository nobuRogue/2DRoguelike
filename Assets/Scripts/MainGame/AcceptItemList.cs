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

	public AcceptItemList() {
		_itemListCallbackFormat = new ListMenu.ListMenuCallbackFormat();

		_itemListCallbackFormat.FreeAccept = AcceptSortItem;
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
		return false;
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

}
