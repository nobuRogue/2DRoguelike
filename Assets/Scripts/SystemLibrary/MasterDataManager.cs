using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// マスターデータ管理
/// </summary>
public class MasterDataManager {
	private static MasterDataManager _instance;

	public static MasterDataManager instance {
		get {
			if (_instance == null) _instance = new MasterDataManager();

			return _instance;
		}
	}
	// マスターデーターのファイルパス
	private const string _DATA_PATH = "MasterData/";

	// メッセージ情報
	private List<List<Entity_MessageData.Param>> _messageData = null;
	// フロア情報
	private List<Entity_FloorData.Param> _floorData = null;
	// キャラクター情報
	private List<Entity_CharacterData.Param> _characterData = null;
	// エネミー出現テーブル
	private List<Entity_EnemyTable.Param> _enemyTable = null;
	// アクション情報
	private List<Entity_ActionData.Param> _actionData = null;
	// アクション効果情報
	private List<Entity_ActionEffectData.Param> _actionEffectData = null;
	// アイテム情報
	private List<Entity_ItemData.Param> _itemData = null;
	// アイテムドロップテーブル
	private List<Entity_ItemDropTable.Param> _itemDropTable = null;
	// 罠情報
	private List<Entity_TrapData.Param> _trapData = null;
	// 罠テーブル
	private List<Entity_TrapTable.Param> _trapTable = null;


	/// <summary>
	/// 全マスターデータ読み込み
	/// </summary>
	private MasterDataManager() {
		// メッセージ情報の読み込み
		_messageData = Load<Entity_MessageData, Entity_MessageData.Sheet, Entity_MessageData.Param>("MessageData");
		// フロア情報の読み込み
		_floorData = Load<Entity_FloorData, Entity_FloorData.Sheet, Entity_FloorData.Param>("FloorData")[0];
		// キャラクター情報の読み込み
		_characterData = Load<Entity_CharacterData, Entity_CharacterData.Sheet, Entity_CharacterData.Param>("CharacterData")[0];
		// エネミーテーブル読み込み
		_enemyTable = Load<Entity_EnemyTable, Entity_EnemyTable.Sheet, Entity_EnemyTable.Param>("EnemyTable")[0];
		// アクション情報読み込み
		_actionData = Load<Entity_ActionData, Entity_ActionData.Sheet, Entity_ActionData.Param>("ActionData")[0];
		// アクション効果情報読み込み
		_actionEffectData = Load<Entity_ActionEffectData, Entity_ActionEffectData.Sheet, Entity_ActionEffectData.Param>("ActionEffectData")[0];
		// アイテム情報読み込み
		_itemData = Load<Entity_ItemData, Entity_ItemData.Sheet, Entity_ItemData.Param>("ItemData")[0];
		// アイテムドロップテーブル読み込み
		_itemDropTable = Load<Entity_ItemDropTable, Entity_ItemDropTable.Sheet, Entity_ItemDropTable.Param>("ItemDropTable")[0];
		// 罠データ読み込み
		_trapData = Load<Entity_TrapData, Entity_TrapData.Sheet, Entity_TrapData.Param>("TrapData")[0];
		// 罠テーブル読み込み
		_trapTable = Load<Entity_TrapTable, Entity_TrapTable.Sheet, Entity_TrapTable.Param>("TrapTable")[0];

	}

	/// <summary>
	/// マスターデータ読み込み
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <param name="dataName"></param>
	/// <returns></returns>
	private List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject {
		// ファイルを読み込む
		T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
		// 名称指定でシートを取得
		FieldInfo sheetField = typeof(T1).GetField("sheets");
		List<T2> sheetListData = sheetField.GetValue(sourceData) as List<T2>;

		// 名称指定で変数を取得
		FieldInfo listField = typeof(T2).GetField("list");
		List<List<T3>> paramList = new List<List<T3>>();
		foreach (object element in sheetListData) {
			List<T3> param = listField.GetValue(element) as List<T3>;
			paramList.Add(param);
		}
		return paramList;
	}

