﻿namespace ActionCat {
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class WalkerState : MonsterState {
        [Header("COMPONENET")]
        [SerializeField] CapsuleCollider2D coll;
        [SerializeField] Animator          anim;
        [SerializeField] Rigidbody2D       rigidBody;

        [Header("MOVEMENT")]
        public float InitMoveSpeed  = 0.5f;
        public float AttackInterval = 2f;

        //Animator Parameters
        int animState = Animator.StringToHash("state");
        int atkState  = Animator.StringToHash("attack");
        int hitParams = Animator.StringToHash("hit");
        int runParams = Animator.StringToHash("isRun");

        //Walker Properties
        float moveSpeed      = 0f;
        float attackTimer    = 0f;
        float attackInterval = 0f;
        bool isFindWall      = false;

        Coroutine actionSpeedCo = null;

        void Foo(float duration, float ratio) {
            //애니메이션 speed 조절하는 테스트 라인
            //1f가 default value 란다 얘야
            anim.speed = 0.5f;

            if (actionSpeedCo != null) {
                StopCoroutine(actionSpeedCo);
            }
            actionSpeedCo = StartCoroutine(ChangeActionSpeed(ratio, duration));

            //bool값을 하나 둬서 현재 속도가 변한경우나 업데이트 한번 해줘서 값받아와야 하는 상황을 인지하게 해줘야 하는지 한번 생각해보기 !
            //현재 velocity값 받아와서 0 이상일 경우 ratio만큼 줄어들게 하면 어떨까
        }

        public override void ValActionSpeed(float ratio, float duration) {
            if (this.currentState == STATETYPE.DEATH) { //Death State에서는 받지 않음
                return;
            }

            if (actionSpeedCo != null) {
                StopCoroutine(actionSpeedCo);
            }
            actionSpeedCo = StartCoroutine(ChangeActionSpeed(ratio, duration));
        }

        /// <summary>
        /// Change MoveSpeed & Animation Speed
        /// </summary>
        /// <param name="ratio">0f~1f, 값이 높게 들어올수록 많이 느려짐</param>
        /// <param name="duration">지속시간</param>
        /// <returns></returns>
        System.Collections.IEnumerator ChangeActionSpeed(float ratio, float duration) {
            var tempRatio = StNum.floatOne - ratio;
            this.currentActionSpeed = tempRatio;
            this.anim.speed         = tempRatio;
            //--> 현재 상태에 따른 속도 업데이트 여기서 해줌. 속도가 없는 상태라면 해줄필요 X
            switch (currentState) { // 현재 State에 따른 Speed값 업데이트
                case STATETYPE.IDLE:                                                                         break;
                case STATETYPE.MOVE:   rigidBody.velocity = Vector2.down * (moveSpeed * currentActionSpeed); break; // Move State Update.
                case STATETYPE.ATTACK:                                                                       break;
                case STATETYPE.DEATH:                                                                        break;
                default: throw new System.NotImplementedException();
            }
            yield return new WaitForSeconds(duration);
            this.currentActionSpeed = defaultActionSpeed;
            this.anim.speed         = defaultActionSpeed;
            switch (currentState) { // 중지 처리 후 Speed값 업데이트
                case STATETYPE.IDLE:                                                                         break;
                case STATETYPE.MOVE:   rigidBody.velocity = Vector2.down * (moveSpeed * currentActionSpeed); break; // Move State Update.
                case STATETYPE.ATTACK:                                                                       break;
                case STATETYPE.DEATH:                                                                        break;
                default: throw new System.NotImplementedException();
            }

            //받던중에 죽으면 이 코루틴을 해제시켜줘야 함, 죽을때도 느리게 죽는 현상이 일어난다 -> 진행중
        }

        public override void BreakState() {
            if (this.actionSpeedCo != null) {
                StopCoroutine(this.actionSpeedCo);
            }

            this.currentActionSpeed = defaultActionSpeed;
            this.anim.speed         = defaultActionSpeed;

            switch (currentState) {
                case STATETYPE.IDLE:                                                                         break;
                case STATETYPE.MOVE:   rigidBody.velocity = Vector2.down * (moveSpeed * currentActionSpeed); break;
                case STATETYPE.ATTACK:                                                                       break;
                case STATETYPE.DEATH:                                                                        break;
                default: throw new System.NotImplementedException();
            }
        }

        void ComponentInit() {
            if(anim == null) {
                anim = GetComponent<Animator>();
            }

            if(rigidBody == null) {
                rigidBody = GetComponent<Rigidbody2D>();
            }

            if(coll == null) {
                coll = GetComponent<CapsuleCollider2D>();
            }
        }

        private void Start() {
            ComponentInit();
            moveSpeed      = InitMoveSpeed;
            attackInterval = AttackInterval;
        }

        private void OnEnable() {
            this.currentActionSpeed = this.defaultActionSpeed;
            this.anim.speed         = this.defaultActionSpeed;
            ChangeState(STATETYPE.IDLE);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = false;
            }
        }

        #region OVERRIDE_STATE

        protected override void State_Idle(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  IdleStart();  break;
                case STATEFLOW.UPDATE: IdleUpdate(); break;
                case STATEFLOW.EXIT:                 break;
            }
        }

        protected override void State_Move(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  WalkStart();  break;
                case STATEFLOW.UPDATE: WalkUpdate(); break;
                case STATEFLOW.EXIT:   WalkExit();   break;
            }
        }

        protected override void State_Death(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER: DeathStart(); break;
                case STATEFLOW.UPDATE:              break;
                case STATEFLOW.EXIT:  DeathExit();  break;
            }
        }

        protected override void State_Attack(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  AttackStart();  break;
                case STATEFLOW.UPDATE: AttackUpdate(); break;
                case STATEFLOW.EXIT:   AttackExit();   break;
            }
        }

        #endregion

        #region ACTION

        void WalkStart() {
            anim.SetInteger(animState, 1);
            rigidBody.velocity = Vector2.down * (moveSpeed * currentActionSpeed);
        }

        void WalkUpdate() {
            if (isFindWall == true) {
                ChangeState(STATETYPE.ATTACK);
            }
        }

        void WalkExit() {
            rigidBody.velocity = Vector2.zero;
        }

        void IdleStart() {
            anim.SetInteger(animState, 0);
        }

        void IdleUpdate() {
            if (GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                ChangeState(STATETYPE.MOVE);
            }
        }

        void AttackStart() {
            //Set Animation on Idle
            anim.SetInteger(animState, 0);
        }

        void AttackUpdate() {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval) {
                anim.SetTrigger(atkState);
                attackTimer = 0f;
            }
        }

        void AttackExit() {
            attackTimer = 0f;
        }

        void DeathStart() {
            anim.SetInteger(animState, 3);
            coll.enabled = false;
            BreakState();
        }

        void DeathExit() {
            coll.enabled = true;
        }

        #endregion

        #region ANIMATION_EVENT
        public override void OnHit() {
            anim.SetTrigger(hitParams);
        }
        #endregion
    }
}
