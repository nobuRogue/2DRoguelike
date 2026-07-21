using Cysharp.Threading.Tasks;
using System.Data;
using UnityEngine;

/// <summary>
/// マスにアイテムを置くアクション
/// </summary>
public class PutonAction {
	// 置けなかった際のログID
	private const int _CANNOT_PUTON_LOG_ID = 3003;
	// 置いた際のログID
	private const int _PUTON_LOG_ID = 3004;

	/// <summary>
	/// アイテムをマスに置く処理実行
	/// </summary>
	/// <param name="putonSquare"></param>
	/// <param name="itemID"></param>
	/// <returns></returns>
	public async UniTask ExecutePuton(SquareObject putonSquare, int itemID) {
		// マスにアイテムが置けるか判定
		if (putonSquare.existObject) {
			// 置けない場合、ログを表示して終了
			MenuManager.instance.Get<RogueLogMenu>().AddLog(_CANNOT_PUTON_LOG_ID.ToMessage());
			return;
		}
		// マスにアイテムを置く
		ItemObject putonItem = ItemManager.instance.GetItem(itemID);
		putonItem.SetSquare(putonSquare);
		// ログの表示
		string putonLog = string.Format(_PUTON_LOG_ID.ToMessage(), putonItem.itemData.GetName());
		MenuManager.instance.Get<RogueLogMenu>().AddLog(putonLog);
		await UniTask.DelayFrame(1);
	}

}
