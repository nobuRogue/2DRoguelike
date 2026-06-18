using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// タイトルパート
/// </summary>
public class TitlePart : PartBase {

	public override async UniTask Initialize() {
		await base.Initialize();
		// メニューの初期化
		MenuManager.instance.Get<TitleMenu>("TitleCanvas").Initialize();
	}

	public override async UniTask Execute() {
		// タイトルメニューの表示
		TitleMenu titleMenu = MenuManager.instance.Get<TitleMenu>();
		await titleMenu.Open();
		// 入力待ち
		await titleMenu.Execute();
		await titleMenu.Close();
		// メインパートに遷移
		UniTask task = PartManager.instance.TransitionPart(eGamePart.MainGame);
		await UniTask.CompletedTask;
	}
}
