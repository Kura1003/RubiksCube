using System.Threading;

namespace Taki.Main.System.RubiksCube
{
    public interface ICubeCancellationToken
    {
        CancellationToken GetToken();
        void CancelAndDispose();
    }
}