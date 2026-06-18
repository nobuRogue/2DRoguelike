using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲームパートの基底
/// </summary>
public abstract class PartBase : MonoBehaviour {

	/// <summary>
	/// アプリケーション開始時に1回だけ呼ばれる初期化処理
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Initialize() {
		// 自身を非アクティブにする
		gameObject.SetActive(false);
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// パートのメイン処理実行前に呼ばれる使用前準備処理
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Setup() {
		// 自身をアクティブにする
		gameObject.SetActive(true);
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// パートの実行処理
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Execute();

	/// <summary>
	/// パートのメイン処理実行後に呼ばれる片付け処理
	/// </summary>
	/// <returns></returns>
	public virtual async UniTask Teardown() {
		// 自身を非アクティブにする
		gameObject.SetActive(false);
		await UniTask.CompletedTask;
	}

}
