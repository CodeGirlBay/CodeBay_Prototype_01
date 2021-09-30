﻿namespace CodingCat_Games
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using CodingCat_Scripts;

    public class AD_BowController : MonoBehaviour
    {
        public static AD_BowController instance;

        public enum BOWPULLING_TYPE //활 조준 타입 -> 추후 User Data에서 받아오도록 방식 변경
        {
            TYPE_AROUND_BOW,         
            TYPE_FIRSTTOUCH_POSITION,
            TYPE_AUTOMATIC_SHOT
        }

        [Header("GENERAL")]
        public Camera MainCam;
        private Touch screenTouch;
        
        [Header("BOW CONTROL VARIABLES")]
        public BOWPULLING_TYPE PullType = BOWPULLING_TYPE.TYPE_AROUND_BOW;
        public Transform RightClampPoint, LeftClampPoint;
        public float TouchRadius = 1f;
        public Image BowCenterPointImg;
        [Range(1f, 20f)] public float SmoothRotateSpeed = 12f;
        [Range(1f, 5f)]  public float ArrowPullingSpeed = 1f;

        private bool bowPullBegan;               //Bow Pull State variable
        private float bowAngle;                  //The Angle Variable (angle between Click point and Bow).
        private Vector2 initialTouchPos;         //처음 터치한 곳을 저장할 벡터
        private Vector2 correctionTouchPosition; //First Touch Type Correction Vector
        private Vector2 limitTouchPosVec;        //Bow GameObject와 거리를 비교할 벡터
        private Vector3 direction;
        private Vector3 currentClickPosition;
        private Vector3 tempEulerAngle;
        public bool BowPullBegan { get { return bowPullBegan; } }   //Bow Pulling Condition property (Used ACSP)                          

        [Header("ARROW VARIABLES")] //Arrow Relation Variables
        [ReadOnly] public AD_Arrow ArrowComponent;
        [ReadOnly] public Transform ArrowParentTr;
        [ReadOnly] public GameObject LoadedArrow;

        private float requiredLaunchForce = 250f;
        private bool launchArrow = false;
        private Vector2 arrowForce;
        private Vector3 arrowPosition;
        private Vector3 initialArrowScale    = new Vector3(1.5f, 1.5f, 0f);
        private Vector3 initialArrowRotation = new Vector3(0f, 0f, -90f);

        /// <summary>
        /// Bow Skills Delegate
        /// </summary>
        public delegate void BowSkillsDel(float rot, float angle, byte arrownum, Transform arrowparent,
                                          AD_BowController bowcontroller, Vector3 initScale, Vector3 initpos, Vector2 force);
        public BowSkillsDel BowSkillSet;

        #region NOT_USED_VARIABLES
        //The Distance between first click and Bowlope
        //private float radius;
        //The Point on the Circumference based on radius and angle.
        //private Vector3 cPoint = Vector3.zero;
        //private Vector2 distance;
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            //Initialize Main Camera Object
            if (MainCam == null) MainCam = Camera.main;

            //Initialize variables For Arrow to be Loaded
            if(RightClampPoint == null || LeftClampPoint == null)
            {
                LeftClampPoint  = this.transform.GetChild(3).GetChild(0);
                RightClampPoint = this.transform.GetChild(3).GetChild(1);

                if(transform.GetChild(3).name != "Bow_ClampPoints")
                {
                    CatLog.WLog("Bow Clamp Point is in the wrong Location. Check The Bow");
                }
            }

            ArrowParentTr = transform.parent;

            //Initialize Bow Center Pivot Image Object
            if (BowCenterPointImg)
                BowCenterPointImg.transform.position = transform.position;

            //yield return new WaitUntil(() => CCPooler.IsInitialized);
            StartCoroutine(this.ArrowReload());
        }

        private void Update()
        {
#if UNITY_ANDROID
            if (Input.touchCount != 0)
            {
                //Get Value On Screen Touch -> Area Designation Func Add
                screenTouch = Input.GetTouch(0);

                if (screenTouch.phase == TouchPhase.Began)
                {
                    //Touch Begin
                    this.BowBegan(screenTouch.position);
                }
                else if (screenTouch.phase == TouchPhase.Moved && bowPullBegan)
                {
                    //Touch Moved
                    this.BowMoved(screenTouch.position);
                }
                else if (screenTouch.phase == TouchPhase.Ended)
                {
                    //Touch Ended
                    this.BowReleased(screenTouch.position);
                }
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                //Click Began
                this.BowBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //Click Ended
                this.BowReleased(Input.mousePosition);
            }

            if (bowPullBegan)
            {
                //Click Moved
                this.BowMoved(Input.mousePosition);
            }
#endif
        }

        private void FixedUpdate()
        {
            if (launchArrow)
            {
                this.LaunchTheArrow();
                launchArrow = false;
            }
        }

        private void BowBegan(Vector2 pos)
        {
            switch (PullType)
            {
                //조건 1. 활 주변의 일정거리 주변을 클릭 | 터치했을때만 조준 가능
                case BOWPULLING_TYPE.TYPE_AROUND_BOW: if (!this.CheckTouchRaius(pos)) return; break;
                //조건 2. 처음 클릭한 곳 기준으로 활의 기준점 지정
                case BOWPULLING_TYPE.TYPE_FIRSTTOUCH_POSITION: this.SetInitialTouchPos(pos);  break;
                //조건 3. 자동으로 적을 인식하고 활을 조준
                case BOWPULLING_TYPE.TYPE_AUTOMATIC_SHOT: CatLog.Log("Not Support this Pull Type, Return Function"); return;
            }

            if(AD_BowRope.instance.arrowCatchPoint == null && ArrowComponent != null)
            {
                AD_BowRope.instance.arrowCatchPoint = ArrowComponent.arrowChatchPoint;
            }

            bowPullBegan = true;
        }

        private void BowMoved(Vector2 pos)
        {
            //Get CurrentClick Position
            currentClickPosition = MainCam.ScreenToWorldPoint(pos);

            //Pull Type 추가에 따른 스크립트 구분
            if (PullType == BOWPULLING_TYPE.TYPE_AROUND_BOW)
            {
                #region ORIGIN_CODES
                //this.direction = currentClickPosition - transform.position;
                //
                ////클릭 위치에 따른 활 자체의 각도를 변경할 변수 저장
                //this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90;
                //
                ////Set Direction of the Bow
                //tempEulerAngle = transform.eulerAngles;
                //tempEulerAngle.z = bowAngle;
                //transform.eulerAngles = tempEulerAngle;
                //
                ////Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
                //cPoint.x = AD_BowRope.instance.transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                //cPoint.y = AD_BowRope.instance.transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
                //
                ////Pull or Drag the arrow ralative to Click Position
                //distance = (AD_BowRope.instance.transform.position - currentClickPosition) -
                //           (AD_BowRope.instance.transform.position - cPoint);
                //Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
                //cPoint.x = transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                //cPoint.y = transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
                //
                //Pull or Drag the arrow ralative to Click Position
                //distance = (transform.position - currentClickPosition) -
                //           (transform.position - cPoint);
                //
                //this.bowAngle = Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed);
                #endregion

                this.direction = currentClickPosition - transform.position;

                this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed),
                                            0f, 180f);

                //Set Direction of the Bow
                tempEulerAngle = transform.eulerAngles;
                tempEulerAngle.z = bowAngle;
                transform.eulerAngles = tempEulerAngle;

                //CatLog.Log($"Temp Euler Angle Z Pos : {bowAngle.ToString()}"); //Bow Angle Debugging
                DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, transform.position);
            }
            else if (PullType == BOWPULLING_TYPE.TYPE_FIRSTTOUCH_POSITION)
            {
                //Touch Correction Vector Setting (보정하지 않으면 활이 바로 아래로 회전해버린다)
                correctionTouchPosition = new Vector2(currentClickPosition.x, currentClickPosition.y - 0.1f);

                this.direction = correctionTouchPosition - initialTouchPos;

                //클릭 위치에 따른 활 자체의 각도를 변경할 변수
                this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed),
                                            0f, 180f);

                //Lerp to Set Direction of the Bow
                tempEulerAngle = transform.eulerAngles;
                tempEulerAngle.z = bowAngle;
                transform.eulerAngles = tempEulerAngle;

                #region ORIGIN_CODES
                //BowRope Controller
                //cPoint.x = initialTouchPos.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                //cPoint.y = initialTouchPos.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
                //
                //Pull or Drag the arrow ralative to Click Position (distance 변수는 )
                //distance = (initialTouchPos - (Vector2)currentClickPosition) -
                //           (initialTouchPos - (Vector2)cPoint);
                #endregion

                DrawTouchPos.Instance.DrawTouchLine(correctionTouchPosition, initialTouchPos);
            }

            if (LoadedArrow != null)
            {
                #region ORIGIN_CODES
                //Before Visualizing
                //arrowPosition = currentLoadedArrow.transform.position;
                //arrowPosition.x = arrowComponent.rightClampPoint.position.x - distance.x;
                //arrowPosition.y = arrowComponent.rightClampPoint.position.y - distance.y;
                //currentLoadedArrow.transform.position = arrowPosition;

                //After Visualizing
                //arrowPosition = currentLoadedArrow.transform.position;
                //arrowPosition = leftClampPoint.position;
                //currentLoadedArrow.transform.position = arrowPosition;

                //if (Vector2.Distance(currentLoadedArrow.transform.position, leftClampPoint.position) > .4f) CatLog.Log("0.1f 보다 멀다");
                //else CatLog.Log("가깝다");
                //CatLog.Log($"Distance of Arrow Position : {Vector2.Distance(currentLoadedArrow.transform.position, leftClampPoint.position).ToString()}");
                #endregion

                //Bow Pulling Over Time
                arrowPosition = LoadedArrow.transform.position;
                arrowPosition = Vector3.MoveTowards(arrowPosition, LeftClampPoint.position, Time.deltaTime * ArrowPullingSpeed); //deltaTime * speed 변수해주면 되겠다
                LoadedArrow.transform.position = arrowPosition;

                arrowForce = LoadedArrow.transform.up * ArrowComponent.power;

                if (arrowForce.magnitude > requiredLaunchForce)
                {
                    //초기 스크립트에 Path Drawer가 있었던 조건문
                    //
                    //
                }
                else
                {

                }
            }
        }

        private void BowReleased(Vector2 pos)
        {
            if (bowPullBegan)
            {
                bowPullBegan = false;
                currentClickPosition = MainCam.ScreenToWorldPoint(pos);

                launchArrow = true;
            }

            DrawTouchPos.Instance.ReleaseTouchLine();
        }

        private void LaunchTheArrow()
        {
            if (LoadedArrow == null)
            {
                //장전할 수 있는 화살이 없음 -> CatPoolManager 체크 또는 Pool Arrow Object 갯수 체크
                CatLog.WLog("Current Loaded Arrow is Null, Can't Launch the Arrow");
                return;
            }

            //일정 이상 당겨져야 발사되도록 할 조건 :: 다시 점검 
            if (arrowForce.magnitude < requiredLaunchForce)
            {
                //Reset Position
                LoadedArrow.transform.position = RightClampPoint.transform.position;

                CatLog.Log("Not Enough Require Force, More Pulling the Bow !");
                return;
            }

            AD_BowRope.instance.arrowCatchPoint = null;

            ArrowComponent.ShotArrow(arrowForce, ArrowParentTr);

            //Active Bow Skill
            BowSkillSet?.Invoke(transform.eulerAngles.z, 30f, 2, ArrowParentTr, this, initialArrowScale,
                                ArrowComponent.arrowChatchPoint.transform.position, arrowForce);

            LoadedArrow = null;
            ArrowComponent     = null;

            //ReLoad Logic Start
            StartCoroutine(this.ArrowReload());
        }

        private bool CheckTouchRaius(Vector2 pos)
        {
            this.limitTouchPosVec = MainCam.ScreenToWorldPoint(pos);
            
            float touchDistanceFromBow = (limitTouchPosVec - (Vector2)transform.position).magnitude;

            if (touchDistanceFromBow <= TouchRadius) return true;
            else CatLog.WLog("Touch Aroud Bow !");   return false;
        }

        private void SetInitialTouchPos(Vector2 pos)
        {
            initialTouchPos = MainCam.ScreenToWorldPoint(pos);

            CatLog.Log("Save First Touch Position");
        }

        private IEnumerator ArrowReload()
        {
#region ORIGIN_RELAOD
            //var arrow = CatPoolManager.Instance.LoadNormalArrow(this);
            //
            //currentLoadedArrow = arrow;
            //arrowComponent     = arrow.gameObject.GetComponent<AD_Arrow>();
            //loadedArrowRbody   = arrow.gameObject.GetComponent<Rigidbody2D>();
            //
            //arrow.transform.SetParent(this.transform, false);
            //arrow.transform.localScale                     = this.initialArrowScale;
            //arrow.transform.localEulerAngles               = this.initialArrowRotation;
            //arrow.transform.position   = ReturnInitArrowPos(arrow.transform.position);
            //
            ////Right, Left Clamp 한번만 잡아주면 다음 Active때 잡아주지 않아도 가능한지?
            //// -> 추후 게임이 시작되기 전에 미리 Clamp 한번에 Initial해주면 어떨지?
            //arrowComponent.leftClampPoint  = this.leftClampPoint;
            //arrowComponent.rightClampPoint = this.rightClampPoint;
#endregion

#region POOL_RELOAD

            yield return null;
            
            LoadedArrow = CCPooler.SpawnFromPool(AD_Data.POOLTAG_MAINARROW, this.transform, initialArrowScale,
                                     RightClampPoint.position, Quaternion.identity);

            LoadedArrow.transform.localEulerAngles = initialArrowRotation;

            ArrowComponent = LoadedArrow.GetComponent<AD_Arrow>();
            ArrowComponent.leftClampPoint = this.LeftClampPoint;
            ArrowComponent.rightClampPoint = this.RightClampPoint;

#endregion
        }

        /// <summary>
        /// this a Function To Adjust Current Loaded Arrow to the Position of Right Clamp Point.
        /// </summary>
        /// <param name="pos">Currnet Loaded Arrow Position Vector</param>
        /// <returns></returns>
        private Vector3 ReturnInitArrowPos(Vector3 pos)
        {
            var changePos = pos;
            changePos = RightClampPoint.position;

            return changePos;
        }
    }
}
