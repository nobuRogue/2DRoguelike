using System.Collections.Generic;
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
}
