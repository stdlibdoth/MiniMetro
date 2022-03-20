using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
public class IntEvent : UnityEvent<int> { }

public class AddStationScript : MonoBehaviour
{
    [SerializeField] private Toggle m_AddButton;
    [SerializeField] private TMP_Dropdown m_StatopnDropDown;


    private BoolEvent m_OnAddButtonClick;
    private StationEvent m_OnStationSet;
    [SerializeField] private Station m_Station;

    public BoolEvent OnAddToggleClick
    { 
        get 
        {
            if (m_OnAddButtonClick == null)
                m_OnAddButtonClick = new BoolEvent();
            return m_OnAddButtonClick; 
        } 
    }

    public StationEvent OnStationSet
    {
        get
        {
            if (m_OnStationSet == null)
                m_OnStationSet = new StationEvent();
            return m_OnStationSet;
        }
    }


    private void Awake()
    {
        m_AddButton.onValueChanged.AddListener(OnAddBtnClick);
        m_StatopnDropDown.onValueChanged.AddListener(OnValueChange);
        m_StatopnDropDown.value = 0;
    }


    private void Update()
    {
        if (m_Station == null && m_AddButton.isOn)
            m_AddButton.isOn = false;
    }

    private void OnValueChange(int val)
    {
        Station old = m_Station;
        m_Station = StationFactory.InstatiateStation(val);
        if (old)
            DestroyImmediate(old.gameObject);
        m_Station?.gameObject.SetActive(m_AddButton.isOn);
        m_OnStationSet.Invoke(m_Station);
    }

    private void OnAddBtnClick(bool toggle)
    {
        if(m_Station == null)
            m_Station = StationFactory.InstatiateStation(m_StatopnDropDown.value);
        m_Station.gameObject.SetActive(toggle);
        m_OnStationSet.Invoke(m_Station);
        m_OnAddButtonClick.Invoke(toggle);
    }

    public void SetAddButton(bool toggle)
    {
        m_AddButton.isOn = toggle;
    }
}
