﻿namespace CodingCat_Games
{
    using UnityEngine;

    public class UI_Equipments : MonoBehaviour
    {
        static UI_Equipments _Inst;

        [Header("Player Equipment UI")]
        public UI_ItemSlot BowItem_Slot;

        [Space(10)]
        public UI_ItemSlot ArrowItem_Slot0;
        public UI_ItemSlot ArrowItem_Slot1;
        public UI_ItemSlot ArrowItem_Slot_Sp;

        [Space(10)]
        public UI_ItemSlot EquipItem_Slot_0;
        public UI_ItemSlot EquipItem_Slot_1;
        public UI_ItemSlot EquipItem_Slot_2;

        [Header("Equip Item Slot Prefab")]
        public GameObject equipSlotPref;

        /*
           EquipSlot Update 되는 시점 : 인벤토리 UI 열린 시점, 
                                       장비 아이템을 장착 시점, 
                                       장비 아이템을 해제 시점                           
        */

        private void Awake() => _Inst = this;

        void OnEnable()
        {
            UpdateEquipUI();
        }

        public void UpdateEquipUI()
        {
            //Equipment Item : Bow
            if(AD_PlayerData.PlayerEquipments.IsEquipBow())
            {
                BowItem_Slot.gameObject.SetActive(true);
                BowItem_Slot.Setup(AD_PlayerData.PlayerEquipments.GetBowItem());
            }
            else
            {
                if (BowItem_Slot.gameObject.activeSelf) BowItem_Slot.Clear();
            }

            //Equipment Item : Arrow (Main)
            if(AD_PlayerData.PlayerEquipments.IsEquipMainArrow())
            {
                ArrowItem_Slot0.gameObject.SetActive(true);
                ArrowItem_Slot0.Setup(AD_PlayerData.PlayerEquipments.GetMainArrow());
            }
            else
            {
                if (ArrowItem_Slot0.gameObject.activeSelf) ArrowItem_Slot0.Clear();
            }

            //Equipment Item : Arrow (Sub)
            if(AD_PlayerData.PlayerEquipments.IsEquipSubArrow())
            {
                ArrowItem_Slot1.gameObject.SetActive(true);
                ArrowItem_Slot1.Setup(AD_PlayerData.PlayerEquipments.GetSubArrow());
            }
            else
            {
                if (ArrowItem_Slot1.gameObject.activeSelf) ArrowItem_Slot1.Clear();
            }
        }

        public static void Update_EquipUI()
        {
            if(AD_PlayerData.PlayerEquipments.IsEquipBow())
            {
                _Inst.BowItem_Slot.gameObject.SetActive(true);
                _Inst.BowItem_Slot.Setup(AD_PlayerData.PlayerEquipments.GetBowItem());
            }
            else
            {
                if (_Inst.BowItem_Slot.gameObject.activeSelf) _Inst.BowItem_Slot.Clear();
            }

            if (AD_PlayerData.PlayerEquipments.IsEquipMainArrow())
            {
                _Inst.ArrowItem_Slot0.gameObject.SetActive(true);
                _Inst.ArrowItem_Slot0.Setup(AD_PlayerData.PlayerEquipments.GetMainArrow());
            }
            else
            {
                if (_Inst.ArrowItem_Slot0.gameObject.activeSelf) _Inst.ArrowItem_Slot0.Clear();
            }

            if (AD_PlayerData.PlayerEquipments.IsEquipSubArrow())
            {
                _Inst.ArrowItem_Slot1.gameObject.SetActive(true);
                _Inst.ArrowItem_Slot1.Setup(AD_PlayerData.PlayerEquipments.GetSubArrow());
            }
            else
            {
                if (_Inst.ArrowItem_Slot1.gameObject.activeSelf) _Inst.ArrowItem_Slot1.Clear();
            }
        }
    }
}