using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Network
{
    public class PlayersList : NetworkBehaviour
    {
        public static readonly List<GameObject> Players = new List<GameObject>();

        public static readonly UnityEvent AllPlayersSaved = new UnityEvent();


        [Server]
        public override void OnStartServer()
        {
            Players.Add(this.gameObject); 
            if(Players.Count == NetworkServer.connections.Count)
                AllPlayersSaved?.Invoke();
        }

        [Server]
        public override void OnStopServer()
        {
            Players.Remove(this.gameObject);
        }
    }
}
