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
		CreateFloorEnemy(8, floorMaster.enemyTableID, roomSquareList);
		// アイテムを生成
		CreateFloorItem(16, floorMaster.itemTableID, roomSquareList);
	}

	/// <summary>
	/// フロア内にエネミー生成
	/// </summary>
	/// <param name="createCount"></param>
	/// <param name="enemyTableID"></param>
	/// <param name="roomSquareList"></param>
	private void CreateFloorEnemy(int createCount, int enemyTableID, List<SquareObject> roomSquareList) {
		// エネミーテーブルを取得
		Entity_EnemyTable.Param enemyTable = MasterDataManager.instance.GetEnemyTable(enemyTableID);
		// 不正値を取り除いたテーブルを生成
		int tableEnemyCount = enemyTable.enemyID.Length;
		List<int> useTable = new List<int>(tableEnemyCount);
		for (int i = 0; i < tableEnemyCount; i++) {
			if (enemyTable.enemyID[i] < 0) continue;

			useTable.Add(enemyTable.enemyID[i]);
		}

		for (int i = 0; i < createCount; i++) {
			if (CommonModule.IsEmpty(roomSquareList)) break;

			int randIndex = Random.Range(0, roomSquareList.Count);
			SquareObject enemySquare = roomSquareList[randIndex];
			// テーブルからランダムに生成
			CharacterManager.instance.CreateEnemy(enemySquare.squareData.ID, useTable[Random.Range(0, useTable.Count)]);
			roomSquareList.Remove(enemySquare);
		}
	}

	/// <summary>
	/// フロアに落ちているアイテムの生成
	/// </summary>
	/// <param name="itemTableID"></param>
	private void CreateFloorItem(int createCount, int itemTableID, List<SquareObject> roomSquareList) {
		// ドロップテーブルを取得
		Entity_ItemDropTable.Param itemDropTable = MasterDataManager.instance.GetItemDropTable(itemTableID);
		// 不正値を取り除いたテーブル生成
		int tableItemCount = itemDropTable.itemID.Length;
		List<int> useTable = new List<int>(tableItemCount);
		for (int i = 0; i < tableItemCount; i++) {
			if (itemDropTable.itemID[i] < 0) continue;

			useTable.Add(itemDropTable.itemID[i]);
		}
		if (CommonModule.IsEmpty(useTable)) return;

		for (int i = 0; i < createCount; i++) {
			int randIndex = Random.Range(0, roomSquareList.Count);
			SquareObject itemSquare = roomSquareList[randIndex];
			// テーブルからランダムに生成
			ItemManager.instance.CreateFloorItem(useTable[Random.Range(0, useTable.Count)], itemSquare);
			roomSquareList.RemoveAt(randIndex);
		}
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
