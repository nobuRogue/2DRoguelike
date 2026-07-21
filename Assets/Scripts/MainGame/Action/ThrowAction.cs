using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// アイテムを投げるアクション
/// </summary>
public class ThrowAction {
	// アイテムを投げた際のログID
	private const int _THROW_LOG_ID = 3013;
	// アイテム投げ演出の1マス分の移動時間
	private const float _THROW_DURATION_SEC = 0.05f;

	public async UniTask ExecuteThrow(CharacterObject sourceCharacter, int itemID) {
		if (sourceCharacter == null) return;

		ItemObject throwItem = ItemManager.instance.GetItem(itemID);
		if (throwItem == null) return;
		// ログの表示
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		logMenu.AddLog(string.Format(_THROW_LOG_ID.ToMessage(), throwItem.itemData.GetName()));
		// 射程処理による対象の取得
		CharacterObject target = null;
		eDirectionEight dir = sourceCharacter.characterData.direction;
		int sourceX = sourceCharacter.characterData.posX, sourceY = sourceCharacter.characterData.posY;
		int currentX = sourceX, currentY = sourceY;
		int range = 10;
		int moveCount;
		for (moveCount = 0; moveCount < range; moveCount++) {
			// 現在座標から行動者の向きの隣接マス取得
			SquareObject square = MapSquareManager.instance.GetToDirSquare(currentX, currentY, dir);
			if (square == null) break;
			// キャラクターが居るなら対象に加えて終了
			if (square.existCharacter) {
				target = CharacterManager.instance.GetCharacter(square.squareData.characterID);
				break;
			}
			// 壁マスなら終了
			if (square.squareData.terrain == eTerrain.Wall) break;
			// 現在座標を隣接マスのものに変更
			currentX = square.squareData.posX;
			currentY = square.squareData.posY;
		}
		// 演出
		SquareObject startSquare = MapSquareManager.instance.GetSquare(sourceX, sourceY);
		SquareObject targetSquare = MapSquareManager.instance.GetSquare(currentX, currentY);
		await MoveItemObject(startSquare.GetObjectRoot().position, targetSquare.GetObjectRoot().position, moveCount, throwItem);
		if (target != null) {
			// 対象のキャラクターが存在するので、アイテムをキャラクターにぶつけた際の処理を実行

		}
		else {
			// 対象のキャラクターが存在しないので、射程末端にアイテムを置く
			// 射程末端に最も近いオブジェクト配置可能マスを取得

			// 取得したマスにアイテムを設置
			throwItem.SetSquare(targetSquare);
		}
	}

	/// <summary>
	/// アイテムを飛ばす演出
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="targetPos"></param>
	/// <param name="moveCount"></param>
	/// <param name="throwItem"></param>
	/// <returns></returns>
	private async UniTask MoveItemObject(Vector3 startPos, Vector3 targetPos, int moveCount, ItemObject throwItem) {
		// 開始、終了位置、所要時間の決定
		float duration = moveCount * _THROW_DURATION_SEC;
		// アイテムの位置を演出のスタート位置に設定する
		throwItem.SetPosition(startPos);
		// アイテムの可視化
		throwItem.SetVisibility(true);
		// 既定の時間をかけてアイテムの位置を行動者の位置から対象の位置に移動させる
		float elapsedSec = 0.0f;
		while (elapsedSec < duration) {
			// 経過時間の累積
			elapsedSec += Time.deltaTime;
			// 補完座標の取得
			float t = elapsedSec / duration;
			Vector3 movePos = Vector3.Lerp(startPos, targetPos, t);
			// アイテムオブジェクトの座標に設定
			throwItem.SetPosition(movePos);
			// 1フレーム待機
			await UniTask.DelayFrame(1);
		}
	}

	/// <summary>
	/// オブジェクト配置可能な最近マスを取得
	/// </summary>
	/// <returns></returns>
	private SquareObject GetNearestPutObjectSquare(SquareObject baseSquare) {
		if (!baseSquare.existObject) return baseSquare;

		int baseX = baseSquare.squareData.posX, baseY = baseSquare.squareData.posY;
		int maxRange = 3;
		for (int i = 1; i <= maxRange; i++) {


		}

		return null;

	}

}
