/**
 * @file MapSquareUtility.cs
 * @brief �}�X�֘A���p�����N���X
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MapSquareUtility {

	public static MapSquareData GetSquareData( int ID ) {
		return MapSquareManager.instance.Get( ID );
	}

	public static MapSquareData GetSquareData( int x, int y ) {
		return MapSquareManager.instance.Get( x, y );
	}

	public static MapSquareData GetSquareData( Vector2Int position ) {
		return MapSquareManager.instance.Get( position );
	}

	/// <summary>
	/// �ړ��۔���
	/// </summary>
	/// <param name="sourcePos"></param>
	/// <param name="moveSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanMove( Vector2Int sourcePos, MapSquareData moveSquare, eDirectionEight dir ) {
		return CanMoveTerrain( sourcePos, moveSquare, dir ) && !moveSquare.existCharacter;
	}

	/// <summary>
	/// �ړ��۔���
	/// </summary>
	/// <param name="sourcePos"></param>
	/// <param name="moveSquare"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public static bool CanMoveTerrain( Vector2Int sourcePos, MapSquareData moveSquare, eDirectionEight dir ) {
		if (moveSquare == null ||
			moveSquare.terrain == eTerrain.Wall) return false;

		if (!dir.IsSlant()) return true;

		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			var checkSquare = MapSquareManager.instance.Get( sourcePos.ToVectorPos( separateDir[i] ) );
			if (checkSquare == null || checkSquare.terrain == eTerrain.Wall) return false;

		}
		return true;
	}

	public static bool CanAttackSquare( Vector2Int sourcePos, MapSquareData attackSquare, eDirectionEight dir ) {
		if (attackSquare == null ||
			attackSquare.terrain == eTerrain.Wall) return false;

		if (!dir.IsSlant()) return true;

		eDirectionFour[] separateDir = dir.Separate();
		for (int i = 0, max = separateDir.Length; i < max; i++) {
			var checkSquare = MapSquareManager.instance.Get( sourcePos.ToVectorPos( separateDir[i] ) );
			if (checkSquare == null || checkSquare.terrain == eTerrain.Wall) return false;

		}
		return true;
	}

	public static void AddRoom( RoomData addRoom ) {
		UserDataHolder.currentData.AddRoom( addRoom );
	}

	public static void ClearRoom() {
		UserDataHolder.currentData.ClearRoom();
	}

	public static RoomData GetRoomData( int roomID ) {
		return UserDataHolder.currentData.GetRoom( roomID );
	}

	public static PlayerCharacter GetVisibleAreaAndPlayer( ref List<int> result, MapSquareData sourceSquare ) {
		InitializeList( ref result );
		if (sourceSquare == null) return null;
		// ���͗א�8�}�X��ǉ�
		List<int> aroundSquareIDList = null;
		GetChebyshevAroudSquare( ref aroundSquareIDList, sourceSquare );
		aroundSquareIDList.Add( sourceSquare.ID );
		List<int> adjacentRoomID = new List<int>();
		PlayerCharacter player = CharacterUtility.GetPlayer();
		bool visiblePlayer = false;
		for (int i = 0, max = aroundSquareIDList.Count; i < max; i++) {
			var targetSquare = GetSquareData( aroundSquareIDList[i] );
			if (targetSquare == null) continue;

			if (result.Exists( squareID => squareID == targetSquare.ID )) continue;

			result.Add( targetSquare.ID );
			if (!visiblePlayer) visiblePlayer = player.ExistMoveTrail( targetSquare.ID );

			if (targetSquare.terrain != eTerrain.Room) continue;

			if (adjacentRoomID.Exists( roomID => roomID == targetSquare.roomID )) continue;

			adjacentRoomID.Add( targetSquare.roomID );
		}
		// �א�8�}�X�ɕ������܂܂�Ă����畔���̃}�X���ǉ�
		int roomSquareCount = adjacentRoomID.Count;
		for (int i = 0; i < roomSquareCount; i++) {
			var targetRoom = GetRoomData( adjacentRoomID[i] );
			if (targetRoom == null) continue;

			if (!visiblePlayer) {
				visiblePlayer = AddRoomVisibleSquareAndPlayer( player, ref result, targetRoom );
			} else {
				AddRoomVisibleSquare( ref result, targetRoom );
			}
		}
		return visiblePlayer ? player : null;
	}

	private static void AddRoomVisibleSquare( ref List<int> result, RoomData targetRoom ) {
		var squareList = targetRoom.squareIDList;
		for (int i = 0, roomSquareMax = squareList.Count; i < roomSquareMax; i++) {
			var roomSquare = squareList[i];
			if (result.Exists( element => element == roomSquare )) continue;

			result.Add( roomSquare );
		}
	}

	private static bool AddRoomVisibleSquareAndPlayer( PlayerCharacter player, ref List<int> result, RoomData targetRoom ) {
		var squareList = targetRoom.squareIDList;
		bool visiblePlayer = false;
		for (int i = 0, roomSquareMax = squareList.Count; i < roomSquareMax; i++) {
			var roomSquare = squareList[i];
			if (result.Exists( element => element == roomSquare )) continue;

			result.Add( roomSquare );
			if (visiblePlayer) continue;

			visiblePlayer = player.ExistMoveTrail( roomSquare );
		}
		return visiblePlayer;
	}

	/// <summary>
	/// ���`�F�r�V�F�t�����̃}�X�S�Ď擾
	/// </summary>
	/// <param name="sourceSquare"></param>
	/// <param name="range"></param>
	/// <param name="result"></param>
	public static void GetChebyshevAroudSquare( ref List<int> result, MapSquareData sourceSquare, int range = 1 ) {
		if (sourceSquare == null) return;

		InitializeList( ref result, range * 4 );
		if (range == 0) {
			result.Add( sourceSquare.ID );
			return;
		}
		int countMax = range * 2;
		int sourceY = sourceSquare.squarePosition.y;
		int sourceX = sourceSquare.squarePosition.x;
		for (int count = 0; count <= countMax; count++) {
			var targetSquare = GetSquareData( sourceX - range + count, sourceY - range );
			if (targetSquare != null) result.Add( targetSquare.ID );

			targetSquare = GetSquareData( sourceX + range, sourceY - range + count );
			if (targetSquare != null) result.Add( targetSquare.ID );

			targetSquare = GetSquareData( sourceX + range - count, sourceY + range );
			if (targetSquare != null) result.Add( targetSquare.ID );

			targetSquare = GetSquareData( sourceX - range, sourceY + range - count );
			if (targetSquare != null) result.Add( targetSquare.ID );
		}
	}

}
