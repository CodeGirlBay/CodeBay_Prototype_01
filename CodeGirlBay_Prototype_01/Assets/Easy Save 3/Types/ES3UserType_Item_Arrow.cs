using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("mainArrowObject", "lessArrowObject", "EquipType", "Item_Id", "Item_Name", "Item_Desc", "Item_Amount", "Item_Sprite", "Item_Type", "Item_Grade")]
	public class ES3UserType_Item_Arrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Arrow() : base(typeof(CodingCat_Games.Item_Arrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Item_Arrow)obj;
			
			writer.WritePrivateFieldByRef("mainArrowObject", instance);
			writer.WritePrivateFieldByRef("lessArrowObject", instance);
			writer.WritePrivateField("EquipType", instance);
			writer.WritePrivateField("Item_Id", instance);
			writer.WritePrivateField("Item_Name", instance);
			writer.WritePrivateField("Item_Desc", instance);
			writer.WritePrivateField("Item_Amount", instance);
			writer.WritePrivateFieldByRef("Item_Sprite", instance);
			writer.WritePrivateField("Item_Type", instance);
			writer.WritePrivateField("Item_Grade", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.Item_Arrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "mainArrowObject":
					reader.SetPrivateField("mainArrowObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "lessArrowObject":
					reader.SetPrivateField("lessArrowObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "EquipType":
					reader.SetPrivateField("EquipType", reader.Read<CodingCat_Games.EQUIP_ITEMTYPE>(), instance);
					break;
					case "Item_Id":
					reader.SetPrivateField("Item_Id", reader.Read<System.Int32>(), instance);
					break;
					case "Item_Name":
					reader.SetPrivateField("Item_Name", reader.Read<System.String>(), instance);
					break;
					case "Item_Desc":
					reader.SetPrivateField("Item_Desc", reader.Read<System.String>(), instance);
					break;
					case "Item_Amount":
					reader.SetPrivateField("Item_Amount", reader.Read<System.Int32>(), instance);
					break;
					case "Item_Sprite":
					reader.SetPrivateField("Item_Sprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "Item_Type":
					reader.SetPrivateField("Item_Type", reader.Read<CodingCat_Games.ITEMTYPE>(), instance);
					break;
					case "Item_Grade":
					reader.SetPrivateField("Item_Grade", reader.Read<CodingCat_Games.ITEMGRADE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.Item_Arrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_ArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_ArrowArray() : base(typeof(CodingCat_Games.Item_Arrow[]), ES3UserType_Item_Arrow.Instance)
		{
			Instance = this;
		}
	}
}