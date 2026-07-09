using UnityEngine;

/// <summary>
/// ©g‚ğ‘¼g—p‚É‚Æ‚éË’ö
/// </summary>
public class ActionRange01_Self : ActionRangeBase {

	public override void Execute(CharacterObject sourceCharacter) {
		targetCharacterList.Clear();
		targetCharacterList.Add(sourceCharacter.characterData.ID);
	}
}
