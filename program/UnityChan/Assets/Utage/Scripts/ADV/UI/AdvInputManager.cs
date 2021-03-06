//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;

/// <summary>
/// ADVの入力処理
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Node2D))]
[AddComponentMenu("Utage/ADV/InputManger")]
public class AdvInputManager : MonoBehaviour
{
	/// <summary>ADVエンジン</summary>
	[SerializeField]
	AdvEngine engine;

	/// <summary>
	/// ボックスコライダー
	/// </summary>
	public BoxCollider2D BoxCollider2D{ get { return boxCollider2D ?? ( boxCollider2D = GetComponent<BoxCollider2D>() );}}
	BoxCollider2D boxCollider2D;

	/// <summary>
	/// 初期化。スクリプトから動的に生成する場合に
	/// </summary>
	/// <param name="engine">ADVエンジン</param>
	/// <param name="width">ゲーム画面の幅</param>
	/// <param name="height">ゲーム画面の高さ</param>
	public void InitOnCreate(AdvEngine engine, float width, float height)
	{
		this.engine = engine;
		BoxCollider2D.size = new Vector2(width, height);
		BoxCollider2D.isTrigger = true;
	}

	void Update()
	{
		switch(engine.UiManager.Status)
		{
			case AdvUiManager.UiStatus.Backlog:	
				break;
			case AdvUiManager.UiStatus.HideMessageWindow:	//メッセージウィンドウが非表示
				//右クリック
				if (InputUtil.IsMousceRightButtonDown())
				{	//通常画面に復帰
					engine.UiManager.Status = AdvUiManager.UiStatus.Default;
				}
				else if (InputUtil.IsInputScrollWheelUp())
				{
					//バックログ開く
					engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
				}
				break;
			case AdvUiManager.UiStatus.Default:
				if (engine.Page.IsWaitPage)
				{	//入力待ち
					if (InputUtil.IsMousceRightButtonDown())
					{	//右クリックでウィンドウ閉じる
						engine.UiManager.Status = AdvUiManager.UiStatus.HideMessageWindow;
					}
					else if (InputUtil.IsInputScrollWheelUp())
					{	//バックログ開く
						engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
					}
					else
					{
						if (engine.Config.IsMouseWheelSendMessage && InputUtil.IsInputScrollWheelDown())
						{
							//メッセージ送り
							engine.Page.InputSendMessage();
						}
					}
				}
				break;
		}
	}

	/// <summary>
	/// タッチされたとき
	/// </summary>
	/// <param name="touch">タッチ入力データ</param>
	protected virtual void OnTouchDown(TouchData2D touch)
	{
		switch(engine.UiManager.Status)
		{
			case AdvUiManager.UiStatus.Backlog:	
				break;
			case AdvUiManager.UiStatus.HideMessageWindow:	//メッセージウィンドウが非表示
				engine.UiManager.Status = AdvUiManager.UiStatus.Default;
				break;
			case AdvUiManager.UiStatus.Default:
				if (engine.Config.IsSkip)
				{
					//スキップ中ならスキップ解除
					engine.Config.ToggleSkip();
				}
				else
				{
					if (engine.Page.IsShowingText)
					{
						if (!engine.Config.IsSkip)
						{
							//文字送り
							engine.Page.InputSendMessage();
						}
					}
				}
				break;
		}
	}
}
