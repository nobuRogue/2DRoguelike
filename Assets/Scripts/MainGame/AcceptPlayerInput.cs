using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーの入力受付、一部行動、処理実行
/// </summary>
public class AcceptPlayerInput {

	// 移動アクションの追加時コールバック
	private System.Action<MoveAction> _addMove = null;
	// アイテムリスト入力の受付
	private AcceptItemList _acceptItemList = null;

	public void Initialize(System.Action<MoveAction> addMove) {
		_addMove = addMove;
		_acceptItemList = new AcceptItemList();
	}

	/// <summary>
	/// プレイヤー入力の受付
	/// </summary>
	/// <returns></returns>
	public async UniTask AcceptInput() {
		while (true) {
			// 移動入力の受付
			if (AcceptMove()) break;
			// 通常攻撃入力の受付
			if (await AcceptAttack()) break;
			// アイテムリストメニュー入力の受付
			if (await AcceptItemList()) break;
			// 方向転換入力の受付
			await AcceptDirChange();
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

	/// <summary>
	/// 方向転換の入力受付、処理
	/// </summary>
	/// <returns></returns>
	private async UniTask AcceptDirChange() {
		// 入力の受付判定
		if (!Input.GetKey(KeyCode.LeftControl)) return;
		// トリガー入力なら隣接エネミーに自動的に方向転換
		if (Input.GetKeyDown(KeyCode.LeftControl)) ChangeDirToEnemy();

		CharacterObject player = CharacterManager.instance.GetPlayer();
		int playerX = player.characterData.posX, playerY = player.characterData.posY;
		// 方向転換キーが離されるまでループ
		while (Input.GetKey(KeyCode.LeftControl)) {
			// 入力に応じて向きを変える
			ChangeCharacterDir(player, AcceptDirInput());
			// 1フレーム待機
			await UniTask.DelayFrame(1);
		}
		// プレイヤーが向いている方向のマスの色を消す
		MapSquareManager.instance.GetToDirSquare(playerX, playerY, player.characterData.direction)?.HideMark();
	}

	/// <summary>
	/// 隣接エネミーの方向を向く
	/// </summary>
	private void ChangeDirToEnemy() {
		// 隣接エネミーの方向に自動的に向く
		CharacterObject player = CharacterManager.instance.GetPlayer();
		int startIndex = (int)player.characterData.direction + 1;
		int playerX = player.characterData.posX, playerY = player.characterData.posY;
		for (int i = 0; i < (int)eDirectionEight.Max; i++) {
			eDirectionEight dir = (startIndex + i).ToDir8();
			SquareObject square = MapSquareManager.instance.GetToDirSquare(playerX, playerY, dir);
			if (square == null || !square.existCharacter) continue;
			// エネミーが居るのでそちらを向く
			ChangeCharacterDir(player, dir);
			return;
		}
		// 隣接エネミーがいない場合、プレイヤーが向いている方向マスに色を付ける
		ChangeCharacterDir(player, player.characterData.direction);
	}

	/// <summary>
	/// 方向転換用キャラ向き変更処理
	/// </summary>
	/// <param name="character"></param>
	/// <param name="dir"></param>
	private void ChangeCharacterDir(CharacterObject character, eDirectionEight dir) {
		if (dir == eDirectionEight.Invalid ||
			dir == eDirectionEight.Max) return;
		// 現在向いている方向のマスの色を消す
		int characterX = character.characterData.posX, characterY = character.characterData.posY;
		MapSquareManager.instance.GetToDirSquare(characterX, characterY, character.characterData.direction)?.HideMark();
		// キャラクターの向きを変更
		character.SetDirection(dir);
		// 現在向いている方向のマスに色を付ける
		MapSquareManager.instance.GetToDirSquare(characterX, characterY, character.characterData.direction)?.ShowMark(Color.red);
	}

	/// <summary>
	/// 攻撃入力の受付
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptAttack() {
		// Zキー入力判定
		if (!Input.GetKeyDown(KeyCode.Z)) return false;
		// 通常攻撃アクション実行
		await ActionManager.instance.ExecuteAction(CharacterManager.instance.GetPlayer(), GameConst.NORMAL_ATTACK_ID);
		return true;
	}

	/// <summary>
	/// アイテムリスト入力の受付
	/// </summary>
	/// <returns></returns>
	private async UniTask<bool> AcceptItemList() {
		// Cキー入力判定
		if (!Input.GetKeyDown(KeyCode.C)) return false;
		// アイテムリスト処理
		return await _acceptItemList.Accept();
	}

}
