using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 1回の移動アクション
/// </summary>
public class MoveAction {
	// フロア終了処理
	public static System.Action<eFloorEndReason> EndFloor = null;
	// ダンジョン終了処理
	public static System.Action<eDungeonEndReason> EndDungeon = null;

	// 移動キャラ
	private CharacterObject _character = null;
	// 移動情報
	private ChebyshevMoveData _moveData = null;

	/// <summary>
	/// 内部的な移動処理
	/// </summary>
	public void ExecuteData(CharacterObject character, ChebyshevMoveData moveData) {
		// 移動情報のキャッシュ
		_character = character;
		_moveData = moveData;
		// 内部的な移動
		character.characterData.SetSquare(MapSquareManager.instance.GetSquare(moveData.targetSquareID));
		character.SetDirection(moveData.dir);
	}

	/// <summary>
	/// 見た目上の移動処理
	/// </summary>
	/// <returns></returns>
	public async UniTask ExecuteObject(float durationSec) {
		// 移動先、移動元座標の取得
		SquareObject startSquare = MapSquareManager.instance.GetSquare(_moveData.sourceSquareID);
		SquareObject goalSquare = MapSquareManager.instance.GetSquare(_moveData.targetSquareID);
		Vector3 startPos = startSquare.GetCharacterRoot().position;
		Vector3 goalPos = goalSquare.GetCharacterRoot().position;
		// 指定時間かけて補完移動
		float elapsedSec = 0.0f;
		// 歩行アニメーションに切り替え
		_character.SetAnimation(eCharacterAnimation.Walk);
		while (elapsedSec < durationSec) {
			// 経過時間の累積
			elapsedSec += Time.deltaTime;
			// 補完座標の取得
			float t = elapsedSec / durationSec;
			Vector3 movePos = Vector3.Lerp(startPos, goalPos, t);
			_character.SetPosition(movePos);
			// 1フレーム待ち
			await UniTask.DelayFrame(1);
		}
		// ゴール座標に設定
		_character.SetPosition(goalPos);
		// 通常移動後処理
		AfterMoveProcess(goalSquare);
	}

	/// <summary>
	/// 移動後処理
	/// </summary>
	private void AfterMoveProcess(SquareObject goalSquare) {
		// プレイヤーでなければ処理しない
		if (!_character.characterData.IsPlayer()) return;
		// TODO:移動先のマスのアイテムを拾う処理

		// 階段処理
		ProcessStair(goalSquare);



	}

	/// <summary>
	/// 階段による移動処理
	/// </summary>
	/// <param name="goalSquare"></param>
	private void ProcessStair(SquareObject goalSquare) {
		// 移動先が階段でなければ処理しない
		if (goalSquare.squareData.terrain != eTerrain.Stair) return;
		// 階数をインクリメント
		UserData userData = UserDataHolder.instance.currentData;
		userData.SetFloorCount(userData.floorCount + 1);
		Entity_FloorData.Param floorMaster = MasterDataManager.instance.GetFloorData(userData.floorCount);
		if (floorMaster != null) {
			// 次の階があればフロア終了
			EndFloor?.Invoke(eFloorEndReason.Stair);
		}
		else {
			// 次の階がなければダンジョン終了
			EndDungeon?.Invoke(eDungeonEndReason.Clear);
		}
	}

}
