using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChangeSensitivity : MonoBehaviour
    {
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private TextMeshProUGUI output;

        private void Awake()
        {
            if (PlayerPrefs.HasKey("Sensitivity"))
            {
                sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity") / 2;
                output.text = Math.Round(sensitivitySlider.value*2, 3).ToString();
            }
            sensitivitySlider.onValueChanged.AddListener(SaveSensitivity);
            
        }

        private void SaveSensitivity(float value)
        {
            var newSensitivity = value * 2;
            
            if (newSensitivity == 0) newSensitivity = 0.001f;

            PlayerPrefs.SetFloat("Sensitivity",newSensitivity);
            output.text = Math.Round(newSensitivity, 3).ToString();
        }
    }
}
