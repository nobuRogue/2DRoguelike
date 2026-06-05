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
		if (!CommonModule.IsEnableIndex(_squareList, ID)) return null;

		return _squareList[ID];
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

}
