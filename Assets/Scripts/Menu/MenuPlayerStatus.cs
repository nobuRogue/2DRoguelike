/**
 * @file MenuPlayerStatus.cs
 * @brief メインゲーム中のプレイヤーステータス表示
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuPlayerStatus : MenuBase {
	[SerializeField]
	private TextMeshProUGUI _FloorCountText = null;

	[SerializeField]
	private TextMeshProUGUI _HPText = null;

	[SerializeField]
	private TextMeshProUGUI _StaminaText = null;

	public void SetFloorCount( int setCount ) {
		_FloorCountText.text = setCount.ToString() + "F";
	}

	public void SetPlayerHP( int currentHP, int maxHP ) {
		_HPText.text = currentHP.ToString() + "/" + maxHP.ToString();
		float hpRatio = (float)currentHP / maxHP;
		if (hpRatio < 0.2f) {
			_HPText.color = Color.red;
		} else if (hpRatio > 0.5) {
			_HPText.color = Color.white;
		} else {
			_HPText.color = Color.yellow;
		}
	}

	public void SetPlayerStamina( int setStamina ) {
		_StaminaText.text = (setStamina / 10).ToString() + "%";
		if (setStamina <= 0) {
			_StaminaText.color = Color.red;
		} else if (setStamina <= 500) {
			_StaminaText.color = Color.yellow;
		} else {
			_StaminaText.color = Color.white;
		}

	}

}
