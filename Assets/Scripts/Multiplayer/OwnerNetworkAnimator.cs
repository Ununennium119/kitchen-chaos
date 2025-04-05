using Unity.Netcode.Components;

namespace Multiplayer {
    public class OwnerNetworkAnimator : NetworkAnimator {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}
