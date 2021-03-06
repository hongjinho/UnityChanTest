//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Utage
{

	//「Utage」のシナリオデータ用のエクセルファイルインポーター
	public class AdvExcelImporter : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			if (CustomProjectSetting.Instance == null) return;

			//制御エディタを通して、管理対象のデータのみインポートする
			AdvScenarioDataBuilderWindow.Import(importedAssets);
		}

		//シナリオデータ
		Dictionary<string, AdvScenarioData> scenarioDataTbl = new Dictionary<string, AdvScenarioData>();

		//ファイルの読み込み
		public bool Import(List<string> pathList)
		{
			//対象のエクセルファイルを全て読み込み
			Dictionary<string,StringGridDictionary> bookDictionary = new Dictionary<string,StringGridDictionary>();
			foreach (string path in pathList)
			{
				if (!string.IsNullOrEmpty(path))
				{
					StringGridDictionary book = ExcelParser.Read(path);
					if (book.List.Count > 0)
					{
						bookDictionary.Add(path, book);
					}
				}
			}

			if (bookDictionary.Count <= 0) return false;

			assetSetting = null;
			//設定データをインポート
			foreach (string path in bookDictionary.Keys)
			{
				ImportSettingBook(bookDictionary[path], path);
				if (assetSetting != null) break;
			}
			if (assetSetting == null) return false;

			AssetFileManager.IsEditorErrorCheck = true;
			AdvCommand.IsEditorErrorCheck = true;
			TextData.CallbackCalcExpression = assetSetting.DefaultParam.CalcExpressionNotSetParam;
			//シナリオデータをインポート
			foreach (string path in bookDictionary.Keys)
			{
				ImportScenarioBook(bookDictionary[path], path);
			}
			TextData.CallbackCalcExpression = null;

			//シナリオラベルのリンクチェック
			ErroeCheckScenarioLabel();

			AdvCommand.IsEditorErrorCheck = false;
			AssetFileManager.IsEditorErrorCheck = false;
			return true;
		}
		AdvSettingDataManager assetSetting;

		//設定データをインポート
		void ImportSettingBook(StringGridDictionary book, string path)
		{
			//インポート後のスクリプタブルオブジェクトを作成
			string assetPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + " Setting.asset";
			foreach (var sheet in book.List )
			{
				StringGrid grid = sheet.Grid;
				//設定データか、シナリオデータかチェック
				if (AdvSettingDataManager.IsBootSheet(sheet.Name) || AdvSettingDataManager.IsSettingsSheet(sheet.Name))
				{
					//設定データのアセットを作成
					if (assetSetting == null)
					{
						assetSetting = UtageEditorToolKit.GetImportedAssetCreateIfMissing<AdvSettingDataManager>(assetPath);
						assetSetting.Clear();
					}
					assetSetting.hideFlags = HideFlags.NotEditable;
					assetSetting.ParseFromExcel(sheet.Name, grid);
				}
			}

			if (assetSetting != null)
			{
				Debug.Log( LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.Import,assetPath));
				//変更を反映
				EditorUtility.SetDirty(assetSetting);
			}
		}

		//ブックのインポート
		void ImportScenarioBook(StringGridDictionary book, string path)
		{
			//シナリオデータ用のスクリプタブルオブジェクトを宣言
			string scenarioAssetPath = Path.ChangeExtension(path, ".asset");
			AdvScenarioDataExported assetScenario = null;

			foreach (var sheet in book.List)
			{
				StringGrid grid = sheet.Grid;
				//設定データか、シナリオデータかチェック
				if (!AdvSettingDataManager.IsBootSheet(sheet.Name) && !AdvSettingDataManager.IsSettingsSheet(sheet.Name))
				{
					//シナリオデータのアセットを作成
					if (assetScenario == null)
					{
						assetScenario = UtageEditorToolKit.GetImportedAssetCreateIfMissing<AdvScenarioDataExported>(scenarioAssetPath);
						assetScenario.Clear();
					}
					assetScenario.hideFlags = HideFlags.NotEditable;
					assetScenario.ParseFromExcel(sheet.Name, grid);
					if (assetSetting != null)
					{
						AdvScenarioData scenarioData = assetScenario.ErrorCheck(sheet.Name, grid, assetSetting);
						scenarioDataTbl.Add(sheet.Name, scenarioData);
					}
				}
			}

			//変更を反映
			if (assetScenario != null)
			{
				Debug.Log(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.Import, scenarioAssetPath));
				EditorUtility.SetDirty(assetScenario);
			}
		}


		/// <summary>
		/// シナリオラベルのリンクチェック
		/// </summary>
		/// <param name="label">シナリオラベル</param>
		/// <returns>あればtrue。なければfalse</returns>
		void ErroeCheckScenarioLabel()
		{
			//リンク先のシナリオラベルがあるかチェック
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				foreach (string label in data.JumpScenarioLabels)
				{
					if (!IsExistScenarioLabel(label))
					{
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotLinkedScenarioLabel, label, data.DataGridName));
					}
				}
			}

			//シナリオラベルが重複しているかチェック
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				foreach (string label in data.ScenarioLabels)
				{
					if (IsExistScenarioLabel(label,data))
					{
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.RedefinitionScenarioLabel, label, data.DataGridName));
					}
				}
			}
		}


		/// <summary>
		/// シナリオラベルがあるかチェック
		/// </summary>
		/// <param name="label">シナリオラベル</param>
		/// <param name="egnoreData">チェックを無視するデータ</param>
		/// <returns>あればtrue。なければfalse</returns>
		bool IsExistScenarioLabel(string label, AdvScenarioData egnoreData = null )
		{
			foreach (AdvScenarioData data in scenarioDataTbl.Values)
			{
				if (data == egnoreData) continue;
				if (data.IsExistScenarioLabel(label))
				{
					return true;
				}
			}
			return false;
		}
	}
}