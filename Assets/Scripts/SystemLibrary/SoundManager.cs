using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class SoundManager : SystemObject {
	// 自身への参照
	public static SoundManager instance;

	// BGM再生用コンポーネント
	[SerializeField]
	private AudioSource _bgmAudioSource = null;
	// SE再生用コンポーネント
	[SerializeField]
	private AudioSource[] _seAudioSourceList = null;
	// BGMリスト
	[SerializeField]
	private BGMAssign _bgmAssign = null;
	// SEリスト
	[SerializeField]
	private SEAssign _seAssign = null;
	// 中断用トークン
	private CancellationToken _ct;

	public override async UniTask Initialize() {
		instance = this;
		_ct = gameObject.GetCancellationTokenOnDestroy();
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// BGM再生
	/// </summary>
	/// <param name="bgmID"></param>
	public void PlayBGM(int bgmID) {
		if (!CommonModule.IsEnableIndex(_bgmAssign.bgmArray, bgmID)) return;
		// AudioClip取得
		AudioClip playBGM = _bgmAssign.bgmArray[bgmID];
		// 再生
		_bgmAudioSource.clip = playBGM;
		_bgmAudioSource.Play();
	}

	/// <summary>
	/// BGM再生停止
	/// </summary>
	public void StopBGM() {
		_bgmAudioSource.Stop();
	}

	/// <summary>
	/// SE再生
	/// </summary>
	/// <param name="seID"></param>
	/// <returns></returns>
	public async UniTask PlaySE(int seID) {
		if (!CommonModule.IsEnableIndex(_seAssign.seArray, seID)) return;
		// AudioClip取得
		AudioClip playSE = _seAssign.seArray[seID];
		// 再生中でないオーディオソースを探して再生
		for (int i = 0; i < _seAudioSourceList.Length; i++) {
			AudioSource audioSource = _seAudioSourceList[i];
			if (audioSource == null ||
				audioSource.isPlaying) continue;
			// SE再生
			audioSource.clip = playSE;
			audioSource.Play();
			// SEの再生終了待ち
			while (audioSource.isPlaying) await UniTask.DelayFrame(1, cancellationToken: _ct);

			return;
		}
	}

}
