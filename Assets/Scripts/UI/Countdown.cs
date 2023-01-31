using System.Collections;
using Game;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Countdown : NetworkBehaviour
    {
        [SerializeField] private int secondsToWait;
        [SerializeField] private TextMeshProUGUI text;
            private void Awake()
        {
            GameState.ReadyState.AddListener(OnReadyState);
        }


        #region Server
        
        [Server]
        private void OnReadyState()
        {
            RpcStartCountdown();
        }
        #endregion

        #region Rpc
        
        [ClientRpc]
        private void RpcStartCountdown()
        {
            StartCoroutine(CountdownRoutine());
        }
        #endregion


        #region Client
        
        [Client]
        private IEnumerator CountdownRoutine()
        {
            text.gameObject.SetActive(true);
            
            for (int i = secondsToWait;i > 0; i--)
            {
                text.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            text.gameObject.SetActive(false);
            
            UpdateGameState();
        }
        #endregion


        #region Command

        [Command(requiresAuthority = false)]
        private void UpdateGameState()
        {
            GameState.ChangeState.Invoke(State.Playing);
        }
        #endregion
        
    }
}
