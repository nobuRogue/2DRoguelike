using UnityEngine;

/// <summary>
/// 起点と周囲8マス
/// </summary>
public class ActionRange04_Around8 : ActionRangeBase {

	public override void Execute(CharacterObject sourceCharacter) {
		targetCharacterList.Clear();
		if (sourceCharacter == null) return;
		// 行動者を対象に追加
		targetCharacterList.Add(sourceCharacter.characterData.ID);
		// 起点の取得
		int sourceX = sourceCharacter.characterData.posX, sourceY = sourceCharacter.characterData.posY;
		// 周囲8マスの走査
		for (int i = 0; i < (int)eDirectionEight.Max; i++) {
			eDirectionEight dir = (eDirectionEight)i;
			// 指定方向への隣接マス取得
			SquareObject square = MapSquareManager.instance.GetToDirSquare(sourceX, sourceY, dir);
			if (!square.existCharacter) continue;
			// 対象に追加
			targetCharacterList.Add(square.squareData.characterID);
		}
	}

}
