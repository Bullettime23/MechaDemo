using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mecha {
    public class UIOptions : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown m_Dropdown;
        [SerializeField] private Toggle m_Fullscreen;

        private List<Resolution> m_SelectedResolutionList = new List<Resolution>();

        Resolution[] m_Resolutions;
        bool m_IsFullScreen;
        int m_SelectedResolution;

        void Start()
        {
            m_IsFullScreen = true;
            m_Resolutions = Screen.resolutions;

            string nextResolution;

            List<string> resList = new List<string>();
            foreach (var resolution in m_Resolutions)
            {
                nextResolution = $"{resolution.width}X{resolution.height}";
                if (!resList.Contains(nextResolution))
                {
                    resList.Add(nextResolution);
                    m_SelectedResolutionList.Add(resolution);
                }
            }

            m_Dropdown.AddOptions(resList);
        }

        public void SetSelectedResolution()
        {
            m_SelectedResolution = m_Dropdown.value;
            Screen.SetResolution(m_SelectedResolutionList[m_SelectedResolution].width, m_SelectedResolutionList[m_SelectedResolution].width, true);
        }
    }
}