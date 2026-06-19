using UnityEngine;

public class UserData {
	// Œ»İ‚ÌŠK”
	public int floorCount { get; private set; } = 0;

	public UserData() {
		SetFloorCount(1);
	}

	/// <summary>
	/// Œ»İ‚ÌŠK”‚Ìİ’è
	/// </summary>
	/// <param name="nextCount"></param>
	public void SetFloorCount(int nextCount) {
		floorCount = nextCount;
		// UI‚É”½‰f‚³‚¹‚é
		MenuManager.instance.Get<RogueMainMenu>()?.SetFloorCount(floorCount);
	}

}
