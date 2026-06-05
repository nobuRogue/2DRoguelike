using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲームのパート管理
/// </summary>
public class PartManager : SystemObject {
	///  シングルトンインスタンスへの参照
	///  カスタムプロパティによってクラス外から参照可、代入不可と設定
	public static PartManager instance { get; private set; } = null;

	/// プロジェクト上のプレハブへの参照
	[SerializeField]
	private PartBase[] _partOriginList = null;
	/// 生成されたパートオブジェクトへの参照
	private PartBase[] _partList = null;
	/// 現在のパート
	private PartBase _currentPart = null;

	public override async UniTask Initialize() {
		// シングルトン：インスタンスが1つしかないことを保証しつつ、その単一のインスタンスをどこからでも参照可能にするデザインパターン
		// ①コンストラクタのアクセシビリティをprivate（クラスの外から new できなくなる）
		// ②public で static な自身への参照を持つ
		instance = this;
		// パート数をキャッシュ
		int partCount = _partOriginList.Length;
		_partList = new PartBase[partCount];
		// 全パートオブジェクトの生成
		for (int i = 0; i < partCount; i++) {
			PartBase origin = _partOriginList[i];
			if (origin == null) continue;
			_partList[i] = Instantiate(origin, transform);
			// パートオブジェクトの初期化処理
			await _partList[i].Initialize();
		}
	}

	/// <summary>
	/// パートの切り替え
	/// </summary>
	/// <param name="nextPart">切り替えるパート</param>
	/// <returns></returns>
	public async UniTask TransitionPart(eGamePart nextPart) {
		// 現在のパートの片付け
		if (_currentPart != null) await _currentPart.Teardown();
		// 次のパートの準備
		_currentPart = _partList[(int)nextPart];
		await _currentPart.Setup();
		// 次のパートの実行
		UniTask task = _currentPart.Execute();
	}

}
