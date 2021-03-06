//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// トランジションの管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/TransitionManager")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Node2D))]
	public class AdvTransitionManager : MonoBehaviour
	{
		LinearValue fadeAlpha = new LinearValue();					//フェード処理用の値

		Node2D node2D;
		Node2D Node2D { get { if (null == node2D) node2D = GetComponent<Node2D>(); return node2D; } }

		/// <summary>
		/// フェード中か
		/// </summary>
		public bool IsWait { get { return !fadeAlpha.IsEnd(); } }

		void Awake()
		{
			Node2D.LocalAlpha = 0;
		}

		/// <summary>
		/// クリア
		/// </summary>
		public void Clear()
		{
			fadeAlpha.Clear();
		}

		/// <summary>
		/// フェードアウト開始
		/// </summary>
		/// <param name="time">フェード時間</param>
		/// <param name="color">フェードカラー</param>
		public void FadeOut(float time, Color color)
		{
			fadeAlpha.Init(time, fadeAlpha.GetValue(), 1);
			color.a = fadeAlpha.GetValue();
			Node2D.LocalColor = color;
		}

		/// <summary>
		/// フェードイン開始
		/// </summary>
		/// <param name="time">フェード時間</param>
		/// <param name="color">フェードカラー</param>
		public void FadeIn(float time, Color color)
		{
			fadeAlpha.Init(time, fadeAlpha.GetValue(), 0);
			color.a = fadeAlpha.GetValue();
			Node2D.LocalColor = color;
		}

		void Update()
		{
			fadeAlpha.IncTime();
			if( Node2D.LocalAlpha != fadeAlpha.GetValue() )
			{
				Node2D.LocalAlpha = fadeAlpha.GetValue();
			}
		}
	}
}