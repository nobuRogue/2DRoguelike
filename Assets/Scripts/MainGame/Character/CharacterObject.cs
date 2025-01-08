/**
 * @file CharacterObject.cs
 * @brief キャラクターオブジェクト
 * @author yaonobu
 * @date 2025/1/4
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class CharacterObject : MonoBehaviour {
	[SerializeField]
	private SpriteRenderer _characterImage = null;

	private static readonly string ANIMATION_SPRITE_PATH = "Design/Sprites/Character/";
	private static readonly string[] ANIMATION_SPRITE_NAME = new string[] { "_wait", "_walk", "_attack", "_damage" };

	private Sprite[][] _characterSpriteList = null;

	private UniTask _animationTask;

	public void Setup( Entity_CharacterData.Param masterData ) {
		string spriteName = masterData.spriteName;
		int animationMax = (int)eCharacterAnimation.Max;
		_characterSpriteList = new Sprite[animationMax][];
		for (int i = 0; i < animationMax; i++) {
			_characterSpriteList[i] = Resources.LoadAll<Sprite>( ANIMATION_SPRITE_PATH + spriteName + ANIMATION_SPRITE_NAME[i] );
		}
		SetAnimation( eCharacterAnimation.Wait );
		if (!_animationTask.Status.IsCompleted()) return;

		_animationTask = SpriteAnimationTask();
	}

	public void Teardown() {

	}

	public void SetPostion( Vector3 setPosition ) {
		// オブジェクトの座標を変更する処理
		transform.position = setPosition;
	}

	public eCharacterAnimation currenAnim = eCharacterAnimation.Wait;
	private int _animIndex = 0;

	public void SetAnimation( eCharacterAnimation setAnim ) {
		if (currenAnim == setAnim) return;

		currenAnim = setAnim;
		_animIndex = 0;
	}

	private async UniTask SpriteAnimationTask() {
		while (true) {
			Sprite[] currenAnimSpriteList = _characterSpriteList[(int)currenAnim];
			if (!IsEnableIndex( currenAnimSpriteList, _animIndex )) {
				_animIndex = 0;
				if (currenAnim == eCharacterAnimation.Attack ||
					currenAnim == eCharacterAnimation.Damage) currenAnim = eCharacterAnimation.Wait;

			}
			_characterImage.sprite = currenAnimSpriteList[_animIndex];
			_animIndex++;
			await UniTask.WaitForSeconds( 0.15f );
		}
	}

	public void SetDirection( eDirectionEight dir ) {
		var scale = _characterImage.transform.localScale;
		switch (dir) {
			case eDirectionEight.UpRight:
			case eDirectionEight.Right:
			case eDirectionEight.DownRight:
				scale.x = 1.0f;
				break;
			case eDirectionEight.DownLeft:
			case eDirectionEight.Left:
			case eDirectionEight.UpLeft:
				scale.x = -1.0f;
				break;
		}
		_characterImage.transform.localScale = scale;
	}
}
