/**
* @file ItemPotion.cs
* @brief ��n�A�C�e���̏��
* @author yaonobu
* @date 2025/1/4
*/
public class ItemPotion : ItemBase {
	/// <summary>
	/// �A�C�e���J�e�S���擾
	/// </summary>
	/// <returns></returns>
	public override eItemCategory GetItemCategory() {
		return eItemCategory.Potion;
	}
}
