/**
 * @file ItemManager.cs
 * @brief アイテムの管理
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ItemManager : MonoBehaviour {
	[SerializeField]
	private ItemObject _itemObjectOrigin = null;

	[SerializeField]
	private Transform _useObjectRoot = null;

	[SerializeField]
	private Transform _unuseObjectRoot = null;

	public static ItemManager instance { get; private set; } = null;

	private static System.Func<List<ItemBase>> _GetItemDataList = null;

	public static void SetItemProcess( System.Func<List<ItemBase>> getItemListProcess ) {
		_GetItemDataList = getItemListProcess;
	}

	private List<List<ItemBase>> _unuseItemList = null;

	private List<ItemObject> _useItemObjectList = null;
	private List<ItemObject> _unuseItemObjectList = null;

	public void Initialize() {
		instance = this;
		ItemBase.SetGetObjectCallback( GetItemObject );

		int itemCount = GameConst.ITEM_MAX;
		int categoryCount = (int)eItemCategory.Max;
		_unuseItemList = new List<List<ItemBase>>( categoryCount );
		for (int i = 0; i < categoryCount; i++) {
			_unuseItemList.Add( new List<ItemBase>( itemCount ) );
			for (int j = 0; j < itemCount; j++) {
				// アイテムデータ生成して未使用状態にする
				_unuseItemList[i].Add( CreateCategoryItem( (eItemCategory)i ) );
			}
		}
		_useItemObjectList = new List<ItemObject>( itemCount );
		_unuseItemObjectList = new List<ItemObject>( itemCount );
		for (int i = 0; i < itemCount; i++) {
			// アイテムオブジェクト生成して未使用状態にする
			_unuseItemObjectList.Add( Instantiate( _itemObjectOrigin ) );
		}
	}

	/// <summary>
	/// キャラクターとオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseItem"></param>
	public void UnuseItem( ItemBase unuseItem ) {
		// 使用リストから取り除く
		int unuseID = unuseItem.ID;
		List<ItemBase> useItemList = _GetItemDataList();
		if (IsEnableIndex( useItemList, unuseID )) useItemList[unuseID] = null;
		// 片付けて未使用リストに加える
		unuseItem.Teardown();
		_unuseItemList[(int)unuseItem.GetItemCategory()].Add( unuseItem );
		if (!IsEnableIndex( _useItemObjectList, unuseID )) return;
		// オブジェクトの未使用化
		UnuseObject( unuseID );
	}

	/// <summary>
	/// キャラクターオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseItemObject"></param>
	public void UnuseObject( int unuseID ) {
		if (!IsEnableIndex( _useItemObjectList, unuseID ) || _useItemObjectList[unuseID] == null) return;

		ItemObject unuseItemObject = _useItemObjectList[unuseID];
		if (unuseItemObject == null) return;

		_useItemObjectList[unuseID] = null;
		_unuseItemObjectList.Add( unuseItemObject );
		unuseItemObject.transform.SetParent( _unuseObjectRoot );
		unuseItemObject.Teardown();
	}

	/// <summary>
	/// アイテム生成
	/// </summary>
	/// <returns></returns>
	public ItemBase UseItem( int masterID, MapSquareData squareData ) {
		// 未使用のアイテムが無ければ生成
		var itemMaster = ItemMasterUtility.GetItemMaster( masterID );
		var itemCategory = (eItemCategory)itemMaster.category;
		ItemBase useItem = GetUsableItem( itemCategory );

		// 使用リストに追加
		int useID = -1;
		List<ItemBase> useItemList = _GetItemDataList();
		for (int i = 0, max = useItemList.Count; i < max; i++) {
			var enemy = useItemList[i];
			if (enemy != null) continue;

			useID = i;
			break;
		}
		if (useID < 0) {
			useID = useItemList.Count;
			useItemList.Add( null );
			_useItemObjectList.Add( null );
		}
		// 生成したアイテムの初期設定
		useItemList[useID] = useItem;
		UseItemObject( useID );
		useItem.Setup( useID, masterID, squareData );
		return useItem;
	}

	public void UseItemObject( int useID ) {
		ItemObject useItemObject = GetUsableItemObject();
		_useItemObjectList[useID] = useItemObject;
		useItemObject.transform.SetParent( _useObjectRoot );
	}

	private ItemBase GetUsableItem( eItemCategory itemCategory ) {
		List<ItemBase> unuseCategoryList = _unuseItemList[(int)itemCategory];
		if (IsEmpty( unuseCategoryList )) return CreateCategoryItem( itemCategory );

		ItemBase result = unuseCategoryList[0];
		unuseCategoryList.RemoveAt( 0 );
		return result;
	}

	public ItemBase CreateCategoryItem( eItemCategory itemCategory ) {
		switch (itemCategory) {
			case eItemCategory.Potion:
				return new ItemPotion();
		}
		return null;
	}

	private ItemObject GetUsableItemObject() {
		if (IsEmpty( _unuseItemObjectList )) return Instantiate( _itemObjectOrigin );

		ItemObject result = _unuseItemObjectList[0];
		_unuseItemObjectList.RemoveAt( 0 );
		return result;
	}

	public ItemBase GetItem( int ID ) {
		List<ItemBase> useItemList = _GetItemDataList();
		if (!IsEnableIndex( useItemList, ID )) return null;

		return useItemList[ID];
	}

	private ItemObject GetItemObject( int objectID ) {
		if (!IsEnableIndex( _useItemObjectList, objectID )) return null;

		return _useItemObjectList[objectID];
	}

	/// <summary>
	/// 全てのキャラクターに指定した処理を行う
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllItem( System.Action<ItemBase> action ) {
		if (action == null) return;

		List<ItemBase> useItemList = _GetItemDataList();
		for (int i = 0, max = useItemList.Count; i < max; i++) {
			if (useItemList[i] == null) continue;

			action.Invoke( useItemList[i] );
		}
	}

	/// <summary>
	/// 全てのキャラクターに指定したタスクを行う
	/// </summary>
	/// <param name="task"></param>
	public async UniTask ExecuteTaskAllItem( System.Func<ItemBase, UniTask> task ) {
		if (task == null) return;

		List<ItemBase> useItemList = _GetItemDataList();
		for (int i = 0, max = useItemList.Count; i < max; i++) {
			if (useItemList[i] == null) continue;

			await task.Invoke( useItemList[i] );
		}
	}

}
