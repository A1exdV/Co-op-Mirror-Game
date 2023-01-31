using UnityEngine;

namespace UI
{
    public class CloseGame : MonoBehaviour
    {

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        private void Close()
        {
            Application.Quit();
        }
    }
}
