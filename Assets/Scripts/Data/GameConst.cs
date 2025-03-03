/**
 * @file GameConst.cs
 * @brief 定数
 * @author yaonobu
 * @date 2025/1/4
 */

public class GameConst {
	// キャラステータス関連
	public static readonly int PLAYER_DEFAULT_STAMINA = 1000;

	// ローグのマップ関連
	public static readonly int MAP_SQUARE_MAX_WIDTH = 32;
	public static readonly int MAP_SQUARE_MAX_HEIGHT = 32;

	public static readonly int MIN_ROOM_SIZE = 3;

	public static readonly int AREA_DEVIDE_COUNT = 8;

	public static readonly int FLOOR_ENEMY_MAX = 16;

	public static readonly int ITEM_MAX = 256;

	// アクション関連
	public static readonly int ATTACK_ACTION_ID = 0;    // 通常攻撃のアクションID
}
