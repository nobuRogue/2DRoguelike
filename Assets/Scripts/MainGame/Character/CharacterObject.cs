using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System.Threading;

/// <summary>
/// キャラクターオブジェクトの見た目情報
/// </summary>
public class CharacterObject : MonoBehaviour {
	// カメラをプレイヤーから離す距離
	private const float _CAMERA_DISTANCE = -10.0f;
	// スプライトアニメーションの切り替え間隔ミリ秒
	private const int _ANIMATION_DELAY_MILLISEC = 150;

	// キャラクターの見た目スプライト
	[SerializeField]
	private SpriteRenderer _characterSprite = null;
	// キャラクターの情報
	public CharacterBase characterData { get; private set; } = null;

	private Sprite[][] _animSpriteList = null;

	// 再生中のアニメーション種類
	public eCharacterAnimation currentAnim { get; private set; } = eCharacterAnimation.Invalid;
	// 再生中のアニメーションスプライト配列のインデクス
	private int _currentAnimIndex = -1;
	// オブジェクトが破棄された際のUnitask中断用トークン
	private CancellationToken _ct;
	// スプライトアニメーションの再生タスク
	private UniTask _animTask;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="character"></param>
	public void Initialize(CharacterBase character) {
		characterData = character;
		gameObject.SetActive(false);
		// アニメーションスプライトのキャッシュ
		_animSpriteList = new Sprite[(int)eCharacterAnimation.Max][];
		_animSpriteList[0] = Resources.LoadAll<Sprite>("Design/Sprites/Character/rogue_player_wait");
		_animSpriteList[1] = Resources.LoadAll<Sprite>("Design/Sprites/Character/rogue_player_walk");
		_animSpriteList[2] = Resources.LoadAll<Sprite>("Design/Sprites/Character/rogue_player_attack");
		_animSpriteList[3] = Resources.LoadAll<Sprite>("Design/Sprites/Character/rogue_player_damage");

		_ct = gameObject.GetCancellationTokenOnDestroy();
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	public void Setup(int ID) {
		characterData.Setup(ID);
		gameObject.SetActive(true);
		SetAnimation(eCharacterAnimation.Wait);

		if (!_animTask.Status.IsCompleted() && !_animTask.Status.IsCanceled()) return;
		// 1度だけアニメーション再生タスク実行
		_animTask = PlayAnimationTask();
	}

	/// <summary>
	/// 使用後片付け
	/// </summary>
	public void Teardown() {
		characterData.Teardown();
		gameObject.SetActive(false);
	}

	/// <summary>
	/// マスにキャラクターを移動
	/// </summary>
	/// <param name="square"></param>
	public void SetSquare(SquareObject square) {
		// 見た目の処理
		SetPosition(square.GetCharacterRoot().position);
		// 内部的な情報の処理
		characterData.SetSquare(square);
	}

	/// <summary>
	/// 3D座標設定
	/// </summary>
	/// <param name="position"></param>
	public void SetPosition(Vector3 position) {
		transform.position = position;
		// プレイヤーでなければ終了
		if (!characterData.IsPlayer()) return;
		// プレイヤーならカメラ移動
		position.z += _CAMERA_DISTANCE;
		CameraManager.instance.MoveCamera(position);
	}

	public void SetDirection(eDirectionEight dir) {
		// データ上の向き変更
		characterData.SetDirection(dir);
		// 見た目上の向き変更
		switch (dir) {
			case eDirectionEight.UpRight:
			case eDirectionEight.Right:
			case eDirectionEight.DownRight:
				// 右向きにする
				Vector3 scale = _characterSprite.transform.localScale;
				scale.x = 1.0f;
				_characterSprite.transform.localScale = scale;
				break;
			case eDirectionEight.DownLeft:
			case eDirectionEight.Left:
			case eDirectionEight.UpLeft:
				// 左向きにする
				scale = _characterSprite.transform.localScale;
				scale.x = -1.0f;
				_characterSprite.transform.localScale = scale;
				break;
		}
	}

	/// <summary>
	///	スプライトアニメーション用のスプライト切り替えタスク
	/// </summary>
	/// <returns></returns>
	private async UniTask PlayAnimationTask() {
		while (true) {
			// 現在のアニメーション取得
			int animIndex = (int)currentAnim;
			if (!CommonModule.IsEnableIndex(_animSpriteList, animIndex)) {
				// 無効なアニメーション処理しない
				await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _ct);
				continue;
			}
			Sprite[] currentAnimSpriteList = _animSpriteList[animIndex];
			// ループ判定、処理
			if (!CommonModule.IsEnableIndex(currentAnimSpriteList, _currentAnimIndex)) AnimationLoopProcess();
			// スプライトの切り替え
			_characterSprite.sprite = currentAnimSpriteList[_currentAnimIndex];
			// 規定秒数待ち、インデクスの増加
			await UniTask.Delay(_ANIMATION_DELAY_MILLISEC, false, PlayerLoopTiming.Update, _ct);
			_currentAnimIndex++;
		}
	}

	/// <summary>
	/// スプライトアニメーションのループ処理
	/// </summary>
	private void AnimationLoopProcess() {
		if (currentAnim.IsLoopAnimation()) {
			// ループアニメーションならインデクスを0に戻す
			_currentAnimIndex = 0;
		}
		else {
			// ループしないアニメーションなら待機に戻す
			SetAnimation(eCharacterAnimation.Wait);
		}
	}

	// アニメーションの再生
	public void SetAnimation(eCharacterAnimation animation) {
		// 再生中のアニメーションなら処理しない
		if (currentAnim == animation) return;

		currentAnim = animation;
		_currentAnimIndex = 0;
	}

}
