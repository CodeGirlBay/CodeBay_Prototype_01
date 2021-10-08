using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("pullingType")]
	public class ES3UserType_GameSettings : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GameSettings() : base(typeof(CodingCat_Games.GameSettings)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.GameSettings)obj;
			
			writer.WritePrivateField("pullingType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.GameSettings)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "pullingType":
					reader.SetPrivateField("pullingType", reader.Read<CodingCat_Games.PULLINGTYPE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.GameSettings();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_GameSettingsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GameSettingsArray() : base(typeof(CodingCat_Games.GameSettings[]), ES3UserType_GameSettings.Instance)
		{
			Instance = this;
		}
	}
}