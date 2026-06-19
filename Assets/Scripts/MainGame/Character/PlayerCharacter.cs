using UnityEngine;

public class PlayerCharacter : CharacterBase {
	/// <summary>
	/// 満腹度
	/// </summary>
	public int stamina { get; private set; } = 100;

	// プレイヤーか否か
	public override bool IsPlayer() {
		return true;
	}

	/// <summary>
	/// 最大HP設定
	/// </summary>
	/// <param name="maxHP"></param>
	public override void SetMaxHP(int maxHP) {
		base.SetMaxHP(maxHP);
		// UI更新
		RogueMainMenu mainMenu = MenuManager.instance.Get<RogueMainMenu>();
		mainMenu.SetHP(HP, maxHP);
	}

	/// <summary>
	/// 現在HP設定
	/// </summary>
	/// <param name="HP"></param>
	public override void SetHP(int HP) {
		base.SetHP(HP);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetHP(HP, maxHP);
	}
	/// <summary>
	/// 攻撃力設定
	/// </summary>
	/// <param name="attack"></param>
	public override void SetAttack(int attack) {
		base.SetAttack(attack);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetAttack(attack);
	}
	/// <summary>
	/// 防御力設定
	/// </summary>
	/// <param name="defense"></param>
	public override void SetDefense(int defense) {
		base.SetDefense(defense);
		// UI更新
		MenuManager.instance.Get<RogueMainMenu>().SetDefense(defense);
	}
}
