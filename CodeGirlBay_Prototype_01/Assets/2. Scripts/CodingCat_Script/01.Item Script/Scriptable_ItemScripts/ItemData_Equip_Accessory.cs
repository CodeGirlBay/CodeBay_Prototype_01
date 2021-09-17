﻿namespace CodingCat_Games
{
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName ="Item_Accessory_Asset", menuName = "Scriptable Object Asset/Item_Accessory_Asset")]
    public class ItemData_Equip_Accessory : ItemData_Equip
    {
        [Header("Accessory Item Data")]
        public string effect = "";
        public MonoScript Effect_AimSight;

        public ItemData_Equip_Accessory() : base()
        {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;
        }

        //public void TEST()
        //{
        //    System.Type effectscript = Effect_AimSight.GetClass();
        //
        //    GameObject test = new GameObject();
        //    test.AddComponent(effectscript);
        //}

#if UNITY_EDITOR
        [MenuItem("CodingCat/Scriptable Object/Accessory Item Asset")]
        static void CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<ItemData_Equip_Accessory>();
            AssetDatabase.CreateAsset(asset, "Assets/05. Scriptable_Object/Equipment_Items/Accessory Item Asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}
