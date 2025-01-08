/**
 * @file PlayerCharacter.cs
 * @brief プレイヤーキャラクター
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase {
	public int stamina { get; protected set; } = -1;

	private List<int> _moveTrailSquareIDList = null;

	private readonly int PLAYER_MOVE_TRAIL_COUNT = 3;

	/// <summary>
	/// オブジェクトを取得するコールバック
	/// </summary>
	private static System.Func<CharacterObject> _GetObject = null;
	public static void SetObejectGetProcess( System.Func<CharacterObject> setProcess ) {
		_GetObject = setProcess;
	}

	private PlayerMoveObserver _moveObserver = null;

	public void Setup( int setMasterID, MapSquareData squareData ) {
		masterID = setMasterID;
		var masterData = CharacterMasterUtility.GetCharacterMaster( masterID );
		_moveTrailSquareIDList = new List<int>( PLAYER_MOVE_TRAIL_COUNT );
		Setup( masterData, squareData );
		GetObject()?.Setup( masterData );
		SetStamina( 1000 );

	}

	public override void Teardown() {
		base.Teardown();
		GetObject()?.Teardown();
	}

	protected override CharacterObject GetObject() {
		return _GetObject();
	}

	/// <summary>
	/// マス情報のみの変更
	/// </summary>
	/// <param name="setPosition"></param>
	public override void SetSquarePosition( MapSquareData square ) {
		base.SetSquarePosition( square );
		square.SetPlayer();
	}

	/// <summary>
	/// 見た目位置のみの変更
	/// </summary>
	/// <param name="set3DPosition"></param>
	public override void Set3DPosition( Vector3 set3DPosition ) {
		GetObject().SetPostion( set3DPosition );
		if (_moveObserver != null) _moveObserver.OnObjectMove( set3DPosition );

	}

	public void SetMoveObserver( PlayerMoveObserver setObserver ) {
		_moveObserver = setObserver;
	}

	public override void SetMaxHP( int setMaxHP ) {
		base.SetMaxHP( setMaxHP );
		MenuManager.instance.Get<MenuPlayerStatus>()?.SetPlayerHP( HP, maxHP );
	}

	public override void SetHP( int setHP ) {
		base.SetHP( setHP );
		MenuManager.instance.Get<MenuPlayerStatus>()?.SetPlayerHP( HP, maxHP );
	}

	public void SetStamina( int setStamina ) {
		stamina = Mathf.Max( setStamina, 0 );
		MenuManager.instance.Get<MenuPlayerStatus>()?.SetPlayerStamina( stamina );
	}

	public void AddStamina( int addStamina ) {
		SetStamina( stamina + addStamina );
	}

	public void DecrementStamina() {
		SetStamina( stamina - 1 );
	}

	public override async UniTask OnEndTurnProcess() {
		await base.OnEndTurnProcess();
		DecrementStamina();
	}

	public override void OnSquare( int squareID ) {
		base.OnSquare( squareID );
		if (_moveTrailSquareIDList.Exists( trailID => trailID == squareID )) return;

		if (_moveTrailSquareIDList.Count >= PLAYER_MOVE_TRAIL_COUNT) _moveTrailSquareIDList.RemoveAt( 0 );

		_moveTrailSquareIDList.Add( squareID );
	}

	public bool ExistMoveTrail( int squareID ) {
		return _moveTrailSquareIDList.Exists( trail => trail == squareID );
	}

}
