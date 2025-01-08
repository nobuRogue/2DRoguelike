/**
 * @file AcceptPlayerInput.cs
 * @brief ターン内のプレイヤー入力受付
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using UnityEngine;

using static MapSquareUtility;

public class AcceptPlayerInput {
	private System.Action<MoveAction> _AddMove = null;
	public void SetActionCallback( System.Action<MoveAction> AddMove ) {
		_AddMove = AddMove;
	}

	public async UniTask AcceptInput() {
		PlayerCharacter player = CharacterUtility.GetPlayer();
		bool isMove = AcceptMove( player );
		if (isMove) return;

		CharacterManager.instance.ExecuteAllCharacter( character => character?.SetAnimation( eCharacterAnimation.Wait ) );
		while (true) {
			isMove = AcceptMove( player );
			if (isMove) break;

			bool isAttack = await AcceptAttack( player );
			if (isAttack) break;

			await AcceptDirection( player );
			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// 攻撃入力の受付
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	private async UniTask<bool> AcceptAttack( PlayerCharacter player ) {
		if (!Input.GetKeyDown( KeyCode.Z )) return false;

		var attackActionMastr = ActionMasterUtility.GetActionMaster( GameConst.ATTACK_ACTION_ID );
		var attackRange = ActionRangeManager.GetRange( attackActionMastr.rangeID );
		attackRange.Setup( player );
		await ActionEffectManager.ExecuteEffect( attackActionMastr.effectID, player, attackRange );
		return true;
	}

	/// <summary>
	/// 移動の受付
	/// </summary>
	/// <param name="player"></param>
	/// <returns>移動するか否か</returns>
	private bool AcceptMove( PlayerCharacter player ) {
		eDirectionEight inputDir = AcceptDirInput();
		if (inputDir == eDirectionEight.Invalid) return false;
		// 移動可否の判定
		Vector2Int sourcePos = player.squarePosition;
		MapSquareData moveSquare = MapSquareManager.instance.Get( sourcePos.ToVectorPos( inputDir ) );
		if (!CanMove( sourcePos, moveSquare, inputDir )) {
			player.SetDirection( inputDir );
			return false;
		}
		int playerSquareID = MapSquareManager.instance.GetID( player.squarePosition );
		var moveAction = new MoveAction();
		moveAction.ProcessData( player, new ChebyshevMoveData( playerSquareID, moveSquare.ID, inputDir ) );
		_AddMove( moveAction );
		return true;
	}

	private async UniTask AcceptDirection( PlayerCharacter player ) {
		if (!Input.GetKey( KeyCode.X )) return;

		MapSquareData playerSquare = GetSquareData( player.squarePosition );
		MapSquareData forwardSquare = GetSquareData( playerSquare.squarePosition.ToVectorPos( player.direction ) );
		forwardSquare.SetRangeSpriteVisibility( true );
		while (Input.GetKey( KeyCode.X )) {
			eDirectionEight inputDir = AcceptDirInput();
			if (inputDir != eDirectionEight.Invalid) {
				forwardSquare.SetRangeSpriteVisibility( false );
				player.SetDirection( inputDir );
				forwardSquare = GetSquareData( playerSquare.squarePosition.ToVectorPos( player.direction ) );
				forwardSquare.SetRangeSpriteVisibility( true );
			}
			await UniTask.DelayFrame( 1 );
		}
		forwardSquare.SetRangeSpriteVisibility( false );
	}

	private eDirectionEight AcceptDirInput() {
		if (Input.GetKey( KeyCode.UpArrow )) {
			if (Input.GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.UpRight;
			} else if (Input.GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.UpLeft;
			} else {
				return eDirectionEight.Up;
			}
		} else if (Input.GetKey( KeyCode.DownArrow )) {
			if (Input.GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.DownRight;
			} else if (Input.GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.DownLeft;
			} else {
				return eDirectionEight.Down;
			}
		} else {
			if (Input.GetKey( KeyCode.RightArrow )) {
				return eDirectionEight.Right;
			} else if (Input.GetKey( KeyCode.LeftArrow )) {
				return eDirectionEight.Left;
			}
		}
		return eDirectionEight.Invalid;
	}



}
