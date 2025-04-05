using System;
using Counter.Logic;
using Manager;
using Player;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio {
    /// <summary>
    /// Manages playing the sound effects and modifying their volume.
    /// </summary>
    /// <remarks>This class is singleton.</remarks>
    public class SoundEffectManager : MonoBehaviour {
        public static SoundEffectManager Instance { get; private set; }


        [SerializeField, Tooltip("Audio clips are stored in this scriptable object.")]
        private AudioClipsSO audioClipsSO;


        private DeliveryManager _deliveryManager;
        /// <summary>
        /// Adjusts the volume of sound effects, configurable by the player in the options menu.
        /// </summary>
        private float _volumeMultiplier;


        /// <returns>Sound effects volume</returns>
        public float GetVolume() {
            return _volumeMultiplier;
        }

        /// <summary>
        /// Increases volume of sound effects by 0.1. If volume is 1, sets it to 0.
        /// </summary>
        public void ChangeVolume() {
            _volumeMultiplier += 0.1f;
            if (_volumeMultiplier >= 1.1f) {
                _volumeMultiplier = 0f;
            }

            PlayerPrefsManager.SetSoundEffectsVolume(_volumeMultiplier);
        }

        public void PlayFootstepSound(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.footstepAudioClips, position, volume);
        }

        public void PlayWarningAudioClip(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.warningAudioClips, position, volume);
        }


        private void Awake() {
            Debug.Log("Setting up SoundEffectManager...");
            if (Instance != null) {
                Debug.Log("There is more than one SoundEffectManager in the scene!");
            }
            Instance = this;

            _volumeMultiplier = PlayerPrefsManager.GetSoundEffectsVolume(defaultValue: 1f);
        }

        private void Start() {
            _deliveryManager = DeliveryManager.Instance;

            _deliveryManager.OnDeliverySuccess += PlayDeliverySuccessAudioClip;
            _deliveryManager.OnDeliveryFail += PlayDeliveryFailAudioClip;
            PlayerController.OnAnyObjectPickup += PlayObjectPickupAudioClip;
            PlayerController.OnAnyObjectDrop += PlayObjectDropAudioClip;
            CuttingCounter.OnAnyCut += PlayChopAudioClip;
            TrashCounter.OnTrash += PlayTrashAudioClip;
        }


        private void PlayDeliverySuccessAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliverySuccessAudioClips, _deliveryManager.transform.position);
        }

        private void PlayDeliveryFailAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliveryFailAudioClips, _deliveryManager.transform.position);
        }

        private void PlayObjectPickupAudioClip(object sender, PlayerController.OnAnyObjectPickupArgs e) {
            PlaySound(audioClipsSO.objectPickupAudioClips, e.Position);
        }

        private void PlayObjectDropAudioClip(object sender, PlayerController.OnAnyObjectDropArgs e) {
            PlaySound(audioClipsSO.objectDropAudioClips, e.Position);
        }

        private void PlayChopAudioClip(object sender, EventArgs e) {
            var cuttingCounter = sender as CuttingCounter;
            if (cuttingCounter == null) return;

            PlaySound(audioClipsSO.chopAudioClips, cuttingCounter.transform.position);
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
