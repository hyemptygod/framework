using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Data;

namespace Framework.Start
{
    /// <summary>
    /// 资源检测工具类
    /// </summary>
    public class AssetVersionHelper
    {
        /// <summary>
        /// 资源信息
        /// </summary>
        [Serializable]
        public class Res
        {
            /// <summary>
            /// 资源路径
            /// </summary>
            public string path;
            /// <summary>
            /// 资源名
            /// </summary>
            public string name;
            /// <summary>
            /// 内容的MD5值
            /// </summary>
            public string md5;
            /// <summary>
            /// 大小
            /// </summary>
            public uint size;

            private string _decryptPath;
            /// <summary>
            /// 解密后的目录名
            /// </summary>
            public string DecryptPath
            {
                get
                {
                    if (string.IsNullOrEmpty(_decryptPath))
                        _decryptPath = EncryptionUtil.DESDecrypt(path);
                    return _decryptPath;
                }
            }

            private string _decryptName;
            /// <summary>
            /// 解密后的文件名
            /// </summary>
            public string DecryptName
            {
                get
                {
                    if (string.IsNullOrEmpty(_decryptName))
                        _decryptName = EncryptionUtil.DESDecrypt(name);
                    return _decryptName;
                }
            }

            /// <summary>
            /// 检查是否为同一文件
            /// </summary>
            /// <param name="res"></param>
            /// <returns></returns>
            public bool CheckFile(Res res)
            {
                if (res == null)
                    return false;
                return path == res.path && name == res.name;
            }

            /// <summary>
            /// 检查内容是否相同
            /// </summary>
            /// <param name="res"></param>
            /// <returns></returns>
            public bool CheckContent(Res res)
            {
                if (res == null)
                    return false;
                return md5 == res.md5;
            }
        }

        /// <summary>
        /// 当前进度信息
        /// </summary>
        public class ProgressInfo
        {
            /// <summary>
            /// 已下载数据大小
            /// </summary>
            public uint size;
            /// <summary>
            /// 当前进度
            /// </summary>
            public float progress;
        }

        private const string VERSION_FILE_NAME = "version";
        private const string UPDATE_CONFIG = "update_config.xml";

        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool NeedUpdate { get; private set; }
        /// <summary>
        /// 需要更新的资源大小
        /// </summary>
        public long UpdateSize { get; private set; }
        /// <summary>
        /// 下载成功
        /// </summary>
        public bool Success { get; private set; }

        private string _newVersion;
        private string _updateDataUrl;
        private List<Res> _updateDatas;

        private string _dataLocalPath;
        private string _dataServerPath;

        public ProgressInfo Current = new ProgressInfo();

        /// <summary>
        /// 
        /// </summary>
        public AssetVersionHelper()
        {
            _dataLocalPath = PathConst.ABStreamingPath;
            _dataServerPath = PathConst.ABStreamingPath;
        }

        /// <summary>
        /// 检测
        /// </summary>
        /// <returns></returns>
        public CoroutineHandle Check()
        {
            return CoroutineManager.RunCoroutine(CheckInternal(), "Start check data version");
        }

        private IEnumerator<float> CheckInternal()
        {
            // 检测版本信息
            yield return CoroutineManager.WaitUntilDone(CheckVersion());

            if (!NeedUpdate)
                yield break;

            // 设置更新路径
            yield return CoroutineManager.WaitUntilDone(GetUpdateDataUrl());

            // 删除多余的数据，并获得需要更新的数据
            yield return CoroutineManager.WaitUntilDone(CheckUpdateConfig());

            UpdateSize = _updateDatas.Sum(item => item.size);
        }

        /// <summary>
        /// 检测数据的版本信息
        /// </summary>
        private CoroutineHandle CheckVersion()
        {
            var neturl = _dataServerPath + VERSION_FILE_NAME;
            return FileUtil.ReadContentByRequest(neturl, net =>
            {
                _newVersion = net;
                NeedUpdate = UserData.DataVersion != net;
            });
        }

