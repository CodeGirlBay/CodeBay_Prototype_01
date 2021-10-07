using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("maxNumberOfEffect", "effects", "specialEffect", "EquipType", "Item_Id", "Item_Name", "Item_Desc", "Item_Amount", "Item_Sprite", "Item_Type", "Item_Grade")]
	public class ES3UserType_Item_Accessory : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Accessory() : base(typeof(CodingCat_Games.Item_Accessory)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Item_Accessory)obj;
			
			writer.WritePrivateField("maxNumberOfEffect", instance);
			writer.WritePrivateField("effects", instance);
			writer.WritePrivateField("specialEffect", instance);
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
			var instance = (CodingCat_Games.Item_Accessory)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "maxNumberOfEffect":
					reader.SetPrivateField("maxNumberOfEffect", reader.Read<System.Int32>(), instance);
					break;
					case "effects":
					reader.SetPrivateField("effects", reader.Read<CodingCat_Games.AccessoryRFEffect[]>(), instance);
					break;
					case "specialEffect":
					reader.SetPrivateField("specialEffect", reader.Read<CodingCat_Games.AccessorySPEffect>(), instance);
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
			var instance = new CodingCat_Games.Item_Accessory();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_AccessoryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_AccessoryArray() : base(typeof(CodingCat_Games.Item_Accessory[]), ES3UserType_Item_Accessory.Instance)
		{
			Instance = this;
		}
	}
}