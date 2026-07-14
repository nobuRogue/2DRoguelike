using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffect008_Knockback : ActionEffectBase {
	// 吹き飛ばしによる1マスの移動にかかる時間[秒]
	private const float _KNOCKBACK_DURATION_SEC = 0.05f;
	// 衝突ダメージ
	private const int _CLASH_DAMAGE = 10;

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 吹き飛ばしマス数の取得
		if (CommonModule.IsEmpty(param)) return;

		int knockbackCount = param[0];
		// 吹き飛ばし方向の決定
		eDirectionEight knockbackDir = sourceCharacter.characterData.direction;
		// すべての対象を吹き飛ばす
		List<int> targetList = range.targetCharacterList;
		for (int i = 0; i < targetList.Count; i++) {
			// 対象キャラ取得
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;
			// 対象1体をノックバックさせる
			await Knockback(target, knockbackCount, knockbackDir);
		}
	}

	private async UniTask Knockback(CharacterObject target, int knockbackCount, eDirectionEight dir) {
		// 指定回数ノックバック
		int currentX = target.characterData.posX, currentY = target.characterData.posY;
		bool isClash = false;
		SquareObject targetSquare = MapSquareManager.instance.GetSquare(currentX, currentY);
		// スタート座標をキャッシュしておく
		Vector3 startPos = targetSquare.GetCharacterRoot().position;
		int moveCount = 0;
		for (moveCount = 0; moveCount < knockbackCount; moveCount++) {
			// 目的地マスと衝突有無を取得
			SquareObject square = MapSquareManager.instance.GetToDirSquare(currentX, currentY, dir);
			// 壁かキャラがいた場合、衝突したとして終了
			if (square == null ||
				square.squareData.terrain == eTerrain.Wall ||
				square.existCharacter) {
				isClash = true;
				break;
			}
			currentX = square.squareData.posX;
			currentY = square.squareData.posY;
			targetSquare = square;
		}
		Vector3 targetPos = targetSquare.GetCharacterRoot().position;
		// 見た目上の移動
		float duration = moveCount * _KNOCKBACK_DURATION_SEC;
		float elapsedSec = 0.0f;
		while (elapsedSec < duration) {
			// 経過時間の累積
			elapsedSec += Time.deltaTime;
			// 補完座標の取得
			float t = elapsedSec / duration;
			Vector3 movePos = Vector3.Lerp(startPos, targetPos, t);
			// キャラクターの移動
			target.SetPosition(movePos);
			// 1フレーム待ち
			await UniTask.DelayFrame(1);
		}
		// 目的マスに移動
		target.SetSquare(targetSquare);
		// 衝突が発生していなければ終了
		if (!isClash) return;
		// 対象に固定10ダメージ
		await ExecuteDamage(_CLASH_DAMAGE, target);
	}

}
