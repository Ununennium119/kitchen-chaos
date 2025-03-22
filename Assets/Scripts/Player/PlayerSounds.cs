using Audio;
using UnityEngine;

namespace Player {
    public class PlayerSounds : MonoBehaviour {
        [SerializeField] private float footstepTimerMax = 0.5f;
        [SerializeField] private float footstepVolume = 1f;


        private AudioManager _audioManager;
        private Player _player;
        private float _footstepTimer;


        private void Awake() {
            _player = GetComponent<Player>();

            _footstepTimer = footstepTimerMax;
        }

        private void Start() {
            _audioManager = AudioManager.Instance;
        }

        private void Update() {
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer > 0f) return;

            _footstepTimer = footstepTimerMax;
            if (_player.IsWalking()) {
                _audioManager.PlayFootstepSound(_player.transform.position, footstepVolume);
            }
        }
    }
}
