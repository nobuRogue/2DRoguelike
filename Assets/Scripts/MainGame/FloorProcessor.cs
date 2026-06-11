using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 1フロア実行
/// </summary>
public class FloorProcessor {
	// ターン実行クラス
	private TurnProcessor _turnProcessor = null;
	// フロア終了要因
	private eFloorEndReason _endReason = eFloorEndReason.Invalid;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		_turnProcessor = new TurnProcessor();
	}

	/// <summary>
	/// 1フロア実行処理
	/// </summary>
	/// <returns></returns>
	public async UniTask Execute() {
		// フロア生成、準備
		SetupFloor();
		// フロア終了するまでループ
		while (_endReason == eFloorEndReason.Invalid) {
			// 1ターン処理
			await _turnProcessor.Execute();
		}
		// フロア片付け
		TeardownFloor();
	}

	/// <summary>
	/// フロア生成、準備
	/// </summary>
	private void SetupFloor() {
		// ランダムフロア生成
		MapCreater.instance.CreateMap();
		// フロア継続状態に設定
		_endReason = eFloorEndReason.Invalid;
	}

	/// <summary>
	/// フロア片付け
	/// </summary>
	private void TeardownFloor() {

	}

}
