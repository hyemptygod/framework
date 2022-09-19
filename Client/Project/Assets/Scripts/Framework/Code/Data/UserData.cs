using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Framework;

namespace Framework.Data
{

    /// <summary>
    /// 用户数据（存至PlayerPrefs）
    /// </summary>
    public class UserData : Singletone<UserData>
    {
        private const string DATA_VERSION = "Data Version";
        private const string LANGUAGE = "Language";

        private string _dataVersion = "0";
        /// <summary>
        /// 数据版本号
        /// </summary>
        public static string DataVersion
        {
            get
            {
                return Instance._dataVersion;
            }
            set
            {
                Instance._dataVersion = value;
                PlayerPrefs.SetString(DATA_VERSION, Instance._dataVersion);
            }
        }

        private SystemLanguage _language = SystemLanguage.Chinese;
        /// <summary>
        /// 语言
        /// </summary>
        public static SystemLanguage Language
        {
            get
            {
                return Instance._language;
            }
            set
            {
                Instance._language = value;
                PlayerPrefs.SetString(LANGUAGE, Instance._language.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserData()
        {
            _dataVersion = PlayerPrefs.GetString(DATA_VERSION, _dataVersion);

            if (PlayerPrefs.HasKey(LANGUAGE))
                _language = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString(LANGUAGE));
        }
    }
}
