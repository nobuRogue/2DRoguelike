using UnityEngine;

/// <summary>
/// BGMの割り当てクラス
/// </summary>
[CreateAssetMenu]
public class BGMAssign : ScriptableObject {
	// BGMのリスト
	public AudioClip[] bgmArray = null;
}
