using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffect007_Warp : ActionEffectBase {
	// ワープ候補マスリスト
	private List<SquareObject> _reserveSquareList = null;

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		_reserveSquareList = new List<SquareObject>();
		// 対象の取得
		List<int> targetList = range.targetCharacterList;
		// すべての対象に対してワープ効果実行
		for (int i = 0; i < targetList.Count; i++) {
			CharacterObject target = CharacterManager.instance.GetCharacter(targetList[i]);
			if (target == null) continue;

			_reserveSquareList.Clear();
			// ワープの候補マスを取得
			MapSquareManager.instance.ExecuteAllSquare(AddWarpReserve);
			if (CommonModule.IsEmpty(_reserveSquareList)) break;
			// 候補からランダムな1マスを選択し、対象を移動させる
			SquareObject warpSquare = _reserveSquareList[Random.Range(0, _reserveSquareList.Count)];
			target.SetSquare(warpSquare);
		}

		await UniTask.DelayFrame(5);
	}

	/// <summary>
	/// ワープ候補マスをリストに追加
	/// </summary>
	/// <param name="square"></param>
	private void AddWarpReserve(SquareObject square) {
		// 部屋マスでないかキャラクターが存在するマスなら候補にならない
		if (square.squareData.terrain != eTerrain.Room ||
			square.existCharacter) return;

		_reserveSquareList.Add(square);
	}

}
