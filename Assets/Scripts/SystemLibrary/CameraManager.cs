/**
 * @file CameraManager.cs
 * @brief カメラ管理
 * @author yaonobu
 * @date 2020/11/09
 */
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// カメラ管理
/// </summary>
public class CameraManager : SystemObject, PlayerMoveObserver {
	/// <summary>
	/// カメラのリスト
	/// </summary>
	public CameraUnit mainCameraUnit { get; private set; } = null;

	public class CameraUnit {
		public Transform moveRoot = null;
		public Camera camera { get; private set; } = null;

		public Vector3 defaultPosition = Vector3.zero;
		public CameraUnit( string rootName ) {
			moveRoot = GameObject.Find( rootName ).transform;
			camera = moveRoot.GetComponentInChildren<Camera>();
			defaultPosition = camera.transform.localPosition;
		}

		public async UniTask Shake( float duration, float magnitude ) {
			var targetCamera = camera;
			var pos = defaultPosition;
			float elapsedTime = 0.0f;
			while (elapsedTime < duration) {
				elapsedTime += Time.deltaTime;
				pos.x = defaultPosition.x + Random.Range( -1f, 1f ) * magnitude;
				pos.y = defaultPosition.y + Random.Range( -1f, 1f ) * magnitude;
				targetCamera.transform.localPosition = pos;
				await UniTask.DelayFrame( 1 );
			}
			targetCamera.transform.localPosition = defaultPosition;
		}
	}

	public static CameraManager instance { get; private set; } = null;

	public override async UniTask Initialize() {
		await base.Initialize();
		instance = this;
		mainCameraUnit = new CameraUnit( "MainCameraRoot" );
	}

	public async UniTask Shake( float duration, float magnitude ) {
		await mainCameraUnit.Shake( duration, magnitude );
	}

	public void OnObjectMove( Vector3 objectPosition ) {
		objectPosition.z -= 1.0f;
		mainCameraUnit.moveRoot.position = objectPosition;
	}
}
