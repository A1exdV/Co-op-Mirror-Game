using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public enum State
    {
        Temp,
        Ready,
        Playing,
        GameOver,
        Restart,
    }
    public class GameState : NetworkBehaviour
    {
        [SerializeField]private float timeOnWin = 5;
        public static readonly UnityEvent<State> ChangeState = new UnityEvent<State>();
    
        public static readonly UnityEvent ReadyState = new UnityEvent();
        public static readonly UnityEvent PlayingState = new UnityEvent();
        public static readonly UnityEvent GameOverState = new UnityEvent();
        public static readonly UnityEvent RestartState = new UnityEvent();
    
        private State _currentState;
        private void Awake()
        {
        
            ChangeState.AddListener(OnChangeState);
        }

        #region Server
        
        [Server]
        private void OnChangeState(State newState)
        {
            if (isServer)
            {
                ChangeStace(newState);
            }

        }

        [Server]
        private void ChangeStace(State newState)
        {
            if(_currentState==newState)
                return;
            
            _currentState = newState;
            print($"State: {_currentState}");
            switch (_currentState)
            {
                case State.Ready:
                    ReadyState?.Invoke();
                    break;
                
                case State.Playing:
                    PlayingState?.Invoke();
                    break;
                
                case State.GameOver:
                    GameOverState?.Invoke();
                    StartCoroutine(OnGameOver());
                    break;
                
                case State.Restart:
                    RestartState?.Invoke();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        [Server]
        private IEnumerator OnGameOver()
        {
            yield return new WaitForSeconds(timeOnWin);
            ChangeState?.Invoke(State.Restart);
            yield return new WaitForEndOfFrame();
            ChangeState?.Invoke(State.Ready);
        }
        #endregion
    }
}