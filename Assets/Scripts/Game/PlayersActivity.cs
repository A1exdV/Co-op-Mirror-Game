using System.Collections.Generic;
using Mirror;
using Network;
using Player;
using UnityEngine;

namespace Game
{
    public class PlayersActivity : NetworkBehaviour
    {
        private void Awake()
        {
            GameState.PlayingState.AddListener(OnPlayingState);
            GameState.GameOverState.AddListener(OnPlayerDisable);
            GameState.RestartState.AddListener(OnPlayerDisable);
        }

        #region Server
        
        [Server]
        private void OnPlayingState()
        {
            if (isServer)
            {
                foreach (var player in PlayersList.Players)
                {
                    RpcChangeActivity(player,true);
                }
            }
        }
        
        [Server]
        private void OnPlayerDisable()
        {
            if (isServer)
            {
                foreach (var player in PlayersList.Players)
                {
                    RpcChangeActivity(player,false);
                    player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }
        #endregion

        #region Rpc

        [ClientRpc]
        private void RpcChangeActivity(GameObject player, bool active)
        {
            player.GetComponent<Dash>().enabled = active;
            player.GetComponent<Rotation>().enabled = active;
            player.GetComponent<Movements>().enabled = active;
        }
        #endregion
        
        
    }
}
