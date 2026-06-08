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