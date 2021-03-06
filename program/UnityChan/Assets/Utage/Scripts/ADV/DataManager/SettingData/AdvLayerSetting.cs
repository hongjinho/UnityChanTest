//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// レイヤー設定のデータ
	/// </summary>	
	[System.Serializable]
	public partial class AdvLayerSettingData : SerializableDictionaryFileReadKeyValue
	{
		/// <summary>
		/// レイヤー名
		/// </summary>
		public string Name { get { return this.Key; } }

		/// <summary>
		/// レイヤーのタイプ
		/// </summary>
		public enum LayerType
		{
			/// <summary>背景</summary>
			Bg,
			/// <summary>キャラクター</summary>
			Character,
			/// <summary>その他スプライト</summary>
			Sprite,
		};
		/// <summary>
		/// レイヤーのタイプ
		/// </summary>
		public LayerType Type { get { return this.type; } }
		[SerializeField]
		LayerType type;

		/// <summary>
		/// 中心座標
		/// </summary>
		public Vector2 Center { get { return this.center; } }
		[SerializeField]
		Vector2 center;

		/// <summary>
		/// 描画順
		/// </summary>
		public int Order { get { return this.order; } }
		[SerializeField]
		int order;
		//	public int SpriteSortingOrderOffset {get {return Depth*1000;}}

		/// <summary>
		/// レイヤーマスク（Unityのレイヤー名）
		/// </summary>
		public string LayerMask { get { return this.layerMask; } }
		[SerializeField]
		string layerMask;

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			string key = AdvParser.ParseCell<string>(row, AdvColumnName.LayerName);
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			else
			{
				InitKey(key);
				this.type = AdvParser.ParseCell<LayerType>(row, AdvColumnName.Type);
				this.center.Set(AdvParser.ParseCellOptional<float>(row, AdvColumnName.X,0), AdvParser.ParseCellOptional<float>(row, AdvColumnName.Y,0));
				this.order = AdvParser.ParseCell<int>(row, AdvColumnName.Order);
				this.layerMask = AdvParser.ParseCellOptional<string>(row, AdvColumnName.LayerMask,"");
				return true;
			}
		}
	}

	/// <summary>
	/// レイヤー設定
	/// </summary>
	[System.Serializable]
	public partial class AdvLayerSetting : SerializableDictionaryFileRead<AdvLayerSettingData>
	{
		/// <summary>
		/// デフォルトの背景用のレイヤー
		/// </summary>
		public AdvLayerSettingData DefaultBGLayer { get { return this.defaultBGLayer; } }
		AdvLayerSettingData defaultBGLayer;

		/// <summary>
		/// デフォルトのキャラクター用のレイヤー
		/// </summary>
		public AdvLayerSettingData DefaultCharacterLayer { get { return this.defaultCharacterLayer; } }
		AdvLayerSettingData defaultCharacterLayer;

		/// <summary>
		/// デフォルトのスプライト用のレイヤー
		/// </summary>
		public AdvLayerSettingData DefaultSpriteLayer { get { return this.defaultSpriteLayer; } }
		AdvLayerSettingData defaultSpriteLayer;

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public void BootInit( AdvBootSetting settingData  )
		{
			//Serializableクラスはprivateにしてもnullで初期化されてない場合がある？ようなので、nullで明示的な初期化
			defaultBGLayer = null;
			defaultCharacterLayer = null;
			defaultSpriteLayer = null;
			foreach (AdvLayerSettingData layer in List)
			{
				if (defaultBGLayer == null && layer.Type == AdvLayerSettingData.LayerType.Bg)
				{
					defaultBGLayer = layer;
				}
				else if (defaultCharacterLayer == null && layer.Type == AdvLayerSettingData.LayerType.Character)
				{
					defaultCharacterLayer = layer;
				}
				else if (defaultSpriteLayer == null && layer.Type == AdvLayerSettingData.LayerType.Sprite)
				{
					defaultSpriteLayer = layer;
				}
			}
		}

		public bool Contains(string layerName, AdvLayerSettingData.LayerType type )
		{
			AdvLayerSettingData data;
			if( this.TryGetValue(layerName, out data ) )
			{
				return data.Type == type;
			}
			return false;
		}
	}
}