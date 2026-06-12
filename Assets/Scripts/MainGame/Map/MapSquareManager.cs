using System.Collections.Generic;
using UnityEngine;

public class MapSquareManager : MonoBehaviour {
	public static MapSquareManager instance = null;

	// マスオブジェクトのオリジナル
	[SerializeField]
	private SquareObject _originObject;

	/// <summary>
	/// 生成された管理中のマスオブジェクトリスト
	/// </summary>
	private List<SquareObject> _squareList = null;

	// オブジェクトプーリング：役割を終えたオブジェクトを破棄せずに、プールしておき、再利用する。
	// 使用中の部屋リスト
	private List<RoomData> _roomList = null;
	// 未使用状態の部屋リスト
	private List<RoomData> _unuseRoomList = null;

	public void Initialize() {
		instance = this;
		// マスオブジェクトを必要数生成
		int squareCount = GameConst.MAP_SQUARE_HEIGHT_COUNT * GameConst.MAP_SQUARE_WIDTH_COUNT;
		_squareList = new List<SquareObject>(squareCount);
		for (int i = 0; i < squareCount; i++) {
			// オブジェクト生成
			SquareObject squareObject = Instantiate(_originObject, transform);
			// セットアップ
			int posX, posY;
			GetPositionFromID(i, out posX, out posY);
			squareObject.Setup(i, posX, posY);
			_squareList.Add(squareObject);
		}
		// 部屋情報の初期化
		_roomList = new List<RoomData>();
		_unuseRoomList = new List<RoomData>();
	}

	/// <summary>
	/// IDからマス座標取得
	/// </summary>
	/// <param name="ID"></param>
	/// <param name="posX"></param>
	/// <param name="posY"></param>
	private void GetPositionFromID(int ID, out int posX, out int posY) {
		posX = ID % GameConst.MAP_SQUARE_WIDTH_COUNT;
		posY = ID / GameConst.MAP_SQUARE_WIDTH_COUNT;
	}

	/// <summary>
	/// マスの座標をIDに変換
	/// </summary>
	/// <param name="posX"></param>
	/// <param name="posY"></param>
	/// <returns></returns>
	private int GetIDFromPosition(int posX, int posY) {
		// マップの範囲内か否かチェック
		if (posX < 0 || posX >= GameConst.MAP_SQUARE_WIDTH_COUNT ||
			posY < 0 || posY >= GameConst.MAP_SQUARE_HEIGHT_COUNT) return -1;

		return posY * GameConst.MAP_SQUARE_WIDTH_COUNT + posX;
	}

	/// <summary>
	/// ID指定のマス情報取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public SquareObject GetSquare(int ID) {
		// 有効なインデクスか判定
		if (!CommonModule.IsEnableIndex(_squareList, ID)) return null;

		return _squareList[ID];
	}

	/// <summary>
	/// 指定座標から指定方向の隣接マスを取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public SquareObject GetToDirSquare(int x, int y, eDirectionFour dir) {
		// 隣接座標取得
		ToDirPosition(ref x, ref y, dir);
		// 座標指定のマス取得
		return GetSquare(x, y);
	}

	/// <summary>
	/// 指定座標から指定方向の隣接マスを取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public SquareObject GetToDirSquare(int x, int y, eDirectionEight dir) {
		// 隣接座標取得
		ToDirPosition(ref x, ref y, dir);
		return GetSquare(x, y);
	}

	/// <summary>
	/// 指定座標の指定方向への隣接座標取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	private void ToDirPosition(ref int x, ref int y, eDirectionFour dir) {
		switch (dir) {
			case eDirectionFour.Up:
				y++;
				break;
			case eDirectionFour.Right:
				x++;
				break;
			case eDirectionFour.Down:
				y--;
				break;
			case eDirectionFour.Left:
				x--;
				break;
		}
	}

	/// <summary>
	/// 指定座標の指定方向への隣接座標取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="dir"></param>
	private void ToDirPosition(ref int x, ref int y, eDirectionEight dir) {
		switch (dir) {
			case eDirectionEight.Up:
				y++;
				break;
			case eDirectionEight.UpRight:
				y++;
				x++;
				break;
			case eDirectionEight.Right:
				x++;
				break;
			case eDirectionEight.DownRight:
				y--;
				x++;
				break;
			case eDirectionEight.Down:
				y--;
				break;
			case eDirectionEight.DownLeft:
				y--;
				x--;
				break;
			case eDirectionEight.Left:
				x--;
				break;
			case eDirectionEight.UpLeft:
				y++;
				x--;
				break;
		}
	}

	/// <summary>
	/// 座標指定のマス取得
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public SquareObject GetSquare(int x, int y) {
		return GetSquare(GetIDFromPosition(x, y));
	}

	/// <summary>
	/// 全てのマスに対して指定処理実行
	/// </summary>
	/// <param name="action">実行する処理(SquareObjectを引数に取る)</param>
	public void ExecuteAllSquare(System.Action<SquareObject> action) {
		if (action == null || CommonModule.IsEmpty(_squareList)) return;

		for (int i = 0; i < _squareList.Count; i++) {
			action(_squareList[i]);
		}
	}

	/// <summary>
	/// 部屋の追加
	/// </summary>
	/// <param name="squareIDList"></param>
	public void AddRoom(List<int> squareIDList) {
		// 使用可能な部屋を取得
		RoomData addRoom = GetUsableRoom();
		// 使用リストに追加
		int roomID = _roomList.Count;
		addRoom.Setup(roomID, squareIDList);
		_roomList.Add(addRoom);
	}

	/// <summary>
	/// 使用可能な部屋取得
	/// </summary>
	/// <returns></returns>
	private RoomData GetUsableRoom() {
		// 未使用がなければインスタンスを生成
		if (CommonModule.IsEmpty(_unuseRoomList)) return new RoomData();
		// 未使用のものがあればそれを返す
		RoomData result = _unuseRoomList[0];
		_unuseRoomList.RemoveAt(0);
		return result;
	}

	/// <summary>
	/// 全ての部屋の削除
	/// </summary>
	public void RemoveAllRoom() {
		if (CommonModule.IsEmpty(_roomList)) return;

		for (int i = 0; i < _roomList.Count; i++) {
			RoomData roomData = _roomList[i];
			if (roomData == null) continue;

			roomData.Teardown();
			_unuseRoomList.Add(roomData);
		}
		_roomList.Clear();
	}

}
