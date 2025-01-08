/**
 * @file ActionRangeBase.cs
 * @brief �˒��̊��N���X
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionRangeBase {
	public bool isPlayerTarget = false;
	public List<int> targetEnemyIDList = null;

	/// <summary>
	/// �Ώۂ̌���
	/// </summary>
	/// <param name="sourceCharacter"></param>
	public abstract void Setup( CharacterBase sourceCharacter );

	/// <summary>
	/// AI�̎g�p�۔���
	/// </summary>
	/// <param name="dir"></param>
	/// <returns></returns>
	public virtual bool CanUse( int sourceSquareID, ref eDirectionEight dir ) {
		return true;
	}

	public abstract ActionRangeBase GetInstance();

}
