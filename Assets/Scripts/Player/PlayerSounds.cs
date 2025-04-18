﻿using Audio;
using UnityEngine;

namespace Player {
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
        }

        private void Start() {
            _soundEffectManager = SoundEffectManager.Instance;
            _playerController = PlayerController.Instance;
        }

        private void Update() {
            _footstepSoundTime -= Time.deltaTime;
            if (_footstepSoundTime > 0f) return;

            _footstepSoundTime = footstepSoundCooldown;
            if (_playerController.IsWalking()) {
                _soundEffectManager.PlayFootstepSound(_playerController.transform.position, footstepSoundVolume);
            }
        }
    }
}
