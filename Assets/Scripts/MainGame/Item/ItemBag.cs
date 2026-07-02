using UnityEngine;

/// <summary>
/// バッグアイテム
/// </summary>
public class ItemBag : ItemBase {
	public override eItemCategory GetCategory() {
		return eItemCategory.Bag;
	}
}
