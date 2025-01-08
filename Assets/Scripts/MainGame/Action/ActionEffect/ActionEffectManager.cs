/**
 * @file ActionEffectManager.cs
 * @brief çsìÆÇÃå¯â èàóù
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class ActionEffectManager : MonoBehaviour {
	private static List<ActionEffectBase> _actionEffectList = null;

	public static void Initialize() {
		_actionEffectList = new List<ActionEffectBase>();
		_actionEffectList.Add( new ActionEffect000_Attack() );
	}

	public static async UniTask ExecuteEffect( int effectType, CharacterBase sourceCharacter, ActionRangeBase range ) {
		if (!IsEnableIndex( _actionEffectList, effectType )) return;

		await _actionEffectList[effectType].Execute( sourceCharacter, range );
	}

}
