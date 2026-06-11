using UnityEngine;

/// <summary>
/// マップの1マスの見た目情報
/// </summary>
public class SquareObject : MonoBehaviour {
	// 座標変換用係数
	private static readonly float _SQUARE_SIZE_RATIO = 0.32f;
	// 地形のスプライト
	[SerializeField]
	private SpriteRenderer _terrainSprite = null;
	// キャラクター配置用のルートオブジェクト
	[SerializeField]
	private Transform _characterRoot = null;

	public MapSquare squareData = null;

	/// <summary>
	/// 使用前の準備
	/// </summary>
	/// <param name="ID"></param>
	/// <param name="posX"></param>
	/// <param name="posY"></param>
	public void Setup(int ID, int posX, int posY) {
		squareData = new MapSquare(ID, posX, posY);
		// マスオブジェクトのポジションを設定
		Vector3 position = Vector3.zero;
		position.x = posX * _SQUARE_SIZE_RATIO;
		position.y = posY * _SQUARE_SIZE_RATIO;
		position.z = posY * 0.1f;
		transform.position = position;
	}

	/// <summary>
	/// 地形の設定
	/// </summary>
	/// <param name="setTerrain"></param>
	public void SetTerrain(eTerrain setTerrain, int index = -1) {
		// データ上の地形変更
		squareData?.SetTerrain(setTerrain);
		// 見た目の地形変更
		_terrainSprite.sprite = TerrainSpriteAssignor.instance.GetTerrainSprite(setTerrain, index);
	}

	/// <summary>
	/// キャラクター用ルートオブジェクト取得
	/// </summary>
	/// <returns></returns>
	public Transform GetCharacterRoot() { return _characterRoot; }

}
