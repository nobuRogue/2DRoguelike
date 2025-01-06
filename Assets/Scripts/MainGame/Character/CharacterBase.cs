/**
 * @file CharacterBase.cs
 * @brief キャラクターの情報
 * @author yaonobu
 * @date 2025/1/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase {
	private static System.Func<int, CharacterObject> _GetObject = null;

	public static void SetObejectGetProcess( System.Func<int, CharacterObject> setProcess ) {
		_GetObject = setProcess;
	}
	public int ID { get; private set; }
	public Vector2Int position { get; private set; } = Vector2Int.zero;

	public int HP { get; private set; } = -1;

	public void Setup( int setID, Vector2Int setPosition ) {
		ID = setID;
		_GetObject( ID )?.Setup();
		SetSquarePosition( setPosition );
	}

	public void Teardown() {
		_GetObject( ID )?.Teardown();
		ID = -1;
	}

	public void SetSquarePosition( Vector2Int setPosition ) {
		position = setPosition;
		_GetObject( ID ).SetSquarePostion();
	}

	public void SetPosition() {

	}

}
