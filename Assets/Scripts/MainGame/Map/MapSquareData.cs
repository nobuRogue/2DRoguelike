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
	public Vector2Int position { get; private set; } = Vector2Int.zero;
	public eTerrain terrain { get; private set; } = eTerrain.Invalid;

	public void Setup( int setID, Vector2Int setPosition ) {
		ID = setID;
		position = setPosition;
		_GetObject( ID )?.Setup( position );
	}

	public void SetTerrain( eTerrain setTerrain ) {
		terrain = setTerrain;
		_GetObject( ID )?.SetTerrain( terrain );
	}
}
