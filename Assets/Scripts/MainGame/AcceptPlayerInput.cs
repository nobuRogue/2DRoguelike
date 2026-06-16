using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーの入力受付、一部行動、処理実行
/// </summary>
public class AcceptPlayerInput {

	// 移動アクションの追加時コールバック
	private System.Action<MoveAction> _addMove = null;

	public void Initialize(System.Action<MoveAction> addMove) {
		_addMove = addMove;
	}

	public async UniTask AcceptInput() {
		while (true) {
			// プレイヤー入力の受付
			// 移動入力の受付
			if (AcceptMove()) break;
			// 1フレーム待機
			await UniTask.DelayFrame(1);
		}
	}

	/// <summary>
	/// 移動入力の受付
	/// </summary>
	/// <returns>移動を実行したらtrue</returns>
	public bool AcceptMove() {
		// 8方向の移動入力の受付
		eDirectionEight inputDir = AcceptDirInput();
		if (inputDir == eDirectionEight.Invalid) return false;
		// 移動可否の判定
		CharacterObject player = CharacterManager.instance.GetPlayer();
		int playerX = player.characterData.posX, playerY = player.characterData.posY;
		SquareObject moveSquare = MapSquareManager.instance.GetToDirSquare(playerX, playerY, inputDir);
		if (!MapUtility.instance.CanMove(playerX, playerY, moveSquare, inputDir)) return false;
		// 移動可能なので移動する
		// MoveAction生成
		MoveAction moveAction = new MoveAction();
		// 受け付けた入力に応じて内部的に移動
		SquareObject playerSquare = MapSquareManager.instance.GetSquare(playerX, playerY);
		ChebyshevMoveData moveData = new ChebyshevMoveData(playerSquare.squareData.ID, moveSquare.squareData.ID, inputDir);
		moveAction.ExecuteData(player, moveData);
		// TurnProcessorの移動リストに追加
		_addMove?.Invoke(moveAction);
		return true;
	}

	/// <summary>
	/// 入力されている移動方向を返す
	/// </summary>
	/// <returns></returns>
	private eDirectionEight AcceptDirInput() {
		if (Input.GetKey(KeyCode.UpArrow)) {
			// 上キーが押されている
			if (Input.GetKey(KeyCode.RightArrow)) {
				// 右上
				return eDirectionEight.UpRight;
			}
			else if (Input.GetKey(KeyCode.LeftArrow)) {
				// 左上
				return eDirectionEight.UpLeft;
			}
			return eDirectionEight.Up;
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			// 下キーが押されている
			if (Input.GetKey(KeyCode.RightArrow)) {
				// 右下
				return eDirectionEight.DownRight;
			}
			else if (Input.GetKey(KeyCode.LeftArrow)) {
				// 左下
				return eDirectionEight.DownLeft;
			}
			return eDirectionEight.Down;
		}
		else {
			// 上も下も押されていない
			if (Input.GetKey(KeyCode.RightArrow)) {
				// 右
				return eDirectionEight.Right;
			}
			else if (Input.GetKey(KeyCode.LeftArrow)) {
				// 左
				return eDirectionEight.Left;
			}
		}
		// 何も押されていない
		return eDirectionEight.Invalid;
	}

}
