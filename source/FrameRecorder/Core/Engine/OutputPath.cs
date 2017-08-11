using System;
using UnityEngine;


namespace UnityEngine.FrameRecorder
{
    [Serializable]
    public struct OutputPath
    {
        public enum ERoot
        {
            Absolute,
            Current,
            PersistentData,
            StreamingAssets,
            TemporaryCache,
            DataPath,
        }

        [SerializeField] ERoot m_root;
        [SerializeField] string m_leaf;

        public ERoot root
        {
            get { return m_root; }
            set { m_root = value; }
        }
        public string leaf
        {
            get { return m_leaf; }
            set { m_leaf = value; }
        }

        public string GetFullPath()
        {
            if (m_root == ERoot.Absolute)
            {
                return m_leaf;
            }
            if (m_root == ERoot.Current)
            {
                return string.IsNullOrEmpty(m_leaf) ? "." : "./" + m_leaf;
            }

            string ret = "";
            switch (m_root)
            {
                case ERoot.PersistentData:
                    ret = Application.persistentDataPath;
                    break;
                case ERoot.StreamingAssets:
                    ret = Application.streamingAssetsPath;
                    break;
                case ERoot.TemporaryCache:
                    ret = Application.temporaryCachePath;
                    break;
                case ERoot.DataPath:
                    ret = Application.dataPath;
                    break;
            }

            if (!m_leaf.StartsWith("/"))
            {
                ret += "/";
            }
            ret += m_leaf;
            return ret;
        }

        public void CreateDirectory()
        {
            try
            {
                var path = GetFullPath();
                if(path.Length > 0 && !System.IO.Directory.Exists(path) )
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            catch(Exception)
            {
            }
        }
    }
}