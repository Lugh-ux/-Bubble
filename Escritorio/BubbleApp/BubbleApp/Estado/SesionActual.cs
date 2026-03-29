using BubbleApp.Modelos;

namespace BubbleApp.Estado
{
    public static class SesionActual
    {
        public static SesionUsuario Current { get; private set; }

        public static bool IsAuthenticated
        {
            get { return Current != null && !string.IsNullOrWhiteSpace(Current.Token); }
        }

        public static void Set(SesionUsuario session)
        {
            Current = session;
        }

        public static void Clear()
        {
            Current = null;
        }
    }
}
