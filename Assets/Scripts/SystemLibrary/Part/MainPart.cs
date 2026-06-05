using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// メインのゲームパート
/// </summary>
public class MainPart : PartBase {
	// マップマス管理クラス
	[SerializeField]
	private MapSquareManager mapManager = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		// マップ初期化
		mapManager?.Initialize();
	}

	public override async UniTask Execute() {

	}
}
