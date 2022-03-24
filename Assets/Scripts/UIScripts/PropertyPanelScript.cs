using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PropertyPanelScript : MonoBehaviour
{
    [Header("Train")]
    [Space]
    [SerializeField] private GameObject m_TrainPanel;
    [SerializeField] private TextMeshProUGUI m_TrainPosX;
    [SerializeField] private TextMeshProUGUI m_TrainPosY;
    [SerializeField] private TextMeshProUGUI m_Passenger;
    [SerializeField] private TextMeshProUGUI m_TrainTransfered;
    [SerializeField] private TextMeshProUGUI m_TrainSpeed;
    [SerializeField] private Slider m_SpeedSlider;


    [Header("Station")]
    [Space]
    [SerializeField] private GameObject m_StationPanel;
    [SerializeField] private Image m_Type;
    [SerializeField] private TextMeshProUGUI m_WaitingPassenger;
    [SerializeField] private TextMeshProUGUI m_Capacity;
    [SerializeField] private TextMeshProUGUI m_SpawnRate;
    [SerializeField] private TextMeshProUGUI m_Tolerance;
    [SerializeField] private GameObject m_CountDownPanel;
    [SerializeField] private TextMeshProUGUI m_Countdown;
    [SerializeField] private Slider m_CapacitySlider;
    [SerializeField] private Slider m_RateSlider;
    [SerializeField] private Slider m_ToleranceSlider;

    private Train m_Train;
    private Station m_Station;



    private void Awake()
    {
        m_SpeedSlider.onValueChanged.AddListener((value) =>
        {
            if (m_TrainPanel.activeSelf)
                m_Train.Speed = value;
        });


        m_CapacitySlider.onValueChanged.AddListener((value) =>
        {
            if (m_StationPanel.activeSelf)
                m_Station.SetThreshold((int)value);
        });
        m_RateSlider.onValueChanged.AddListener((value) =>
        {
            if (m_StationPanel.activeSelf)
                m_Station.SetSpawnRate((int)value);
        });
        m_ToleranceSlider.onValueChanged.AddListener((value) =>
        {
            if (m_StationPanel.activeSelf)
                m_Station.SetThresholdTime((int)value);
        });

    }

    private void Update()
    {
        if(m_TrainPanel.activeSelf && m_Train!= null)
        {
            SetTrainPropertyPanel(m_Train);
        }
        else if(m_StationPanel.activeSelf && m_Station != null)
        {
            SetStationPanel(m_Station);
        }
    }

    #region Train


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
    #endregion

    #region Station

    public void DisplayStationProperty(Station station)
    {
        m_Station?.ActiveOutline(false);
        station.ActiveOutline(true);
        m_Station = station;
        m_StationPanel.SetActive(true);
    }

    public void HideStationProperty()
    {
        m_Station?.ActiveOutline(false);
        m_CapacitySlider.interactable = true;
        m_RateSlider.interactable = true;
        m_ToleranceSlider.interactable = true;
        m_StationPanel.SetActive(false);
    }

    private void SetStationPanel(Station station)
    {
        StationInfo info = station.GetStationInfo();

        m_Type.sprite = StationFactory.GetPassengerSprite(info.type);
        m_WaitingPassenger.text = info.passengers.ToString();
        m_Capacity.text = info.threshold.ToString();
        m_SpawnRate.text = info.spawnRate.ToString();
        m_Tolerance.text = info.thresholdTime.ToString();
        m_Countdown.text = info.countDown.ToString();
        m_CapacitySlider.SetValueWithoutNotify(info.threshold);
        m_RateSlider.SetValueWithoutNotify(info.spawnRate);
        m_ToleranceSlider.SetValueWithoutNotify(info.thresholdTime);
        m_CountDownPanel.SetActive(info.isCountingDown);
        m_CapacitySlider.interactable = !info.isCountingDown;
        m_RateSlider.interactable = !info.isCountingDown;
        m_ToleranceSlider.interactable = !info.isCountingDown;
    }


    #endregion
}
