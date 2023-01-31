using System.Collections;
using System.Collections.Generic;
using Game;
using Mirror;
using UnityEngine;

namespace Player
{
    public class CollisionHandling : NetworkBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float untouchableTime = 3;
        [SerializeField] private Color  untouchableColor = Color.black;

        private bool _isUntouchable;

        private List<GameObject> _players;
        private Dash _dash;
        
        
        private void Awake()
        {
            _dash = GetComponent<Dash>();
        }

        #region Server
        
        
        private void OnCollisionEnter(Collision collision)
        {
            if(!isServer)
                return;
            if (_dash.InDash & collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.GetComponent<CollisionHandling>().GetTouched())
                {
                    ScoreManager.OnAddScore.Invoke(gameObject);
                }
            }
        }

        [Server]
        public bool GetTouched()
        {
            if (_isUntouchable)
            {
                return false;
            }
            RpcUpdateClients();
            return true;

        }
        #endregion

        #region Rpc
        
        [ClientRpc]
        private void RpcUpdateClients()
        {
            StartCoroutine(UntouchableRoutine());
        }
        #endregion

        #region Client
        
        [Client]
        private IEnumerator UntouchableRoutine()
        {
            _isUntouchable = true;
            var color = meshRenderer.material.color;
            meshRenderer.material.color= untouchableColor;

            yield return new WaitForSeconds(untouchableTime);
            
            _isUntouchable = false;
            meshRenderer.material.color = color;
        }
        #endregion
    }
}
