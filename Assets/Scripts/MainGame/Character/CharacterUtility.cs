/**
 * @file CharacterUtility.cs
 * @brief �L�����N�^�[�֘A���p�����N���X
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUtility {
	private static System.Action<eDungeonEndReason> _EndDungeon = null;

	public static void SetEndDungeonProcess( System.Action<eDungeonEndReason> EndDungeonProcess ) {
		_EndDungeon = EndDungeonProcess;
	}


	public static PlayerCharacter GetPlayer() {
		return CharacterManager.instance.GetPlayer();
	}

	public static EnemyCharacter GetEnemy( int ID ) {
		return CharacterManager.instance.GetEnemy( ID );
	}

	public static async UniTask DeadCharacter( CharacterBase deadCharacter ) {
		var enemy = deadCharacter as EnemyCharacter;
		bool isEnemy = enemy != null;
		if (isEnemy) {
			// �G�l�~�[�폜
			CharacterManager.instance.UnuseEnemy( enemy );
		} else {
			// �v���C���[���S�Ń_���W�����I��
			_EndDungeon?.Invoke( eDungeonEndReason.Dead );
		}
	}

	public static void UnuseAllEnemy() {
		CharacterManager.instance.ExecuteAllCharacter( character => {
			EnemyCharacter enemy = character as EnemyCharacter;
			if (enemy == null) return;

			CharacterManager.instance.UnuseEnemy( enemy );
		} );
	}
}
