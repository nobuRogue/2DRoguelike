using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視界内のプレイヤー追跡AI
/// </summary>
public class EnemyAI00_ChasePlayer : EnemyAIBase {
	public EnemyAI00_ChasePlayer(int characterID) : base(characterID) {
	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public override void ThinkAction() {
		// プレイヤーが視界に居るか判定
		CharacterObject sourceCharacter = CharacterManager.instance.GetCharacter(_sourceCharacterID);
		SquareObject sourceSquare = MapSquareManager.instance.GetSquare(sourceCharacter.characterData.posX, sourceCharacter.characterData.posY);
		List<SquareObject> visibleArea = MapUtility.instance.GetVisibleArea(sourceSquare);
		CharacterObject player = CharacterManager.instance.GetPlayer();
		SquareObject playerSquare = MapSquareManager.instance.GetSquare(player.characterData.posX, player.characterData.posY);
		bool isInVisbleArea = visibleArea.Exists(element => element == playerSquare);
		if (isInVisbleArea) {
			// TODO:プレイヤーが視界に居るなら可能な移動以外の行動を思考

			// 可能な移動以外の行動がなければプレイヤーに近づく移動

		}
		else {
			// ランダム移動
			RandomMove();
		}
	}

	private void RandomMove() {
		// ランダムに1マス移動する
		CharacterObject sourceCharacter = CharacterManager.instance.GetCharacter(_sourceCharacterID);
		if (sourceCharacter == null) return;

		int sourceX = sourceCharacter.characterData.posX, sourceY = sourceCharacter.characterData.posY;
		// 移動可能な方向を全て取得
		int dirMax = (int)eDirectionEight.Max;
		List<eDirectionEight> canMoveDirList = new List<eDirectionEight>(dirMax);
		for (int i = 0; i < dirMax; i++) {
			eDirectionEight dir = (eDirectionEight)i;
			// キャラクターの隣接マスを取得
			SquareObject square = MapSquareManager.instance.GetToDirSquare(sourceX, sourceY, dir);
			if (square == null) continue;
			// 移動不可なら処理しない
			if (!MapUtility.instance.CanMove(sourceX, sourceY, square, dir)) continue;

			canMoveDirList.Add(dir);
		}
		// 移動可能な方向が1つもなければ終わり
		if (CommonModule.IsEmpty(canMoveDirList)) return;
		// ランダムに決定
		eDirectionEight moveDir = canMoveDirList[Random.Range(0, canMoveDirList.Count)];
		// 内部的な移動
		MoveAction moveAction = new MoveAction();
		SquareObject sourceSquare = MapSquareManager.instance.GetSquare(sourceX, sourceY);
		SquareObject moveSquare = MapSquareManager.instance.GetToDirSquare(sourceX, sourceY, moveDir);
		ChebyshevMoveData moveData = new ChebyshevMoveData(sourceSquare.squareData.ID, moveSquare.squareData.ID, moveDir);
		moveAction.ExecuteData(sourceCharacter, moveData);
		// TurnProcessorの移動リストに追加
		addMove?.Invoke(moveAction);
	}

}
