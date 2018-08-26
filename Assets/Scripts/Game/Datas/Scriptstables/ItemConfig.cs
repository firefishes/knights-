﻿using UnityEngine;

namespace Knights.Game
{
    /// <summary>
    /// 物品配置
    /// </summary>
    [CreateAssetMenu(menuName = "Knights/Game/Assets/ItemConfig")]
    public class ItemConfig : ScriptableItem, INamableItem
    {
        /// <summary>价值</summary>
        public int price = 0;
        /// <summary>名称本地化id</summary>
        public int nameID;

        protected string mName;

        public override IScriptableItem Copy()
        {
            ItemConfig config = GetNewConfig() as ItemConfig;
            config.nameID = nameID;
            config.price = price;
            return (config as IScriptableItem);
        }

        protected override IScriptableItem GetNewConfig()
        {
            return CreateInstance<ItemConfig>();
        }

        public virtual void SetName(ref string value)
        {
            mName = value;
        }

        public string Name
        {
            get
            {
                return mName;
            }
        }
    }

}