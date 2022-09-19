using System;
using System.Reflection;
using HotFix.Base.Data;

namespace HotFix.Base
{
    public enum DispatchProgressEvent
    {
        Init,
        Update,
        Complete
    }

    public class ProgressInfo
    {
        public string title;
        /// <summary>
        /// 预估时间
        /// </summary>
        public float frames = 10f;
        public float step = 1f;
        public float current = 0f;

        public float Progress
        {
            get
            {
                return current / frames;
            }
        }

        public void Update()
        {
            current += step;
            current = Math.Min(current, frames - step);
        }

        public void Complete()
        {
            current = frames;
        }
    }

    public static class HotFixUtil
    {
        public static DataContainer<T> GetDontainer<T>() where T : BaseData
        {
            return new DataContainer<T>();
        }

        public static DataContainer<T> AddData<T>(T data, DataContainer<T> container) where T : BaseData
        {
            container.Add(data);
            return container;
        }

        public static void ToExcel<T>(DataContainer<T> container, string name, Type t) where T : BaseData
        {
            container.datas.ToExcel(name, t.GetFields(BindingFlags.Instance | BindingFlags.Public));
        }
    }
}
