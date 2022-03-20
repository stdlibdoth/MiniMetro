using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSVPicker;
using UnityEngine.Events;

public class LineEvent : UnityEvent<Line> { }

public class LinePanelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ToggleGroup m_ToggleGroup;
    [SerializeField] private ToggleGroupDetect m_ToggleDetect;
    [SerializeField] private Button m_AddLineBtn;
    [SerializeField] private ColorPicker m_ColorPicker;
    [SerializeField] private Button m_ColorPickerOk;
    [SerializeField] private Button m_ColorPickerCancel;
    [SerializeField] private LineFactory m_LineFactory;

    [SerializeField] private LineToggleBtnScript m_TogglePrefab;



    private LineEvent m_OnLineSet;


    public LineEvent OnLineSet 
    { 
        get 
        {
            if (m_OnLineSet == null)
                m_OnLineSet = new LineEvent();
            return m_OnLineSet;
        } 
    }

    private void Awake()
    {
        m_AddLineBtn.onClick.AddListener(OnAddBtnClick);
        m_ColorPickerOk.onClick.AddListener(OnOkBtnClick);
        m_ColorPickerCancel.onClick.AddListener(OnCancelBtnClick);
        m_ToggleDetect.OnAnyToggleOn.AddListener(OnAnyToggle);
    }


    public void SetLineIcon(Line line)
    {
        LineToggleBtnScript[] toggles = GetComponentsInChildren<LineToggleBtnScript>();
        for (int i = 0; i < toggles.Length;i++)
        {
            if (toggles[i].LineId == line.Id)
                toggles[i].GetComponent<Toggle>().isOn = true;
        }
    }

    private void OnAddBtnClick()
    {
        ShowColorPicker(true);
    }


    private void OnOkBtnClick()
    {
        m_LineFactory.RegistorLineColor(m_ColorPicker.CurrentColor);
        Line line = m_LineFactory.InstatiateLine(m_ColorPicker.CurrentColor);
        LineToggleBtnScript t = Instantiate(m_TogglePrefab, transform);
        t.Init(line.Id, line.color);
        t.GetComponent<Toggle>().group = m_ToggleGroup;
        m_ToggleDetect.AddToggle(t.GetComponent<Toggle>());
        m_OnLineSet.Invoke(line);
        ShowColorPicker(false);
    }


    private void OnCancelBtnClick()
    {
        ShowColorPicker(false);
    }

    private void ShowColorPicker(bool show)
    {
        m_ColorPicker.gameObject.SetActive(show);
        m_ColorPickerOk.gameObject.SetActive(show);
        m_ColorPickerCancel.gameObject.SetActive(show);
    }

    private void OnAnyToggle(Toggle toggle)
    {
        int id = toggle.GetComponent<LineToggleBtnScript>().LineId;
        m_OnLineSet.Invoke(m_LineFactory.GetLine(id));
    }
}
