using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// タイトルパート
/// </summary>
public class TitlePart : PartBase {
	public override async UniTask Execute() {
		// メインパートに遷移
		UniTask task = PartManager.instance.TransitionPart(eGamePart.MainGame);
	}
}
