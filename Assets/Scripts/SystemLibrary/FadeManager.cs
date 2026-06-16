using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェード管理
/// </summary>
public class FadeManager : SystemObject {
	public static FadeManager instance = null;

	[SerializeField]
	private Image _fadeImage = null;

	public override async UniTask Initialize() {
		instance = this;
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// フェードアウト、暗くする
	/// </summary>
	/// <returns></returns>
	public async UniTask FadeOut(float duration = 0.3f) {
		await FadeTargetAlpha(1.0f, duration);
	}

	/// <summary>
	/// フェードイン、明るくする
	/// </summary>
	/// <returns></returns>
	public async UniTask FadeIn(float duration = 0.3f) {
		await FadeTargetAlpha(0.0f, duration);
	}

	private async UniTask FadeTargetAlpha(float targetAlpha, float duration) {
		float elapsedSec = 0.0f;
		Color targetColor = _fadeImage.color;
		float startAlpha = targetColor.a;
		while (elapsedSec < duration) {
			elapsedSec += Time.deltaTime;
			targetColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedSec / duration);
			_fadeImage.color = targetColor;

			await UniTask.DelayFrame(1);
		}
		targetColor.a = targetAlpha;
		_fadeImage.color = targetColor;

	}

}
