using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleEvent : UnityEvent<Toggle> { }

public class ToggleGroupDetect : MonoBehaviour
{
    private List<Toggle> m_Toggles;
    private ToggleEvent m_OnAnyToggleOn;

    public ToggleEvent OnAnyToggleOn
    {
        get 
        {
            if (m_OnAnyToggleOn == null)
                m_OnAnyToggleOn = new ToggleEvent();
            return m_OnAnyToggleOn; 
        }
    }


    private void Awake()
    {
        m_Toggles = new List<Toggle>();
    }

    public void AddToggle(Toggle toggle)
    {
        if (m_Toggles.Contains(toggle))
            return;

        m_Toggles.Add(toggle);
        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool active)
    {
        if(active)
        {
            foreach (var toggle in m_Toggles)
            {
                if (toggle.isOn)
                {
                    m_OnAnyToggleOn.Invoke(toggle);
                }
            }
        }
    }
}
