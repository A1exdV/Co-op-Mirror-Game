using System;
using System.Collections.Generic;
using Mirror;
using Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class PlayersConfiguration : NetworkBehaviour
    {

        [SerializeField] private List<Material> playersColors;
        [SerializeField] private List<String> defaultNames;

        [SerializeField] private List<Transform> spawnPoints;

        private void Awake()
        {
            PlayersList.AllPlayersSaved.AddListener(OnPlayersCreated);
            GameState.RestartState.AddListener(RandomSpawn);
        }

        #region Server
        
        [Server]
        private void OnPlayersCreated()
        {
            if (isServer)
            {
                RpcChangePlayersColor(PlayersList.Players);
            }
                
        }

        [Server]
        private void RandomSpawn()
        {
            if (spawnPoints.Count < PlayersList.Players.Count)
            {
                throw new Exception("There are more players than spawn points!");
            }

            List<Transform> tempPoints = new List<Transform>(spawnPoints);
            foreach (var player in PlayersList.Players)
            {
                var index = Random.Range(0, tempPoints.Count - 1);
                
                player.transform.position = tempPoints[index].position;
                player.transform.rotation = tempPoints[index].rotation;

                tempPoints.Remove(tempPoints[index]);
            }
        }
        #endregion


        #region Rpc
        
        [ClientRpc]
        private void RpcChangePlayersColor(List<GameObject> players)
        {
            Cursor.lockState = CursorLockMode.Locked;
            var index = 0;
            foreach (var player in players)
            {
                var playerMesh = player.GetComponentInChildren<MeshRenderer>();
                playerMesh.material = playersColors[index];
                player.name = defaultNames[index];
                index++;
            }
        
            UpdateGameState();
        }
        #endregion


        #region Command
        
        [Command(requiresAuthority = false)]
        private void UpdateGameState()
        {
            GameState.ChangeState.Invoke(State.Ready);
            RandomSpawn();
        }
        #endregion
    }
}
