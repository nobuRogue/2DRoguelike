/**
 * @file PartMain.cs
 * @brief メインゲームパート
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartMain : PartBase {
	public override async UniTask Initialize() {
		TerrainSpriteAssignor.Initialize();
	}

	public override async UniTask Setup() {

	}

	public override async UniTask Execute() {
		while (MapSquareManager.instance == null) await UniTask.DelayFrame( 1 );

		MapSquareManager.instance.Initialize();
		MapCreater.CreateMap();
	}

	public override async UniTask Cleannup() {

	}

}
