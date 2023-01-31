using Game;
using UnityEngine;

namespace UI
{
    public class CursorConfined :MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
            print("Confined");
        }
    }
}
