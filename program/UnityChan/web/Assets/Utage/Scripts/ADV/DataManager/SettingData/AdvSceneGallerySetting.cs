//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// シーン回想のデータ
	/// </summary>
	[System.Serializable]
	public partial class AdvSceneGallerySettingData : SerializableDictionaryFileReadKeyValue
	{
		/// <summary>
		/// シナリオラベル
		/// </summary>
		public string ScenarioLabel { get { return this.Key; } }
		
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title { get { return this.title; } }
		[SerializeField]
		string title;

		/// <summary>
		/// サムネイル用ファイル名
		/// </summary>
		[SerializeField]
		string thumbnailName;

		/// <summary>
		/// サムネイル用ファイルパス
		/// </summary>
		public string ThumbnailPath { get { return this.thumbnailPath; } }
		string thumbnailPath;

		/// <summary>
		/// サムネイル用ファイルのバージョン
		/// </summary>
		public int ThumbnailVersion { get { return this.thumbnailVersion; } }
		[SerializeField]
		int thumbnailVersion;

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			string key = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.ScenarioLabel);
			InitKey(key);
			this.title = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Title,"");
			this.thumbnailName = AdvParser.ParseCell<string>(row, AdvColumnName.Thumbnail);
			this.thumbnailVersion = AdvParser.ParseCellOptional<int>(row, AdvColumnName.ThumbnailVersion, 0);
			return true;
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			thumbnailPath = settingData.ThumbnailDirInfo.FileNameToPath(thumbnailName);
		}
	}

	/// <summary>
	/// シーン回想のデータ
	/// </summary>
	[System.Serializable]
	public partial class AdvSceneGallerySetting : SerializableDictionaryFileRead<AdvSceneGallerySettingData>
	{
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit(AdvBootSetting settingData)
		{
			foreach (AdvSceneGallerySettingData data in List)
			{
				//初期化
				data.BootInit(settingData);
				//ファイルマネージャーにバージョンの登録
				AssetFile file = AssetFileManager.GetFileCreateIfMissing(data.ThumbnailPath);
				file.Version = data.ThumbnailVersion;
			}
		}

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public void DownloadAll()
		{
			//ファイルマネージャーにバージョンの登録
			foreach (AdvSceneGallerySettingData data in List)
			{
				AssetFileManager.Download(data.ThumbnailPath);
			}
		}
	}
}