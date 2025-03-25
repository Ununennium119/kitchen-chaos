using System;
using Counter;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio {
    public class AudioManager : MonoBehaviour {
        private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";


        public static AudioManager Instance { get; private set; }


        [SerializeField] private AudioClipsSO audioClipsSO;


        private DeliveryManager _deliveryManager;
        private Player.Player _player;
        private float _volumeMultiplier;


        public float GetVolume() {
            return _volumeMultiplier;
        }

        public void ChangeVolume() {
            _volumeMultiplier += 0.1f;
            if (_volumeMultiplier >= 1.1f) {
                _volumeMultiplier = 0f;
            }

            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volumeMultiplier);
            PlayerPrefs.Save();
        }

        public void PlayFootstepSound(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.footstepAudioClips, position, volume);
        }

        public void PlayWarningAudioClip(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.warningAudioClips, position, volume);
        }


        private void Awake() {
            if (Instance != null) {
                Debug.Log("There is already another AudioManager in the scene!");
            }
            Instance = this;

            _volumeMultiplier = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
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

            PlaySound(audioClipsSO.chopAudioClips, cuttingCounter.transform.position);
        }

        private void PlayDeliverySuccessAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliverySuccessAudioClips, _deliveryManager.transform.position);
        }

        private void PlayDeliveryFailAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliveryFailAudioClips, _deliveryManager.transform.position);
        }

        private void PlayObjectPickupAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.objectPickupAudioClips, _player.transform.position);
        }

        private void PlayObjectDropAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.objectDropAudioClips, _player.transform.position);
        }

        private void PlayTrashAudioClip(object sender, EventArgs e) {
            var trashCounter = sender as TrashCounter;
            if (trashCounter == null) return;

            PlaySound(audioClipsSO.trashAudioClips, trashCounter.transform.position);
        }

        private void PlaySound(AudioClip[] clip, Vector3 position, float volume = 1f) {
            var selectedClip = clip[Random.Range(0, clip.Length)];
            PlaySound(selectedClip, position, volume);
        }

        private void PlaySound(AudioClip clip, Vector3 position, float volume = 1f) {
            AudioSource.PlayClipAtPoint(clip, position, volume * _volumeMultiplier);
        }
    }
}
