using System.Collections.Generic;
using Mirror;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ScoreManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI winText;
        [SerializeField] private int scoreToWin = 3;

        public static readonly UnityEvent<GameObject> OnAddScore = new UnityEvent<GameObject>();

        private List<int> _score = new List<int>();
        private void Awake()
        {
            
            GameState.ReadyState.AddListener(SetUpScore);   
            GameState.RestartState.AddListener(OnGameRestart);
            OnAddScore.AddListener(OnScoreAdd);
        }


        #region Server
        
        [Server]
        private void SetUpScore()
        {
            _score.Clear();
            foreach (var player in PlayersList.Players)
            {
                _score.Add(0);
            }
            UpdateTextScore();
        }

        [Server]
        private void UpdateTextScore()
        {
            var text = "\t";
            for (var index = 0; index < _score.Count; index++)
            {
                text += $"{PlayersList.Players[index].name}: {_score[index]}\t";
            }

            RpcUpdateTextScore(text);
        }

        [Server]
        private void OnGameRestart()
        {
            SetUpScore();
            RpcHideWinText();
        }
        
        [Server]
        private void CheckForWin()
        {
            for (var index = 0; index < _score.Count; index++)
            {
                if (_score[index] == scoreToWin)
                {
                    RpcShowWinText(PlayersList.Players[index].name.ToString());
                    GameState.ChangeState?.Invoke(State.GameOver);
                    return;
                }
            }
        }
        #endregion


        #region Command
        
        [Command (requiresAuthority = false)]
        private void OnScoreAdd(GameObject attacker)
        {
            for (var index = 0; index < _score.Count; index++)
            {
                if (PlayersList.Players[index] == attacker)
                {
                    _score[index]++;
                    break;
                }
            }
            UpdateTextScore();
            CheckForWin();
        }
        #endregion

        #region Rpc
        
        [ClientRpc]
        private void RpcUpdateTextScore(string text)
        {
            scoreText.text = text;
        }

        [ClientRpc]
        private void RpcShowWinText(string player)
        {
            winText.text = $"{player} win!";
            winText.gameObject.SetActive(true);
        }

        [ClientRpc]
        private void RpcHideWinText()
        {
            winText.gameObject.SetActive(false);
        }
        #endregion
    }
}
