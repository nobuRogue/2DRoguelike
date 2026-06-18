using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleMenu : MenuBase {

	public async UniTask Execute() {
		await FadeManager.instance.FadeIn();
		// zƒL[“ü—Í‘Ò‚¿
		while (true) {
			if (Input.GetKeyDown(KeyCode.Z)) break;

			await UniTask.DelayFrame(1);
		}
		await FadeManager.instance.FadeOut();
	}

}
