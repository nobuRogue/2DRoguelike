using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class TrapTable_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/MasterData/TrapTable.xlsx";
	private static readonly string exportPath = "Assets/Resources/MasterData/TrapTable.asset";
	private static readonly string[] sheetNames = { "TrapTable", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_TrapTable data = (Entity_TrapTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_TrapTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_TrapTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_TrapTable.Sheet s = new Entity_TrapTable.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_TrapTable.Param p = new Entity_TrapTable.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.trapID = new int[8];
					cell = row.GetCell(1); p.trapID[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.trapID[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.trapID[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.trapID[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.trapID[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.trapID[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.trapID[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.trapID[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
