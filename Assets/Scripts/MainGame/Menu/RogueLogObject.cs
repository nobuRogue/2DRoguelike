using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Threading;

/// <summary>
/// 単体のログオブジェクト
/// </summary>
public class RogueLogObject : MonoBehaviour {
	// ログ1行分の移動にかける時間[秒]
	private static readonly float _FLOW_DURATION_SEC = 0.3f;
	// 表示されるログテキスト
	[SerializeField]
	private TextMeshProUGUI _logText = null;
	// 自身の矩形
	[SerializeField]
	private RectTransform _rectTransform = null;

	private CancellationToken _ct;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// オブジェクト破棄時のUnitask中断トークン取得
		_ct = gameObject.GetCancellationTokenOnDestroy();
	}

	/// <summary>
	/// 使用前の準備
	/// </summary>
	public void Setup(string text) {
		_logText.text = text;
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(true);
	}
	/// <summary>
	/// 使用後の片付け
	/// </summary>
	public void Teardown() {
		_logText.text = string.Empty;
		gameObject.SetActive(false);
	}

	/// <summary>
	/// 自身を1行分上に流す
	/// </summary>
	/// <returns></returns>
	public async UniTask FlowLog() {
		// スタートと目的地の設定
		Vector3 startPos = transform.position;
		Vector3 goalPos = startPos;
		goalPos.y += _rectTransform.sizeDelta.y;
		// 規定の秒数をかけて補完移動
		float elapsedSec = 0.0f;
		while (elapsedSec < _FLOW_DURATION_SEC) {
			// 経過時間の累積
			elapsedSec += Time.deltaTime;
			// 補完座標の設定
			float t = elapsedSec / _FLOW_DURATION_SEC;
			transform.position = Vector3.Lerp(startPos, goalPos, t);
			// 1フレーム待機
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _ct);
		}
		// ゴール座標に設定
		transform.position = goalPos;
	}

}
