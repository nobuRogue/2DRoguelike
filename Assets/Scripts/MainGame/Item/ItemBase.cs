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

	public int ID { get; protected set; } = -1;

	public int masterID { get; protected set; } = -1;

	public void Setup( int useID, int setMasterID, MapSquareData squareData ) {
		ID = useID;
		masterID = setMasterID;
		SetSquare( squareData );
		_GetItemObject( ID ).Setup( ItemMasterUtility.GetItemMaster( masterID ) );
	}

	public virtual void Teardown() {

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
		SetSquarePosition( square );
		Set3DPosition( square.GetObjectRoot().position );
	}

	/// <summary>
	/// 情報のみの変更
	/// </summary>
	/// <param name="setPosition"></param>
	public virtual void SetSquarePosition( MapSquareData square ) {
		MapSquareData prevSquare = MapSquareUtility.GetSquareData( squarePosition );
		if (prevSquare != null) prevSquare.RemoveCharacter();

		squarePosition = square.squarePosition;
	}

	/// <summary>
	/// 見た目のみの変更
	/// </summary>
	/// <param name="set3DPosition"></param>
	public void Set3DPosition( Vector3 setPosition ) {
		_GetItemObject( ID ).SetPostion( setPosition );
	}

}
