/**
 * @file MapSquareData.cs
 * @brief マップ上の1マスの情報
 * @author yaonobu
 * @date 2025/1/4
 */
using UnityEngine;

public class MapSquareData {
	private static System.Func<int, MapSquareObject> _GetObject = null;

	public static void SetObejectGetProcess( System.Func<int, MapSquareObject> setProcess ) {
		_GetObject = setProcess;
	}

	public int ID { get; private set; } = -1;
	public int roomID { get; private set; } = -1;
	public Vector2Int squarePosition { get; private set; } = Vector2Int.zero;
	public eTerrain terrain { get; private set; } = eTerrain.Invalid;

	public bool existCharacter {
		get { return existPlayer || enemyID >= 0; }
	}
	public bool existPlayer { get; private set; } = false;
	public int enemyID { get; private set; } = -1;

	public int itemID { get; private set; } = -1;

	public void Setup( int setID, Vector2Int setSquarePosition ) {
		ID = setID;
		squarePosition = setSquarePosition;
		_GetObject( ID )?.Setup( squarePosition );
	}

	public void SetTerrain( eTerrain setTerrain ) {
		terrain = setTerrain;
		roomID = -1;
		_GetObject( ID )?.SetTerrain( terrain );
	}

	public Transform GetObjectRoot() {
		return _GetObject( ID ).GetCharacterRoot();
	}

	public void SetRoomID( int setRoomID ) {
		roomID = setRoomID;
	}

	public void SetPlayer() {
		existPlayer = true;
	}

	public void SetEnemy( int setEnemyID ) {
		enemyID = setEnemyID;
	}

	public void RemoveCharacter() {
		existPlayer = false;
		enemyID = -1;
	}

	public void SetRangeSpriteVisibility( bool isVisible ) {
		_GetObject( ID ).SetTrailSpriteVisibility( isVisible );
	}

	public void SetItem( int setItemID ) {
		itemID = setItemID;
	}

	public void RemoveItem() {
		itemID = -1;
	}
}
