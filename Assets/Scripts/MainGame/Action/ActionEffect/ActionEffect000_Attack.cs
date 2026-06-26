using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通常攻撃ダメージ効果
/// </summary>
public class ActionEffect000_Attack : ActionEffectBase {
	/// <summary>
	/// 効果の実行
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="range"></param>
	/// <returns></returns>
	public override async UniTask Execute(
		CharacterObject sourceCharacter,
		ActionRangeBase range,
		int[] param) {
		// 行動者の攻撃アニメーション再生
		sourceCharacter.SetAnimation(eCharacterAnimation.Attack);
		// 対象ごとにダメージ付与
		List<int> targetList = range.targetCharacterList;
		int targetCount = targetList.Count;
		List<UniTask> taskList = new List<UniTask>(targetCount);
		int sourceAttack = sourceCharacter.characterData.attack;
		// 威力による攻撃力修正
		sourceAttack *= param[0];
		sourceAttack /= 100;
		for (int i = 0; i < targetCount; i++) {
			// ダメージ = 行動者攻撃力 * (15/16)^対象の防御力
			// 引き算式：レベルデザインが難しい、0除算が発生しない
			// 除算式：レベルデザインがしやすい、0除算が発生する可能性がある
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// ダメージの決定
			int damage = (int)(sourceAttack * Mathf.Pow(15.0f / 16.0f, target.characterData.defense));
			// 対象へのダメージ付与
			taskList.Add(ExecuteDamage(damage, target));
		}
		// 全タスクの終了待ち
		while (sourceCharacter.currentAnim == eCharacterAnimation.Attack) await UniTask.DelayFrame(1);

		await UniTask.WhenAll(taskList);
	}

	/// <summary>
	/// 対象単体のダメージ付与処理
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	private async UniTask ExecuteDamage(int damage, CharacterObject target) {
		// 対象の被ダメージモーション
		target.SetAnimation(eCharacterAnimation.Damage);
		// 被ダメージモーションの終了待ち
		while (target.currentAnim == eCharacterAnimation.Damage) await UniTask.DelayFrame(1);
		// ダメージの付与
		target.characterData.RemoveHP(damage);
		Debug.Log("Add damage " + damage + " to " + target);
		// 死亡判定
		if (target.characterData.isDead) target.Dead();

	}

}
