//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：背景表示・切り替え
	/// </summary>
	internal class AdvCommandBg : AdvCommand
	{
		public AdvCommandBg(StringGridRow row, AdvSettingDataManager dataManager)
		{
			string label = AdvParser.ParseCell<string>(row, AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(label))
			{
				Debug.LogError(row.ToErrorString(label + " is not contained in file setting"));
			}

			this.file = AddLoadFile(dataManager.TextureSetting.LabelToFilePath(label));
			//表示位置
			float x;
			if (AdvParser.TryParseCell<float>(row, AdvColumnName.Arg4, out x))
			{
				this.x = x;
			}
			float y;
			if (AdvParser.TryParseCell<float>(row, AdvColumnName.Arg5, out y))
			{
				this.y = y;
			}
			//フェード時間
			this.fadeTime = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.LayerManager.SetBgSprite(file, x, y, fadeTime);
			engine.LayerManager.IsEventMode = false;
		}
		AssetFile file;
		object x = null;
		object y = null;
		float fadeTime;
	}
}