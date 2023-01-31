using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class Rotation : NetworkBehaviour
    {
        [SerializeField] private Transform cameraPivot;

        [SerializeField] private float sensitivity;

        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        
        private Vector2 _mouseInput;
        private Vector3 _rotate;

        private void Awake()
        {
            if (isLocalPlayer & PlayerPrefs.HasKey("Sensitivity"))
                sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }

        private void Update()
        {
            if(!isLocalPlayer)
                return;
            
            GetInput();
            ApplyRotation();
        }

        #region Client
        
        [Client]
        private void GetInput()
        {
            _mouseInput = new Vector2(Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"));
            _rotate = new Vector3(_mouseInput.x * sensitivity, _mouseInput.y * sensitivity, 0);
        }


        [Client]
        private void ApplyRotation()
        {
            if(!isServer) //If host and server are same client
            {
                cameraPivot.localEulerAngles = new Vector3(Mathf.Clamp(cameraPivot.localEulerAngles.x -_rotate.x,minAngle,maxAngle), 0, 0);
                transform.localEulerAngles += new Vector3(0, _rotate.y, 0);
            }
            CmdApplyRotation(_rotate);
        }
        #endregion

        #region Command
        
        [Command]
        private void CmdApplyRotation(Vector3 rotate)
        {
            cameraPivot.localEulerAngles = new Vector3(Mathf.Clamp(cameraPivot.localEulerAngles.x -rotate.x,minAngle,maxAngle), 0, 0);
            transform.localEulerAngles += new Vector3(0, rotate.y, 0);
        }
        #endregion
    }
    
}
