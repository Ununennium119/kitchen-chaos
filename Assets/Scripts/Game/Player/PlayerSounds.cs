using System;
using Game.Audio;
using UnityEngine;

namespace Game.Player {
    /// <summary>
    /// Manages footstep sounds for the player.
    /// </summary>
    public class PlayerSounds : MonoBehaviour {
        [SerializeField, Tooltip("The duration between player walking sound effect")]
        private float footstepSoundCooldown = 0.5f;
        [SerializeField, Tooltip("Volume of the footstep sound")]
        private float footstepSoundVolume = 1f;


        private SoundEffectManager _soundEffectManager;
        private PlayerController _playerController;
        private float _footstepSoundTime;


        private void Awake() {
            _footstepSoundTime = footstepSoundCooldown;

            PlayerController.OnLocalPlayerNetworkSpawned += OnLocalPlayerNetworkSpawnedAction;
        }

        private void Start() {
            _soundEffectManager = SoundEffectManager.Instance;
        }

        /// <summary>
        /// Plays footstep sound if the player is walking and cooldown has expired.
        /// </summary>
        private void Update() {
            _footstepSoundTime -= Time.deltaTime;
            if (_footstepSoundTime > 0f) return;

            _footstepSoundTime = footstepSoundCooldown;
            if (_playerController?.IsWalking() == true) {
                _soundEffectManager.PlayFootstepSound(_playerController.transform.position, footstepSoundVolume);
            }
        }


        /// <summary>
        /// Sets the reference to the local <see cref="PlayerController"/>.
        /// </summary>
        /// <remarks>
        /// Invoked when the <see cref="PlayerController.OnLocalPlayerNetworkSpawned"/> event is triggered.
        /// </remarks>
        private void OnLocalPlayerNetworkSpawnedAction(object sender, EventArgs e) {
            _playerController = PlayerController.LocalInstance;
        }
    }
}
