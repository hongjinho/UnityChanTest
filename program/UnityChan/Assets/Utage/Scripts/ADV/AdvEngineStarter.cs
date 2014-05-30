//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;


namespace Utage
{

	/// <summary>
	/// ゲーム起動処理のサンプル
	/// </summary>
	[AddComponentMenu("Utage/ADV/EngineStarter")]
	public class AdvEngineStarter : MonoBehaviour
	{
		public enum LoadType
		{
			Local,
			Server,
		};

		/// <summary>開始フレームで自動でADVエンジンを起動する</summary>
		[SerializeField]
		bool isAutomaticPlay = false;

		/// <summary>開始フレームで自動でADVエンジンを起動する</summary>
		[SerializeField]
		string startScenario = "";

		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
		[SerializeField]
		AdvEngine engine;

		/// <summary>シナリオデータのロードタイプ</summary>
		public LoadType ScenarioDataLoadType
		{
			get { return scenarioDataLoadType; }
		}
		[SerializeField]
		LoadType scenarioDataLoadType;

		/// <summary>サーバーから起動する場合の開始ファイルのパス</summary>
		[SerializeField]
		string urlScenarioData;

		/// <summary>サーバーから起動する場合の開始ファイルのバージョン(-1なら毎回ダウンロードしなおす)</summary>
		public int ScenarioVersion
		{
			get { return scenarioVersion; }
			set { scenarioVersion = value; }
		}
		[SerializeField]
		int scenarioVersion = -1;

		/// <summary>
		/// 設定データ
		/// </summary>
		[SerializeField]
		AdvSettingDataManager settingDataManager;

		/// <summary>
		/// エクスポートしたシナリオデータがあればここに設定可能
		/// </summary>
		[SerializeField]
		AdvScenarioDataExported[] exportedScenarioDataTbl;

		/// <summary>リソースのロードタイプ</summary>
		public LoadType ResourceLoadType
		{
			get { return resourceLoadType; }
		}
		[SerializeField]
		LoadType resourceLoadType;

		/// <summary>リソースディレクトリのサーバーアドレス</summary>
		[SerializeField]
		string urlResourceDir;

		/// <summary>リソースディレクトリのルートパス</summary>
		[SerializeField]
		string rootResourceDir;

		[SerializeField]
		string ResourceDir { get { return (ResourceLoadType == LoadType.Server ? urlResourceDir : rootResourceDir); } }

		void Start()
		{
			Engine.gameObject.SetActive(true);
			//ADVエンジンの初期化を開始
			switch(ScenarioDataLoadType)
			{
				case LoadType.Server:
					if (string.IsNullOrEmpty(urlScenarioData)) { Debug.LogError("Not set URL ScenarioData",this); return; }
					if (string.IsNullOrEmpty(ResourceDir)) { Debug.LogError("Not set ResourceData", this); return; }
					Engine.BootFromCsv(urlScenarioData, ResourceDir, ScenarioVersion );
					break;
				case LoadType.Local:
					if (settingDataManager == null) { Debug.LogError("Not set SettingDataManager", this); return; }
					if (exportedScenarioDataTbl.Length <= 0) { Debug.LogError("Not set ExportedScenarioDataTbl", this); return; }
					if (string.IsNullOrEmpty(ResourceDir)) { Debug.LogError("Not set ResourceData", this); return; }
					Engine.BootFromExportData(settingDataManager, exportedScenarioDataTbl, ResourceDir);
					break;
			}
			if (isAutomaticPlay)
			{
				StartCoroutine(CoPlayEngine());
			}
		}

		public void StartEngine()
		{
			StartCoroutine(CoPlayEngine());
		}

		IEnumerator CoPlayEngine()
		{
			while (Engine.IsWaitBootLoading) yield return 0;
			if (string.IsNullOrEmpty(startScenario))
			{
				Engine.StartGame();
			}
			else
			{
				Engine.StartGame(startScenario);
			}
		}
	}
}