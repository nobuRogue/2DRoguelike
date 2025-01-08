
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MasterDataManager {
	private static string _DATA_PATH = "MasterData/";

	public static List<List<Entity_FloorData.Param>> floorData = null;

	public static List<List<Entity_CharacterData.Param>> characterData = null;

	public static void LoadAllData() {
		floorData = Load<Entity_FloorData, Entity_FloorData.Sheet, Entity_FloorData.Param>( "FloorData" );
		characterData = Load<Entity_CharacterData, Entity_CharacterData.Sheet, Entity_CharacterData.Param>( "CharacterData" );
	}

	private static List<List<T3>> Load<T1, T2, T3>( string dataName ) where T1 : ScriptableObject {
		var sourceData = Resources.Load<T1>( _DATA_PATH + dataName );// データ読込
		var sheetField = typeof( T1 ).GetField( "sheets" );              // 名称指定のフィールド取得プロパティ
		List<T2> listData = sheetField.GetValue( sourceData ) as List<T2>;             // 名称指定でフィールドを取得

		var listField = typeof( T2 ).GetField( "list" );
		List<List<T3>> paramList = new List<List<T3>>();
		foreach (var element in listData) {
			var param = listField.GetValue( element ) as List<T3>;
			paramList.Add( param );
		}
		return paramList;
	}

}
