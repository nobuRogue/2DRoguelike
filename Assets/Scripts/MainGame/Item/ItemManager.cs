using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム管理
/// </summary>
public class ItemManager : MonoBehaviour {
	public static ItemManager instance { get; private set; } = null;
	// アイテムプレハブへの参照
	[SerializeField]
	private ItemObject _originObject = null;
	// 使用リスト
	private List<ItemObject> _useList = null;
	// 未使用リスト
	private List<List<ItemObject>> _unuseList = null;
	// 初期化時に確保されるカテゴリごとのアイテムキャッシュ
	private const int _ITEM_MAX = 256;
	// アイテムのカテゴリ毎のスプライト
	private Sprite[] _itemCategorySpriteList = null;
	// アイテムのカテゴリ毎のスプライトのファイルパス
	private const string _ITEM_CATEGORY_SPRITE_PATH = "Design/Sprites/Item/itemIcons";

	public void Initialize() {
		instance = this;
		// カテゴリごとのスプライト読み込み
		_itemCategorySpriteList = Resources.LoadAll<Sprite>(_ITEM_CATEGORY_SPRITE_PATH);
		// アイテムをある程度生成して未使用状態にしておく
		_useList = new List<ItemObject>(_ITEM_MAX);
		int categoryMax = (int)eItemCategory.Max;
		_unuseList = new List<List<ItemObject>>(categoryMax);
		// カテゴリごとに未使用リスト生成ループ
		for (int categoryIndex = 0; categoryIndex < categoryMax; categoryIndex++) {
			_unuseList.Add(new List<ItemObject>(_ITEM_MAX));
			// 256未使用オブジェクト追加ループ
			for (int itemCount = 0; itemCount < _ITEM_MAX; itemCount++) {
				// アイテムオブジェクト生成
				ItemObject createItem = Instantiate(_originObject, transform);
				ItemBase createItemData = CreateCategoryItemData((eItemCategory)categoryIndex);
				// アイテムオブジェクト初期化
				createItem.Initialize(createItemData, _itemCategorySpriteList[categoryIndex]);
				// 未使用リストに追加
				DeleteItem(createItem);
			}
		}
	}

	/// <summary>
	/// アイテムを生成しフロアに配置
	/// </summary>
	/// <param name="masterID"></param>
	/// <param name="square"></param>
	public void CreateFloorItem(int masterID, SquareObject square) {
		Entity_ItemData.Param itemMaster = MasterDataManager.instance.GetItemData(masterID);
		int categoryIndex = itemMaster.category;
		// 使用可能なアイテムインスタンスを取得
		ItemObject createItem;
		if (CommonModule.IsEmpty(_unuseList[categoryIndex])) {
			// なければ生成する
			createItem = Instantiate(_originObject, transform);
			ItemBase createItemData = CreateCategoryItemData((eItemCategory)categoryIndex);
			createItem.Initialize(createItemData, _itemCategorySpriteList[categoryIndex]);
		}
		else {
			// 未使用オブジェクトがあればそこから使う
			createItem = _unuseList[categoryIndex][0];
			_unuseList[categoryIndex].RemoveAt(0);
		}
		// 使用リストに追加
		int useID = -1;
		for (int i = 0; i < _useList.Count; i++) {
			if (_useList[i] != null) continue;
			// 使用リストの未使用箇所が見つかったので使う
			useID = i;
			_useList[useID] = createItem;
			break;
		}
		if (useID < 0) {
			// 使用可能な箇所が見つからなかったので末尾に追加
			useID = _useList.Count;
			_useList.Add(createItem);
		}
		// 使用前準備処理
		createItem.Setup(useID, itemMaster);
		// 指定されたマスに置く
		createItem.SetSquare(square);
	}

	/// <summary>
	/// アイテム削除処理
	/// </summary>
	/// <param name="deleteItem"></param>
	public void DeleteItem(ItemObject deleteItem) {
		// 使用リストから取り除く
		int deleteID = deleteItem.itemData.ID;
		if (CommonModule.IsEnableIndex(_useList, deleteID)) _useList[deleteID] = null;
		// 片付け処理
		deleteItem.Teardown();
		// 未使用リストに追加
		_unuseList[(int)deleteItem.itemData.GetCategory()].Add(deleteItem);
	}

	/// <summary>
	/// カテゴリに対応したアイテム情報クラスの取得
	/// </summary>
	/// <param name="itemCategory"></param>
	/// <returns></returns>
	private ItemBase CreateCategoryItemData(eItemCategory itemCategory) {
		switch (itemCategory) {
			case eItemCategory.Potion:
				return new ItemPotion();
			case eItemCategory.Food:
				return new ItemFood();
			case eItemCategory.Wand:
				return new ItemWand();
			case eItemCategory.Scroll:
				return new ItemScroll();
			case eItemCategory.Bag:
				return new ItemBag();
			case eItemCategory.Throwing:
				return new ItemThrowing();
			case eItemCategory.Weapon:
				return new ItemWeapon();
			case eItemCategory.Armor:
				return new ItemArmor();
		}
		return null;
	}

}
