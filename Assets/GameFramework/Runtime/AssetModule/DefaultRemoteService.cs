
namespace GameFramework.Resource
{
    /// <summary>
    /// 默认实现的资源更新地址
    /// </summary>
    public sealed class DefaultRemoteService : YooAsset.IRemoteServices
    {
        /// <summary>
        /// 资源更新远程地址
        /// </summary>
        private readonly string m_MainURL;

        /// <summary>
        /// 资源更新备用地址
        /// </summary>
        private readonly string m_FallbackURL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainURL"></param>
        /// <param name="fallbackURL"></param>
        public DefaultRemoteService(string mainURL, string fallbackURL)
        {
            m_MainURL = mainURL;
            m_FallbackURL = fallbackURL;
        }

        public string GetRemoteMainURL(string fileName)
        {
            return Utility.Text.Format("{0}/{1}", m_MainURL, fileName);
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return Utility.Text.Format("{0}/{1}", m_FallbackURL, fileName);
        }

    }
}