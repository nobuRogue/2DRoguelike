using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// カメラ管理
/// </summary>
public class CameraManager : SystemObject {
	// 自身への参照
	public static CameraManager instance;
	// 管理中のカメラ
	private Camera _camera = null;
	//
	private const string _CAMERA_NAME = "Main Camera";

	public override async UniTask Initialize() {
		instance = this;
		_camera = GameObject.Find(_CAMERA_NAME).GetComponent<Camera>();
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// カメラ移動
	/// </summary>
	/// <param name="movePos"></param>
	public void MoveCamera(Vector3 movePos) {
		_camera.transform.position = movePos;
	}

}
