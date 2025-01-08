/**
 * @file ActionEffectBase.cs
 * @brief �s���̌��ʂ̊��
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectBase {
	public abstract UniTask Execute( CharacterBase sourceCharacter, ActionRangeBase rangeData );

	public virtual void Teardown() {

	}
}
