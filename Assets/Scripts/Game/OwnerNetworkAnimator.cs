using Unity.Netcode.Components;

namespace Game {
    public class OwnerNetworkAnimator : NetworkAnimator {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}
