using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 1フロア実行
/// </summary>
public class FloorProcessor {
	// ターン実行クラス
	private TurnProcessor _turnProcessor = null;
	// フロア終了要因
	private eFloorEndReason _endReason = eFloorEndReason.Invalid;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize(System.Action<eDungeonEndReason> EndDungeon) {
		_turnProcessor = new TurnProcessor();
		_turnProcessor.Initialize(EndFloor, EndDungeon);
	}

	/// <summary>
	/// 1フロア実行処理
	/// </summary>
	/// <returns></returns>
	public async UniTask Execute() {
		// フロア生成、準備
		SetupFloor();
		await FadeManager.instance.FadeIn();
		// フロア終了するまでループ
		while (_endReason == eFloorEndReason.Invalid) {
			// 1ターン処理
			await _turnProcessor.Execute();
		}
		await FadeManager.instance.FadeOut();
		// フロア片付け
		TeardownFloor();
	}

	/// <summary>
	/// フロア生成、準備
	/// </summary>
	private void SetupFloor() {
		// フロア継続状態に設定
		_endReason = eFloorEndReason.Invalid;
		// 現在の階層の地形スプライトを設定
		UserData userData = UserDataHolder.instance.currentData;
		Entity_FloorData.Param floorMaster = MasterDataManager.instance.GetFloorData(userData.floorCount);
		TerrainSpriteAssignor.instance.SetSpriteType(floorMaster.spriteIndex);
		// ランダムフロア生成
		MapCreater.instance.CreateMap();
		// ランダムな部屋マスにプレイヤーを配置
		List<SquareObject> roomSquareList = new List<SquareObject>();
		// ラムダ式を用いた関数の実装
		// roomSquareListにすべての部屋マスを集約
		MapSquareManager.instance.ExecuteAllSquare(square => {
			if (square == null || square.squareData.terrain != eTerrain.Room) return;

			roomSquareList.Add(square);
		});
		// 集約された部屋マスからランダムに1つを選択
		if (CommonModule.IsEmpty(roomSquareList)) return;

		int randIndex = Random.Range(0, roomSquareList.Count);
		SquareObject playerSquare = roomSquareList[randIndex];
		CharacterManager.instance.GetPlayer()?.SetSquare(playerSquare);
		roomSquareList.RemoveAt(randIndex);
		// エネミーを生成
		int enemyCount = 3;
		for (int i = 0; i < enemyCount; i++) {
			if (CommonModule.IsEmpty(roomSquareList)) break;

			randIndex = Random.Range(0, roomSquareList.Count);
			SquareObject enemySquare = roomSquareList[randIndex];
			CharacterManager.instance.CreateEnemy(enemySquare.squareData.ID, 1);
			roomSquareList.Remove(enemySquare);
		}
		// アイテムを生成
		randIndex = Random.Range(0, roomSquareList.Count);
		SquareObject itemSquare = roomSquareList[randIndex];
		ItemManager.instance.CreateFloorItem(0, itemSquare);
		roomSquareList.RemoveAt(randIndex);

		randIndex = Random.Range(0, roomSquareList.Count);
		itemSquare = roomSquareList[randIndex];
		ItemManager.instance.CreateFloorItem(100, itemSquare);
		roomSquareList.RemoveAt(randIndex);
	}

	/// <summary>
	/// フロア片付け
	/// </summary>
	private void TeardownFloor() {
		// フロアのエネミーを全削除
		CharacterManager.instance.ExecuteAllCharacter(DeleteEnemy);
		// フロアのアイテムを全削除
		// 全てのアイテムに対して、床落ちアイテムなら削除処理を実行
		ItemManager.instance.ExecuteAllItem(DeleteFloorItem);
		// プレイヤーの移動軌跡をリセット
		CharacterManager.instance.GetPlayer()?.characterData.ClearMoveTrail();
	}

	/// <summary>
	/// 床落ちアイテムなら削除
	/// </summary>
	/// <param name="item"></param>
	private void DeleteFloorItem(ItemObject item) {
		// 床落ちアイテムでなければ終了
		if (!item.isFloorItem) return;
		// 床落ちアイテムなので削除
		ItemManager.instance.DeleteItem(item);
	}

	/// <summary>
	/// エネミーなら削除する
	/// </summary>
	/// <param name="character"></param>
	private void DeleteEnemy(CharacterObject character) {
		if (character.characterData.IsPlayer()) return;

		CharacterManager.instance.DeleteCharacter(character);
	}

	/// <summary>
	/// フロアを終了させる
	/// </summary>
	public void EndFloor(eFloorEndReason endReason) {
		_endReason = endReason;
	}

}
