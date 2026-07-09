using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動効果の基底
/// </summary>
public abstract class ActionEffectBase {
	// ダメージ付与ログメッセージID
	private const int _ADD_DAMAGE_LOG_ID = 3000;

	/// <summary>
	/// 効果の実行
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Execute(
		CharacterObject sourceCharacter, 
		ActionRangeBase range,
		int[] param);

	/// <summary>
	/// 対象単体のダメージ付与処理
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	protected async UniTask ExecuteDamage(int damage, CharacterObject target) {
		// 対象の被ダメージモーション
		target.SetAnimation(eCharacterAnimation.Damage);
		// 被ダメージモーションの終了待ち
		while (target.currentAnim == eCharacterAnimation.Damage) await UniTask.DelayFrame(1);
		// ダメージの付与
		target.characterData.RemoveHP(damage);
		string logMessage = string.Format(_ADD_DAMAGE_LOG_ID.ToMessage(), target.characterData.GetName(), damage);
		Debug.Log(logMessage);
		// ログメニューの取得
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		// メニューにログを追加
		logMenu.AddLog(logMessage);
		// 死亡判定
		if (target.characterData.isDead) target.Dead();

	}

}
