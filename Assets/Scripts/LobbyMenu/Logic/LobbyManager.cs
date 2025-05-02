using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace LobbyMenu.Logic {
    public class LobbyManager : MonoBehaviour {
        private const float MAX_HEARTBEAT_TIMER = 15f;
        private const float LOBBY_REFRESH_TIMER = 5f;
        private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";
        private const string CONNECTION_TYPE = RelayServerEndpoint.ConnectionTypeWss;


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


        private static async Task InitializeUnityAuthentication() {
            try {
                if (UnityServices.State != ServicesInitializationState.Initialized) {
                    var options = new InitializationOptions();
                    await UnityServices.InitializeAsync(options);
                }
                if (!AuthenticationService.Instance.IsSignedIn) {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private static async Task<Allocation> AllocateRelay() {
            try {
                var allocation = await RelayService.Instance.CreateAllocationAsync(
                    MultiplayerManager.MAX_PLAYER_COUNT - 1
                );
                return allocation;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return null;
        }

        private static async Task<JoinAllocation> JoinAllocation(string joinCode) {
            try {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return joinAllocation;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return null;
        }

        private static async Task<string> GetRelayJoinCode(Allocation allocation) {
            try {
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return joinCode;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return null;
        }


        public Lobby GetJoinedLobby() {
            return _joinedLobby;
        }

        public async void CreateLobby(string lobbyName, bool isPrivate) {
            try {
                OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                    lobbyName: lobbyName,
                    maxPlayers: MultiplayerManager.MAX_PLAYER_COUNT,
                    options: new CreateLobbyOptions {
                        IsPrivate = isPrivate
                    }
                );

                var relayAllocation = await AllocateRelay();
                var relayJoinCode = await GetRelayJoinCode(relayAllocation);
                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                        {
                            RELAY_JOIN_CODE_KEY, new DataObject(
                                DataObject.VisibilityOptions.Member,
                                relayJoinCode
                            )
                        }
                    }
                });
                Debug.Log(
                    $"{relayJoinCode}, {relayAllocation.Region}, {relayAllocation.ServerEndpoints}, {relayAllocation.AllocationId}, {relayAllocation.RelayServer}");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    relayAllocation.ToRelayServerData(CONNECTION_TYPE)
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

                await JoinRelay();

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

                await JoinRelay();

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

                await JoinRelay();

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

            InitializeLobby();
        }

        private void Update() {
            HandleLobbyRefresh();
            if (IsLobbyHost()) {
                HandleHeartbeat();
            }
        }


        private async void InitializeLobby() {
            try {
                await InitializeUnityAuthentication();
                ListLobbies();
            } catch (Exception e) {
                Debug.LogError(e);
            }
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
            if (!SceneLoader.IsSceneActive(SceneLoader.Scene.LobbyScene)) return;
            if (!AuthenticationService.Instance.IsSignedIn) return;

            _lobbyRefreshTimer -= Time.deltaTime;
            if (_lobbyRefreshTimer <= 0) {
                _lobbyRefreshTimer = LOBBY_REFRESH_TIMER;
                ListLobbies();
            }
        }


        private async Task JoinRelay() {
            try {
                var joinCode = _joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
                var joinAllocation = await JoinAllocation(joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinAllocation.ToRelayServerData(CONNECTION_TYPE)
                );
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }


        private bool IsLobbyHost() {
            if (_joinedLobby == null) return false;
            return _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
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
