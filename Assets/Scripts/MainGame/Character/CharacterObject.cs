using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System.Threading;
using System.Text;

/// <summary>
/// キャラクターオブジェクトの見た目情報
/// </summary>
public class CharacterObject : MonoBehaviour {
	// ダンジョン終了処理
	public static System.Action<eDungeonEndReason> EndDungeon = null;
	// スプライトファイル名指定用
	private static StringBuilder _spriteNameBuilder = null;
	// カメラをプレイヤーから離す距離
	private const float _CAMERA_DISTANCE = -10.0f;
	// スプライトアニメーションの切り替え間隔ミリ秒
	private const int _ANIMATION_DELAY_MILLISEC = 150;
	// スプライト画像のパス
	private const string _SPRITE_PATH = "Design/Sprites/Character/rogue_";
	// アニメーション名の配列
	private readonly string[] _ANIMATION_NAME = new string[] { "_wait", "_walk", "_attack", "_damage" };

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
		_ct = gameObject.GetCancellationTokenOnDestroy();
		if (_spriteNameBuilder == null) _spriteNameBuilder = new StringBuilder();

	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	public void Setup(int ID, int masterID) {
		// マスターデータ取得
		Entity_CharacterData.Param characterMaster = MasterDataManager.instance.GetCharacterData(masterID);
		// アニメーションスプライトのキャッシュ
		int animCount = (int)eCharacterAnimation.Max;
		_animSpriteList = new Sprite[animCount][];
		for (int i = 0; i < animCount; i++) {
			_spriteNameBuilder.Clear();
			_spriteNameBuilder.Append(_SPRITE_PATH);
			_spriteNameBuilder.Append(characterMaster.spriteName);
			_spriteNameBuilder.Append(_ANIMATION_NAME[i]);
			_animSpriteList[i] = Resources.LoadAll<Sprite>(_spriteNameBuilder.ToString());
		}

		characterData.Setup(ID, characterMaster);
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

	/// <summary>
	/// ターン終了時処理
	/// </summary>
	public void OnEndTurn() {
		// ターン終了時処理
		characterData?.OnEndTurn();
		// 死亡判定
		if (characterData.HP <= 0) Dead();

	}

	/// <summary>
	/// 死亡処理
	/// </summary>
	public void Dead() {
		if (characterData.IsPlayer()) {
			// プレイヤーならゲームオーバー
			EndDungeon(eDungeonEndReason.GameOver);
		}
		else {
			// エネミーなら自身を削除する
			CharacterManager.instance.DeleteCharacter(this);
		}
	}
}