        /// <summary>
        /// 本地Streamming目录里的数据与服务器的数据是否一致
        /// 如果相同则直接拷贝本地的数据到Persistent目录
        /// 否则下载服务端的数据
        /// </summary>
        /// <returns></returns>
        private CoroutineHandle GetUpdateDataUrl()
        {
            var localurl = _dataLocalPath + VERSION_FILE_NAME;

            return FileUtil.ReadContentByRequest(localurl, local =>
            {
                _updateDataUrl = local != _newVersion ? _dataServerPath : _dataLocalPath;
            });
        }

        /// <summary>
        /// 删除原Persistent目录的多余的数据
        /// 将需要更新的数据加入到更新列表
        /// </summary>
        /// <returns></returns>
        private CoroutineHandle CheckUpdateConfig()
        {
            var current = XmlUtil.FromXmlFile<List<Res>>(PathConst.ABPersistentPath, UPDATE_CONFIG) ?? new List<Res>();

            return FileUtil.ReadContentByRequest(_updateDataUrl + UPDATE_CONFIG, netData =>
            {
                var target = XmlUtil.FromXmlString<List<Res>>(netData);
                if (current != null)
                {
                    // target中不存在current的数据，需要删除
                    var removers = current.Where(c => !target.Exists(t => t.CheckFile(c)));
                    if (removers != null)
                    {
                        foreach (var item in removers)
                        {
                            if (item == null || string.IsNullOrEmpty(item.path))
                                continue;
                            FileUtil.RemoveFile(PathConst.ABPersistentPath.PathCombine(item.DecryptPath, item.DecryptName));
                        }
                    }
                }

                // Need to update
                _updateDatas = target.Where(t =>
                {
                    var item = current.Find(c => c.CheckFile(t));

                    if (item == null) return true;

                    return !item.CheckContent(t);
                }).ToList();

                FileUtil.RemoveFile(PathConst.ABPersistentPath.PathCombine(UPDATE_CONFIG));
                FileUtil.CreateFile(PathConst.ABPersistentPath, UPDATE_CONFIG, netData);
            });
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <returns></returns>
        public CoroutineHandle UnZip()
        {
            Success = false;
            return CoroutineManager.RunCoroutine(UnZipInternal(), "data_version:unZip new file");
        }

        private IEnumerator<float> UnZipInternal()
        {
            var total = _updateDatas.Count;
            var num = 0;

            foreach (var item in _updateDatas)
            {
                if (item == null || string.IsNullOrEmpty(item.path))
                    continue;

                FileUtil.ReadBytesByRequest(_updateDataUrl.PathCombine(item.DecryptPath, item.DecryptName), bytes =>
                {
                    if (bytes == null)
                        return;

                    Current.progress = (float)num / total;
                    FileUtil.CreateFile(PathConst.ABPersistentPath.PathCombine(item.DecryptPath), item.DecryptName, bytes);
                    Current.size += item.size;
                    num++;
                });
            }

            while (num < total)
                yield return CoroutineManager.WaitForOneFrame;

            UserData.DataVersion = _newVersion;

            Success = true;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 生成 update_config.xml
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        public static void CreateUpdateConfig(string path, List<string> files)
        {
            var reslist = new List<Res>();

            foreach (var file in files)
            {
                var info = FileUtil.GetFileInfo(file);
                if (info == null)
                    continue;
                var name = info.Name;
                var dir = file.Replace(path + "/", "").Replace(name, "");
                var content = FileUtil.ReadBytesFromFile(file);
                if (content == null)
                    continue;
                reslist.Add(new Res()
                {
                    path = EncryptionUtil.DESEncrypt(dir),
                    name = EncryptionUtil.DESEncrypt(name),
                    md5 = EncryptionUtil.MD5Encrypt(content),
                    size = (uint)content.Length,
                });
            }

            reslist.ToXml(path, UPDATE_CONFIG);

            FileUtil.CreateFile(path, VERSION_FILE_NAME, Encoding.UTF8.GetBytes(Timer.NowTimeStampSecond.ToString()));
        }

#endif
    }
}
