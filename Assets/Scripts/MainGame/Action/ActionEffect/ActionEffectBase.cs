using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// s“®Œø‰Ê‚ÌŠî’ê
/// </summary>
public abstract class ActionEffectBase {
	/// <summary>
	/// Œø‰Ê‚ÌÀs
	/// </summary>
	/// <returns></returns>
	public abstract UniTask Execute(
		CharacterObject sourceCharacter, 
		ActionRangeBase range,
		int[] param);

}
