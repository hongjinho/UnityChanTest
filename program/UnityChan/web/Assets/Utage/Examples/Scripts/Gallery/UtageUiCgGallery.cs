//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

/// <summary>
/// CGギャラリー画面のサンプル
/// </summary>
[AddComponentMenu("Utage/Examples/CgGallery")]
public class UtageUiCgGallery : UtageUiView
{
	/// <summary>
	/// CG表示画面
	/// </summary>
	public UtageUiCgGalleryViewer CgView;

	/// <summary>
	/// リストビュー
	/// </summary>
	public ListView listView;

	/// <summary>
	/// リストビューアイテムのリスト
	/// </summary>
	List<AdvCgGalleryData> itemDataList = new List<AdvCgGalleryData>();

	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	bool isInit = false;


	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	void OnOpen()
	{
		isInit = false;
		this.listView.Close();	///いったん閉じる
		StartCoroutine(CoWaitOpen());
	}

	/// <summary>
	/// クローズしたときに呼ばれる
	/// </summary>
	void OnClose()
	{
		this.listView.Close();
	}

	//起動待ちしてから開く
	IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading)
		{
			yield return 0;
		}

		itemDataList = Engine.DataManager.SettingDataManager.TextureSetting.CreateCgGalleryList( Engine.SystemSaveData.GalleryData );
		listView.Open(itemDataList.Count, CallBackCreateItem);
		isInit = true;
	}


	/// <summary>
	/// リストビューのアイテムが作成されるときに呼ばれるコールバック
	/// </summary>
	/// <param name="go">作成されたアイテムのGameObject</param>
	/// <param name="index">作成されたアイテムのインデックス</param>
	void CallBackCreateItem(GameObject go, int index)
	{
		UtageUiCgGalleryItem item = go.GetComponent<UtageUiCgGalleryItem>();
		AdvCgGalleryData data = itemDataList[index];
		item.Init(data, index );
		Button button = go.GetComponent<Button>();
		button.Target = this.gameObject;
	}

	void Update()
	{
		//右クリックで戻る
		if (isInit && InputUtil.IsMousceRightButtonDown())
		{
			Back();
		}
	}

	/// <summary>
	/// 各アイテムが押された
	/// </summary>
	/// <param name="button">押されたアイテム</param>
	void OnTap(Button button)
	{
		Close();
		CgView.Open(this,itemDataList[button.Index]);
	}
}
