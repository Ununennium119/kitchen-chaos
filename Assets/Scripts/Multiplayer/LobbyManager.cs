using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer {
    public class LobbyManager : MonoBehaviour {
        private const float MAX_HEARTBEAT_TIMER = 15f;
        private const float LOBBY_REFRESH_TIMER = 5f;


        public static LobbyManager Instance { get; private set; }


        public event EventHandler OnCreateLobbyStarted;
        public event EventHandler OnCreateLobbyFailed;
        public event EventHandler OnJoinLobbyStarted;
        public event EventHandler OnJoinLobbyFailed;
        public event EventHandler OnQuickJoinNotFound;
        public event EventHandler<OnLobbyListRefreshedEventArgs> OnLobbyListRefreshed;
        public class OnLobbyListRefreshedEventArgs : EventArgs {
            public List<Lobby> LobbyList;
        }


        private Lobby _joinedLobby;
        private float _heartbeatTimer = MAX_HEARTBEAT_TIMER;
        private float _lobbyRefreshTimer = LOBBY_REFRESH_TIMER;


        public Lobby GetJoinedLobby() {
            return _joinedLobby;
        }

        public async void CreateLobby(string lobbyName, bool isPrivate) {
            try {
                OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName: lobbyName,
                    maxPlayers: MultiplayerManager.MAX_PLAYER_COUNT,
                    options: new CreateLobbyOptions() {
                        IsPrivate = isPrivate
                    }
                );

                MultiplayerManager.Instance.StartHost();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (Exception e) {
                Debug.LogError(e);
                OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void JoinLobbyByCode(string lobbyCode) {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                MultiplayerManager.Instance.StartClient();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (Exception e) {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void JoinLobbyById(string lobbyId) {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                MultiplayerManager.Instance.StartClient();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (Exception e) {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void QuickJoinLobby() {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                MultiplayerManager.Instance.StartClient();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (LobbyServiceException e) {
                if (e.Reason == LobbyExceptionReason.NoOpenLobbies) {
                    OnQuickJoinNotFound?.Invoke(this, EventArgs.Empty);
                } else {
                    Debug.LogError(e);
                    OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
                }
            } catch (Exception e) {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void DeleteLobby() {
            try {
                if (_joinedLobby == null) return;

                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public async void LeaveLobby() {
            try {
                if (_joinedLobby == null) return;

                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public async void KickPlayer(string playerId) {
            try {
                if (!IsLobbyHost()) return;

                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }


        private void Awake() {
            Debug.Log("Setting up LobbyManager...");
            if (Instance != null) {
                Debug.LogError("There is more than one instance of LobbyManager!");
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);

            InitializeUnityAuthentication();
        }

        private void Update() {
            HandleLobbyRefresh();
            if (IsLobbyHost()) {
                HandleHeartbeat();
            }
        }


        private async void InitializeUnityAuthentication() {
            try {
                if (UnityServices.State != ServicesInitializationState.Initialized) {
                    var options = new InitializationOptions();
                    options.SetProfile(Random.Range(0, 1000).ToString());

                    await UnityServices.InitializeAsync();
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private bool IsLobbyHost() {
            if (_joinedLobby == null) return false;
            return _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private void HandleHeartbeat() {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0) {
                _heartbeatTimer = MAX_HEARTBEAT_TIMER;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }

        private void HandleLobbyRefresh() {
            if (_joinedLobby != null) return;
            if (!AuthenticationService.Instance.IsSignedIn) return;

            _lobbyRefreshTimer -= Time.deltaTime;
            if (_lobbyRefreshTimer <= 0) {
                _lobbyRefreshTimer = LOBBY_REFRESH_TIMER;
                ListLobbies();
            }
        }

        private async void ListLobbies() {
            try {
                var options = new QueryLobbiesOptions {
                    Filters = new List<QueryFilter> {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };
                var response = await LobbyService.Instance.QueryLobbiesAsync(options);
                OnLobbyListRefreshed?.Invoke(this, new OnLobbyListRefreshedEventArgs {
                    LobbyList = response.Results
                });
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }
}
