using Unity.Netcode.Components;

namespace Common.Logic {
    /// <summary>
    /// A custom implementation of <see cref="NetworkAnimator"/> that overrides the server authoritative check.
    /// </summary>
    public class OwnerNetworkAnimator : NetworkAnimator {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}
