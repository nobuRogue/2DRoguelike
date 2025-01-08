/**
 * @file EnemyCharacter.cs
 * @brief �G�l�~�[�L�����N�^�[
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase {
	/// <summary>
	/// �I�u�W�F�N�g���擾����R�[���o�b�N
	/// </summary>
	private static System.Func<int, CharacterObject> _GetObject = null;
	public static void SetObejectGetProcess( System.Func<int, CharacterObject> setProcess ) {
		_GetObject = setProcess;
	}

	public int ID { get; private set; } = -1;

	private EnemyAIBase _enemyAI = null;

	public void Setup( int setID, int setMasterID, MapSquareData squareData ) {
		ID = setID;
		masterID = setMasterID;
		var masterData = CharacterMasterUtility.GetCharacterMaster( masterID );
		Setup( masterData, squareData );
		GetObject()?.Setup( masterData );
		_enemyAI = new EnemyAI_00Normal( () => this );
	}

	public override void Teardown() {
		base.Teardown();
		GetObject()?.Teardown();
		ID = -1;
	}

	protected override CharacterObject GetObject() {
		return _GetObject( ID );
	}


	/// <summary>
	/// �}�X���݂̂̕ύX
	/// </summary>
	/// <param name="setPosition"></param>
	public override void SetSquarePosition( MapSquareData square ) {
		base.SetSquarePosition( square );
		square.SetEnemy( ID );
	}

	/// <summary>
	/// �����ڈʒu�݂̂̕ύX
	/// </summary>
	/// <param name="set3DPosition"></param>
	public override void Set3DPosition( Vector3 set3DPosition ) {
		GetObject().SetPostion( set3DPosition );
	}

	public MoveAction ThinkAction() {
		return _enemyAI.ThinkAction();
	}

	public async UniTask ExecuteScheduleAction() {
		await _enemyAI.ExecuteScheduleAction();
	}
}
