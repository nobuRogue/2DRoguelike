using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 地形スプライト割り当てモジュール
/// </summary>
public class TerrainSpriteAssignor {
	// シングルトンインスタンス
	private static TerrainSpriteAssignor _instance = null;
	// インスタンスへの参照プロパティ
	public static TerrainSpriteAssignor instance {
		get {
			if (_instance == null) _instance = new TerrainSpriteAssignor();

			return _instance;
		}
	}
	// スプライトリソースフォルダへのパス
	private const string _MAP_SPRITE_PATH = "Design/Sprites/Map/";

	private readonly string[][] _MAP_SPRITE_NAME_LIST = new string[][] {
		new string[]{ "rogue_map_sand_floor","rogue_map_sand_wall","rogue_map_sand_stair" },
		new string[]{ "rogue_map_snow_floor", "rogue_map_snow_wall", "rogue_map_snow_stair" },
		new string[]{ "rogue_map_urban_floor", "rogue_map_urban_wall", "rogue_map_urban_stair" }
	};

	private List<List<Sprite[]>> _terrainSpriteList = null;
	// 地形スプライトの種類のインデクス
	private int _spriteTypeIndex = -1;

	/// <summary>
	/// シングルトンなのでコンストラクタを private にしている（クラス外からnew（インスタンス化）できなくする）
	/// </summary>
	private TerrainSpriteAssignor() {
		int spriteTypeCount = _MAP_SPRITE_NAME_LIST.Length;
		int terrainSpriteCount = _MAP_SPRITE_NAME_LIST[0].Length;
		_terrainSpriteList = new List<List<Sprite[]>>(spriteTypeCount);
		// マップのタイプで回す
		for (int typeIndex = 0; typeIndex < spriteTypeCount; typeIndex++) {
			_terrainSpriteList.Add(new List<Sprite[]>(terrainSpriteCount));
			// 地形ごとの使用スプライトの読み込み
			for (int i = 0; i < spriteTypeCount; i++) {
				Sprite[] loadSpriteList = Resources.LoadAll<Sprite>(_MAP_SPRITE_PATH + _MAP_SPRITE_NAME_LIST[typeIndex][i]);
				_terrainSpriteList[typeIndex].Add(loadSpriteList);
			}
		}
	}

	/// <summary>
	/// 地形に対応したスプライトを取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public Sprite GetTerrainSprite(eTerrain terrain, int index = -1) {
		if (!CommonModule.IsEnableIndex(_terrainSpriteList, _spriteTypeIndex)) return null;

		Sprite[] terrainSprite = _terrainSpriteList[_spriteTypeIndex][GetTerrainSpriteIndex(terrain)];
		if (CommonModule.IsEmpty(terrainSprite)) return null;
		// 無効なインデクスならランダムなスプライトを返す
		if (!CommonModule.IsEnableIndex(terrainSprite, index)) index = Random.Range(0, terrainSprite.Length);

		return terrainSprite[index];
	}

	/// <summary>
	/// 地形に対応したスプライト名取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	private int GetTerrainSpriteIndex(eTerrain terrain) {
		switch (terrain) {
			case eTerrain.Passage:
			case eTerrain.Room:
				return 0;
			case eTerrain.Wall:
				return 1;
			case eTerrain.Stair:
				return 2;
		}
		return 0;
	}

	/// <summary>
	/// 地形スプライトの種類を指定
	/// </summary>
	/// <param name="index"></param>
	public void SetSpriteType(int index) {
		_spriteTypeIndex = index;
		// マップ背景画像にランダムな壁地形画像を設定
		Sprite BGSprite = GetTerrainSprite(eTerrain.Wall);
		MapSquareManager.instance.SetBGSprite(BGSprite);
	}

}
