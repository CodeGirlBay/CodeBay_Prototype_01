﻿namespace ActionCat
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameGlobal
    {
        public static Vector2 ScreenOffset = new Vector2(2f, 2f);
        public static Vector3 ArrowScale = new Vector3(1.5f, 1.5f, 1f);
        public static readonly int RandomIntRangeCorrection = 1;

        /// <summary>
        /// int 배열에서 무작위 "index"를 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RandomIndexInArray(int[] value)
        {
            int randomIndexInRange = UnityEngine.Random.Range(0, value.Length);
            return randomIndexInRange;
        }

        /// <summary>
        /// int 배열에서 무작위 요소를 result에 할당합니다.
        /// </summary>
        /// <param name="intarray">Length가 1이상인 배열</param>
        /// <param name="result"></param>
        public static void RandomIntInRange(int[] intarray, ref int result)
        {
            if (intarray.Length <= 0) return;

            int randomIndex = GameGlobal.RandomIndexInArray(intarray);
            int valueOfRange = intarray[randomIndex];
            result = valueOfRange;
        }

        /// <summary>
        /// Int배열에서 랜덤한 Index의 값을 반환합니다.
        /// </summary>
        /// <param name="intArray"></param>
        /// <returns></returns>
        public static int RandomIntInArray(int[] intArray)
        {
            if (intArray.Length <= 0)
            {
                CatLog.WLog("Int Array Parameter Size 0, return 1");
                return 1;
            }

            int valueOfArray = intArray[GameGlobal.RandomIndexInArray(intArray)];
            return valueOfArray;
        }

        public static int RandomQauntityInRange(int[] quantityArray)
        {
            if (quantityArray.Length <= 0)
            {
                //DropItem Class Quantity Array Size 가 0인 경우
                CatLog.WLog("Quantity Array Size is 0, return int value 1");
                return 1;
            }
            else if (quantityArray.Length == 1)
            {
                //DropItem Class Quantity Array Size 1의 경우 (주로 장비아이템)
                return quantityArray[0];
            }
            else
            {
                //DropItem Class Quantity Array Size 2의 경우 (소모품, 재료 아이템) MinQuantity ~ MaxQuantity 사이의 값 Return
                int quantityInArray = Random.Range(quantityArray[0], quantityArray[1] + RandomIntRangeCorrection);
                return quantityInArray;
            }
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static GameObject GetBowGameObjectInScene()
        {
            GameObject BowGameObject = GameObject.FindWithTag(AD_Data.OBJECT_TAG_BOW);
            if (BowGameObject)                                        return BowGameObject;
            else CatLog.WLog("Bow GameObject Not Found This Scene."); return null;
        }

        public static Vector3 FixedVectorOnScreen(Vector2 position) {
            return new Vector3(position.x, position.y, 0f);
        }

        public static void FixedPosOnScreen(ref Vector3 pos) {
            pos = new Vector3(pos.x, pos.y, 0f);
        }

        public static T[] ArrayRemoveAll<T>(T[] array, System.Predicate<T> predicate)
        {
            List<T> list = new List<T>(array);
            list.RemoveAll(predicate);

            //for (int i = 0; i < list.Count; i++)
            //{
            //    if(predicate(list[i]))
            //    {
            //        list.Remove(list[i]);
            //        i = 0;
            //    }
            //}

            return list.ToArray();
        }

        public static T[] ArrayRemoveAt<T>(T[] array, int index)
        {
            if (array.Length <= index)
            {
                CatLog.WLog("Target Index Number is bigger than, Array Size");
                return array;
            }

            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            return list.ToArray();
        }

        /// <summary>
        /// 매개변수로 들어온 Array는 본 메서드에서 직접 수정될 수 없습니다. 선언적 루프입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ArrayForeach<T>(T[] array, System.Action<T> action)
        {
            if (array.Length <= 0 || action is null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        /// <summary>
        /// 배열을 직접 조건에 따라 수정하여 반환합니다.
        /// 요소제거는 RemoveAll, RemoveAt을 사용합시다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        //public static T[] ReturnArrayForeach<T>(T[] array, System.Action<T> action)
        //{
        //    if (array is null || action is null) return array;
        //
        //    List<T> tempList = new List<T>(array);
        //    //tempList.ForEach((x) => action(x));
        //
        //    for (int i = 0; i < tempList.Count; i++)
        //    {
        //        action(tempList[i]);
        //    }
        //
        //    return tempList.ToArray();
        //} // -> 이 방법도 안댐 ㅎ

        /// <summary>
        /// Swap Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs  = rhs;
            rhs  = temp;
        }

        static void TEST()
        {
            List<string> intList = new List<string>();
            intList.RemoveAll((x) => x == "string" || x == "int");
        }
    }

    #region ENUMS_BATTLE

    /// <summary>
    ///  정의. Bow Controller 화살 장전 타입
    /// </summary>
    public enum LOAD_ARROW_TYPE
    {
        ARROW_MAIN = 0,
        ARROW_SUB  = 1
    }

    #endregion

    #region ENUMS_SKILL
    /// <summary>
    /// 정의. Bow, Accessory Special Skill Level
    /// </summary>
    public enum SKILL_LEVEL
    {
        LEVEL_LOW    = 0,
        LEVEL_MEDIUM = 1,
        LEVEL_HIGH   = 2,
        LEVEL_UNIQUE = 3
    }
    /// <summary>
    /// 정의. 구현된 Bow Skill Type
    /// 추가 화살 갯수, 방향, 패턴관여
    /// </summary>
    public enum BOWSKILL_TYPE
    {
        SKILL_EMPTY,
        SKILL_SPREAD_SHOT,
        SKILL_RAPID_SHOT,
        SKILL_ARROW_RAIN
    }
    /// <summary>
    /// 정의. 구현된 Accessory Special Effect Type
    /// 전투 중, 독자적인 효과 부여 (중첩 불가)
    /// </summary>
    public enum ACSP_TYPE
    {
        SPEFFECT_NONE,
        SPEFFECT_AIMSIGHT,
        SPEEFECT_SLOWTIME
    }
    /// <summary>
    /// 정의. 구현된 ReinForcement Effect Type 
    /// 주로 수치 상향, 조정에 적용 (중첩 가능)
    /// </summary>
    public enum RFEF_TYPE
    {
        RFEFFECT_NONE,
        RFEFFECT_INCREASE_DAMAGE,
        RFEFFECT_INCREASE_PHYSICAL_DAMAGE,
        RFEFFECT_INCREASE_MAGICAL_DAMAGE,
        RFEFFECT_INCREASE_HEALTH
    }
    /// <summary>
    /// 정의. 구현된 Arrow Skill Type
    /// </summary>
    public enum ARROWSKILL
    {
        SKILL_REBOUND,
        SKILL_HOMING,
        SKILL_SPLIT,
        SKILL_PIERCING,
    }
    /// <summary>
    /// 정의. Skill 발동 UI 타입
    /// </summary>
    public enum SKILL_ACTIVATIONS_TYPE
    {
        COOLDOWN_ACTIVE,
        CHARGING_ACTIVE,
        KILLCOUNT_ACTIVE,
        HITSCOUNT_ACTIVE
    }
    /// <summary>
    /// 정의. ARROW SKILL 발동 타입
    /// </summary>
    public enum ARROWSKILL_ACTIVETYPE
    {
        FULL,
        ATTACK_AIR,
        ATTACK_ADDPROJ,
        ATTACK,
        AIR_ADDPROJ,
        AIR,
        ADDPROJ,
        EMPTY,
    }
    ///[7] TTT : FULL SKILL
    ///[8] TTF : ATK, AIR
    ///[9] TFT : ATK, ADD PROJ
    ///[10] TFF : ATK
    ///[11] FTT : AIR, ADD PROJ
    ///[12] FTF : AIR
    ///[13] FFT : ADD PROJ
    ///[14] FFF : EMPTY
    #endregion

    #region ENUMS_ITEM

    #endregion
}
