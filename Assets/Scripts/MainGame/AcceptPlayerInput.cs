/**
 * @file AcceptPlayerInput.cs
 * @brief �^�[�����̃v���C���[���͎�t
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AcceptPlayerInput {
	private System.Action<MoveAction> _AddMove = null;
	public void SetActionCallback( System.Action<MoveAction> AddMove ) {
		_AddMove = AddMove;
	}

	public async UniTask AcceptInput() {
		PlayerCharacter player = CharacterManager.instance.GetPlayer();
		bool isMove = AcceptMove( player );
		if (isMove) return;

		CharacterManager.instance.ExecuteAllCharacter( character => character?.SetAnimation( eCharacterAnimation.Wait ) );
		while (true) {
			isMove = AcceptMove( player );
			if (isMove) break;

			await UniTask.DelayFrame( 1 );
		}
	}

	/// <summary>
	/// �ړ��̎�t
	/// </summary>
	/// <param name="player"></param>
	/// <returns>�ړ����邩�ۂ�</returns>
	private bool AcceptMove( PlayerCharacter player ) {
		eDirectionEight inputDir = AcceptDirInput();
		if (inputDir == eDirectionEight.Invalid) return false;
		// �ړ��ۂ̔���
		Vector2Int sourcePos = player.squarePosition;
		MapSquareData moveSquare = MapSquareManager.instance.Get( sourcePos.ToVectorPos( inputDir ) );
		if (!MapSquareUtility.CanMove( sourcePos, moveSquare, inputDir )) return false;

		int playerSquareID = MapSquareManager.instance.GetID( player.squarePosition );
		var moveAction = new MoveAction();
		moveAction.ProcessData( player, new ChebyshevMoveData( playerSquareID, moveSquare.ID, inputDir ) );
		_AddMove( moveAction );
		return true;
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
