using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動者の周囲8マスとそれらに所属する部屋の全てのマス
/// </summary>
public class ActionRange03_RoomAll : ActionRangeBase {
	public override void Execute(CharacterObject sourceCharacter) {
		SquareObject baseSquare = MapSquareManager.instance.GetSquare(sourceCharacter.characterData.posX, sourceCharacter.characterData.posY);
		// キャラクターの視界マスを取得
		List<SquareObject> visibleArea = MapUtility.instance.GetVisibleArea(baseSquare);
		// 視界マスに存在するすべてのキャラクターを対象に追加
		for (int i = 0; i < visibleArea.Count; i++) {
			SquareObject square = visibleArea[i];
			if (square == null || !square.existCharacter) continue;
			// マスにいるキャラクターを対象に追加

		}
	}
}
