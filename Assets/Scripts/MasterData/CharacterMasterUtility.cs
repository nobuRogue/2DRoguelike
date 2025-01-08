/**
 * @file CharacterMasterUtility.cs
 * @brief 
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMasterUtility {
	public static Entity_CharacterData.Param GetCharacterMaster( int ID ) {
		var characterMasterList = MasterDataManager.characterData[0];
		for (int i = 0, max = characterMasterList.Count; i < max; i++) {
			var characterMaster = characterMasterList[i];
			if (characterMaster.ID != ID) continue;

			return characterMaster;
		}
		return null;
	}
}
