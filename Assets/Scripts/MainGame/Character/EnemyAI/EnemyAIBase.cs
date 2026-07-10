using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーAIの基底
/// </summary>
public abstract class EnemyAIBase {
	// 移動アクションの追加時コールバック
	public static System.Action<MoveAction> addMove = null;
	// 持ち主のID
	protected int _sourceCharacterID = -1;
	// 予定行動ID
	protected int _scheduleActionID = -1;
	// 行動のIDリスト
	protected List<int> _actionIDList = null;

	public EnemyAIBase(int characterID, int[] actionIDList) {
		_sourceCharacterID = characterID;
		// 不正値を取り除いた行動IDリストをキャッシュしておく
		int actionCount = actionIDList.Length;
		_actionIDList = new List<int>(actionCount);
		for (int i = 0; i < actionCount; i++) {
			if (actionIDList[i] < 0) continue;
			// 0以上の値だけをリストに追加
			_actionIDList.Add(actionIDList[i]);
		}
	}

	/// <summary>
	/// 行動の思考
	/// </summary>
	public abstract void ThinkAction();

	/// <summary>
	/// 予定行動実行
	/// </summary>
	/// <returns></returns>
	public async UniTask ExecuteScheduleAction() {
		if (_scheduleActionID < 0) return;
		// 使用可否の判定
		Entity_ActionData.Param actionData = MasterDataManager.instance.GetActionData(_scheduleActionID);
		ActionRangeBase range = ActionRangeManager.instance.GetRange(actionData.rangType);
		eDirectionEight canUseDir = eDirectionEight.Invalid;
		CharacterObject sourceCharacter = CharacterManager.instance.GetCharacter(_sourceCharacterID);
		if (range.CanUse(sourceCharacter, ref canUseDir)) {
			// 実行
			sourceCharacter.SetDirection(canUseDir);
			await ActionManager.instance.ExecuteAction(sourceCharacter, _scheduleActionID);
		}
		_scheduleActionID = -1;
	}

}
