/**
 * @file CharacterUtility.cs
 * @brief キャラクター関連実用処理クラス
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
			// エネミー削除
			CharacterManager.instance.UnuseEnemy( enemy );
		} else {
			// プレイヤー死亡でダンジョン終了
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
