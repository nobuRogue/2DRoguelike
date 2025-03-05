/**
 * @file MenuItemList.cs
 * @brief アイテムリスト
 * @author yaonobu
 * @date 2025/3/3
 */

using Cysharp.Threading.Tasks;

using static CommonModule;

public class MenuItemList : MenuList {

	public override async UniTask Initialize() {
		await base.Initialize();
		// コールバックの設定
		var itemListFortmat = new MenuListCallbackFormat();
		itemListFortmat.OnDecide = OnDecide;
		itemListFortmat.OnCancel = OnCancel;
		SetCallbackFormat( itemListFortmat );

	}

	public async UniTask Setup( int[] itemList ) {
		await SetIndex( -1 );
		RemoveAllItem();
		if (IsEmpty( itemList )) return;

		bool existItem = false;
		for (int i = 0, max = itemList.Length; i < max; i++) {
			if (itemList[i] < 0) break;

			if (!existItem) existItem = true;

			var addItem = AddItem() as ItemListItem;
			addItem.Setup( itemList[i] );
		}
		await SetIndex( 0 );
	}

	public override async UniTask Close() {
		await base.Close();
		RemoveAllItem();
	}

	private async UniTask<bool> OnDecide( MenuListItem decideItem ) {
		await UniTask.CompletedTask;
		return false;
	}

	private async UniTask<bool> OnCancel( MenuListItem cancelItem ) {
		await UniTask.CompletedTask;
		return false;
	}

}
