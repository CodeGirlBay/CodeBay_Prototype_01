﻿namespace CodingCat_Games
{
    using UnityEngine;
    using UnityEngine.UI;
    using CodingCat_Scripts;
    using DG.Tweening;
    using System.Collections.Generic;

    public class BattleSceneRoute : MonoBehaviour
    {
        //Screen Limit Variable
        [Header("DISABLE OBJECT LINE DRAW")]
        public bool IsVisible;
        [Range(0f, 1.0f)] 
        public float LineWidth = 0.1f;
        public Material DefaultLineMat;

        [Header("START FADE OPTION")]
        public CanvasGroup ImgFade;
        public float FadeTime = 1.0f;

        [Header("PANEL's")]
        public GameObject PausePanel;
        public GameObject ResultPanel;
        public GameObject GameOverPanel;
        public Canvas BattleSceneUICanvas;
        public float PanelOpenFadeTime = 0.5f;

        [Header("RESULT PANEL VARIABLE's")]
        public Transform SlotParentTr;
        public GameObject DropItemSlotPref;
        public List<UI_ItemDataSlot> DropItemSlots;

        private float screenZpos = 90f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 2f);

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

            #endregion
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

        public void Btn_ContinueGame()
        {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(0f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false)
                      .OnComplete(() => { PausePanel.SetActive(false);
                                          GameManager.Instance.ResumeBattle();
                      });
        }

        public void Btn_LoadMainScene()
        {
            ImgFade.DOFade(1f, FadeTime)
                   .OnStart(() => { ImgFade.blocksRaycasts = false;
                                    ImgFade.gameObject.SetActive(true);
                                    ReleaseBattleScene();})
                   .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN));
        }

        /// <summary>
        /// Main Scene이 Load되기 전 정리되어야할 UI들 처리
        /// </summary>
        private void ReleaseBattleScene()
        {
            //Release CCPooler 
            CCPooler.DestroyCCPooler();

            //Release Item Info Tootip
            ActionCat.Games.UI.ItemTooltip.Instance.ReleaseParent();
        }

        #endregion

        private void OnSceneEnteringFadeOut()
        {
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

        public void OnEnableResultPanel(List<DropItem> items)
        {
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
                DropItemSlots[i].Setup(items[i].ItemAsset, items[i].Quantity, BattleSceneUICanvas);
            }
        }
    }
}