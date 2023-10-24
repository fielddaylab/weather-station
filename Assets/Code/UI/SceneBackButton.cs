using System;
using BeauUtil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeatherStation
{
    public class SceneBackButton : MonoBehaviour
    {
        [SerializeField, Required] private Button m_Button = null;
        [SerializeField] private bool m_StopMusic = true;

        private void Awake()
        {
            m_Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (m_StopMusic)
                Services.Audio.StopMusic();
            StateUtil.LoadPreviousSceneWithWipe();
        }
    }
}