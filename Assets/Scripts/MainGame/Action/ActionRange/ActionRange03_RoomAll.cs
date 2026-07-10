using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動者の周囲8マスとそれらに所属する部屋の全てのマス
/// </summary>
public class ActionRange03_RoomAll : ActionRangeBase {
	public override void Execute(CharacterObject sourceCharacter) {
		targetCharacterList.Clear();
		SquareObject baseSquare = MapSquareManager.instance.GetSquare(sourceCharacter.characterData.posX, sourceCharacter.characterData.posY);
		// キャラクターの視界マスを取得
		List<SquareObject> visibleArea = MapUtility.instance.GetVisibleArea(baseSquare);
		// 視界マスに存在するすべてのキャラクターを対象に追加
		for (int i = 0; i < visibleArea.Count; i++) {
			SquareObject square = visibleArea[i];
			if (square == null || !square.existCharacter) continue;
			// マスにいるキャラクターを対象に追加
			CharacterObject target = CharacterManager.instance.GetCharacter(square.squareData.characterID);
			if (target == null) continue;
			// 相対敵でなければ対象にしない
			if (!IsRelativeEnemy(sourceCharacter, target)) continue;
			// 対象リストに追加
			targetCharacterList.Add(square.squareData.characterID);
		}
	}
}
