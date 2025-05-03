using System;
using Common.Utility;
using Game.Counter.Logic;
using Game.Manager;
using Game.Player;
using Game.ScriptableObjects;
using UnityEngine;
using Logger = Common.Utility.Logger;
using Random = UnityEngine.Random;

namespace Game.Audio {
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

        /// <summary>
        /// Plays a footstep sound effect.
        /// </summary>
        /// <param name="position">The world position where the sound should be played.</param>
        /// <param name="volume">The volume of the sound, ranging from 0 to 1. Default is 1f.</param>
        public void PlayFootstepSound(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.footstepAudioClips, position, volume);
        }

        /// <summary>
        /// Plays a warning sound effect.
        /// </summary>
        /// <param name="position">The world position where the sound should be played.</param>
        /// <param name="volume">The volume of the sound, ranging from 0 to 1. Default is 1f.</param>
        public void PlayWarningAudioClip(Vector3 position, float volume = 1f) {
            PlaySound(audioClipsSO.warningAudioClips, position, volume);
        }


        private void Awake() {
            Logger.LogInitializingInstance(this);
            if (Instance != null) {
                Logger.LogMultipleInstancesError(this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Logger.LogInstanceInitialized(this);

            UpdateVolumeMultiplier();
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


        /// <remarks>
        /// Invoked when the <see cref="DeliveryManager.OnDeliverySuccess"/> event is triggered.
        /// </remarks>
        private void PlayDeliverySuccessAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliverySuccessAudioClips, _deliveryManager.transform.position);
        }

        /// <remarks>
        /// Invoked when the <see cref="DeliveryManager.OnDeliveryFail"/> event is triggered.
        /// </remarks>
        private void PlayDeliveryFailAudioClip(object sender, EventArgs e) {
            PlaySound(audioClipsSO.deliveryFailAudioClips, _deliveryManager.transform.position);
        }

        /// <remarks>
        /// Invoked when the <see cref="PlayerController.OnAnyObjectPickup"/> event is triggered.
        /// </remarks>
        private void PlayObjectPickupAudioClip(object sender, PlayerController.OnAnyObjectPickupArgs e) {
            PlaySound(audioClipsSO.objectPickupAudioClips, e.Position);
        }

        /// <remarks>
        /// Invoked when the <see cref="PlayerController.OnAnyObjectDrop"/> event is triggered.
        /// </remarks>
        private void PlayObjectDropAudioClip(object sender, PlayerController.OnAnyObjectDropArgs e) {
            PlaySound(audioClipsSO.objectDropAudioClips, e.Position);
        }

        /// <remarks>
        /// Invoked when the <see cref="CuttingCounter.OnAnyCut"/> event is triggered.
        /// </remarks>
        private void PlayChopAudioClip(object sender, EventArgs e) {
            var cuttingCounter = sender as CuttingCounter;
            if (cuttingCounter == null) return;

            PlaySound(audioClipsSO.chopAudioClips, cuttingCounter.transform.position);
        }

        /// <remarks>
        /// Invoked when the <see cref="TrashCounter.OnTrash"/> event is triggered.
        /// </remarks>
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


        private void UpdateVolumeMultiplier() {
            _volumeMultiplier = PlayerPrefsManager.GetSoundEffectsVolume(defaultValue: 1f);
        }
    }
}
