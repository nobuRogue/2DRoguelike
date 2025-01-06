/**
 * @file CharacterManager.cs
 * @brief キャラクターの管理
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class CharacterManager : MonoBehaviour {
	[SerializeField]
	private CharacterObject _characterObjectOrigin = null;

	[SerializeField]
	private Transform _useObjectRoot = null;

	[SerializeField]
	private Transform _unuseObjectRoot = null;

	public static CharacterManager instance { get; private set; } = null;

	private List<CharacterBase> _useList = null;
	private List<CharacterBase> _unuseList = null;

	private List<CharacterObject> _useObjectList = null;
	private List<CharacterObject> _unuseObjectList = null;

	void Start() {
		instance = this;
	}

	public void Initialize() {
		CharacterBase.SetObejectGetProcess( GetCharacterObject );
		int characterCount = 32;
		_useList = new List<CharacterBase>( characterCount );
		_unuseList = new List<CharacterBase>( characterCount );
		_useObjectList = new List<CharacterObject>( characterCount );
		_unuseObjectList = new List<CharacterObject>( characterCount );
		for (int i = 0; i < characterCount; i++) {
			CreateUnuse();
		}
	}

	private void CreateUnuse() {
		// キャラクターオブジェクト生成して未使用状態にする
		UnuseObject( Instantiate( _characterObjectOrigin ) );
		// キャラクターデータ生成して未使用状態にする
		Unuse( new CharacterBase() );
	}

	/// <summary>
	/// キャラクターとオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseCharacter"></param>
	public void Unuse( CharacterBase unuseCharacter ) {
		CharacterObject unuseCharacterObject = null;
		if (IsEnableIndex( _useObjectList, unuseCharacter.ID )) unuseCharacterObject = _useObjectList[unuseCharacter.ID];

		unuseCharacter.Teardown();
		_useList.Remove( unuseCharacter );
		_unuseList.Add( unuseCharacter );
		if (unuseCharacterObject == null) return;

		UnuseObject( unuseCharacterObject );
	}

	/// <summary>
	/// キャラクターオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseCharacterObject"></param>
	private void UnuseObject( CharacterObject unuseCharacterObject ) {
		_useObjectList.Remove( unuseCharacterObject );
		_unuseObjectList.Add( unuseCharacterObject );
		unuseCharacterObject.transform.SetParent( _unuseObjectRoot );
	}



	public CharacterBase GetCharacter( int ID ) {
		if (!IsEnableIndex( _useList, ID )) return null;

		return _useList[ID];
	}

	private CharacterObject GetCharacterObject( int objectID ) {
		if (!IsEnableIndex( _useObjectList, objectID )) return null;

		return _useObjectList[objectID];
	}

	/// <summary>
	/// 全てのマスに指定した処理を行う
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllSquare( System.Action<CharacterBase> action ) {
		if (action == null) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			action?.Invoke( _useList[i] );
		}

	}

}
