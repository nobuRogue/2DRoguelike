// 列挙体定義

/// <summary>
/// ゲームのパート
/// </summary>
public enum eGamePart {
	Invalid = -1,
	Title,
	MainGame,
	Ending,
}

/// <summary>
/// マスの地形情報
/// </summary>
public enum eTerrain {
	Invalid = -1,
	Passage,    // 通路
	Room,       // 部屋
	Wall,       // 壁
	Stair,      // 階段
}

/// <summary>
/// 4方向
/// </summary>
public enum eDirectionFour {
	Invalid = -1,
	Up,
	Right,
	Down,
	Left,
	Max,
}
/// <summary>
/// 8方向
/// </summary>
public enum eDirectionEight {
	Invalid = -1,
	Up,
	UpRight,
	Right,
	DownRight,
	Down,
	DownLeft,
	Left,
	UpLeft,
	Max,
}

/// <summary>
/// フロア終了要因
/// </summary>
public enum eFloorEndReason {
	Invalid,    // 終了していない
	GameOver,   // ゲームオーバー
	Stair,      // 階段で次のフロアへ
}

/// <summary>
/// ダンジョン終了要因
/// </summary>
public enum eDungeonEndReason {
	Invalid,    // 終了していない
	GameOver,   // ゲームオーバー
	Clear,      // ダンジョンクリア
}