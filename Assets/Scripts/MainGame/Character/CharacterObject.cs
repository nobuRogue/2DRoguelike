using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// キャラクターオブジェクトの見た目情報
/// </summary>
public class CharacterObject : MonoBehaviour {
	// キャラクターの見た目スプライト
	[SerializeField]
	private SpriteRenderer _characterSprite = null;
	// キャラクターの情報
	public CharacterBase characterData { get; private set; } = null;

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="character"></param>
	public void Initialize(CharacterBase character) {
		characterData = character;
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	public void Setup(int ID) {
		characterData.Setup(ID);
		gameObject.SetActive(true);
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
	}

}
