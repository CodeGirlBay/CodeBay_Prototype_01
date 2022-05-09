using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("increaseValue", "abilityType")]
	public class ES3UserType_ElementalActivation : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ElementalActivation() : base(typeof(ActionCat.ElementalActivation)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ElementalActivation)obj;
			
			writer.WritePrivateField("increaseValue", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ElementalActivation)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "increaseValue":
					reader.SetPrivateField("increaseValue", reader.Read<System.Int16>(), instance);
					break;
					case "abilityType":
					reader.SetPrivateField("abilityType", reader.Read<ActionCat.ABILITY_TYPE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ElementalActivation();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ElementalActivationArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ElementalActivationArray() : base(typeof(ActionCat.ElementalActivation[]), ES3UserType_ElementalActivation.Instance)
		{
			Instance = this;
		}
	}
}