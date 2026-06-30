using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class RogueLogMenu : MenuBase {
	// 画面上に表示されるログオブジェクトの最大数
	private const int _SHOW_LOG_COUNT = 4;
	// 待機テキストリストの初期確保数
	private const int _STANDBY_TEXT_COUNT = 256;

	// ログ単体のプレハブへの参照
	[SerializeField]
	private RogueLogObject _originLogObject = null;
	// ログの親オブジェクト
	[SerializeField]
	private Transform _logRoot = null;
	// ログオブジェクトの使用リスト
	List<RogueLogObject> _useList = null;
	// ログオブジェクトの未使用リスト
	List<RogueLogObject> _unuseList = null;
	// 待機テキストリスト
	List<string> _standbyTextList = null;
	// Unitask中断トークン
	private CancellationToken _ct;

	/// <summary>
	/// 初期化
	/// </summary>
	public override void Initialize() {
		base.Initialize();
		// オブジェクト破棄時のUnitask中断トークン取得
		_ct = gameObject.GetCancellationTokenOnDestroy();
		_standbyTextList = new List<string>(_STANDBY_TEXT_COUNT);
		_useList = new List<RogueLogObject>(_SHOW_LOG_COUNT);
		_unuseList = new List<RogueLogObject>(_SHOW_LOG_COUNT);
		// ログオブジェクトを必要分生成して未使用状態にしておく
		for (int i = 0; i < _SHOW_LOG_COUNT; i++) {
			RogueLogObject createLog = Instantiate(_originLogObject, _logRoot);
			UnuseLog(createLog);
		}
		UniTask task = FlowLogTask();
	}

	/// <summary>
	/// 指定ログを未使用状態にする
	/// </summary>
	/// <param name="unuseLog"></param>
	private void UnuseLog(RogueLogObject unuseLog) {
		// 使用リストから取り除き、片付け
		_useList.Remove(unuseLog);
		unuseLog.Teardown();
		// 未使用リストに追加
		_unuseList.Add(unuseLog);
	}

	/// <summary>
	/// 指定テキストでログを使用状態にする
	/// </summary>
	/// <param name="text"></param>
	private void UseLog(string text) {
		if (CommonModule.IsEmpty(_unuseList)) return;
		// 未使用リストの0番目の要素を取得し使用状態にする
		RogueLogObject useLog = _unuseList[0];
		_unuseList.RemoveAt(0);
		useLog.Setup(text);
		_useList.Add(useLog);
	}

	/// <summary>
	/// 表示ログの追加
	/// </summary>
	/// <param name="logText"></param>
	public void AddLog(string logText) {
		// 待機テキストリストに追加
		_standbyTextList.Add(logText);
	}

	/// <summary>
	/// 待機テキストを順番にログオブジェクトとして生成して流す
	/// </summary>
	/// <returns></returns>
	private async UniTask FlowLogTask() {
		while (true) {
			// 待機テキストリストに要素が追加されるまで待機
			while (CommonModule.IsEmpty(_standbyTextList)) await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _ct);
			// 最も古い待機テキストをログオブジェクトとして生成
			UseLog(_standbyTextList[0]);
			_standbyTextList.RemoveAt(0);
			List<UniTask> taskList = new List<UniTask>(_useList.Count);
			// 全ての表示中のログを1行分移動させる
			for (int i = 0; i < _useList.Count; i++) {
				taskList.Add(_useList[i].FlowLog());
			}
			await UniTask.WhenAll(taskList);
			// 範囲外のログオブジェクトを未使用状態にする
			if (_useList.Count >= _SHOW_LOG_COUNT) UnuseLog(_useList[0]);

		}
	}

}
