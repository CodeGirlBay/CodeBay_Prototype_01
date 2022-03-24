﻿using System;
using UnityEngine;
using DG.Tweening;
using ActionCat;
using ActionCat.Interface;

public class MainSceneRoute : MonoBehaviour {
    static MainSceneRoute _inst;
    [Header("CANVAS")]
    [SerializeField] RectTransform mainUICanvas = null;

    [Header("MENU")]
    [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
    [SerializeField] GameObject[] panels = null;
    IMainMenu[] menus = new IMainMenu[] { };

    [Header("POPUP")]
    public BattlePopup battlePop;
    public ItemInfoPop itemInfoPop;

    [Header("FADE")]
    public CanvasGroup ImgFade = null;
    public float FadeTime = 2.0f;

    [Header("SAVE & LOAD")]
    public bool isAutoLoad;

    private void Awake() {
        _inst = this;
        for (int i = 0; i < panels.Length; i++) {
            if (panels[i].TryGetComponent<IMainMenu>(out IMainMenu iMainMenu)) {
                menus = GameGlobal.AddArray<IMainMenu>(menus, iMainMenu);
            }
        }

        if(menus.Length != panels.Length) {
            CatLog.WLog("Not Full-Cached MainMenu Interface !");
        }
    }

    private void Start() {
        GameManager.Instance.Initialize();

        //Init Notify
        Notify.Inst.Init(mainUICanvas);

        //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
        FadeOut();

        //User SaveData Auto Load
        if (isAutoLoad) {
            ActionCat.GameManager.Instance.AutoLoadUserData();
        }
            
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            var allCraftingSlots = ActionCat.Data.CCPlayerData.infos.CraftingInfos;
            foreach (var slot in allCraftingSlots) {
                slot.Update();
            }
        }
    }

    private void OnDestroy() {
        _inst = null;
    }

    #region BUTTON_EVENT

    //======================================================================================================================================
    //============================================================ [ MAIN MENU ] ===========================================================

    public void BE_OPEN_MAINMENU(int index) {
        //Ignore Panel
        bool isPossibleMenuChange = (menus.TrueForAll(menu => menu.IsTweenPlaying() == false));
        switch (index) {
            case 0: if (openedPanelType == PANELTYPE.INVENTORY) isPossibleMenuChange = false; break;
            case 1: if (openedPanelType == PANELTYPE.CRAFT)     isPossibleMenuChange = false; break;
            case 2: if (openedPanelType == PANELTYPE.SHOP)      isPossibleMenuChange = false; break;
            case 3: if (openedPanelType == PANELTYPE.BATTLE)    isPossibleMenuChange = false; break;
            default: throw new System.NotImplementedException();
        }
        if (isPossibleMenuChange == false) {
            CatLog.Log("Canceled Menu Change.");
            return;
        }

        //Close Opened Panel
        switch (openedPanelType) {
            case PANELTYPE.NONE:                            break;
            case PANELTYPE.INVENTORY: menus[0].MenuClose(); break;
            case PANELTYPE.CRAFT:     menus[1].MenuClose(); break;
            case PANELTYPE.SHOP:      menus[2].MenuClose(); break;
            case PANELTYPE.BATTLE:    menus[3].MenuClose(); break;
            default: throw new System.NotImplementedException();
        }

        //Open Panel
        switch (index) {
            case 0: menus[0].MenuOpen(); openedPanelType = PANELTYPE.INVENTORY; break;
            case 1: menus[1].MenuOpen(); openedPanelType = PANELTYPE.CRAFT;     break;
            case 2: menus[2].MenuOpen(); openedPanelType = PANELTYPE.SHOP;      break;
            case 3: menus[3].MenuOpen(); openedPanelType = PANELTYPE.BATTLE;    break;
            default: throw new System.NotImplementedException();
        }
    }

    public void BE_CLOSE_MAINMENU(int index) {
        switch (index) {
            case 0: menus[0].MenuClose(); break;
            case 1: menus[1].MenuClose(); break;
            case 2: menus[2].MenuClose(); break;
            case 3: menus[3].MenuClose(); break;
            default: throw new System.NotImplementedException();
        }
        openedPanelType = PANELTYPE.NONE;
    }

    //======================================================================================================================================
    //============================================================== [ POPUP ] =============================================================

    public void BE_CLOSE_POPUP(GameObject go) {
        go.SetActive(false);
    }

