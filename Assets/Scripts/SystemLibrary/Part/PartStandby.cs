/**
 * @file PartStandby.cs
 * @brief �����p�[�g
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class PartStandby : PartBase {
	public override async UniTask Initialize() {

	}

	public override async UniTask Setup() {

	}

	public override async UniTask Execute() {
		UniTask task = PartManager.instance.TransitionPart( eGamePart.Title );
	}

	public override async UniTask Cleannup() {

	}

}
