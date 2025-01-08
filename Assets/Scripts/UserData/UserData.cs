/**
 * @file UserData.cs
 * @brief ユーザー情報
 * @author yaonobu
 * @date 2020/12/6
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

[System.Serializable]
/// <summary>
/// ユーザー情報
/// </summary>
public class UserData {
	// ニューゲームフラグ
	public bool isNewGame { get; private set; } = true;

	// 現在ダンジョン情報
	public int floorCount { get; private set; } = -1;

	private List<MapSquareData> _squareList = null;
	private List<RoomData> _roomList = null;

	private PlayerCharacter _player = null;
	private List<EnemyCharacter> _enemyList = null;

	public UserData() {
		isNewGame = true;
		SetFloorCount( 1 );
		int squareCount = GameConst.MAP_SQUARE_MAX_HEIGHT * GameConst.MAP_SQUARE_MAX_WIDTH;
		_squareList = new List<MapSquareData>( squareCount );
		_roomList = new List<RoomData>( GameConst.AREA_DEVIDE_COUNT + 1 );
		_enemyList = new List<EnemyCharacter>( GameConst.FLOOR_ENEMY_MAX );
	}

	public void SetIsNewGame( bool setIsNewGame ) {
		isNewGame = setIsNewGame;
	}

	public void SetFloorCount( int setCount ) {
		floorCount = setCount;
	}

	public void IncrementFloorCount() {
		floorCount++;
	}

	public List<MapSquareData> GetSquareList() {
		return _squareList;
	}

	public PlayerCharacter GetPlayer() {
		return _player;
	}

	public void SetPlayer( PlayerCharacter player ) {
		_player = player;
	}

	public List<EnemyCharacter> GetEnemyList() {
		return _enemyList;
	}

	public void AddRoom( RoomData addRoom ) {
		int roomID = _roomList.Count;
		addRoom.SetRoomID( roomID );

		_roomList.Add( addRoom );
	}

	public RoomData GetRoom( int roomID ) {
		if (!IsEnableIndex( _roomList, roomID )) return null;

		return _roomList[roomID];
	}

	public void ClearRoom() {
		_roomList.Clear();
	}
}
