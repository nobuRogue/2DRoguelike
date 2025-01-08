/**
 * @file MenuGameOver.cs
 * @brief ゲームオーバー画面
 * @author yaonobu
 * @date 2025/1/4
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameOver : MenuBase {

	public override async UniTask Open() {
		await base.Open();
		await FadeManager.instance.FadeIn();
		while (!Input.GetKeyDown( KeyCode.Return )) await UniTask.DelayFrame( 1 );

		await FadeManager.instance.FadeOut( Color.black );
	}

}
