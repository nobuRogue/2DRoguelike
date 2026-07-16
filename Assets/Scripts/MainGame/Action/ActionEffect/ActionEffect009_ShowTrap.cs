using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ƒtƒƒA“à‚Ì‚·‚×‚Ä‚Ìã©‚ğ‰Â‹‰»
/// </summary>
public class ActionEffect009_ShowTrap : ActionEffectBase {

	public override async UniTask Execute(CharacterObject sourceCharacter, ActionRangeBase range, int[] param) {
		// ‚·‚×‚Ä‚Ìã©‚ğ‰Â‹‰»
		TrapManager.instance.ExecuteAllTrap(ShowTrap);
		await UniTask.DelayFrame(5);
	}

	/// <summary>
	/// ã©‚Ì‰Â‹‰»
	/// </summary>
	/// <param name="trap"></param>
	private void ShowTrap(TrapObject trap) {
		trap.Show();
	}

}
