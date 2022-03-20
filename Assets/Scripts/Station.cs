using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;


public class StationEvent:UnityEvent<Station>
{

}

public class Station : MonoBehaviour, IDragHandler, IPointerClickHandler, IEndDragHandler
{
    [SerializeField] private int m_Type;


    [Header("Data")]
    //[SerializeField] private int[] m_PassengerTypes;
    [SerializeField] private int m_PassengerSpawnRate;
    [SerializeField] private int m_PassengerThreshold;
    [SerializeField] private int m_PassengerThresholdTime;



    [Header("Reference")]
    [SerializeField] private GameObject m_PassengerHolder;
    [SerializeField] private Canvas m_Canvas;

    [SerializeField] private Node m_Node;
    private List<Line> m_Lines;
    [SerializeField] private List<int> m_Passengers;
    private List<GameObject> m_PassengerSlots;
    private bool m_OperationFlag;
    private FrameTimer m_PassengerSpawnTimer;
    private FrameTimer m_PassengerThresTimer;
    private int m_ThresholdOverflow;
    private Material m_Mat;

    private StationEvent m_OnStationDrag;
    private StationEvent m_OnStationClick;
    private StationEvent m_OnStationEndDrag;

    private Dictionary<Line, TrainEvent> m_trainEvents;

    public Node Node 
    { 
        get { return m_Node; }
        set { m_Node = value; }
    }
    public List<Line> Lines { get { return m_Lines; } }

    public int Type { get { return m_Type; }}

    public StationEvent OnStationDrag { get { return m_OnStationDrag; } }
    public StationEvent OnStationClick { get { return m_OnStationClick; } }
    public StationEvent OnStationEndDrag { get { return m_OnStationEndDrag; } }

    private void Awake()
    {
        m_OnStationDrag = new StationEvent();
        m_OnStationClick = new StationEvent();
        m_Passengers = new List<int>();
        m_OnStationEndDrag = new StationEvent();
        m_Lines = new List<Line>();
        m_trainEvents = new Dictionary<Line, TrainEvent>();
        m_PassengerSlots = new List<GameObject>();
        m_Mat = GetComponent<SpriteRenderer>().material;
        m_OperationFlag = false;
        m_ThresholdOverflow = 0;
        m_Type = 0;
    }

    private void Update()
    {
        bool endCondition = m_Lines.Count == 1 && m_Lines[0].Stations.Length == 1 && m_Lines[0].Stations[0] == this;
        bool startCondition = m_Lines.Count > 0 && m_Lines[0].Stations.Length > 1;
        if (!m_OperationFlag && startCondition)
            StartOperation();
        else if (m_OperationFlag && endCondition)
            EndOperation();

        if (m_PassengerThresTimer!= null && m_PassengerThresTimer.IsCounting && Time.timeScale == 1)
        {
            m_ThresholdOverflow++;
            Color color = Color.red;
            int frameThreshCount = m_PassengerThresholdTime * GameManager.FramesPerTimeUnit;
            float percent = (m_ThresholdOverflow * 1f) / (frameThreshCount*1f);
            percent = Mathf.Clamp01(percent);
            color.g = 1 - percent;
            color.b = 1 - percent;
            m_Mat.SetColor("_Color", color);
            m_Mat.SetFloat("_ShakeUvX", percent * 5f);
            m_Mat.SetFloat("_ShakeUvY", percent * 5f);
            m_Mat.SetFloat("_ShakeUvSpeed", percent * 20f);
        }

    }

    private Image GetPassengerSlot(int type)
    {
        for (int i = 0; i < m_PassengerSlots.Count; i++)
        {
            if (!m_PassengerSlots[i].gameObject.activeSelf)
            {
                Image image = m_PassengerSlots[i].gameObject.GetComponent<Image>();
                image.sprite = StationFactory.GetPassengerSprite(type);
                image.gameObject.SetActive(true);
                return image;
            }
        }
        Image slot = StationFactory.GeneratePassengerImage(type);
        m_PassengerSlots.Add(slot.gameObject);
        slot.transform.SetParent(m_PassengerHolder.transform);
        return slot;
    }

