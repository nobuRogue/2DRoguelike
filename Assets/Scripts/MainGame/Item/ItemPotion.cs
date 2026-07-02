using UnityEngine;

/// <summary>
/// 薬アイテム
/// </summary>
public class ItemPotion : ItemBase {
	/// <summary>
	/// カテゴリ取得
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetCategory() {
		return eItemCategory.Potion;
	}

}
