/**
 * @file FadeManager.cs
 * @brief フェードイン、アウトを行う
 * @author yaonobu
 * @date 2020/11/11
 */
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class FadeManager : SystemObject {
	/// <summary>
	/// フェード画像
	/// </summary>
	[SerializeField]
	private Image _fadeImage = null;

	private float _DEFAULT_DURATION = 0.3f;

	public static FadeManager instance { get; private set; } = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		instance = this;
	}

	/// <summary>
	/// フェードアウト
	/// 暗くなる
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public async UniTask FadeOut( Color color, float duration = -1 ) {
		await FadeTargetAlpha( color, 1.0f, duration );
	}

	/// <summary>
	/// フェードイン
	/// 明るくなる
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public async UniTask FadeIn( float duration = -1 ) {
		await FadeTargetAlpha( Color.black, 0.0f, duration );
	}

	/// <summary>
	/// 指定透明度までラープ
	/// </summary>
	/// <param name="targetAlpha"></param>
	/// <param name="duration"></param>
	/// <returns></returns>
	private async UniTask FadeTargetAlpha( Color color, float targetAlpha, float duration ) {
		// 二重では走らせない
		if (_fadeImage == null) return;

		float elapsedTime = 0.0f;
		//float startAlpha = _fadeImage.color.a;
		Color startColor = _fadeImage.color;
		Color targetColor = color;
		targetColor.a = targetAlpha;
		if (duration < 0) duration = _DEFAULT_DURATION;

		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			Color imageColor = Color.Lerp( startColor, targetColor, t );
			_fadeImage.color = imageColor;
			await UniTask.DelayFrame( 1 );
		}
		_fadeImage.color = targetColor;
	}
}
