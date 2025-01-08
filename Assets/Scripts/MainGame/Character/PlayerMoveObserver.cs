/**
 * @file PlayerMoveObserver.cs
 * @brief �v���C���[�ړ����̃I�u�U�[�o
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerMoveObserver {

	void OnObjectMove( Vector3 objectPosition );

}
