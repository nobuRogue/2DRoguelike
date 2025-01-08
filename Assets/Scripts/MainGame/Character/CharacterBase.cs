/**
 * @file CharacterBase.cs
 * @brief キャラクターの情報
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase {
	public Vector2Int squarePosition { get; protected set; } = new Vector2Int( -1, -1 );

	public int masterID { get; protected set; } = -1;

	public int maxHP { get; protected set; } = -1;
	public int HP { get; protected set; } = -1;

	public int attack { get; protected set; } = -1;
	public int defense { get; protected set; } = -1;

	public eDirectionEight direction = eDirectionEight.Invalid;
	protected void Setup( Entity_CharacterData.Param masterData, MapSquareData squareData ) {
		SetMaxHP( masterData.HP );
		SetHP( masterData.HP );
		attack = masterData.Attack;
		defense = masterData.Defense;
		SetSquare( squareData );
		SetDirection( eDirectionEight.Down );
	}

	public virtual void Teardown() {

	}

	protected abstract CharacterObject GetObject();

	/// <summary>
	/// 見た目と情報、両方の変更
	/// </summary>
	/// <param name="setPosition"></param>
	/// <param name="set3DPosition"></param>
	public void SetSquare( MapSquareData square ) {
		SetSquarePosition( square );
		Set3DPosition( square.GetCharacterRoot().position );
	}

	/// <summary>
	/// 情報のみの変更
	/// </summary>
	/// <param name="setPosition"></param>
	public virtual void SetSquarePosition( MapSquareData square ) {
		MapSquareData prevSquare = MapSquareUtility.GetSquareData( squarePosition );
		if (prevSquare != null) prevSquare.RemoveCharacter();

		squarePosition = square.squarePosition;
		OnSquare( square.ID );
	}

	/// <summary>
	/// 見た目のみの変更
	/// </summary>
	/// <param name="set3DPosition"></param>
	public abstract void Set3DPosition( Vector3 setPosition );

	public virtual void SetMaxHP( int setMaxHP ) {
		maxHP = setMaxHP;
	}

	public virtual void SetHP( int setHP ) {
		HP = Mathf.Max( setHP, 0 );
	}

	public virtual async UniTask OnEndTurnProcess() {

	}

	public void SetAnimation( eCharacterAnimation setAnim ) {
		GetObject()?.SetAnimation( setAnim );
	}

	/// <summary>
	/// マスに移動した時に行われる内部処理
	/// </summary>
	public virtual void OnSquare( int squareID ) {

	}

	public void SetDirection( eDirectionEight setDir ) {
		if (direction == setDir) return;

		direction = setDir;
		GetObject()?.SetDirection( direction );
	}
}
