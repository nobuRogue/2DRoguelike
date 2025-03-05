/**
 * @file ItemBase.cs
 * @brief アイテムの情報
 * @author yaonobu
 * @date 2025/1/4
 */
using UnityEngine;

public abstract class ItemBase {
	private static System.Func<int, ItemObject> _GetItemObject = null;

	public static void SetGetObjectCallback( System.Func<int, ItemObject> setProcess ) {
		_GetItemObject = setProcess;
	}

	public Vector2Int squarePosition { get; protected set; } = new Vector2Int( -1, -1 );

	public bool isPlayerItem = false;
	public int characterID { get; protected set; } = -1;

	public int ID { get; protected set; } = -1;

	public int masterID { get; protected set; } = -1;

	public void Setup( int useID, int setMasterID, MapSquareData squareData ) {
		ID = useID;
		masterID = setMasterID;
		SetSquare( squareData );
		_GetItemObject( ID ).Setup( ItemMasterUtility.GetItemMaster( masterID ) );
	}

	public virtual void Teardown() {
		RemoveCurrentPlace();
		ID = -1;
		masterID = -1;
	}

	/// <summary>
	/// アイテムカテゴリ取得
	/// </summary>
	/// <returns></returns>
	public abstract eItemCategory GetItemCategory();

	/// <summary>
	/// 見た目と情報、両方の変更
	/// </summary>
	/// <param name="setPosition"></param>
	/// <param name="set3DPosition"></param>
	public void SetSquare( MapSquareData square ) {
		RemoveCurrentPlace();

		Set3DPosition( square.GetObjectRoot().position );
		squarePosition = square.squarePosition;
		square.SetItem( ID );
	}

	public void RemoveCurrentPlace() {
		if (squarePosition.x >= 0 || squarePosition.y >= 0) {
			MapSquareUtility.GetSquareData( squarePosition )?.RemoveItem();
			_GetItemObject( ID )?.UnuseSelf( ID );
			return;
		}

	}

	/// <summary>
	/// 見た目のみの変更
	/// </summary>
	/// <param name="set3DPosition"></param>
	public void Set3DPosition( Vector3 setPosition ) {
		var itemObject = _GetItemObject( ID );
		if (itemObject == null) ItemManager.instance.UseItemObject( ID );

		_GetItemObject( ID )?.SetPostion( setPosition );
	}

	public void AddPlayerItem() {
		RemoveCurrentPlace();
		isPlayerItem = true;
	}

}
