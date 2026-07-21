using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動、効果の実行それらの管理クラス
/// </summary>
public class ActionManager {
	private static ActionManager _instance;
	// 行動実行時のログID
	private const int _USE_ACTION_LOG_ID = 3010;
	// アイテム使用時のログID
	private const int _USE_ITEM_LOG_ID = 3005;
	// 罠を踏んだ際のログID
	private const int _STEPON_TRAP_LOG_ID = 3011;
	// 罠が発動しなかった際のログID
	private const int _NOT_ACTIVATE_TRAP_LOG_ID = 3012;

	public static ActionManager instance {
		get {
			if (_instance == null) _instance = new ActionManager();

			return _instance;
		}
	}

	// 効果リスト
	private List<ActionEffectBase> _effectList = null;

	private ActionManager() {
		// 使用する行動効果をすべてキャッシュしておく
		_effectList = new List<ActionEffectBase>();
		_effectList.Add(new ActionEffect000_Attack());
		_effectList.Add(new ActionEffect001_HealHP());
		_effectList.Add(new ActionEffect002_HealStamina());
		_effectList.Add(new ActionEffect003_FixDamage());
		_effectList.Add(new ActionEffect004_BurnItem());
		_effectList.Add(new ActionEffect005_LotItem());
		_effectList.Add(new ActionEffect006_Replace());
		_effectList.Add(new ActionEffect007_Warp());
		_effectList.Add(new ActionEffect008_Knockback());
		_effectList.Add(new ActionEffect009_ShowTrap());
		_effectList.Add(new ActionEffect010_ExplosionDamage());
	}

	/// <summary>
	/// 罠を踏んだ際の処理
	/// </summary>
	/// <param name="steponCharacter"></param>
	/// <param name="trapID"></param>
	/// <returns></returns>
	public async UniTask StepOnTrap(CharacterObject steponCharacter, int trapID) {
		TrapObject trap = TrapManager.instance.GetTrap(trapID);
		if (trap == null) return;
		// 罠の可視化
		trap.Show();
		// ログの表示
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		logMenu.AddLog(string.Format(_STEPON_TRAP_LOG_ID.ToMessage(), trap.GetName()));
		// 発動判定
		int activateRatio = trap.trapData.trapMaster.activateRatio;
		if (UnityEngine.Random.Range(0, 100) >= activateRatio) {
			// 発動しなかったログ表示
			logMenu.AddLog(string.Format(_NOT_ACTIVATE_TRAP_LOG_ID.ToMessage(), trap.GetName()));
			return;
		}
		// 効果実行
		await ExecuteAction(steponCharacter, trap.trapData.trapMaster.actionID);
		// 消滅判定
		int deleteRatio = trap.trapData.trapMaster.deleteRatio;
		if (UnityEngine.Random.Range(0, 100) >= deleteRatio) return;
		// 罠の消滅
		TrapManager.instance.RemoveTrap(trap);
	}

	/// <summary>
	/// ログありのアクション実行
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="actionID"></param>
	/// <returns></returns>
	public async UniTask UseAction(CharacterObject sourceCharacter, int actionID) {
		// 行動のマスターデータ取得
		Entity_ActionData.Param actionMaster = MasterDataManager.instance.GetActionData(actionID);
		if (actionMaster == null) return;
		// ログ表示
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		logMenu.AddLog(string.Format(_USE_ACTION_LOG_ID.ToMessage(), sourceCharacter.characterData.GetName(), actionMaster.nameID.ToMessage()));
		// アクション実行
		await ExecuteAction(sourceCharacter, actionID);
	}

	/// <summary>
	/// アイテム効果実行
	/// </summary>
	/// <param name="useCharacter"></param>
	/// <param name="useItem"></param>
	/// <returns></returns>
	public async UniTask UseItem(CharacterObject useCharacter, int itemID) {
		ItemObject useItem = ItemManager.instance.GetItem(itemID);
		if (useItem == null) return;
		// アイテムのマスターデータを取得する
		Entity_ItemData.Param itemMaster = useItem.itemData.itemMaster;
		// ログの表示
		RogueLogMenu logMenu = MenuManager.instance.Get<RogueLogMenu>();
		logMenu.AddLog(string.Format(_USE_ITEM_LOG_ID.ToMessage(), useItem.itemData.GetName()));
		// アクションの実行
		await ExecuteAction(useCharacter, itemMaster.actionID);
		// アイテムの消費
		useItem.itemData.Consume();
	}

	/// <summary>
	/// 行動の実行
	/// </summary>
	/// <returns></returns>
	private async UniTask ExecuteAction(CharacterObject sourceCharacter, int actionID) {
		// アクションマスターデータ取得
		Entity_ActionData.Param actionMaster = MasterDataManager.instance.GetActionData(actionID);
		if (actionMaster == null) return;
		// 射程による対象の取得
		ActionRangeBase range = ActionRangeManager.instance.GetRange(actionMaster.rangType);
		if (range == null) return;
		// 射程による対象取得処理
		range.Execute(sourceCharacter);
		// マスターデータに則ったアクション効果の実行
		int[] effectArray = actionMaster.effectID;
		for (int i = 0; i < effectArray.Length; i++) {
			await ExecuteActionEffect(sourceCharacter, range, effectArray[i]);
		}

	}

	/// <summary>
	/// 1効果の実行
	/// </summary>
	/// <param name="sourceCharacter"></param>
	/// <param name="range"></param>
	/// <param name="effectID"></param>
	/// <returns></returns>
	private async UniTask ExecuteActionEffect(CharacterObject sourceCharacter, ActionRangeBase range, int effectID) {
		// 効果マスターデータ取得
		Entity_ActionEffectData.Param effectMaster = MasterDataManager.instance.GetActionEffectData(effectID);
		if (effectMaster == null) return;
		// 効果マスターから実行する効果を取得、実行
		if (!CommonModule.IsEnableIndex(_effectList, effectMaster.effectType)) return;
		// 効果の実行
		await _effectList[effectMaster.effectType].Execute(sourceCharacter, range, effectMaster.param);
	}

}
