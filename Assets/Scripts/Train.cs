using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrainEvent : UnityEvent<Train> { }

public class Train : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private int m_Direction;

    [SerializeField] [Range(0, 6)] private int m_MaxPassenger; 
    [SerializeField] private List<Image> m_PassengerSlots;
    [SerializeField] private SpriteRenderer m_SRenderer;

    [SerializeField] private int[] m_Passengers;



    private Line m_Line;
    private Link m_Link;
    private Vector3[] m_WayPoints;
    private int m_Index;
    private bool m_IsMoving;

    public bool spawnFlag;


    public bool IsMoving
    {
        get { return m_IsMoving; }
    }

    public int Direction 
    { 
        get { return m_Direction; }
        set { m_Direction = value; }
    }

    public Link Location
    {
        get { return m_Link; }
    }

    public Line Line
    {
        get { return m_Line; }
    }

    private void Awake()
    {
        m_Passengers = new int[m_PassengerSlots.Count];
        for (int i = 0; i < m_Passengers.Length; i++)
        {
            m_Passengers[i] = -1;
        }
    }

    private void Update()
    {
        if (!m_IsMoving)
            return;
        float step = m_Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, m_WayPoints[m_Index], step);
        UpdateRotation();
        if (Vector3.Distance(transform.position, m_WayPoints[m_Index]) <= 0.05f
            && m_Index < m_WayPoints.Length - 1)
            m_Index++;
    }

    public void UpdateRotation()
    {
        Vector3 target = m_WayPoints[m_Index];
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right,dir);
    }


    public void Init(Line line)
    {
        m_Line = line;
        m_Direction = 1;
        Color color = line.color;
        m_SRenderer.color = color;
    }


    public void StartTrain()
    {
        m_IsMoving = true;
    }

    public void StopTrain()
    {
        m_IsMoving = false;
    }

    public void SetWayPoint(Link link, Vector3[] way_points)
    {
        m_Link = link;
        m_WayPoints = way_points;
        if (m_WayPoints.Length > 1)
            m_Index = 1;
    }

    public void Alight(Station station)
    {
        for (int i = 0; i < m_Passengers.Length; i++)
        {
            if (m_Passengers[i] == -1)
                continue;
            if(station.Type == m_Passengers[i])
            {
                m_Passengers[i] = -1;
                m_PassengerSlots[i].enabled = false;
                GameManager.thumbsUp++;
            }
        }
    }

    public int Aboard(int[] passengers)
    {
        int boardedCount = 0;
        for (int i = 0; i < m_PassengerSlots.Count; i++)
        {
            if (m_Passengers[i] == -1 && passengers.Length > boardedCount)
            {
                m_Passengers[i] = passengers[boardedCount];
                boardedCount++;
            }
        }
        for (int i = 0; i < m_Passengers.Length; i++)
        {
            if (m_Passengers[i] == -1)
                continue;
            m_PassengerSlots[i].sprite = StationFactory.GetPassengerSprite(m_Passengers[i]);
            m_PassengerSlots[i].enabled = true;
        }


        return boardedCount;
    }


    private void OnDestroy()
    {
        GameManager.thumbsDown += m_Passengers.Length;
    }
}
