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

public enum eCharacterAnimation {
	Invalid = -1,
	Wait,
	Walk,
	Attack,
	Damage,
	Max,
}

// アイテムカテゴリ
public enum eItemCategory {
	Potion, // 薬
	Food,   // 食べ物
	Wand,   // 杖
	Scroll, // 巻物
	Bag,    // バッグ
	Throwing,// 飛び道具
	Weapon, // 武器
	Armor,  // 防具
	Max,
}

// アイテムのコマンド
public enum eItemCommand {
	Invalid = -1,
	Use,        // 使う
	Puton,      // 置く
	Equip,      // 装備
	Remove,     // 装備を外す
}

// マスに設置されるオブジェクトの種類
public enum eSqaureObjectType {
	Invalid = -1,
	Item,
	Trap
}