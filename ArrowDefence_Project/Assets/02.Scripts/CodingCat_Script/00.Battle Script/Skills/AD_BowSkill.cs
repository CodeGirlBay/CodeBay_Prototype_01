﻿namespace ActionCat {
    using UnityEngine;

    public abstract class AD_BowSkill {
        protected string id;
        protected string name;
        protected string desc;
        protected Sprite iconSprite;
        protected SKILL_LEVEL level;
        protected BOWSKILL_TYPE skillType;

        #region PROPERTY
        public string Id { get => id; }
        public string Name { get => name; }
        public string Description { get => desc; }
        public Sprite IconSprite
        {
            get
            {
                if (this.iconSprite != null)
                    return iconSprite;
                else
                    return null;
            }
        }
        public SKILL_LEVEL Level { get => level; }
        public BOWSKILL_TYPE Type { get => skillType; }
        #endregion

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public AD_BowSkill() { }
        ~AD_BowSkill() { }

        protected AD_BowSkill(string skillid, string skillname, string skilldesc, SKILL_LEVEL level, BOWSKILL_TYPE type, Sprite sprite) {
            this.id         = skillid;
            this.name       = skillname;
            this.desc       = skilldesc;
            this.level      = level;
            this.skillType  = type;
            this.iconSprite = sprite;
        }

        public abstract void Init();

        public abstract void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type);
        //BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, LOAD_ARROW_TYPE arrowType);
    }
}