using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター管理
/// </summary>
public class CharacterManager : MonoBehaviour {
	public static CharacterManager instance = null;

	// キャラクタープレハブへの参照
	[SerializeField]
	private CharacterObject _characterOrigin = null;

	// 使用中キャラリスト
	private List<CharacterObject> _useList = null;
	// 未使用キャラリスト
	private List<CharacterObject> _unusePlayerList = null;
	private List<CharacterObject> _unuseEnemyList = null;

	/// <summary>
	/// キャラクターオブジェクトの生成、確保
	/// </summary>
	public void Initialize() {
		instance = this;
		_useList = new List<CharacterObject>(GameConst.ENEMY_MAX_COUNT + 1);
		// プレイヤーオブジェクトを必要数生成して未使用状態にしておく
		_unusePlayerList = new List<CharacterObject>(1);
		CharacterObject player = Instantiate(_characterOrigin, transform);
		player.Initialize(new PlayerCharacter());
		_unusePlayerList.Add(player);

		// エネミーオブジェクトを必要数生成して未使用状態にしておく
		_unuseEnemyList = new List<CharacterObject>(GameConst.ENEMY_MAX_COUNT);
		for (int i = 0; i < GameConst.ENEMY_MAX_COUNT; i++) {
			CharacterObject enemy = Instantiate(_characterOrigin, transform);
			enemy.Initialize(new EnemyCharacter());
			_unuseEnemyList.Add(enemy);
		}
	}

	/// <summary>
	/// プレイヤー生成
	/// </summary>
	public void CreatePlayer(int squareID, int masterID) {
		// 使用可能なプレイヤーオブジェクトを取得
		CharacterObject player;
		if (CommonModule.IsEmpty(_unusePlayerList)) {
			// 生成して使う
			player = Instantiate(_characterOrigin, transform);
			player.Initialize(new PlayerCharacter());
		}
		else {
			// 未使用リストから使う
			player = _unusePlayerList[0];
			_unusePlayerList.RemoveAt(0);
		}
		// 使用可能なIDを取得して使用リストに追加
		int useID = -1;
		for (int i = 0; i < _useList.Count; i++) {
			if (_useList[i] != null) continue;
			// 未使用箇所が見つかったので使う
			useID = i;
			_useList[i] = player;
			break;
		}
		// 未使用箇所が見つからなければ末尾に追加
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(player);
		}
		// セットアップ
		player.Setup(useID, masterID);
		// 指定マスに置く
		player.SetSquare(MapSquareManager.instance.GetSquare(squareID));
	}

	/// <summary>
	/// キャラクター削除
	/// </summary>
	/// <param name="ID"></param>
	public void DeleteCharacter(CharacterObject deleteCharacter) {
		if (deleteCharacter == null) return;
		// 使用リストから取り除く
		_useList[deleteCharacter.characterData.ID] = null;
		// 片付け処理を呼ぶ
		deleteCharacter.Teardown();
		// 未使用リストに追加
		if (deleteCharacter.characterData.IsPlayer()) {
			_unusePlayerList.Add(deleteCharacter);
		}
		else {
			_unuseEnemyList.Add(deleteCharacter);
		}
	}

	/// <summary>
	/// ID指定のキャラ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public CharacterObject GetCharacter(int ID) {
		// 有効なインデクスか判定
		if (!CommonModule.IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// プレイヤー取得
	/// </summary>
	/// <returns></returns>
	public CharacterObject GetPlayer() {
		if (CommonModule.IsEmpty(_useList)) return null;

		for (int i = 0; i < _useList.Count; i++) {
			CharacterObject character = _useList[i];
			if (character == null) continue;
			// プレイヤーでなければcontinue
			if (!character.characterData.IsPlayer()) continue;

			return character;
		}
		// プレイヤーが見つからないのでnullを返す
		return null;
	}

	/// <summary>
	/// すべてのキャラクターに指定処理実行
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllCharacter(System.Action<CharacterObject> action) {
		if (action == null || CommonModule.IsEmpty(_useList)) return;

		for (int i = 0; i < _useList.Count; i++) {
			CharacterObject character = _useList[i];
			if (character == null) continue;
			// 指定処理の実行
			action(character);
		}
	}

}
