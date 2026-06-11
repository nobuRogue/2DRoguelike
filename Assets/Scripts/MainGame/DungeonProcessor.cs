using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 1ダンジョン実行
/// </summary>
public class DungeonProcessor {
	// フロア実行クラス
	private FloorProcessor _floorProcessor = null;
	// ダンジョン終了要因
	private eDungeonEndReason _eDungeonEndReason = eDungeonEndReason.Invalid;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// フロア実行処理初期化
		_floorProcessor = new FloorProcessor();
		_floorProcessor.Initialize();
	}

	/// <summary>
	/// ダンジョン実行
	/// </summary>
	/// <returns></returns>
	public async UniTask<eDungeonEndReason> Execute() {
		// ダンジョン継続状態に設定
		_eDungeonEndReason = eDungeonEndReason.Invalid;
		// ダンジョンが終了するまでフロア実行処理をループ
		while (_eDungeonEndReason == eDungeonEndReason.Invalid) {
			// 1フロアの実行
			await _floorProcessor.Execute();
		}
		// 終了要因を返す
		return _eDungeonEndReason;
	}

}
