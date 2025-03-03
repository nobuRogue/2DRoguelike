
public enum eGamePart {
	Invalid = -1,
	Standby,
	Title,
	MainGame,
	Ending,
	Max,
}

public enum eXBoxAcceptInput {
	Button0,
	Button1,
	Button2,
	Button3,
	Button4,
	Button5,
	Button6,
	Button7,
	Button8,
	Button9,

	HorizontalPositive,
	HorizontalNegative,
	VerticalPositive,
	VerticalNegative,
	RT,
	LT,
	RStickHPositive,
	RStickHNegative,
	RStickVPositive,
	RStickVNegative,
	LStickHPositive,
	LStickHNegative,
	LStickVPositive,
	LStickVNegative,
	Max
}

public enum eInputButton {
	Invalid = -1,   //
	Cancel,         // XB_B
	Decide,         // XB_A
	Direction,      // XB_X
	ItemMenu,       // XB_Y

	Minimap,        // XB_LB
	TracementChange,// XB_LT

	iDash,          // XB_RB
	Slant,          // XB_RT

	System,         // XB_START
	Select,         // XB_BACK

	MoveRight,
	MoveLeft,
	MoveUp,
	MoveDown,

	Max,
}

public enum eDirectionFour {
	Invalid = -1,
	Up,
	Right,
	Down,
	Left,
	Max,
}

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
/// ダンジョン終了要因
/// </summary>
public enum eDungeonEndReason {
	Invalid = -1,
	Dead,           // 死亡
	Clear,          // クリア
	ReturnToTitle,  // タイトルに戻る
}

/// <summary>
/// フロア終了要因
/// </summary>
public enum eFloorEndReason {
	Invalid = -1,
	Stair,      // 階段
	Dead,       // 死亡
	Clear,      // クリア
	Escape,     // 脱出
	Event,      // イベント
	ReturnToTitle,// タイトルに戻る
}

/// <summary>
/// 死因タイプ
/// </summary>
public enum eDeadReason {
	Unknown,    // 不明
	Character,  // キャラの行動
	Trap,       // 罠
	Item,       // アイテム
	Hunger,     // 空腹
}
/// <summary>
/// マップマスの地形
/// </summary>
public enum eTerrain {
	Invalid = -1,// 無効
	Passage,
	Room,
	Wall,
	Stair,
	Max,
}

public enum eCharacterAnimation {
	Wait,
	Walk,
	Attack,
	Damage,
	Max,
}

public enum eItemCategory {
	Potion,
	Max
}