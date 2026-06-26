using UnityEngine;

/// <summary>
/// キャラクターの前方1マス
/// </summary>
public class ActionRange00_DirForward : ActionRangeBase {
	/// <summary>
	/// 対象取得の実行処理
	/// </summary>
	public override void Execute(CharacterObject sourceCharacter) {
		targetCharacterList.Clear();
		// 行動者の取得
		if (sourceCharacter == null) return;
		// 行動者の向いている方向隣接1マスの取得
		CharacterBase character = sourceCharacter.characterData;
		SquareObject targetSquare = MapSquareManager.instance.GetToDirSquare(character.posX, character.posY, character.direction);
		// 攻撃可能マスか判定
		if (!CanRangeTarget(character.posX, character.posY, targetSquare, character.direction)) return;
		// 隣接1マスにキャラクターがいる場合それを対象に追加
		CharacterObject target = CharacterManager.instance.GetCharacter(targetSquare.squareData.characterID);
		if (target == null) return;
		// 対象に追加
		targetCharacterList.Add(target.characterData.ID);
	}

	/// <summary>
	/// 角抜け不可判定
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="moveSquare"></param>
	/// <param name="moveDir"></param>
	/// <returns></returns>
	private bool CanRangeTarget(int startX, int startY, SquareObject moveSquare, eDirectionEight moveDir) {
		// 対象のマスが壁地形なら対象不可
		if (!moveSquare.squareData.terrain.CanRangeTarget()) return false;
		// 斜め方向でなければ対象可能
		if (!moveDir.IsSlant()) return true;
		// 斜め方向なら、方向を分割し各方向の地形判定
		eDirectionFour[] separateDir = moveDir.Separate();
		for (int i = 0; i < separateDir.Length; i++) {
			SquareObject square = MapSquareManager.instance.GetToDirSquare(startX, startY, separateDir[i]);
			if (square == null) continue;
			// 対象可能なら継続
			if (square.squareData.terrain.CanRangeTarget()) continue;
			// 対象不可
			return false;
		}
		return true;
	}

}
