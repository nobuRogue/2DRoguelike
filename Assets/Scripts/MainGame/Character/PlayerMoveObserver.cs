/**
 * @file PlayerMoveObserver.cs
 * @brief プレイヤー移動時のオブザーバ
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerMoveObserver {

	void OnObjectMove( Vector3 objectPosition );

}
