using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class BoolEvent:UnityEvent<bool>
{

}

public class FloatEvent : UnityEvent<float>
{

}

public class ControlPanelScripts : MonoBehaviour
{
    [SerializeField] private Button m_PlayBtn;
    [SerializeField] private Button m_PauseBtn;
    [SerializeField] private TextMeshProUGUI m_ThumbsUp;
    [SerializeField] private TextMeshProUGUI m_ThumbsDown;
    private bool m_PlayingFlag;

    public BoolEvent onControlBtnClick;
    public FloatEvent onSpeedSliderChange;

    private void Awake()
    {
        m_PlayingFlag = false;
        m_PlayBtn.gameObject.SetActive(false);
        m_PauseBtn.gameObject.SetActive(true);
        m_PlayBtn.onClick.AddListener(OnControlBtnClick);
        m_PauseBtn.onClick.AddListener(OnControlBtnClick);
        onControlBtnClick = new BoolEvent();
        onSpeedSliderChange = new FloatEvent();
    }


    private void Update()
    {
        m_ThumbsUp.text = GameManager.thumbsUp.ToString();
        m_ThumbsDown.text = GameManager.thumbsDown.ToString();
    }


    private void OnControlBtnClick()
    {
        m_PlayingFlag = !m_PlayingFlag;
        m_PlayBtn.gameObject.SetActive(m_PlayingFlag);
        m_PauseBtn.gameObject.SetActive(!m_PlayingFlag);
        if(m_PlayingFlag)
        {
            Time.timeScale = 1;
            FrameTimerManager.StartAll();
        }
        else
        {
            Time.timeScale = 0;
            FrameTimerManager.StopAll();
        }
        onControlBtnClick.Invoke(m_PlayingFlag);
    }

}
