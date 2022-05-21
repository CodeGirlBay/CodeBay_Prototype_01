﻿namespace ActionCat.Data {
    public static class CCPlayerData {
        public static AD_Inventory      inventory  = new AD_Inventory();
        public static Player_Equipments equipments = new Player_Equipments();
        public static PlayerAbility     ability    = new PlayerAbility();
        public static PlayerInfo        infos      = new PlayerInfo();
        public static GameSettings      settings   = new GameSettings();

        static readonly string KEY_INVENTORY = "KEY_INVENTORY";
        static readonly string KEY_EQUIPMENT = "KEY_EQUIPMENT";
        static readonly string KEY_INFOS     = "KEY_INFOS";
        static readonly string KEY_SETTINGS  = "KEY_SETTINGS";

        //public static bool IsExistsDefaultFile = false;

        public static string PersistentDataPath {
            get {
                return UnityEngine.Application.persistentDataPath;
            }
        }

        public static string DataPath {
            get {
                return UnityEngine.Application.dataPath;
            }
        }

        public static string SettingsJsonFilePath { 
            //지금 Application.DataPath로 잡고있는데 만약에 빌드해보고 문제 생기면 Persistent path 로 잡아도 상관없음
            get {
                return UnityEngine.Application.dataPath + "/ArrDef_Settings.es3";
            }
        }

        #region DEBUGGING

        public static void Debug_LoadUserJson() {
            inventory  = (ES3.KeyExists(KEY_INVENTORY)) ? ES3.Load<AD_Inventory>(KEY_INVENTORY)      : throw new System.Exception("INVENTORY KEY NOT EXISTS.");
            equipments = (ES3.KeyExists(KEY_EQUIPMENT)) ? ES3.Load<Player_Equipments>(KEY_EQUIPMENT) : throw new System.Exception("EQUIPMENT KEY NOT EXISTS.");
            infos      = (ES3.KeyExists(KEY_INFOS))     ? ES3.Load<PlayerInfo>(KEY_INFOS)            : throw new System.Exception("INFO KEY NOT EXISTS.");
        }

        public static void Debug_SaveUserjson() {
            ES3.Save(KEY_INVENTORY, inventory);
            ES3.Save(KEY_EQUIPMENT, equipments);
            ES3.Save(KEY_INFOS, infos);
        }

        public static void Unlock() {
            infos.AddCraftSlot(3);
            infos.OpenSlot(0, 1, 2);
        }

        #endregion

        public static void SupplyInitItem() {
#if UNITY_EDITOR
            infos.AddCraftSlot(3);   //초기 할당 슬롯
            infos.OpenSlot(0, 1, 2); //초기 오픈 슬롯
#elif UNITY_ANDRIOD
            infos.AddCraftSlot(3); //초기 할당 슬롯
            infos.OpenSlot(0);     //초기 오픈 슬롯
#else
            infos.AddCraftSlot(3); //초기 할당 슬롯
            infos.OpenSlot(0);     //초기 오픈 슬롯
#endif
        }

        public static bool SaveUserDataJson() {
            //ES3.Save<AD_Inventory>(inventoryKey, inventory);
            try {
                ES3.Save(KEY_INVENTORY, inventory);
                ES3.Save(KEY_EQUIPMENT, equipments);
                ES3.Save(KEY_INFOS, infos);
                CatLog.Log(StringColor.GREEN, "Success Save All User Data.");
            }
            catch (System.Exception ex) {
                CatLog.ELog("Failed Save UserData json. Exception: \n" + ex.Message);
                return false;
            }

            return true;
        }

        public static bool LoadUserDataJson(out byte log) {
            log = 0;
            if (!ES3.FileExists()) { //UserSave Json 존재하지 않는경우. 'Log=1' (세이브파일 없음)
                CatLog.WLog("User Data File is Not Exsist.");
                log = 1;
                return false;   
            }

            try { //이 로직 테스트한번 진행해보기. throw new Exception 났을 때, catch에서 어떻게잡는지 한번 보기
                //inventory  = (ES3.KeyExists(KEY_INVENTORY)) ? ES3.Load<AD_Inventory>(KEY_INVENTORY)      : throw new System.Exception("INVENTORY KEY NOT EXISTS.");
                //equipments = (ES3.KeyExists(KEY_EQUIPMENT)) ? ES3.Load<Player_Equipments>(KEY_EQUIPMENT) : throw new System.Exception("EQUIPMENT KEY NOT EXISTS.");
                //infos      = (ES3.KeyExists(KEY_INFOS))     ? ES3.Load<PlayerInfo>(KEY_INFOS)            : throw new System.Exception("INFO KEY NOT EXISTS.");

                //KEY Verification
                if (!ES3.KeyExists(KEY_INVENTORY)) {
                    log = 2;
                    throw new System.Exception("KEY ERROR: Inventory Key Not Exist.");
                }
                if (!ES3.KeyExists(KEY_EQUIPMENT)) {
                    log = 3;
                    throw new System.Exception("KEY ERROR: Equipment Key Not Exist.");
                }
                if (!ES3.KeyExists(KEY_INFOS)) {
                    log = 4;
                    throw new System.Exception("KEY ERROR: Information Key Not Exist.");
                }

                //Load Json
                inventory  = ES3.Load<AD_Inventory>(KEY_INVENTORY);
                equipments = ES3.Load<Player_Equipments>(KEY_EQUIPMENT);
                infos      = ES3.Load<PlayerInfo>(KEY_INFOS);
            }
            catch (System.Exception ex) { 
                CatLog.ELog("Failed to Load UserData Json: \n" + ex.Message);
                log = (log == 0) ? (byte)255 : log;
                return false;
            }

            CatLog.Log(StringColor.GREEN, "User Data Json Loaded Successfully !"); //ES3 로드 성공 !
            return true;
        }

        public static bool LoadSettingsJson(out string log) {
            try {
                var isExistsFile = ES3.FileExists(CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                if (!isExistsFile) { //SettingsJson이 존재하지 않는경우, 새로운 SettingsJson을 생성하고 저장
                    settings = GameSettings.defaultSettings;
                    ES3.Save<GameSettings>(KEY_SETTINGS, settings, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                    CatLog.WLog("Settings Json Not Exists in DataPath. Create New Settings Json.");
                }

                //SettingsJson이 존재하지만 저장될 때와는 다른 KEY를 사용하여 저장된 경우, 새로운 Json을 생성함.
                if (ES3.KeyExists(KEY_SETTINGS, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings) == false) {
                    ES3.DeleteFile(CCPlayerData.SettingsJsonFilePath);  //Different Key를 가진 Settings Json제거
                    ES3.Save<GameSettings>(KEY_SETTINGS, settings, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                    CatLog.WLog("the SettingsJson file Exists, but it is a file Saved using a different key. \n" + "Remove the Existing Json and Create a new Json.");
                }

                settings = ES3.Load<GameSettings>(KEY_SETTINGS, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
            }
            catch (System.Exception ex) {
                log = ex.Message;
                return false;
            }

            log = "Settings Json Load Completed !";
            return true;
        }

        public static void SaveSettingsJson() {
            try { //현재 GameSettings 저장
                ES3.Save<GameSettings>(KEY_SETTINGS, settings, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
            }
            catch (System.Exception ex) {
                CatLog.ELog("Failed to Save Settings Json. Exception: \n" + ex.Message);
            }
        }
    }
}