    private void SpawnPassenger()
    {
        if (m_Passengers.Count >= m_PassengerThreshold)
            return;
        List<int> passengerTypes = new List<int>();
        for (int i = 0; i < m_Lines.Count; i++)
        {
            passengerTypes.AddRange(m_Lines[i].StationTypes);
        }
        HashSet<int> h = new HashSet<int>(passengerTypes);
        passengerTypes = new List<int>(h);
        passengerTypes.Remove(Type);
        if (passengerTypes.Count > 0)
        {
            int index = Random.Range(0, passengerTypes.Count);
            m_Passengers.Add(passengerTypes[index]);
            if (m_Passengers.Count == m_PassengerThreshold)
            {
                m_Mat.EnableKeyword("SHAKEUV_ON");
                m_PassengerThresTimer.Start();
            }
        }
        UpdatePassengerImage();
    }

    public void StartOperation()
    {
        print("start");
        m_OperationFlag = true;
        m_PassengerSpawnTimer = FrameTimerManager.GetTimer(m_PassengerSpawnRate * GameManager.FramesPerTimeUnit,
            FrameTimerMode.Repeat);
        m_PassengerSpawnTimer.OnTimeUp.AddListener(SpawnPassenger);
        m_PassengerSpawnTimer.Start();

        m_PassengerThresTimer = FrameTimerManager.GetTimer(m_PassengerThresholdTime * GameManager.FramesPerTimeUnit,
            FrameTimerMode.Repeat);
        m_PassengerThresTimer.OnTimeUp.AddListener(OnReachThreshold);
    }

    public void EndOperation()
    {
        print("end");
        m_OperationFlag = false;
        GameManager.thumbsDown += m_Passengers.Count;
        m_Passengers.Clear();
        UpdatePassengerImage();
        m_PassengerSpawnTimer.Stop();
        m_PassengerThresTimer.Stop();
    }

    public void Init(int type)
    {
        m_Type = type;
        m_Canvas.worldCamera = Camera.main;
    }


    private void UpdatePassengerImage()
    {
        for (int i = 0; i < m_PassengerSlots.Count; i++)
        {
            m_PassengerSlots[i].SetActive(false);
        }
        for (int i = 0; i < m_Passengers.Count; i++)
        {
            GetPassengerSlot(m_Passengers[i]);
        }
    }

    private void OnReachThreshold()
    {
        m_ThresholdOverflow = 0;
        m_Mat.SetColor("_Color", Color.white);
        m_Mat.DisableKeyword("SHAKEUV_ON");
        GameManager.thumbsDown += m_Passengers.Count;
        m_Passengers.Clear();
        UpdatePassengerImage();
        m_PassengerSpawnTimer.Reset();
        m_PassengerThresTimer.Stop();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_OnStationDrag.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_OnStationClick.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_OnStationEndDrag.Invoke(this);
    }


    public TrainEvent GetEnterStaionEvent(Line line)
    {
        if (!m_trainEvents.ContainsKey(line))
            m_trainEvents.Add(line, new TrainEvent());
        return m_trainEvents[line];
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Train"))
        {
            Train train = other.GetComponent<Train>();
            Line line = train.Line;
            m_trainEvents[line].Invoke(train);
            train.Alight(this);
            int aboardedNum = train.Aboard(m_Passengers.ToArray());
            m_Passengers.RemoveRange(0, aboardedNum);
            UpdatePassengerImage();
            if (m_Passengers.Count < m_PassengerThreshold)
            {
                m_PassengerThresTimer.Stop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Train"))
        {
            Train train = other.GetComponent<Train>();
            if (train.spawnFlag)
                train.spawnFlag = false;
        }
    }

    private void OnDestroy()
    {
        FrameTimerManager.DisposeTimer(m_PassengerSpawnTimer);
        FrameTimerManager.DisposeTimer(m_PassengerThresTimer);
    }
}
