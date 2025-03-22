using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu]
    public class AudioClipsSO : ScriptableObject {
        public AudioClip[] chopAudioClips;
        public AudioClip[] deliverySuccessAudioClips;
        public AudioClip[] deliveryFailAudioClips;
        public AudioClip[] footstepAudioClips;
        public AudioClip[] objectPickupAudioClips;
        public AudioClip[] objectDropAudioClips;
        public AudioClip[] trashAudioClips;
        public AudioClip[] warningAudioClips;
        public AudioClip panSizzleAudioClip;
    }
}
