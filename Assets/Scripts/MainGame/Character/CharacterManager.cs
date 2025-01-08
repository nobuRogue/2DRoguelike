/**
 * @file CharacterManager.cs
 * @brief キャラクターの管理
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
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

	private static System.Func<PlayerCharacter> _GetPlayerData = null;
	private static System.Action<PlayerCharacter> _SetPlayerData = null;
	private static System.Func<List<EnemyCharacter>> _GetEnemyDataList = null;

	public static void SetCharacterProcess( System.Func<PlayerCharacter> getPlayerProcess, System.Action<PlayerCharacter> setPlayerProcess, System.Func<List<EnemyCharacter>> getEnemyListProcess ) {
		_GetPlayerData = getPlayerProcess;
		_SetPlayerData = setPlayerProcess;
		_GetEnemyDataList = getEnemyListProcess;
	}

	private CharacterObject _playerObject = null;

	private List<EnemyCharacter> _unuseEnemyList = null;

	private List<CharacterObject> _useEnemyObjectList = null;
	private List<CharacterObject> _unuseEnemyObjectList = null;

	public void Initialize() {
		instance = this;
		EnemyCharacter.SetObejectGetProcess( GetEnemyObject );
		PlayerCharacter.SetObejectGetProcess( GetPlayerObject );

		int enemyCount = GameConst.FLOOR_ENEMY_MAX;
		_unuseEnemyList = new List<EnemyCharacter>( enemyCount );
		_useEnemyObjectList = new List<CharacterObject>( enemyCount );
		_unuseEnemyObjectList = new List<CharacterObject>( enemyCount );
		for (int i = 0; i < enemyCount; i++) {
			CreateUnuseEnemy();
		}
	}

	private void CreateUnuseEnemy() {
		// キャラクターオブジェクト生成して未使用状態にする
		UnuseObject( -1, Instantiate( _characterObjectOrigin ) );
		// キャラクターデータ生成して未使用状態にする
		UnuseEnemy( new EnemyCharacter() );
	}

	/// <summary>
	/// キャラクターとオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseEnemy"></param>
	public void UnuseEnemy( EnemyCharacter unuseEnemy ) {
		// マスから取り除く
		MapSquareUtility.GetSquareData( unuseEnemy.squarePosition )?.RemoveCharacter();
		// 使用リストから取り除く
		int unuseID = unuseEnemy.ID;
		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		if (IsEnableIndex( useEnemyList, unuseID )) useEnemyList[unuseID] = null;
		// 片付けて未使用リストに加える
		unuseEnemy.Teardown();
		_unuseEnemyList.Add( unuseEnemy );
		if (!IsEnableIndex( _useEnemyObjectList, unuseID )) return;
		// オブジェクトの未使用化
		UnuseObject( unuseID, _useEnemyObjectList[unuseID] );
	}

	/// <summary>
	/// キャラクターオブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseCharacterObject"></param>
	private void UnuseObject( int unuseID, CharacterObject unuseCharacterObject ) {
		if (unuseCharacterObject == null) return;

		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		if (IsEnableIndex( useEnemyList, unuseID )) _useEnemyObjectList[unuseID] = null;

		_unuseEnemyObjectList.Add( unuseCharacterObject );
		unuseCharacterObject.transform.SetParent( _unuseObjectRoot );
	}

	/// <summary>
	/// プレイヤーの生成
	/// </summary>
	/// <returns></returns>
	public PlayerCharacter UsePlayer( int masterID, MapSquareData squareData ) {
		_playerObject = Instantiate( _characterObjectOrigin, _useObjectRoot );
		PlayerCharacter _player = new PlayerCharacter();
		_player.Setup( masterID, squareData );
		_SetPlayerData( _player );
		return _player;
	}

	/// <summary>
	/// エネミーキャラクター生成
	/// </summary>
	/// <returns></returns>
	public EnemyCharacter UseEnemy( int masterID, MapSquareData squareData ) {
		// 未使用のキャラクターが無ければ生成
		if (IsEmpty( _unuseEnemyList )) CreateUnuseEnemy();
		// 未使用のキャラクターリストから取得
		EnemyCharacter useEnemy = _unuseEnemyList[0];
		CharacterObject useObject = _unuseEnemyObjectList[0];
		_unuseEnemyList.RemoveAt( 0 );
		_unuseEnemyObjectList.RemoveAt( 0 );
		// 使用リストに追加
		int useID = -1;
		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		for (int i = 0, max = useEnemyList.Count; i < max; i++) {
			var enemy = useEnemyList[i];
			if (enemy != null) continue;

			useID = i;
			break;
		}
		if (useID < 0) {
			useID = useEnemyList.Count;
			useEnemyList.Add( null );
			_useEnemyObjectList.Add( null );
		}
		// 生成したエネミーの初期設定
		useEnemyList[useID] = useEnemy;
		_useEnemyObjectList[useID] = useObject;
		useObject.transform.SetParent( _useObjectRoot );
		useEnemy.Setup( useID, masterID, squareData );
		return useEnemy;
	}

	public PlayerCharacter GetPlayer() {
		return _GetPlayerData();
	}

	public EnemyCharacter GetEnemy( int ID ) {
		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		if (!IsEnableIndex( useEnemyList, ID )) return null;

		return useEnemyList[ID];
	}

	private CharacterObject GetEnemyObject( int objectID ) {
		if (!IsEnableIndex( _useEnemyObjectList, objectID )) return null;

		return _useEnemyObjectList[objectID];
	}

	private CharacterObject GetPlayerObject() {
		return _playerObject;
	}

	/// <summary>
	/// 全てのキャラクターに指定した処理を行う
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllCharacter( System.Action<CharacterBase> action ) {
		if (action == null) return;

		action.Invoke( GetPlayer() );
		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		for (int i = 0, max = useEnemyList.Count; i < max; i++) {
			if (useEnemyList[i] == null) continue;

			action.Invoke( useEnemyList[i] );
		}
	}

	/// <summary>
	/// 全てのキャラクターに指定したタスクを行う
	/// </summary>
	/// <param name="task"></param>
	public async UniTask ExecuteTaskAllCharacter( System.Func<CharacterBase, UniTask> task ) {
		if (task == null) return;

		await task.Invoke( GetPlayer() );
		List<EnemyCharacter> useEnemyList = _GetEnemyDataList();
		for (int i = 0, max = useEnemyList.Count; i < max; i++) {
			if (useEnemyList[i] == null) continue;

			await task.Invoke( useEnemyList[i] );
		}
	}

}
