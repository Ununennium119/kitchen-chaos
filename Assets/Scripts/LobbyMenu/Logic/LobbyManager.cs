using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Logic;
using Common.Utility;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Logger = Common.Utility.Logger;

namespace LobbyMenu.Logic {
    public class LobbyManager : MonoBehaviour {
        private const float MAX_HEARTBEAT_TIMER = 15f;
        private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";


        public static LobbyManager Instance { get; private set; }


        [SerializeField, Tooltip("The network connection type")]
        private ConnectionType connectionType;
        
        [SerializeField, Tooltip("Specifies whether to use relay")]
        private bool useRelay = false;


        /// <summary>
        /// This event is triggered whenever a lobby creation starts.
        /// </summary>
        public event EventHandler OnCreateLobbyStarted;

        /// <summary>
        /// This event is triggered whenever a lobby creation fails.
        /// </summary>
        public event EventHandler OnCreateLobbyFailed;

        /// <summary>
        /// This event is triggered whenever a lobby join starts.
        /// </summary>
        public event EventHandler OnJoinLobbyStarted;

        /// <summary>
        /// This event is triggered whenever a lobby join fails.
        /// </summary>
        public event EventHandler OnJoinLobbyFailed;

        /// <summary>
        /// This event is triggered whenever a quick join attempt fails due to no available lobbies.
        /// </summary>
        public event EventHandler OnQuickJoinNotFound;

        /// <summary>
        /// This event is triggered whenever the lobby list is refreshed.
        /// </summary>
        public event EventHandler<OnLobbyListRefreshedEventArgs> OnLobbyListRefreshed;
        public class OnLobbyListRefreshedEventArgs : EventArgs {
            public List<Lobby> LobbyList;
        }


        private Lobby _joinedLobby;
        private float _heartbeatTimer = MAX_HEARTBEAT_TIMER;


        /// <summary>
        /// Initializes the Unity Authentication service and signs the user in anonymously.
        /// </summary>
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

        /// <summary>
        /// Allocates a relay server for the game session.
        /// </summary>
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

        /// <summary>
        /// Joins an existing relay session using a provided join code.
        /// </summary>
        private static async Task<JoinAllocation> JoinAllocation(string joinCode) {
            try {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                return joinAllocation;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return null;
        }

        /// <summary>
        /// Retrieves the relay join code for an allocation.
        /// </summary>
        private static async Task<string> GetRelayJoinCode(Allocation allocation) {
            try {
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return joinCode;
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return null;
        }

        /// <summary>
        /// Gets the currently joined lobby.
        /// </summary>
        /// <returns>The <see cref="Lobby"/> object representing the joined lobby.</returns>
        public Lobby GetJoinedLobby() {
            return _joinedLobby;
        }

        /// <summary>
        /// Creates a new lobby with the specified name and privacy settings.
        /// </summary>
        /// <param name="lobbyName">The name of the new lobby.</param>
        /// <param name="isPrivate">Indicates whether the lobby is private.</param>
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

                if (useRelay) {
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
                }

                MultiplayerManager.Instance.StartHost();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (Exception e) {
                Debug.LogError(e);
                OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Joins a lobby using a provided lobby code.
        /// </summary>
        /// <param name="lobbyCode">The code of the lobby to join.</param>
        public async void JoinLobbyByCode(string lobbyCode) {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                if (useRelay) {
                    await JoinRelay();
                }

                MultiplayerManager.Instance.StartClient();
                SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
            } catch (Exception e) {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Joins a lobby using a provided lobby ID.
        /// </summary>
        /// <param name="lobbyId">The ID of the lobby to join.</param>
        public async void JoinLobbyById(string lobbyId) {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                if (useRelay) {
                    await JoinRelay();
                }

                MultiplayerManager.Instance.StartClient();
            } catch (Exception e) {
                Debug.LogError(e);
                OnJoinLobbyFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Attempts to quickly join an available lobby.
        /// </summary>
        public async void QuickJoinLobby() {
            try {
                OnJoinLobbyStarted?.Invoke(this, EventArgs.Empty);

                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                if (useRelay) {
                    await JoinRelay();
                }

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

        /// <summary>
        /// Deletes the current lobby if one is joined.
        /// </summary>
        public async void DeleteLobby() {
            try {
                if (_joinedLobby == null) return;

                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Leaves the current lobby if one is joined.
        /// </summary>
        public async void LeaveLobby() {
            try {
                if (_joinedLobby == null) return;

                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Kicks a player from the current lobby if the player is the host.
        /// </summary>
        /// <param name="playerId">The ID of the player to kick.</param>
        public async void KickPlayer(string playerId) {
            try {
                if (!IsLobbyHost()) return;

                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                _joinedLobby = null;
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Refreshes the list of available lobbies.
        /// </summary>
        public async void RefreshLobbyList() {
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


        private void Awake() {
            Logger.LogInitializingInstance(this);
            if (Instance != null) {
                Logger.LogMultipleInstancesError(this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Logger.LogInstanceInitialized(this);

            DontDestroyOnLoad(gameObject);

            InitializeLobby();
        }

        private void Update() {
            if (IsLobbyHost()) {
                HandleHeartbeat();
            }
        }


        /// <summary>
        /// Initializes the lobby system by initializing authentication and refreshing the lobby list.
        /// </summary>
        private async void InitializeLobby() {
            try {
                await InitializeUnityAuthentication();
                RefreshLobbyList();
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }


        /// <summary>
        /// Handles the sending of periodic heartbeat pings to keep the lobby active.
        /// </summary>
        private void HandleHeartbeat() {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0) {
                _heartbeatTimer = MAX_HEARTBEAT_TIMER;
                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }


        /// <summary>
        /// Joins the relay service for the current lobby.
        /// </summary>
        private async Task JoinRelay() {
            try {
                var joinCode = _joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
                var joinAllocation = await JoinAllocation(joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinAllocation.ToRelayServerData(connectionType.GetValue())
                );
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }


        /// <summary>
        /// Checks if the current player is the host of the joined lobby.
        /// </summary>
        /// <returns>True if the player is the host, otherwise false.</returns>
        private bool IsLobbyHost() {
            if (_joinedLobby == null) return false;
            return _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
    }
}
