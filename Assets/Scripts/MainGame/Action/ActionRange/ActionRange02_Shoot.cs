using UnityEngine;

/// <summary>
/// 前方10マス射程
/// </summary>
public class ActionRange02_Shoot : ActionRangeBase {

	public override void Execute(CharacterObject sourceCharacter) {
		targetCharacterList.Clear();
		if (sourceCharacter == null) return;
		// 行動者の向いている方向、座標取得
		eDirectionEight dir = sourceCharacter.characterData.direction;
		int currentX = sourceCharacter.characterData.posX;
		int currentY = sourceCharacter.characterData.posY;
		// 10マス射程処理
		int range = 10;
		for (int i = 0; i < range; i++) {
			// 現在座標から向いている方向の隣接マス取得
			SquareObject square = MapSquareManager.instance.GetToDirSquare(currentX, currentY, dir);
			if (square == null) break;
			// キャラクターが居るなら対象に加えて終了
			if (square.existCharacter) {
				targetCharacterList.Add(square.squareData.characterID);
				break;
			}
			// 壁マスなら終了
			if (square.squareData.terrain == eTerrain.Wall) break;
			// 現在座標を隣接マスのものに変更
			currentX = square.squareData.posX;
			currentY = square.squareData.posY;
		}
	}

}
