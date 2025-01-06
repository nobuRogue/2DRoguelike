/**
 * @file PartBase.cs
 * @brief ゲームパートの基底
 * @author yaonobu
 * @date 2024/6/28
 */
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class PartBase : MonoBehaviour {
	public abstract UniTask Initialize();
	public abstract UniTask Setup();
	public abstract UniTask Execute();
	public abstract UniTask Cleannup();
}
