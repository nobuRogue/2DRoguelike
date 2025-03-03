/**
* @file ItemPotion.cs
* @brief 薬系アイテムの情報
* @author yaonobu
* @date 2025/1/4
*/
public class ItemPotion : ItemBase {
	/// <summary>
	/// アイテムカテゴリ取得
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetItemCategory() {
		return eItemCategory.Potion;
	}
}