    public void BE_LOAD_TITLE() {
        //Go to Title Scene
        ImgFade.DOFade(1f, FadeTime)
               .OnStart(() => {
                   ImgFade.blocksRaycasts = false;
                   ImgFade.gameObject.SetActive(true);
               })
               .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.SCENE_TITLE));
    }

    public void BE_STAGE_SELECTOR(int index) {
        battlePop.EnablePopup(index);
    }

    #endregion

    //======================================================================================================================================
    //============================================================= [ ITEMINFO ] ===========================================================

    public static void OPEN_ITEMINFO_PREVIEW(AD_item previewitem) {
        _inst.itemInfoPop.OpenPreview(previewitem);
    }

    /// <summary>
    /// Open Item Information Popup When Click in the Invnetory Items
    /// </summary>
    /// <param name="item"></param>
    public static void OPEN_ITEMINFO(AD_item item) {
        switch (item) {
            case Item_Consumable  conItem: _inst.itemInfoPop.OpenPopup_ConsumableItem(conItem);  break;
            case Item_Material    matItem: _inst.itemInfoPop.OpenPopup_MaterialItem(matItem);    break; 
            case Item_Equipment equipItem: _inst.itemInfoPop.OpenPopup_EquipmentItem(equipItem); break;
            default: throw new System.NotImplementedException();
        }
    }

    //======================================================================================================================================
    //=============================================================== [ FADE ] =============================================================

    public static void FadeIn(Action startAction, Action completeAction) {
        _inst.ImgFade.DOFade(1f, _inst.FadeTime)
                     .OnStart(() => {
                         _inst.ImgFade.blocksRaycasts = false;
                         _inst.ImgFade.gameObject.SetActive(true);
                         startAction();
                     })
                     .OnComplete(() => {
                         completeAction();
                     });
    }

    private void FadeOut() {
        if (GameManager.Instance.IsDevMode) {
            return;
        }

        ImgFade.alpha = 1f;

        ImgFade.DOFade(0f, FadeTime)
               .OnStart(() => ImgFade.blocksRaycasts = false)
               .OnComplete(() => 
               {   ImgFade.blocksRaycasts = true;
                   ImgFade.gameObject.SetActive(false);
               });

    }

    //======================================================================================================================================
    //======================================================================================================================================

    enum PANELTYPE {
        NONE,
        INVENTORY,
        CRAFT,
        SHOP,
        BATTLE
    }
}

namespace ActionCat.UI.MainMenu {
    using DG.Tweening;
    internal sealed class MainMenuTween {
        Sequence mainSequence  = null;
        float openMenuTime = 0.5f;
        float closeMenuTime = 0.3f;
        
        //PROPERTY
        public bool IsTweenPlaying {
            get {
                return mainSequence.IsActive();
            }
        }

        /// <summary>
        /// other가 지정되어 있다면, tween이 시작되기 전에, other object를 활성화 합니다.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="cg"></param>
        /// <param name="other"></param>
        public void MenuOpenTween(RectTransform rt, CanvasGroup cg, GameObject other = null) {
            rt.localScale = Vector3.zero;
            cg.alpha      = StNum.floatZero;
            if (other == null) {
                rt.gameObject.SetActive(true);
            }
            else {
                other.SetActive(true);
            }
            mainSequence = OpenSequence(rt, cg);
        }

        /// <summary>
        /// other가 지정되어 있다면, tween이 시작되기 전에, other object를 비 활성화 합니다.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="cg"></param>
        /// <param name="other"></param>
        public void MenuCloseTween(RectTransform rt, CanvasGroup cg, GameObject other = null) {
            cg.DOFade(StNum.floatZero, closeMenuTime)
              .OnStart(() => cg.blocksRaycasts = false)
              .OnComplete(() => {
                  cg.blocksRaycasts = true;
                  if(other == null) {
                      rt.gameObject.SetActive(false);
                  }
                  else {
                      other.SetActive(false);
                  }
              });
        }

        Sequence OpenSequence(RectTransform rt, CanvasGroup cg) {
            return DOTween.Sequence()
                          .SetAutoKill(true)
                          .OnStart(() => {
                              cg.blocksRaycasts = false;
                          })
                          .Append(rt.DOScale(StNum.floatOne, openMenuTime))
                          .Join(cg.DOFade(StNum.floatOne, openMenuTime))
                          .OnComplete(() => {
                              cg.blocksRaycasts = true;
                          });
        }

        Sequence CloseSequence(RectTransform rt, CanvasGroup cg) {
            return DOTween.Sequence()
                          .SetAutoKill(true)
                          .OnStart(() => {
                              cg.blocksRaycasts = false;
                          })
                          .Append(rt.DOScale(StNum.floatZero, closeMenuTime))
                          .Join(cg.DOFade(StNum.floatZero, closeMenuTime))
                          .OnComplete(() => {
                              cg.blocksRaycasts = false;
                          });
        }

        internal MainMenuTween(float openTime, float closeTime) {
            openMenuTime  = openTime;
            closeMenuTime = closeTime;
        }
    }
}
