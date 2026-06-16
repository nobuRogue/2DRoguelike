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
	public void Initialize() {
		_turnProcessor = new TurnProcessor();
		_turnProcessor.Initialize(EndFloor);
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
		if (!CommonModule.IsEmpty(roomSquareList)) {
			SquareObject playerSquare = roomSquareList[Random.Range(0, roomSquareList.Count)];
			CharacterManager.instance.GetPlayer()?.SetSquare(playerSquare);
		}

		// フロア継続状態に設定
		_endReason = eFloorEndReason.Invalid;
	}

	/// <summary>
	/// フロア片付け
	/// </summary>
	private void TeardownFloor() {

	}

	/// <summary>
	/// フロアを終了させる
	/// </summary>
	public void EndFloor(eFloorEndReason endReason) {
		_endReason = endReason;
	}

}
