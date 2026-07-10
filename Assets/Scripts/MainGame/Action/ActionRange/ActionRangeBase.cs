using System.Collections.Generic;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

/// <summary>
/// 行動の射程の基底クラス
/// </summary>
public abstract class ActionRangeBase {
	// 射程の対象になるキャラクター
	public List<int> targetCharacterList = null;

	public ActionRangeBase() {
		targetCharacterList = new List<int>();
	}

	/// <summary>
	/// 対象取得の実行処理
	/// </summary>
	public abstract void Execute(CharacterObject sourceCharacter);

	/// <summary>
	/// AI用使用可否判定
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="dir"></param>
	/// <returns></returns>
	public virtual bool CanUse(CharacterObject sourceCharacter, ref eDirectionEight dir) {
		return true;
	}

	/// <summary>
	/// 相対敵か否か判定
	/// </summary>
	/// <returns></returns>
	protected bool IsRelativeEnemy(CharacterObject source, CharacterObject target) {
		if (source == null || target == null) return false;
		// プレイヤーならエネミーを、エネミーならプレイヤーを敵とみなす
		return source.characterData.IsPlayer() != target.characterData.IsPlayer();
	}

}
