using UnityEngine;

/// <summary>
/// 杖アイテム
/// </summary>
public class ItemWand : ItemBase {
	// 残り使用回数
	private int _count = -1;
	// 杖の名前表示用メッセージID
	private const int _WAND_NAME_ID = 2299;

	public override eItemCategory GetCategory() {
		return eItemCategory.Wand;
	}

	public override void Setup(int ID, Entity_ItemData.Param itemMaster) {
		base.Setup(ID, itemMaster);
		_count = Random.Range(itemMaster.minValue, itemMaster.maxValue + 1);
	}

	/// <summary>
	/// 杖用アイテム名取得
	/// </summary>
	/// <returns></returns>
	public override string GetName() {
		return string.Format(_WAND_NAME_ID.ToMessage(), base.GetName(), _count);
	}

	/// <summary>
	/// 杖の消費処理
	/// </summary>
	public override void Consume() {
		// カウントを1減らし0になったら消える
		_count--;
		if (_count < 1) base.Consume();

	}
}
