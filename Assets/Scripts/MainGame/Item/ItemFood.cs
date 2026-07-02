using UnityEngine;

/// <summary>
/// 食べ物アイテム
/// </summary>
public class ItemFood : ItemBase {
	public override eItemCategory GetCategory() {
		return eItemCategory.Food;
	}
}
