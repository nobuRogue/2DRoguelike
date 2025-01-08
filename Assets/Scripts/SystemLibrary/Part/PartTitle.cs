/**
 * @file PartTitle.cs
 * @brief タイトルパート
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class PartTitle : PartBase {
	public override async UniTask Initialize() {
		await MenuManager.instance.Get<MenuTitle>( "Prefabs/Menu/MenuTitle" ).Initialize();
	}

	public override async UniTask Setup() {

	}

	public override async UniTask Execute() {
		var titleMenu = MenuManager.instance.Get<MenuTitle>();
		await titleMenu.Open();
		await titleMenu.Close();
		if (UserDataHolder.currentData == null) UserDataHolder.SetCurrentData( new UserData() );

		UniTask task = PartManager.instance.TransitionPart( eGamePart.MainGame );
	}

	public override async UniTask Cleannup() {

	}

}
