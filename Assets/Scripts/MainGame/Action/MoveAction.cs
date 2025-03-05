/**
 * @file MoveAction.cs
 * @brief �L�����N�^�[�̈ړ��A�N�V�����N���X
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MoveAction {
	private static System.Action _EndFloor = null;
	private static System.Action<eDungeonEndReason> _EndDungeon = null;

	public static void SetEndProcess( System.Action setEndFloor, System.Action<eDungeonEndReason> setEndDungeon ) {
		_EndFloor = setEndFloor;
		_EndDungeon = setEndDungeon;
	}

	private int _enemyID = -1;
	private bool _isPlayer = false;

	private ChebyshevMoveData _moveData = null;

	/// <summary>
	/// �����I�Ȉړ�����
	/// </summary>
	public void ProcessData( EnemyCharacter enemy, ChebyshevMoveData moveData ) {
		_enemyID = enemy.ID;
		_isPlayer = false;

		_moveData = moveData;
		enemy.SetSquarePosition( MapSquareManager.instance.Get( _moveData.moveSquareID ) );
	}

	public void ProcessData( PlayerCharacter player, ChebyshevMoveData moveData ) {
		_enemyID = -1;
		_isPlayer = true;

		_moveData = moveData;
		player.SetSquarePosition( MapSquareManager.instance.Get( _moveData.moveSquareID ) );
	}

	/// <summary>
	/// �����ڏ�̈ړ�����
	/// </summary>
	/// <param name="duration"></param>
	/// <returns></returns>
	public async UniTask ProcessObject( float duration ) {
		var moveCharacter = GetCharacter();
		Vector3 sourcePos = MapSquareManager.instance.Get( _moveData.sourceSquareID ).GetObjectRoot().position;
		Vector3 movePos = MapSquareManager.instance.Get( _moveData.moveSquareID ).GetObjectRoot().position;
		float elapsedTime = 0.0f;
		moveCharacter.SetDirection( _moveData.dir );
		moveCharacter.SetAnimation( eCharacterAnimation.Walk );
		while (elapsedTime < duration) {
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			moveCharacter.Set3DPosition( Vector3.Lerp( sourcePos, movePos, t ) );
			// 1�t���[���҂�
			await UniTask.DelayFrame( 1 );
		}
		moveCharacter.Set3DPosition( movePos );

		AfterMoveProcess( moveCharacter );
	}

	private void AfterMoveProcess( CharacterBase moveCharacter ) {
		// �v���C���[�Ȃ�t���A�ړ�����
		if (!_isPlayer) return;

		var moveSquare = MapSquareManager.instance.Get( _moveData.moveSquareID );
		ProcessItem( moveSquare );
		ProcessStair( moveSquare, moveCharacter );
	}

	private void ProcessItem( MapSquareData moveSquare ) {
		if (moveSquare.itemID < 0) return;

		CharacterUtility.AddPlayerItem( moveSquare.itemID );
	}

	private void ProcessStair( MapSquareData moveSquare, CharacterBase moveCharacter ) {
		if (moveSquare.terrain != eTerrain.Stair) return;
		// �t���A�ړ�����
		moveCharacter.SetAnimation( eCharacterAnimation.Wait );
		UserData userData = UserDataHolder.currentData;
		int floorCount = userData.floorCount;
		var floorMaster = FloorMasterUtility.GetFloorMaster( floorCount + 1 );
		if (floorMaster == null) {
			// ���̃t���A�}�X�^�[�f�[�^��������΃Q�[���N���A
			_EndDungeon( eDungeonEndReason.Clear );
		} else {
			// ���̃t���A�}�X�^�[�f�[�^������̂Ŏ��̃t���A�ֈړ�
			_EndFloor();
			userData.SetFloorCount( floorCount + 1 );
		}
	}

	private CharacterBase GetCharacter() {
		if (_isPlayer) return CharacterUtility.GetPlayer();

		return CharacterUtility.GetEnemy( _enemyID );
	}

}
