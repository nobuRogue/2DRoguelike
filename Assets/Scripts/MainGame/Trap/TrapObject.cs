using UnityEngine;

/// <summary>
/// 罠の見た目情報
/// </summary>
public class TrapObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _trapSprite = null;
	// 内部情報への参照
	public TrapData trapData { get; private set; } = null;

	/// <summary>
	/// 初期化
	/// 一度しか呼ばれない
	/// </summary>
	public void Initilize() {
		trapData = new TrapData();
		// 不可視化
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	/// <param name="ID"></param>
	/// <param name="square"></param>
	public void Setup(int ID, SquareObject square, int masterID) {
		Entity_TrapData.Param masterData = MasterDataManager.instance.GetTrapData(masterID);
		// スプライトの設定
		_trapSprite.sprite = TrapManager.instance.GetTrapSprite(0);
		trapData.Setup(ID, square, masterData);
		// マスに設置
		transform.position = square.GetObjectRoot().position;
	}

	/// <summary>
	/// 使用後片付け
	/// </summary>
	public void Teardown() {
		trapData.Teardown();
		// 不可視化
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 可視化
	/// </summary>
	public void Show() {
		gameObject.SetActive(true);
	}

	/// <summary>
	/// 名前取得
	/// </summary>
	/// <returns></returns>
	public string GetName() {
		return trapData.trapMaster.nameID.ToMessage();
	}

}
