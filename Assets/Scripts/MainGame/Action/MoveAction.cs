using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 1回の移動アクション
/// </summary>
public class MoveAction {
	// フロア終了処理
	public static System.Action<eFloorEndReason> EndFloor = null;
	// ダンジョン終了処理
	public static System.Action<eDungeonEndReason> EndDungeon = null;

	// アイテムを拾った時のログメッセージID
	private const int _ADD_ITEM_LOG_ID = 3001;
	// アイテムを拾えなかった時のログメッセージID
	private const int _NOT_ADD_LOG_ID = 3002;
	// 階段選択の任意メッセージID
	private const int _CONFIRM_STAIR_ID = 30;

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
			// キャラクターの移動
			_character.SetPosition(movePos);
			// 1フレーム待ち
			await UniTask.DelayFrame(1);
		}
		// ゴール座標に設定
		_character.SetPosition(goalPos);
		// 通常移動後処理
		await AfterMoveProcess(goalSquare);
	}

	/// <summary>
	/// 移動後処理
	/// </summary>
	private async UniTask AfterMoveProcess(SquareObject goalSquare) {
		// プレイヤーでなければ処理しない
		if (!_character.characterData.IsPlayer()) return;
		// 移動先のマスのオブジェクトに依る処理
		await ProcessSquareObject(goalSquare);
		// 階段処理
		await ProcessStair(goalSquare);
	}

	private async UniTask ProcessSquareObject(SquareObject square) {
		switch (square.squareData.objectType) {
			case eSqaureObjectType.Item:
				// 移動先のマスにアイテムがあるか判定
				ItemObject item = ItemManager.instance.GetItem(square.squareData.objectID);
				if (item == null) return;
				// アイテムがあるなら獲得可否判定（プレイヤーがアイテムを拾えるか）
				RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
				if (!_character.characterData.CanAddItem()) {
					// 拾えなければログを表示して終了
					logMenu.AddLog(string.Format(_NOT_ADD_LOG_ID.ToMessage(), item.itemData.GetName()));
					return;
				}
				// 拾えるならアイテム獲得、ログ表示
				item.SetCharacter(_character);
				logMenu.AddLog(string.Format(_ADD_ITEM_LOG_ID.ToMessage(), item.itemData.GetName()));
				break;
			case eSqaureObjectType.Trap:
				// 罠を踏んだ際の処理
				await ActionManager.instance.StepOnTrap(_character, square.squareData.objectID);
				break;
		}
	}

	/// <summary>
	/// 階段による移動処理
	/// </summary>
	/// <param name="goalSquare"></param>
	private async UniTask ProcessStair(SquareObject goalSquare) {
		// 移動先が階段でなければ処理しない
		if (goalSquare.squareData.terrain != eTerrain.Stair) return;
		// 階段使用の選択待ち
		ConfirmDialog confirmDialog = MenuManager.instance.Get<ConfirmDialog>();
		await confirmDialog.Setup(_CONFIRM_STAIR_ID.ToMessage());
		await confirmDialog.Open();
		await confirmDialog.AcceptInput();
		await confirmDialog.Close();
		// はいが選択されていなければ終了
		if (confirmDialog.result != eConfirmResult.Yes) {
			await UniTask.DelayFrame(5);
			return;
		}
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
