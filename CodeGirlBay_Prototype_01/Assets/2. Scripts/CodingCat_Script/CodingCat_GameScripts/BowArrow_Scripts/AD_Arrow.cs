﻿namespace CodingCat_Scripts
{
    using CodingCat_Games;
    using DG.Tweening;
    using UnityEngine;

    public class AD_Arrow : MonoBehaviour
    {
        //The Left, Right Clamp Point for the Arrow.
        public Transform leftClampPoint, rightClampPoint;
        public Transform arrowChatchPoint;

        [HideInInspector]
        public bool islaunched;
        [HideInInspector]
        public float power;

        //Controll Arrow Position (Before Launched)
        private Vector3 arrowPosition;

        //Launch Power for the Arrow
        private float powerFactor = 2000;

        //Arrow Attributes
        private AD_GameScripts.ArrowAttrubute arrowAttribute;
        public AD_GameScripts.ArrowAttrubute ArrowAttribute { get { return arrowAttribute; } }

        private void Start()
        {
            //Set Normal Arrow (TEST)
            arrowAttribute = AD_GameScripts.ArrowAttrubute.Arrow_Normal;

            if(arrowChatchPoint == null)
            {
                //Find Arrow Chatch Point Function
            }
        }

        private void Update()
        {
            if (!islaunched)
            {
                ClampPosition();
                CalculatePower();
            }
        }

        private void ClampPosition()
        {
            //Get the Current Position of the Arrow
            arrowPosition = transform.position;
            //Clamp the X Y position Between min and Max Points
            arrowPosition.x = Mathf.Clamp(arrowPosition.x, Mathf.Min(rightClampPoint.position.x, leftClampPoint.position.x),
                                                           Mathf.Max(rightClampPoint.position.x, leftClampPoint.position.x));
            arrowPosition.y = Mathf.Clamp(arrowPosition.y, Mathf.Min(rightClampPoint.position.y, leftClampPoint.position.y),
                                                           Mathf.Max(rightClampPoint.position.y, leftClampPoint.position.y));

            //Set new Position for the Arrow
            transform.position = arrowPosition;
        }

        private void CalculatePower()
        {
            this.power = Vector2.Distance(transform.position, rightClampPoint.position) * powerFactor;
        }

        public void DestroyArrow()
        {
            Destroy(this.gameObject);
        }
    }
}
