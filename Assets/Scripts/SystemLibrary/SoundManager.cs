/**
 * @file SoundManager.cs
 * @brief サウンド管理
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class SoundManager : SystemObject {
	static public SoundManager instance { get; private set; } = null;

	[SerializeField, Range( 0, 1 ), Tooltip( "マスタ音量" )]
	private float volume = 1;
	[SerializeField, Range( 0, 1 ), Tooltip( "BGMの音量" )]
	private float bgmVolume = 1;
	[SerializeField, Range( 0, 1 ), Tooltip( "内部BGMの音量" )]
	private float systemBgmVolume = 1;
	[SerializeField, Range( 0, 1 ), Tooltip( "SEの音量" )]
	private float seVolume = 1;

	[SerializeField]
	private AudioSource bgmAudioSource;
	[SerializeField]
	private AudioSource seAudioSource;

	[SerializeField]
	private BGMAssign _bgmAssign = null;

	[SerializeField]
	private SEAssign _seAssign = null;

	public float Volume {
		set {
			volume = Mathf.Clamp01( value );
			UpdateVolume();
		}
		get {
			return volume;
		}
	}

	public float BgmVolume {
		set {
			bgmVolume = Mathf.Clamp01( value );
			UpdateVolume();
		}
		get {
			return bgmVolume;
		}
	}

	public float SystemBgmVolume {
		set {
			systemBgmVolume = Mathf.Clamp01( value );
			UpdateVolume();
		}
		get {
			return bgmVolume;
		}
	}

	public float SeVolume {
		set {
			seVolume = Mathf.Clamp01( value );
			UpdateVolume();
		}
		get {
			return seVolume;
		}
	}

	private void UpdateVolume() {
		bgmAudioSource.volume = bgmVolume * systemBgmVolume * volume;
		seAudioSource.volume = seVolume * volume;
	}

	public override async UniTask Initialize() {
		instance = this;
	}

	//BGM再生
	public void PlayBgm( int index ) {
		PlayBgmByClip( GetBGM( index ) );
	}

	public AudioClip GetBGM( int index ) {
		if (_bgmAssign == null || !IsEnableIndex( _bgmAssign.bgmList, index )) return null;

		return _bgmAssign.bgmList[index];
	}

	public void PlayBgmByClip( AudioClip audioClip ) {
		if (audioClip == null) return;

		bgmAudioSource.clip = audioClip;
		bgmAudioSource.loop = true;
		bgmAudioSource.volume = BgmVolume * Volume;
		bgmAudioSource.Play();
	}

	public void StopBgm() {
		bgmAudioSource.Stop();
		bgmAudioSource.clip = null;
	}

	//SE再生
	public async UniTask PlaySE( int index ) {
		await PlaySEByClip( GetSE( index ) );
	}

	public AudioClip GetSE( int index ) {
		if (_seAssign == null || !IsEnableIndex( _seAssign.seList, index )) return null;

		return _seAssign.seList[index];
	}

	public async UniTask PlaySEByClip( AudioClip audioClip ) {
		if (audioClip == null) return;

		seAudioSource.volume = SeVolume * Volume;
		seAudioSource.clip = audioClip;
		seAudioSource.Play();
		while (seAudioSource.isPlaying) await UniTask.DelayFrame( 1 );

	}

	public void StopSE() {
		seAudioSource.Stop();
		seAudioSource.clip = null;
	}

}

