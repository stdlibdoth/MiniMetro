using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PropertyPanelScript : MonoBehaviour
{
    [Header("Train")]
    [SerializeField] private GameObject m_TrainPanel;
    [SerializeField] private TextMeshProUGUI m_TrainPosX;
    [SerializeField] private TextMeshProUGUI m_TrainPosY;
    [SerializeField] private TextMeshProUGUI m_Passenger;
    [SerializeField] private TextMeshProUGUI m_TrainTransfered;
    [SerializeField] private TextMeshProUGUI m_TrainSpeed;
    [SerializeField] private Slider m_SpeedSlider;


    private Train m_Train;


    public void DisplayTrainProperty(Train train)
    {

        m_Train?.ActiveOutline(false);
        train.ActiveOutline(true);
        m_Train = train;
        m_TrainPanel.SetActive(true);
    }

    public void HideTrainProperty()
    {
        m_Train?.ActiveOutline(false);
        m_TrainPanel.SetActive(false);
    }

    private void SetTrainPropertyPanel(Train train)
    {
        TrainInfo info = train.GetTrainInfo();

        m_TrainPosX.text = "X:" + info.position.x.ToString("0.0");
        m_TrainPosY.text = "Y:" + info.position.y.ToString("0.0");
        m_Passenger.text = info.passengers.ToString();
        m_TrainTransfered.text = info.transfered.ToString();
        m_TrainSpeed.text = info.speed.ToString("0.00");
        m_SpeedSlider.SetValueWithoutNotify(info.speed);
    }

    private void Awake()
    {
        m_SpeedSlider.onValueChanged.AddListener((value) =>
        {
            if (m_TrainPanel.activeSelf)
                m_Train.Speed = value;
        });
    }

    private void Update()
    {
        if(m_TrainPanel.activeSelf && m_Train!= null)
        {
            SetTrainPropertyPanel(m_Train);
        }
    }
}
