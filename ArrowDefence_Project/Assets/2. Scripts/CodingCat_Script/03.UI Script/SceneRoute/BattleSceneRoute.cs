﻿namespace ActionCat
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;
    using DG.Tweening;
    using Games.UI;

    public class BattleSceneRoute : MonoBehaviour
    {
        public class ArrowSwapSlotInitData {
            public bool IsActiveSlot { get; private set; }
            public Sprite IconSprite { get; private set; }
            public System.Action SlotCallback { get; private set; }
            public ArrowSwapSlotInitData(bool isactiveslot, Sprite iconsprite, System.Action callback)
            {
                IsActiveSlot = isactiveslot;
                IconSprite   = iconsprite;
                SlotCallback = callback;
            }
        }

        //Screen Limit Variable
        [Header("DEV OPTIONS")]
        public bool IsVisible;
        [Range(0f, 1.0f)] 
        public float LineWidth = 0.1f;
        public Material DefaultLineMat;
        public bool isOnFPS = false;
        public bool isActiveScreenHit = true;

        [Header("ENTERING SCENE FADE OPTION")]
        public CanvasGroup ImgFade;
        public float FadeTime = 1.0f;

        [Header("PAUSE PANEL VARIABLES")]
        public GameObject PausePanel;
        public float PanelOpenFadeTime = 0.5f;

        [Header("RESULT PANEL VARIABLES")]
        public GameObject ResultPanel;
        public Transform SlotParentTr;
        public GameObject DropItemSlotPref;
        public List<UI_ItemDataSlot> DropItemSlots;

        [Header("GAMEOVER PANEL VARIABLES")]
        [SerializeField] GameObject overPanel = null;
        [SerializeField] GameObject overFrontPanel = null;
        [SerializeField] Transform overLogo = null;
        [SerializeField] Image overBackPanel = null;
        [SerializeField] CanvasGroup canvasGroupOverButton = null;
        [SerializeField] TextMeshProUGUI tmp_tips = null;
        [TextArea(3, 5)] public string StageTips = "";
        Sequence overSeq = null;

        [Header("ARROW SLOT")]
        [SerializeField] EventTrigger mainArrowSlot;
        [SerializeField] EventTrigger subArrowSlot;
        [SerializeField] EventTrigger specialArrowSlot;

        [Header("SKILL SLOT")]
        [SerializeField] Transform skillSlotTr;
        [SerializeField] GameObject prefCooldownType;
        [SerializeField] GameObject prefChargingType;
        [SerializeField] GameObject prefHitsType;
        [SerializeField] GameObject prefkillType;

        [Header("HIT SCREEN")]
        [SerializeField] Material hitScreenMaterial = null;
        public float ScreenHitFadeTime = .5f;
        public float ScreenHitAlpha = 0.4f;
        float hitFadeTimer = 0f;
        string generalAlpha = "_Alpha";

        private float screenZpos = 0f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 2f);

        [Header("ITEM SLOT TOOLTIP")]
        public Canvas BattleSceneUICanvas;
        public Camera UICamera;

        void Start()
        {
            #region LIMIT_LINE_MAKER
            if (IsVisible)
            {
                topLeftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
                bottomRightPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f));

                limitPoints[0] = new Vector3(topLeftPoint.x - offset.x, topLeftPoint.y + offset.y, screenZpos);
                limitPoints[1] = new Vector3(bottomRightPoint.x + offset.x, topLeftPoint.y + offset.y, screenZpos);
                limitPoints[2] = new Vector3(bottomRightPoint.x + offset.x, bottomRightPoint.y - offset.y, screenZpos);
                limitPoints[3] = new Vector3(topLeftPoint.x - offset.x, bottomRightPoint.y - offset.y, screenZpos);
                limitPoints[4] = new Vector3(topLeftPoint.x - offset.x, (topLeftPoint.y + offset.y) +
                                             (LineWidth * 0.5f), screenZpos);

                arrowLimitLine = gameObject.AddComponent<LineRenderer>();
                arrowLimitLine.positionCount = 5;
                arrowLimitLine.SetPosition(0, limitPoints[0]);
                arrowLimitLine.SetPosition(1, limitPoints[1]);
                arrowLimitLine.SetPosition(2, limitPoints[2]);
                arrowLimitLine.SetPosition(3, limitPoints[3]);
                arrowLimitLine.SetPosition(4, limitPoints[4]);
                arrowLimitLine.startWidth = LineWidth;

                if (DefaultLineMat != null) arrowLimitLine.material = DefaultLineMat;

                arrowLimitLine.hideFlags = HideFlags.HideInInspector;
            }
            #endregion

            #region BATTLE_SCENE_INITIALIZING

            PausePanel.GetComponent<CanvasGroup>().alpha = 0f; //이거 무엇?

            //Item Data Slot Initializing
            for (int i = 0; i < SlotParentTr.childCount; i++)
            {
                DropItemSlots.Add(SlotParentTr.GetChild(i).GetComponent<UI_ItemDataSlot>());
            }

            //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
            this.OnSceneEnteringFadeOut();

            //FPS Checker Initialize
            if (isOnFPS)
                gameObject.AddComponent<FrameRateCheck>();

            #endregion
        }

        void Update() {
            HitScreenUpdate();

            if(Input.GetKeyDown(KeyCode.I)) {
                OnHitScreen();
            }
        }

        void OnDisable() {
            //Hit Screen Fade Color Check
            if(hitScreenMaterial.GetFloat(generalAlpha) != 0f) {
                hitScreenMaterial.SetFloat(generalAlpha, 0f);
            }
        }

        private void OnSceneEnteringFadeOut() {
            if (GameManager.Instance.IsDevMode) return;

            //씬 진입 시 alpha 값 바꾸기 전 상황이 나오는 지 체크, alpha 값 바꿔주기 전 상황이 나오면
            //Build 시 alpha 값을 살려놓은 상태로 빌드..?
            ImgFade.alpha = 1f;

            ImgFade.DOFade(0f, FadeTime)
                   .OnStart(() =>
                   {
                       ImgFade.blocksRaycasts = false;
                   })
                   .OnComplete(() =>
                   {
                       ImgFade.blocksRaycasts = true;
                       ImgFade.gameObject.SetActive(false);
                   });
        }

        #region BUTTON_EVENT

        public void Btn_OpenPausePanel()
        {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(1f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => { PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                                       PausePanel.SetActive(true);
                                       GameManager.Instance.PauseBattle();})
                      .OnComplete(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = true);
        }

        public void Btn_ContinueGame() {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(0f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false)
                      .OnComplete(() => { PausePanel.SetActive(false);
                                          GameManager.Instance.ResumeBattle();
                      });
        }

        public void Btn_LoadMainScene() {
            ImgFade.DOFade(1f, FadeTime)
                   .SetUpdate(true)
                   .OnStart(() => { ImgFade.blocksRaycasts = false;
                                    ImgFade.gameObject.SetActive(true);
                   }) 
                   .OnComplete(() => { TooltipRelease();
                                       SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN);
                   });
        }

        public void Btn_ReloadScene() {
            ImgFade.DOFade(1f, FadeTime)
                   .SetUpdate(true)
                   .OnStart(() => {
                       ImgFade.blocksRaycasts = false;
                       ImgFade.gameObject.SetActive(true);
                   })
                   .OnComplete(() => {
                       TooltipRelease();
                       SceneLoader.Instance.ReloadScene();
                   });
        }

        public void Btn_Resurrection() {
            //1. Kill All Monster's
            //2. Return the GameManager.GameState is In-Game State.
        }

        /// <summary>
        /// Item Tooltip Release
        /// </summary>
        void TooltipRelease() {
            //Release Item Info Tooltip
            ItemTooltip.Instance.ReleaseParent();
        }

        #endregion

        #region ENABLE_POPUP
        public void OnEnableResultPanel(List<DropItem> items) {
            if (ResultPanel.activeSelf) return;

            ResultPanel.SetActive(true);

            CatLog.Log("Result Panel Updated !");

            //Scene에 미리 깔려있는 Slot들은 Awake때 캐싱됨 
            //slotCount Number Object Disable된 상황에서도 잘 잡히는거 확인

            int slotCount = SlotParentTr.childCount;

            if(items.Count > slotCount)
            {
                int moreSlotCount = items.Count - slotCount;

                for (int i = 0; i < moreSlotCount; i++)
                {
                    var newSlot = Instantiate(DropItemSlotPref, SlotParentTr).GetComponent<UI_ItemDataSlot>();
                    newSlot.gameObject.SetActive(false);
                    DropItemSlots.Add(newSlot);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                DropItemSlots[i].gameObject.SetActive(true);
                DropItemSlots[i].Setup(items[i].ItemAsset, items[i].Quantity, BattleSceneUICanvas, UICamera);
            }
        }

        public void OnEnableGameOverPanel() {
            //GameOver Back Panel Alpha Set.
            var tempBackPanelColor = overBackPanel.color;
            var backPanelOriginAlpha = tempBackPanelColor.a;
            tempBackPanelColor.a = 0f;
            overBackPanel.color = tempBackPanelColor;
            //Prepare Logo Position Set. (Save Origin Position.)
            var tempPos = overLogo.position;
            var originPos = tempPos;
            tempPos.y += 10f;
            overLogo.position = tempPos;
            //Button Group Fade Out
            canvasGroupOverButton.alpha = 0f;
            //Clear Tips String
            tmp_tips.text = "";

            overPanel.SetActive(true);
            overSeq = DOTween.Sequence()
                             .Append(overLogo.DOMove(originPos, 1f))
                             .Prepend(overBackPanel.DOFade(backPanelOriginAlpha, .5f))
                             .Append(canvasGroupOverButton.DOFade(1f, 0.3f))
                             .Insert(1f, tmp_tips.DOText(StageTips, 2f))
                             .OnComplete(() => overFrontPanel.SetActive(false));
            //Don't set SetAutoKill because no re-run is required.
        }

        /// <summary>
        /// Skip Panel Touch Event
        /// </summary>
        public void OnGameOverPanelSkip() {
            if(overSeq.IsPlaying()) {
                overSeq.Complete(); //Skipping GameOver Sequence.
            }

            //Disable Front Panel
            overFrontPanel.SetActive(false);
        }

        #endregion 

        #region BATTLE_SLOTS

        public void InitArrowSlots(ArrowSwapSlotInitData[] datas)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if (i == 0)      //Main Arrow Swap Slot
                {
                    if (datas[i].IsActiveSlot == false)
                    {
                        mainArrowSlot.gameObject.SetActive(false);
                        continue;
                    }

                    mainArrowSlot.gameObject.SetActive(true);
                    var arrowIcon = mainArrowSlot.transform.GetChild(0).GetComponent<Image>();
                    arrowIcon.enabled = true;
                    arrowIcon.preserveAspect = true;
                    arrowIcon.sprite = datas[i].IconSprite;

                    //Click Event 처리
                    var tempIndex = i;
                    EventTrigger.Entry mainSlotEntry = new EventTrigger.Entry();
                    mainSlotEntry.eventID = EventTriggerType.PointerClick;
                    mainSlotEntry.callback.AddListener((pointereventdata) => datas[tempIndex].SlotCallback());
                    mainArrowSlot.triggers.Add(mainSlotEntry);

                    //Controller Pulling 예외처리
                    GameManager.Instance.PreventionPulling(mainArrowSlot);
                }
                else if (i == 1) //Sub Arrow Swap Slot
                {
                    if(datas[i].IsActiveSlot == false)
                    {
                        subArrowSlot.gameObject.SetActive(false);
                        continue;
                    }

                    subArrowSlot.gameObject.SetActive(true);
                    var arrowIcon = subArrowSlot.transform.GetChild(0).GetComponent<Image>();
                    arrowIcon.enabled = true;
                    arrowIcon.preserveAspect = true;
                    arrowIcon.sprite = datas[i].IconSprite;

                    var tempIndex = i;
                    EventTrigger.Entry subSlotEntry = new EventTrigger.Entry();
                    subSlotEntry.eventID = EventTriggerType.PointerClick;
                    subSlotEntry.callback.AddListener((pointereventdata) => datas[tempIndex].SlotCallback());
                    subArrowSlot.triggers.Add(subSlotEntry);

                    GameManager.Instance.PreventionPulling(subArrowSlot);
                }
            }

            specialArrowSlot.gameObject.SetActive(false);
        }

        public void InitArrowSlots(bool isActiveSlot_m, bool isActiveSlot_s, Sprite icon_m, Sprite icon_s,
                                   System.Action slotAction_m, System.Action slotAction_s)
        {
            //Active || Disable Arrow Swap Slot GameObject.
            //Event Registration according to the value of isActive boolean.

            if (isActiveSlot_m == true)
            {
                mainArrowSlot.gameObject.SetActive(true);
                var arrowicon            = mainArrowSlot.transform.GetChild(0).GetComponent<Image>();
                arrowicon.enabled        = true;
                arrowicon.preserveAspect = true;
                arrowicon.sprite         = icon_m;

                EventTrigger.Entry m_slotEntry = new EventTrigger.Entry();
                m_slotEntry.eventID = EventTriggerType.PointerClick;
                m_slotEntry.callback.AddListener((data) => slotAction_m());
                mainArrowSlot.triggers.Add(m_slotEntry);

                //Bow Controller Pulling Limit [Down, Up Event]
                //EventTrigger.Entry m_slotEntryDown = new EventTrigger.Entry();
                //m_slotEntryDown.eventID = EventTriggerType.PointerDown;
                //m_slotEntryDown.callback.AddListener((data) => GameManager.Instance.SetBowPullingStop(true));
                //mainArrowSlot.triggers.Add(m_slotEntryDown);
                //
                //EventTrigger.Entry m_slotEntryUp = new EventTrigger.Entry();
                //m_slotEntryUp.eventID = EventTriggerType.PointerUp;
                //m_slotEntryUp.callback.AddListener((data) => GameManager.Instance.SetBowPullingStop(false));
                //mainArrowSlot.triggers.Add(m_slotEntryUp);

                //Swap시, Bow Pulling 방지
                GameManager.Instance.PreventionPulling(mainArrowSlot);
            }
            else mainArrowSlot.gameObject.SetActive(false);

            if (isActiveSlot_s == true)
            {
                subArrowSlot.gameObject.SetActive(true);
                var arrowicon            = subArrowSlot.transform.GetChild(0).GetComponent<Image>();
                arrowicon.enabled        = true;
                arrowicon.preserveAspect = true;
                arrowicon.sprite         = icon_s;

                EventTrigger.Entry s_slotEntry = new EventTrigger.Entry();
                s_slotEntry.eventID = EventTriggerType.PointerClick;
                s_slotEntry.callback.AddListener((data) => slotAction_s());
                subArrowSlot.triggers.Add(s_slotEntry);

                //Swap시, Bow Pulling 방지
                GameManager.Instance.PreventionPulling(subArrowSlot);
            }
            else subArrowSlot.gameObject.SetActive(false);

            //Init Special Arrow Slot GameObject
            specialArrowSlot.gameObject.SetActive(false);

            //if Init the Special Arrow
            //if (isActiveSlot_m == false && isActiveSlot_s == false) return;
            //또는 SP Arrow가 장착된 경우
            //if(isActiveSlot_sp)
            //{
            //    if (isActiveSlot_m) { }
            //    if (isActiveSlot_s) { }
            //}
            //else
            //{
            //    if (isActiveSlot_s && isActiveSlot_m) { }
            //}
        }

        public void InitSkillSlots(AccessorySkillSlot.ActiveSkillSlotInitData[] datas)
        {
            foreach (var data in datas)
            {
                //if (data == null) continue; //-> 로직 변경으로 체크할 필요없어졌다.

                switch (data.ActiveType)
                {
                    case SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE:
                        var cooldownslot = Instantiate(prefCooldownType, skillSlotTr).GetComponent<AccessorySkillSlot>();
                        cooldownslot.InitCoolDownSkillButton(data); break;

                    case SKILL_ACTIVATIONS_TYPE.CHARGING_ACTIVE:
                        var chargingslot = Instantiate(prefChargingType, skillSlotTr).GetComponent<AccessorySkillSlot>();
                        chargingslot.InitStackingSkillButton(data); break;

                    case SKILL_ACTIVATIONS_TYPE.KILLCOUNT_ACTIVE: break;

                    case SKILL_ACTIVATIONS_TYPE.HITSCOUNT_ACTIVE: break;

                    default: break;
                }
            }
        }

        #endregion

        #region SCREEN_HIT

        public void OnHitScreen() {
            //hitScreenMaterial.SetFloat(generalAlpha, ScreenHitAlpha);
            hitFadeTimer = ScreenHitFadeTime;
        }

        void HitScreenUpdate() {
            if(hitFadeTimer > 0f) {
                hitFadeTimer -= Time.unscaledDeltaTime;
                hitScreenMaterial.SetFloat(generalAlpha, Mathf.Lerp(ScreenHitAlpha, 0f, (1 - (hitFadeTimer / ScreenHitFadeTime))));
                if(hitFadeTimer <= 0f) {
                    hitScreenMaterial.SetFloat(generalAlpha, 0f);
                }
            }
        }

        #endregion
    }
}
