using Mirror;
using UnityEngine;

namespace Player
{
    public class Movements : NetworkBehaviour
    {
        [SerializeField] private float speed;
        
        private Rigidbody _rigidbody;
        
        
        private Vector2 _direction;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
       
        
        #region Client
        
        [Client]
        private void Update()
        {
            if(!isLocalPlayer)
                return;

            MovePlayer();
        }
        
        [Client]
        private void MovePlayer()
        {
            var velocity = Vector3.zero;
       
            if (Input.GetKey(KeyCode.W))
            {
                velocity += transform.forward;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                velocity -= transform.right;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                velocity -= transform.forward;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                velocity += transform.right;
            }
            
            _rigidbody.velocity = velocity * speed;
            CmdSendVelocity(velocity);
        }
        #endregion

        #region Command
        
        [Command]
        private void CmdSendVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity * speed;
        }
        #endregion
    }
}
