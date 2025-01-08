/**
 * @file RoomData.cs
 * @brief •”‰®î•ñ
 * @author yaonobu
 * @date 2020/12/6
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData {
	public int roomID { get; private set; } = -1;

	public List<int> squareIDList { get; private set; } = null;
	public RoomData() {
		squareIDList = new List<int>( (GameConst.MIN_ROOM_SIZE + 1) * 2 );
	}

	public void SetRoomID( int setID ) {
		roomID = setID;
		for (int i = 0, max = squareIDList.Count; i < max; i++) {
			MapSquareManager.instance.Get( squareIDList[i] )?.SetRoomID( roomID );
		}
	}

	public void AddSquare( int squareID ) {
		squareIDList.Add( squareID );
	}
}
