using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MetroState
{
    LineEditing,
    StationAdding,
    TrainEditing,
    GridEditing,
    PropertyEditing,
}

public class MetroManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_Cam;
    [SerializeField] private AddStationScript m_AddStationScript;
    [SerializeField] private LinePanelScript m_LinePanelScript;
    [SerializeField] private PropertyPanelScript m_PropertyPanelScript;
    [SerializeField] private Toggle m_TrainToggle;

    [Header("Prefabs")]
    [SerializeField] private Line m_LinePrefab;



    [Header("Params")]
    [SerializeField] private MetroState m_MetroState;
    [SerializeField] private int m_LineEditingState;


    [SerializeField] private bool l1bool;
    private Line m_Line;

    private Node m_tempNode;
    private Station m_fromStation;
    private bool m_StationDragFlag;
    private Station m_TempStation;

    public MetroState State { get { return m_MetroState; } }


    private void Awake()
    {
        m_AddStationScript.OnStationSet.AddListener(OnTempStationSet);
        m_AddStationScript.OnAddToggleClick.AddListener(OnAddToggleClick);
        m_LinePanelScript.OnLineSet.AddListener(OnLineSet);
        m_TrainToggle.onValueChanged.AddListener(OnTrainBtnToggle);
    }

    private void Update()
    {
        switch (m_MetroState)
        {
            case MetroState.LineEditing:
                LineEditUpdate();
                break;
            case MetroState.StationAdding:
                AddStationUpdate();
                break;
            case MetroState.TrainEditing:
                TrainEditingUpdate();
                break;
            case MetroState.GridEditing:
                break;
            case MetroState.PropertyEditing:
                break;
            default:
                break;
        }

    }


    private void OnAddToggleClick(bool toggle)
    {
        m_MetroState = toggle ? MetroState.StationAdding : MetroState.LineEditing;
        m_TrainToggle.SetIsOnWithoutNotify(false);
        m_PropertyPanelScript.HideStationProperty();
    }

    #region Train Related Handler

    private void OnTrainBtnToggle(bool toggle)
    {
        m_MetroState = toggle ? MetroState.TrainEditing : MetroState.LineEditing;
        m_AddStationScript.SetAddButton(false);
        m_PropertyPanelScript.HideStationProperty();
    }

    #endregion

    #region Line Related Handler

    private void OnLineSet(Line line)
    {
        m_Line = line;
    }
    #endregion

    #region Station related Handler
    private void OnTempStationSet(Station station)
    {
        m_TempStation = station;
    }
    private void StationDragHandler(Station station)
    {
        m_StationDragFlag = true;
        if (m_MetroState != MetroState.LineEditing)
            return;
        if (Input.GetMouseButton(1))
        {
            Vector2 pos = m_Cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int coord = GameManager.Grid.SnapNearest(pos);
            station.Node.Coord = coord;
            station.transform.position = new Vector3(coord.x, coord.y, station.transform.position.z);
        }
    }

    private void StationEndDragHandler(Station station)
    {
        m_StationDragFlag = false;
    }

    private void StationUpHandler(Station station)
    {
        if (m_MetroState != MetroState.LineEditing)
            return;
        if(Input.GetMouseButtonUp(1))
        {
            if (!m_StationDragFlag)
            {
                if (m_Line)
                    m_Line.RemoveStation(station);
                else if (station.Lines.Count == 0)
                    Destroy(station.gameObject);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            m_PropertyPanelScript.DisplayStationProperty(station);
        }
    }

    #endregion

    #region Updates every frame

    private void AddStationUpdate()
    {
        if (!m_TempStation)
            return;
        Vector2 pos = m_Cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coord = GameManager.Grid.SnapNearest(pos);
        m_TempStation.transform.position = new Vector3(coord.x, coord.y, -0.1f);
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Station")))
            {
                Station station = Instantiate(m_TempStation);
                station.Init(m_TempStation.Type);
                station.OnStationDrag.AddListener(StationDragHandler);
                station.OnStationClick.AddListener(StationUpHandler);
                station.OnStationEndDrag.AddListener(StationEndDragHandler);
                station.Node = new Node(coord);
                station.GetComponent<BoxCollider>().enabled = true;
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            Destroy(m_TempStation.gameObject);
            m_MetroState = MetroState.LineEditing;
        }
    }

    private void LineEditUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Floor")))
            {
                m_PropertyPanelScript.HideStationProperty();
            }
        }

        if (m_Line == null)
            return;
        if (Input.GetMouseButtonDown(0) && m_LineEditingState == 0)
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Station")))
            {
                m_LineEditingState = 1;
                m_fromStation = hitinfo.transform.GetComponent<Station>();
            }
        }
        else if (Input.GetMouseButton(0) && m_LineEditingState == 1)
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Floor"));
            Vector2Int coord = GameManager.Grid.SnapNearest(hitinfo.point);
            m_tempNode = new Node(coord);
            m_Line.InsertTempBranch(m_fromStation.Node, m_tempNode);
        }
        else if (Input.GetMouseButtonUp(0) && m_LineEditingState == 1)
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Station")))
            {
                Station endStation = hitinfo.transform.GetComponent<Station>();
                Node fromNode = m_fromStation.Node;
                if (m_fromStation != endStation)
                {
                    Node endNode = endStation.Node;
                    if (!m_Line.Contains(endNode) && m_Line.Contains(fromNode))
                    {
                        print("1");
                        Node nextNode = m_Line.NextNode(fromNode);
                        Node prevNode = m_Line.PrevNode(fromNode);
                        if (nextNode == fromNode)
                            m_Line.AppendTail(endNode);
                        else if (prevNode == fromNode)
                            m_Line.ExtendHead(endNode);
                        else
                            m_Line.InsertNodeBetween(endNode, fromNode, nextNode);
                        endStation.Lines.Add(m_Line);
                        m_Line.AddStation(endStation);
                    }
                    else if (m_Line.Contains(endNode) && !m_Line.Contains(fromNode))
                    {
                        print("2");
                        Node nextNode = m_Line.NextNode(endNode);
                        Node prevNode = m_Line.PrevNode(endNode);
                        if (nextNode == endNode)
                            m_Line.AppendTail(fromNode, true);
                        else if (prevNode == endNode)
                            m_Line.ExtendHead(fromNode, true);
                        else
                            m_Line.InsertNodeBetween(fromNode, endNode, nextNode);
                        m_fromStation.Lines.Add(m_Line);
                        m_Line.AddStation(m_fromStation);
                    }
                    else if (!m_Line.Contains(endStation.Node) && !m_Line.Contains(fromNode))
                    {
                        if (m_Line.Nodes.Length == 0)
                        {
                            print("3");
                            m_Line.AppendTail(fromNode);
                            m_Line.AppendTail(endNode);
                            m_Line.AddStation(m_fromStation);
                            m_Line.AddStation(endStation);
                            m_fromStation.Lines.Add(m_Line);
                            endStation.Lines.Add(m_Line);
                        }
                    }
                    else
                    {
                        print("4");
                        if (fromNode == m_Line.PrevNode(endNode) || fromNode == m_Line.NextNode(endNode))
                        {
                            m_Line.Replace(fromNode, endNode);
                        }
                    }
                }
            }
            m_tempNode = null;
            m_Line.RemoveTempBranch();
            m_LineEditingState = 0;
        }
    }

    private void TrainEditingUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Line")))
            {
                m_Line = hitinfo.transform.GetComponent<Segment>().Line;
                m_LinePanelScript.SetLineIcon(m_Line);
                Train train = m_Line.SpawnTrain();
                if (train == null)
                    return;
                FrameTimer timer = FrameTimerManager.GetTimer(30, FrameTimerMode.OneShot);
                timer.OnTimeUp.AddListener(() => { train.StartTrain(); FrameTimerManager.DisposeTimer(timer); });
                m_PropertyPanelScript.HideTrainProperty();
                timer.Start();
            }
            else if(Physics.Raycast(ray, out RaycastHit hitinfo1, 100, LayerMask.GetMask("Train")))
            {
                Train train = hitinfo1.transform.GetComponent<Train>();
                m_PropertyPanelScript.DisplayTrainProperty(train);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 100, LayerMask.GetMask("Train")))
            {
                Train train = hitinfo.transform.GetComponent<Train>();
                train?.Line.RemoveTrain(train);
                m_PropertyPanelScript.HideTrainProperty();
            }
            else if (Physics.Raycast(ray, out RaycastHit hitinfo2, 100, LayerMask.GetMask("Floor")))
            {
                m_PropertyPanelScript.HideTrainProperty();
            }
        }
    }
    #endregion
}
