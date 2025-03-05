/**
 * @file MoveAction.cs
 * @brief キャラクターの移動アクションクラス
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
	/// 内部的な移動処理
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
	/// 見た目上の移動処理
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
			// 1フレーム待ち
			await UniTask.DelayFrame( 1 );
		}
		moveCharacter.Set3DPosition( movePos );

		AfterMoveProcess( moveCharacter );
	}

	private void AfterMoveProcess( CharacterBase moveCharacter ) {
		// プレイヤーならフロア移動判定
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
		// フロア移動する
		moveCharacter.SetAnimation( eCharacterAnimation.Wait );
		UserData userData = UserDataHolder.currentData;
		int floorCount = userData.floorCount;
		var floorMaster = FloorMasterUtility.GetFloorMaster( floorCount + 1 );
		if (floorMaster == null) {
			// 次のフロアマスターデータが無ければゲームクリア
			_EndDungeon( eDungeonEndReason.Clear );
		} else {
			// 次のフロアマスターデータがあるので次のフロアへ移動
			_EndFloor();
			userData.SetFloorCount( floorCount + 1 );
		}
	}

	private CharacterBase GetCharacter() {
		if (_isPlayer) return CharacterUtility.GetPlayer();

		return CharacterUtility.GetEnemy( _enemyID );
	}

}
