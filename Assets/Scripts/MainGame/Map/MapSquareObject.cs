/**
 * @file MapSquareObject.cs
 * @brief マップ上の1マスの情報
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSquareObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _sprite = null;

	[SerializeField]
	private Transform _characterRoot = null;

	public Transform characterRoot { get; private set; } = null;

	public void Setup( Vector2Int position ) {
		var localPosition = transform.localPosition;
		localPosition.x = position.x * 0.32f;
		localPosition.y = position.y * 0.32f;
		localPosition.z = position.y * 0.1f;
		transform.localPosition = localPosition;
	}

	public void SetTerrain( eTerrain terrain ) {
		_sprite.sprite = TerrainSpriteAssignor.GetTerrainSprite( terrain );
	}

	public Transform GetCharacterRoot() {
		return _characterRoot;
	}
}
