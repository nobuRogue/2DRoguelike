using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// エンディングパート
/// </summary>
public class EndingPart : PartBase {
	public override async UniTask Execute() {
		UniTask task = PartManager.instance.TransitionPart(eGamePart.Title);
		await UniTask.CompletedTask;
	}
}
