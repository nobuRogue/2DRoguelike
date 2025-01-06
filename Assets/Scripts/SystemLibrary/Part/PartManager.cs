/**
 * @file PartManager.cs
 * @brief パート管理
 * @author yaonobu
 * @date 2020/11/11
 */
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using static CommonModule;

/// <summary>
/// パート管理
/// </summary>
public class PartManager : SystemObject {
	[SerializeField]
	private List<PartBase> _partList = null;

	public static PartManager instance { get; private set; } = null;

	private PartBase[] partList = null;
	public PartBase currentPartClass { get; private set; } = null;

	public eGamePart currentPart { get; private set; } = eGamePart.Invalid;

	public override async UniTask Initialize() {
		await base.Initialize();
		instance = this;
		int partMax = _partList.Count;
		partList = new PartBase[partMax];
		List<UniTask> taskList = new List<UniTask>( partMax );
		for (int i = 0; partMax > i; i++) {
			var partOrigin = _partList[i];
			if (partOrigin == null) continue;

			var createPart = Instantiate( partOrigin, transform );
			partList[i] = createPart;
			taskList.Add( createPart.Initialize() );
		}
		await WaitTask( taskList );
	}

	public void Execute() {
		UniTask task = TransitionPart( eGamePart.Standby );
	}

	// パートの遷移
	public async UniTask TransitionPart( eGamePart nextPart ) {
		if (currentPartClass != null) await currentPartClass.Cleannup();

		currentPart = nextPart;
		currentPartClass = partList[(int)currentPart];
		await currentPartClass.Setup();
		UniTask task = currentPartClass.Execute();
	}
}
