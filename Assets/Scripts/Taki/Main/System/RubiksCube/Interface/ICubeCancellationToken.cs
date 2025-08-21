using System.Threading;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeCancellationToken
    {
        CancellationToken GetToken();
        void CancelAndDispose();
    }
}