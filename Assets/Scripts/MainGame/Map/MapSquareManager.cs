/**
 * @file MapSquareManager.cs
 * @brief マスの管理
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class MapSquareManager : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _outBGSprite = null;

	[SerializeField]
	private MapSquareObject _squareObjectOrigin = null;

	[SerializeField]
	private Transform _objectRoot = null;
	public static MapSquareManager instance { get; private set; } = null;

	private static System.Func<List<MapSquareData>> _GetUseDataList = null;

	public static void SetGetSquareListProcess( System.Func<List<MapSquareData>> setProcess ) {
		_GetUseDataList = setProcess;
	}

	private List<MapSquareObject> _useObjectList = null;

	public void Initialize() {
		instance = this;
		MapSquareData.SetObejectGetProcess( GetSquareObject );
		int squareMaxCount = GameConst.MAP_SQUARE_MAX_HEIGHT * GameConst.MAP_SQUARE_MAX_WIDTH;
		List<MapSquareData> useDataList = _GetUseDataList();
		_useObjectList = new List<MapSquareObject>( squareMaxCount );
		for (int i = 0; i < squareMaxCount; i++) {
			// マスオブジェクト生成
			MapSquareObject createObject = Instantiate( _squareObjectOrigin, _objectRoot );
			_useObjectList.Add( createObject );
			// マスデータ生成
			MapSquareData createSquare = new MapSquareData();
			createSquare.Setup( i, GetPosition( i ) );
			useDataList.Add( createSquare );
			createSquare.SetTerrain( eTerrain.Wall );
		}
	}

	public void SetoutBGSprite( Sprite bgSprite ) {
		_outBGSprite.sprite = bgSprite;
	}

	public MapSquareData Get( int ID ) {
		List<MapSquareData> useDataList = _GetUseDataList();
		if (!IsEnableIndex( useDataList, ID )) return null;

		return useDataList[ID];
	}

	public MapSquareData Get( Vector2Int position ) {
		return Get( GetID( position ) );
	}

	public MapSquareData Get( int x, int y ) {
		return Get( GetID( x, y ) );
	}

	public int GetID( Vector2Int position ) {
		return GetID( position.x, position.y );
	}

	public int GetID( int x, int y ) {
		if (x < 0 || y < 0) return -1;

		return y * GameConst.MAP_SQUARE_MAX_WIDTH + x;
	}

	private Vector2Int GetPosition( int suqareID ) {
		Vector2Int result = new Vector2Int();
		result.x = suqareID % GameConst.MAP_SQUARE_MAX_WIDTH;
		result.y = suqareID / GameConst.MAP_SQUARE_MAX_WIDTH;
		return result;
	}

	private MapSquareObject GetSquareObject( int objectID ) {
		if (!IsEnableIndex( _useObjectList, objectID )) return null;

		return _useObjectList[objectID];
	}

	/// <summary>
	/// 全てのマスに指定した処理を行う
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllSquare( System.Action<MapSquareData> action ) {
		if (action == null) return;

		List<MapSquareData> useDataList = _GetUseDataList();
		for (int i = 0, max = useDataList.Count; i < max; i++) {
			action?.Invoke( useDataList[i] );
		}
	}

}
