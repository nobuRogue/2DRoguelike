using Cysharp.Threading.Tasks;
using System.Security.Cryptography;
using UnityEngine;

public class ActionEffect006_Replace : ActionEffectBase {

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// 射程から1体だけ対象を取得
		if (CommonModule.IsEmpty(range.targetCharacterList)) return;

		CharacterObject target = CharacterManager.instance.GetCharacter(range.targetCharacterList[0]);
		if (sourceCharacter == null || target == null) return;
		// 対象と行動者の場所を入れ替える
		SquareObject sourceSquare = MapSquareManager.instance.GetSquare(sourceCharacter.characterData.posX, sourceCharacter.characterData.posY);
		SquareObject targetSquare = MapSquareManager.instance.GetSquare(target.characterData.posX, target.characterData.posY);
		// 対象をマスから取り除く
		target.characterData.RemoveSquare();
		// 行動者を対象のマスに置く
		sourceCharacter.SetSquare(targetSquare);
		// 対象を行動者のマスに置く
		target.SetSquare(sourceSquare);
		await UniTask.DelayFrame(5);
	}

}
