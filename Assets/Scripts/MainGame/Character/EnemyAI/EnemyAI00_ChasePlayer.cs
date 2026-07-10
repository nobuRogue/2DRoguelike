using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視界内のプレイヤー追跡AI
/// </summary>
public class EnemyAI00_ChasePlayer : EnemyAIBase {
	public EnemyAI00_ChasePlayer(int characterID, int[] actionIDList) : base(characterID, actionIDList) {
	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public override void ThinkAction() {
		CharacterObject sourceCharacter = CharacterManager.instance.GetCharacter(_sourceCharacterID);
		SquareObject sourceSquare = MapSquareManager.instance.GetSquare(sourceCharacter.characterData.posX, sourceCharacter.characterData.posY);
		CharacterObject player = CharacterManager.instance.GetPlayer();
		SquareObject playerSquare = MapSquareManager.instance.GetSquare(player.characterData.posX, player.characterData.posY);
		// プレイヤーが視界に居るか判定
		if (IsInVisbleArea(sourceSquare, player)) {
			// プレイヤーが視界に居るなら可能な移動以外の行動を思考
			if (ScheduleAction(sourceCharacter)) return;
			// 可能な移動以外の行動がなければプレイヤーに近づく移動
			CloseMoveToPlayer(sourceCharacter, sourceSquare, playerSquare);
		}
		else {
			// ランダム移動
			//RandomMove();
		}
	}

	/// <summary>
	/// プレイヤーが視界に居るか（否か）判定
	/// </summary>
	/// <param name="sourceSquare"></param>
	/// <param name="player"></param>
	/// <returns>プレイヤーが視界に居るならtrue、居なければfalse</returns>
	private bool IsInVisbleArea(SquareObject sourceSquare, CharacterObject player) {
		List<SquareObject> visibleArea = MapUtility.instance.GetVisibleArea(sourceSquare);
		return visibleArea.Exists(player.characterData.ExistMoveTrail);
	}

	/// <summary>
	/// 実行可能な行動を予約
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <returns></returns>
	private bool ScheduleAction(CharacterObject sourceCharacter) {
		if (CommonModule.IsEmpty(_actionIDList)) return false;
		// 行動リストからランダムな1つを選択する
		int actionID = _actionIDList[Random.Range(0, _actionIDList.Count)];
		// 選択された行動の対象有無を判定
		Entity_ActionData.Param actionData = MasterDataManager.instance.GetActionData(actionID);
		ActionRangeBase actionRange = ActionRangeManager.instance.GetRange(actionData.rangType);
		eDirectionEight canUseDir = eDirectionEight.Invalid;
		if (!actionRange.CanUse(sourceCharacter, ref canUseDir)) return false;
		// 予定行動に設定
		_scheduleActionID = actionID;
		return true;
	}

	/// <summary>
	/// プレイヤーに近づく移動実行
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="sourceSquare"></param>
	/// <param name="playerSquare"></param>
	private void CloseMoveToPlayer(CharacterObject sourceCharacter, SquareObject sourceSquare, SquareObject playerSquare) {
		List<ChebyshevMoveData> route = RouteSearcher.instance.RouteSearchChebyshev(sourceSquare.squareData.ID, playerSquare.squareData.ID, CanPass);
		// 経路が1以下なら終了
		if (CommonModule.IsEmpty(route) || route.Count <= 1) return;

		MoveAction moveAction = new MoveAction();
		moveAction.ExecuteData(sourceCharacter, route[0]);
		addMove?.Invoke(moveAction);
	}

	// 経路探索用の通行可否判定
	private bool CanPass(SquareObject sourceSquare, SquareObject targetSquare, eDirectionEight dir) {
		// プレイヤーのいるマスには地形のみの移動可否判定
		CharacterObject character = CharacterManager.instance.GetCharacter(targetSquare.squareData.characterID);
		int sourceX = sourceSquare.squareData.posX, sourceY = sourceSquare.squareData.posY;
		bool isPlayer = character != null && character.characterData.IsPlayer();
		if (isPlayer) return MapUtility.instance.CanMoveTerrain(sourceX, sourceY, targetSquare, dir);
		// 移動可否判定
		return MapUtility.instance.CanMove(sourceX, sourceY, targetSquare, dir);
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
