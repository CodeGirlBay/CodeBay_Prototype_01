﻿namespace ActionCat {
    using UnityEngine;

    public class Item_SpArr : Item_Equipment {
        private GameObject spArrowPref = null;
        private ASInfo[] skillInfos    = null;
        private float specialArrDefaultSpeed = 12f;
        private SpArrCondition condition = null;
        #region PROPERTY
        public ASInfo[] GetSkillInfos {
            get {
                if(skillInfos == null) {
                    skillInfos = new ASInfo[0];
                }
                return skillInfos;
            }
        }
        public SpArrCondition Condition {
            get {
                if(condition == null) {
                    throw new System.Exception("Special Arrow Condition is Null.");
                }
                return condition;
            }
        }
        #endregion
        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            return new Ability[0]; 
        }

        public void Init(PlayerAbilitySlot ability, int poolQuatity) {
            if(spArrowPref.TryGetComponent<AD_Arrow>(out AD_Arrow arrow)) {
                arrow.SetSpeed(specialArrDefaultSpeed);
            }
            CCPooler.AddPoolList(AD_Data.POOLTAG_SPECIAL_ARROW, poolQuatity, spArrowPref, false);
        }

        public Item_SpArr(ItemDt_SpArr data) : base() {
            EquipType   = EQUIP_ITEMTYPE.EQUIP_ARROW;
            Item_Id     = data.Item_Id;
            Item_Name   = data.Item_Name;
            Item_Desc   = data.Item_Desc;
            Item_Sprite = data.Item_Sprite;
            Item_Grade  = data.Item_Grade;

            //Ability
            abilities = GetNewAbilities(data.abilityDatas);

            //Special Arrow Prefab
            spArrowPref = data.MainArrowObj;

            //Skills Info
            var tempList = new System.Collections.Generic.List<ASInfo>();
            if (data.ArrowSkillFst != null) tempList.Add(new ASInfo(data.ArrowSkillFst));
            if (data.ArrowSkillSec != null) tempList.Add(new ASInfo(data.ArrowSkillSec));
            skillInfos = tempList.ToArray();

            condition = new SpArrCondition(data.ChargeType, data.MaxCost, data.MaxStackCount, data.CostIncrease);
        }
        #region ES3
        public Item_SpArr() { }
        ~Item_SpArr() { }
        #endregion
    }

    public sealed class SpArrCondition {
        //SAVED VALUE
        private CHARGETYPE chargeType;
        private float costIncrease;
        private int maxCost;
        private int maxStackCount;

        //NON-SAVED VALUE
        private float tempCost;
        private float currentCost;
        private float addIncCost;
        private int currentStackedCount;
        private UI.SwapSlots spSlot = null;

        #region PROPERTY
        public bool IsTypeTime {
            get {
                if (chargeType == CHARGETYPE.TIME) return true;
                else                               return false;
            }
        }

        public bool IsInitSlot {
            get {
                if (spSlot == null) return false;
                else                return true;
            }
        }
        #endregion

        public SpArrCondition(CHARGETYPE type, int cost, int count, float increase) {
            if(type == CHARGETYPE.NONE || count <= 0) {
                throw new System.Exception("New Condition instnace: Missing condition");
            }

            this.chargeType    = type;
            this.maxStackCount = count;
            this.maxCost       = cost;
            this.costIncrease  = increase;
        }
        #region ES3
        public SpArrCondition() { }
        #endregion

        public void Initialize(UI.SwapSlots slot) {
            addIncCost = GameManager.Instance.GetGlobalAbility().IncreaseSpArrCost;
            switch (chargeType) {
                case CHARGETYPE.KILL: InitTypeKill(); break;
                case CHARGETYPE.TIME: InitTypeAtck(); break;
                case CHARGETYPE.ATCK: InitTypeTime(); break;
                default: throw new System.NotImplementedException();
            }

            if (slot == null) throw new System.Exception("Arrow Swap Slot is Null.");
            spSlot = slot;
            UpdateInterface();
        }

        #region INITIALIZE
        void InitTypeKill() {
            BattleProgresser.OnMonsterDeath += CostIncByKill;
        }

        void InitTypeAtck() {
            BattleProgresser.OnMonsterHit += CostIncByAtck;
        }

        void InitTypeTime() {

        }
        #endregion

        #region INCREASE
        void CostIncByAtck() {
            if (currentStackedCount >= maxStackCount) return;

            tempCost = currentCost + (costIncrease + addIncCost);
            if(tempCost >= maxCost) {
                currentStackedCount++;
                if(currentStackedCount < maxStackCount) {
                    tempCost -= maxCost;
                }
                else {
                    tempCost = 0f;
                }
            }
            currentCost = tempCost;
            UpdateInterface();
        }
        void CostIncByKill() {
            if (currentStackedCount >= maxStackCount) return;

            tempCost = currentCost + (costIncrease + addIncCost);
            if (tempCost >= maxCost) {
                currentStackedCount++;
                if (currentStackedCount < maxStackCount) {
                    tempCost -= maxCost;
                }
                else {
                    tempCost = 0f;
                }
            }
            currentCost = tempCost;
            UpdateInterface();
        }
        public void CostIncByTime() {
            if (currentStackedCount >= maxStackCount) return;
            currentCost = Time.deltaTime + (addIncCost * 0.01f);
            if(currentCost >= maxCost) {
                currentStackedCount++;
                currentCost = 0f;
            }
            UpdateInterface();
        }
        #endregion

        public void Clear() {
            currentCost = 0f;
            addIncCost  = 0f;
            currentStackedCount = 0;
            tempCost = 0f;
            spSlot = null;

            switch (chargeType) {
                case CHARGETYPE.NONE:                                                   break;
                case CHARGETYPE.KILL: BattleProgresser.OnMonsterDeath -= CostIncByKill; break;
                case CHARGETYPE.TIME:                                                   break;
                case CHARGETYPE.ATCK: BattleProgresser.OnMonsterHit   -= CostIncByAtck; break;
                default: break;
            }
        }

        void UpdateInterface() {
            if (!IsInitSlot) return;
            spSlot.SSlotUpdateCost(currentCost / maxCost);
            spSlot.SSSlotUpdateStack(currentStackedCount);
        }
    }
}