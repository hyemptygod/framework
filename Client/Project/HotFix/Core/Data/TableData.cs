using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using HotFix.Base.Data;

namespace HotFix.Core
{
    [Serializable]
    public class Reward
    {
        public uint[][] item = { };
        public uint[][] money = { };
    }

    /// <summary>
    /// 测试数据
    /// </summary>
    [Serializable]
    [TableData("table_testdata")]
    public class TestData : BaseData
    {
        [Description("名", (uint)CommonFieldType.String)]
        public string name = "";

        [Description("描述", (uint)CommonFieldType.Array)]
        public string[] dsc = { };

        [Description("子", (uint)CommonFieldType.Array)]
        public int[] children = { };

        [Description("奖励", (uint)CommonFieldType.Object)]
        public Reward reward = new Reward();
    }

    /// <summary>
    /// 测试数据
    /// </summary>
    [Serializable]
    [TableData("table_weapon")]
    public class Weapon : BaseData
    {
        [Description("名", (uint)CommonFieldType.String)]
        public string name = "";
    }
}
