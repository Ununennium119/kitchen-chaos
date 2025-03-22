using System;
using Counter;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio {
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }


        [SerializeField] private AudioClipsSO audioClipsSO;
        [SerializeField] private float audioVolume = 1f;


        private DeliveryManager _deliveryManager;
        private Player.Player _player;


        private static void PlaySound(AudioClip[] clip, Vector3 position, float volume = 1f) {
            var selectedClip = clip[Random.Range(0, clip.Length)];
            PlaySound(selectedClip, position, volume);
        }

        private static void PlaySound(AudioClip clip, Vector3 position, float volume = 1f) {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }


        public void PlayFootstepSound(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.footstepAudioClips, position, volume);
        }


        private void Awake() {
            if (Instance != null) {
                Debug.Log("There is already another AudioManager in the scene!");
            }
            Instance = this;
        }

        private void Start() {
            _deliveryManager = DeliveryManager.Instance;
            _player = Player.Player.Instance;

            CuttingCounter.OnAnyCut += PlayChopAudioClip;
            _deliveryManager.OnDeliverySuccess += PlayDeliverySuccessAudioClip;
            _deliveryManager.OnDeliveryFail += PlayDeliveryFailAudioClip;
            _player.OnObjectPickup += PlayObjectPickupAudioClip;
            _player.OnObjectDrop += PlayObjectDropAudioClip;
            TrashCounter.OnTrash += PlayTrashAudioClip;
        }


        private void PlayChopAudioClip(object sender, EventArgs e) {
            var cuttingCounter = sender as CuttingCounter;
            if (cuttingCounter == null) return;

            PlaySound(audioClipsSO.chopAudioClips, cuttingCounter.transform.position, audioVolume);
        }

        private void PlayDeliverySuccessAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliverySuccessAudioClips, _deliveryManager.transform.position, audioVolume);
        }

        private void PlayDeliveryFailAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliveryFailAudioClips, _deliveryManager.transform.position, audioVolume);
        }

        private void PlayObjectPickupAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.objectPickupAudioClips, _player.transform.position, audioVolume);
        }

        private void PlayObjectDropAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.objectDropAudioClips, _player.transform.position, audioVolume);
        }

        private void PlayTrashAudioClip(object sender, EventArgs e) {
            var trashCounter = sender as TrashCounter;
            if (trashCounter == null) return;

            PlaySound(audioClipsSO.trashAudioClips, trashCounter.transform.position, audioVolume);
        }
    }
}
