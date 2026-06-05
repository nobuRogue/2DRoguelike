using System.Collections.Generic;
using UnityEngine;

public class MapSquareManager : MonoBehaviour {
	// マスオブジェクトのオリジナル
	[SerializeField]
	private SquareObject _originObject;

	/// <summary>
	/// 生成された管理中のマスオブジェクトリスト
	/// </summary>
	private List<SquareObject> _squareList = null;

	public void Initialize() {

		// マスオブジェクトを必要数生成
		int squareCount = GameConst.MAP_SQUARE_HEIGHT_COUNT * GameConst.MAP_SQUARE_WIDTH_COUNT;
		for (int i = 0; i < squareCount; i++) {
			// オブジェクト生成
			SquareObject squareObject = Instantiate(_originObject, transform);
			// セットアップ
			int posX, posY;
			GetPositionFromID(i, out posX, out posY);
			squareObject.Setup(i, posX, posY);
			// 壁地形に設定
			squareObject.SetTerrain(eTerrain.Wall);
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

}
