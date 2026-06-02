using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲーム全体で使用される機能の基底
/// </summary>
public abstract class SystemObject : MonoBehaviour {
	/// <summary>
	/// 初期化処理
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Initialize();
}
