/**
 * @file CharacterBase.cs
 * @brief �L�����N�^�[�̏��
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public abstract class CharacterBase {
	private static readonly int _POSSESS_ITEM_MAX = 10;

	public Vector2Int squarePosition { get; protected set; } = new Vector2Int( -1, -1 );

	public int masterID { get; protected set; } = -1;

	public int maxHP { get; protected set; } = -1;
	public int HP { get; protected set; } = -1;

	public int attack { get; protected set; } = -1;
	public int defense { get; protected set; } = -1;

	public eDirectionEight direction = eDirectionEight.Invalid;

	public int[] possessItemList { get; private set; } = null;

	protected void Setup( Entity_CharacterData.Param masterData, MapSquareData squareData ) {
		SetMaxHP( masterData.HP );
		SetHP( masterData.HP );
		attack = masterData.Attack;
		defense = masterData.Defense;
		SetSquare( squareData );
		SetDirection( eDirectionEight.Down );

		possessItemList = new int[_POSSESS_ITEM_MAX];
		for (int i = 0; i < _POSSESS_ITEM_MAX; i++) {
			possessItemList[i] = -1;
		}
	}

	public virtual void Teardown() {

	}

	protected abstract CharacterObject GetObject();

	/// <summary>
	/// �����ڂƏ��A�����̕ύX
	/// </summary>
	/// <param name="setPosition"></param>
	/// <param name="set3DPosition"></param>
	public void SetSquare( MapSquareData square ) {
		SetSquarePosition( square );
		Set3DPosition( square.GetObjectRoot().position );
	}

	/// <summary>
	/// ���݂̂̕ύX
	/// </summary>
	/// <param name="setPosition"></param>
	public virtual void SetSquarePosition( MapSquareData square ) {
		MapSquareData prevSquare = MapSquareUtility.GetSquareData( squarePosition );
		if (prevSquare != null) prevSquare.RemoveCharacter();

		squarePosition = square.squarePosition;
		OnSquare( square.ID );
	}

	/// <summary>
	/// �����ڂ݂̂̕ύX
	/// </summary>
	/// <param name="set3DPosition"></param>
	public abstract void Set3DPosition( Vector3 setPosition );

	public virtual void SetMaxHP( int setMaxHP ) {
		maxHP = setMaxHP;
	}

	public virtual void SetHP( int setHP ) {
		HP = Mathf.Clamp( setHP, 0, maxHP );
	}

	public virtual async UniTask OnEndTurnProcess() {

	}

	public void SetAnimation( eCharacterAnimation setAnim ) {
		GetObject()?.SetAnimation( setAnim );
	}

	/// <summary>
	/// �}�X�Ɉړ��������ɍs�����������
	/// </summary>
	public virtual void OnSquare( int squareID ) {

	}

	public void SetDirection( eDirectionEight setDir ) {
		if (direction == setDir) return;

		direction = setDir;
		GetObject()?.SetDirection( direction );
	}

	/// <summary>
	/// �_���[�W��^����
	/// </summary>
	/// <param name="damageValue"></param>
	public virtual void Damage( int damageValue ) {
		SetHP( HP - damageValue );
	}

	public eCharacterAnimation GetCurrentAnimation() {
		return GetObject().currenAnim;
	}

	public bool IsDead() {
		return HP <= 0;
	}

	/// <summary>
	/// �A�C�e�������ł��邩
	/// </summary>
	/// <returns></returns>
	public bool CanAddItem() {
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (possessItemList[i] < 0) return true;

		}
		return false;
	}

	/// <summary>
	/// �����A�C�e���ǉ�
	/// </summary>
	/// <param name="itemID"></param>
	public void AddItem( int itemID ) {
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (possessItemList[i] >= 0) continue;

			possessItemList[i] = itemID;
			return;
		}
	}

	/// <summary>
	/// ID�w��̏����A�C�e���폜
	/// </summary>
	/// <param name="itemID"></param>
	public void RemoveIDItem( int itemID ) {
		bool doneRemove = false;
		for (int i = 0, max = possessItemList.Length; i < max; i++) {
			if (!doneRemove) doneRemove = possessItemList[i] == itemID;

			if (!doneRemove) continue;

			if (IsEnableIndex( possessItemList, i + 1 )) {
				possessItemList[i] = possessItemList[i + 1];
			} else {
				possessItemList[i] = -1;
			}
		}
	}

	/// <summary>
	/// ID�w��̏����A�C�e���폜
	/// </summary>
	/// <param name="itemIndex"></param>
	public void RemoveIndexItem( int itemIndex ) {
		if (!IsEnableIndex( possessItemList, itemIndex )) return;

		for (int i = itemIndex, max = possessItemList.Length; i < max; i++) {
			if (IsEnableIndex( possessItemList, i + 1 )) {
				possessItemList[i] = possessItemList[i + 1];
			} else {
				possessItemList[i] = -1;
			}
		}
	}

}