	/// <summary>
	/// フロアマスターデータ取得
	/// </summary>
	/// <param name="floorCount"></param>
	/// <returns></returns>
	public Entity_FloorData.Param GetFloorData(int floorCount) {
		for (int i = 0; i < _floorData.Count; i++) {
			if (_floorData[i].floorCount != floorCount) continue;
			// データが一致するものを返す
			return _floorData[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定のメッセージ取得
	/// </summary>
	/// <param name="messageID"></param>
	/// <returns></returns>
	public string GetMessage(int messageID) {
		for (int i = 0; i < _messageData.Count; i++) {
			List<Entity_MessageData.Param> messageList = _messageData[i];
			for (int j = 0; j < messageList.Count; j++) {
				if (messageList[j].ID != messageID) continue;
				// IDに一致するマスターデータ発見
				string[] textArray = messageList[j].text;
				return textArray[0];
			}
		}
		return string.Empty;// ""と同じ
	}

	/// <summary>
	/// ID指定のキャラクターデータ取得
	/// </summary>
	/// <param name="characterID"></param>
	/// <returns></returns>
	public Entity_CharacterData.Param GetCharacterData(int characterID) {
		for (int i = 0; i < _characterData.Count; i++) {
			if (_characterData[i].ID != characterID) continue;

			return _characterData[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定のエネミー出現テーブル取得
	/// </summary>
	/// <param name="tableID"></param>
	/// <returns></returns>
	public Entity_EnemyTable.Param GetEnemyTable(int tableID) {
		for (int i = 0; i < _enemyTable.Count; i++) {
			if (_enemyTable[i].ID != tableID) continue;

			return _enemyTable[i];

		}
		return null;
	}

	/// <summary>
	/// ID指定のアクション情報取得
	/// </summary>
	/// <param name="actionID"></param>
	/// <returns></returns>
	public Entity_ActionData.Param GetActionData(int actionID) {
		for (int i = 0; i < _actionData.Count; i++) {
			if (_actionData[i].ID != actionID) continue;

			return _actionData[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定のアクション効果取得
	/// </summary>
	/// <param name="effectID"></param>
	/// <returns></returns>
	public Entity_ActionEffectData.Param GetActionEffectData(int effectID) {
		for (int i = 0; i < _actionEffectData.Count; i++) {
			if (_actionEffectData[i].ID != effectID) continue;

			return _actionEffectData[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定のアイテムデータ取得
	/// </summary>
	/// <param name="itemID"></param>
	/// <returns></returns>
	public Entity_ItemData.Param GetItemData(int itemID) {
		for (int i = 0; i < _itemData.Count; i++) {
			if (_itemData[i].ID != itemID) continue;

			return _itemData[i];
		}
		return null;
	}
	/// <summary>
	/// ID指定のアイテムドロップテーブル取得
	/// </summary>
	/// <param name="tableID"></param>
	/// <returns></returns>
	public Entity_ItemDropTable.Param GetItemDropTable(int tableID) {
		for (int i = 0; i < _itemDropTable.Count; i++) {
			if (_itemDropTable[i].ID != tableID) continue;

			return _itemDropTable[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定の罠マスターデータ取得
	/// </summary>
	/// <param name="trapID"></param>
	/// <returns></returns>
	public Entity_TrapData.Param GetTrapData(int trapID) {
		for (int i = 0; i < _trapData.Count; i++) {
			if (_trapData[i].ID != trapID) continue;

			return _trapData[i];
		}
		return null;
	}

	/// <summary>
	/// ID指定の罠テーブル取得
	/// </summary>
	/// <param name="tableID"></param>
	/// <returns></returns>
	public Entity_TrapTable.Param GetTrapTable(int tableID) {
		for (int i = 0; i < _trapTable.Count; i++) {
			if (_trapTable[i].ID != tableID) continue;

			return _trapTable[i];
		}
		return null;
	}
}
