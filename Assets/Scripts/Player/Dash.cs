using Mirror;
using UnityEngine;

namespace Player
{
    public class Dash : NetworkBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private float force;
    
        private Rigidbody _rigidbody;
        private Movements _movements;
    
        private Vector3 _startPosition;
        private Vector3 _startVelocity;

        public bool InDash { private set; get; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _movements = GetComponent<Movements>();
            InDash = false;
        }
    
        private void Update()
        {
            if(isServer & InDash)
                if(!CheckForEndDash())
                    return;

            if (!isLocalPlayer | InDash)
            {
                return;
            }
            GetInput();
        }

        #region Client
        
        [Client]
        private void GetInput()
        { 
            if (Input.GetMouseButtonDown(0))
            {
                MakeDash();
            }
        }
        
        [Client]
        private void MakeDash()
        {
            _startPosition = transform.position;
            _movements.enabled = false;
            InDash = true;
            _rigidbody.AddForce(_rigidbody.velocity.normalized*force,ForceMode.VelocityChange);
            CmdMakeDash();
        }
        
        [Client]
        private void OnDisable()
        {
            InDash = false;
            if(isLocalPlayer)
                CmdOnDisable();
        }
        #endregion

        #region Command

        [Command]
        private void CmdMakeDash()
        {
            _startPosition = transform.position;
            _movements.enabled = false;
            InDash = true;
            _startVelocity = _rigidbody.velocity;
            _rigidbody.AddForce(_startVelocity.normalized*force,ForceMode.VelocityChange);
            RpcUpdateDash(true);
        }

        [Command]
        private void CmdOnDisable()
        {
            InDash = false;
        }
        #endregion

        #region Server
        
        [Server]
        private bool CheckForEndDash()
        {
            if (Vector3.Distance(_rigidbody.velocity, Vector3.zero)< Vector3.Distance(_startVelocity, Vector3.zero) |
                Vector3.Distance(_startPosition, transform.position) > distance |
                _rigidbody.velocity == new Vector3(0,0,_rigidbody.velocity.z))
            {
                StopDash();
                return false;
            }

            return true;
        }
        

        [Server]
        private void StopDash()
        {
            InDash = false;
            _movements.enabled = true;
            RpcUpdateDash(false);
        }
        #endregion

        #region Rpc

        [TargetRpc]
        private void RpcUpdateDash(bool isActive)
        {
            if (this.enabled)
            {
                _movements.enabled = !isActive;
                InDash = isActive;
            }
            
        }
        #endregion
        

        

        

        
    }
}
