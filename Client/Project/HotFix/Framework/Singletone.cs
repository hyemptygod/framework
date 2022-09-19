

namespace HotFix
{
    public class HotFixSingletone<T> where T : HotFixSingletone<T>, new()
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new T();
                return m_Instance;
            }
        }
    }
}
