
namespace Joy.Base.Config
{
    /// <summary>
    /// Dll清单描述文件
    /// </summary>
    [System.Serializable]
    public class DllManifest
    {
        /// <summary>
        /// AOT Dll集合
        /// </summary>
        public string[] aotDlls;

        /// <summary>
        /// Hotfix Dll集合
        /// </summary>
        public string[] hotfixDlls;
    }
}