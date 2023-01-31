using Mirror;
using UnityEngine;

namespace Player
{
    public class AttachCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 cameraOffset;
        private Camera _camera;
    
        private void Start()
        {
            if(!transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer)
                return;
            _camera = Camera.main;
            if (_camera != null)
            {
                _camera.transform.parent = gameObject.transform;
                _camera.transform.localPosition = cameraOffset;
                _camera.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
