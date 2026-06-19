using TMPro;
using UnityEngine;

/// <summary>
/// インゲームの上部常設メニュー
/// </summary>
public class RogueMainMenu : MenuBase {
	// 階層表示
	[SerializeField]
	private TextMeshProUGUI _floorCount = null;
	// HP表示
	[SerializeField]
	private TextMeshProUGUI _HPText = null;
	// 満腹度表示
	[SerializeField]
	private TextMeshProUGUI _staminaText = null;
	// 攻撃力表示
	[SerializeField]
	private TextMeshProUGUI _attackText = null;
	// 防御力表示
	[SerializeField]
	private TextMeshProUGUI _defenseText = null;


	// 階数表示メッセージID
	private const int _FLOOR_MESSAGEID = 0;
	// HP表示用メッセージID
	private const int _HP_MESSAGEID = 1;
	// 満腹度表示用メッセージID
	private const int _STAMINA_MESSAGEID = 2;

	/// <summary>
	/// 階数表示更新
	/// </summary>
	public void SetFloorCount(int floorCount) {
		_floorCount.text = string.Format(_FLOOR_MESSAGEID.ToMessage(), floorCount);
	}

	/// <summary>
	/// HP表示更新
	/// </summary>
	/// <param name="currentHP"></param>
	/// <param name="maxHP"></param>
	public void SetHP(int currentHP, int maxHP) {
		_HPText.text = string.Format(_HP_MESSAGEID.ToMessage(), currentHP, maxHP);
	}

	/// <summary>
	/// 満腹度表示更新
	/// </summary>
	/// <param name="stamina"></param>
	public void SetStamina(int stamina) {
		_staminaText.text = string.Format(_STAMINA_MESSAGEID.ToMessage(), stamina);
	}

	/// <summary>
	/// 攻撃力表示更新
	/// </summary>
	/// <param name="attack"></param>
	public void SetAttack(int attack) {
		_attackText.text = attack.ToString();
	}

	/// <summary>
	/// 防御力表示更新
	/// </summary>
	public void SetDefense(int defense) {
		_defenseText.text = defense.ToString();
	}

}
