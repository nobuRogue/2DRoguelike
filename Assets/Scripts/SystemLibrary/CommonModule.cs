/**
 * @file CommonModule.cs
 * @brief 共用ライブラリ
 * @author yaonobu
 * @date 2020/10/25
 */
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;

/// <summary>
/// 共用ライブラリ
/// </summary>
public class CommonModule {
	/// <summary>
	/// 空か返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>( IReadOnlyCollection<T> list ) {
		return list == null || list.Count < 1;
	}

	/// <summary>
	/// 空か返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>( T[] array ) {
		return array == null || array.Length < 1;
	}

	/// <summary>
	/// 空か返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static bool IsEmpty( string array ) {
		return array == null || array.Length < 1;
	}

	/// <summary>
	/// 有効なインデクスか返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsEnableIndex<T>( IReadOnlyCollection<T> list, int index ) {
		if (index < 0) return false;

		if (IsEmpty( list )) return false;

		return list.Count > index;
	}

	/// <summary>
	/// 有効なインデクスか返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsEnableIndex<T>( T[] array, int index ) {
		if (index < 0) return false;

		if (IsEmpty( array )) return false;

		return array.Length > index;
	}

	/// <summary>
	/// 有効なインデクスか返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsEnableIndex( string array, int index ) {
		if (index < 0) return false;

		if (IsEmpty( array )) return false;

		return array.Length > index;
	}

	public static void InitializeList<T>( ref List<T> list, int capacity = -1 ) {
		if (list == null) {
			if (capacity < 0) {
				list = new List<T>();
			} else {
				list = new List<T>( capacity );
			}
		} else {
			if (capacity > 0 && list.Capacity < capacity) list.Capacity = capacity;

			list.Clear();
		}
	}

	public static void InitializeStringBuilder( ref StringBuilder initBuilder ) {
		if (initBuilder == null) {
			initBuilder = new StringBuilder();
		} else {
			initBuilder.Clear();
		}
	}

	/// <summary>
	/// 被りなしでリストから指定個数をランダム取得
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin"></param>
	/// <param name="result"></param>
	/// <param name="count"></param>
	public static void GetRandomCountContents<T>( List<T> origin, ref List<T> result, int count ) {
		InitializeList( ref result );
		if (IsEmpty( origin ) || count < 1) return;

		List<T> temporaryList = new List<T>( origin.Count );
		temporaryList.AddRange( origin );

		for (int i = 0; i < count; i++) {
			int index = Random.Range( 0, temporaryList.Count );
			result.Add( temporaryList[index] );

			temporaryList.RemoveAt( i );

			if (IsEmpty( temporaryList )) break;
		}
	}

	public static IEnumerator<float> WaitTask( List<UniTask> handleList ) {
		while (!IsEmpty( handleList )) {
			for (int i = handleList.Count - 1; i >= 0; i--) {
				if (!handleList[i].Status.IsCompleted() &&
					!handleList[i].Status.IsCanceled() &&
					!handleList[i].Status.IsFaulted()) continue;

				handleList.RemoveAt( i );
			}
			if (IsEmpty( handleList )) break;

			yield return 0.0f;
		}
	}

	/// <summary>
	/// 重複無しでマージ
	/// </summary>
	public static void MergeList<T>( ref List<T> mainList, List<T> subList ) {
		if (IsEmpty( subList )) return;

		int subCount = subList.Count;
		if (mainList == null) mainList = new List<T>();

		for (int i = 0; i < subCount; i++) {
			var sub = subList[i];
			if (sub == null) continue;

			if (mainList.Contains( sub )) continue;

			mainList.Add( sub );
		}
	}

}
