using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LineDirection
{
    Forward,
    Reverse,
}

public class Line : MonoBehaviour
{
    private List<Node> m_Nodes;
    [ SerializeField]private List<Link> m_Links;
    private List<Station> m_Stations;
    private List<Train> m_Trains;
    private Link m_TempLink;
    private int m_Id;

    public Color color;

    [SerializeField] private Train m_TrainPrefab;
    [SerializeField] private Segment m_SegmentPrefab;
    [SerializeField] private Transform m_SegmentHolder;


    public int Id { get { return m_Id; } }
    public Node[] Nodes { get { return m_Nodes.ToArray(); } }

    public Station[] Stations { get { return m_Stations.ToArray(); } }

    public List<int> StationTypes
    {
        get
        {
            List<int> types = new List<int>();
            for (int i = 0; i < m_Stations.Count; i++)
            {
                if (!types.Contains(m_Stations[i].Type))
                {
                    types.Add(m_Stations[i].Type);
                }
            }
            return types;
        }
    }

    private void Awake()
    {
        m_Links = new List<Link>();
        m_Nodes = new List<Node>();
        m_Stations = new List<Station>();
        m_Trains = new List<Train>();
    }


    private void Update()
    {
        Draw();
    }

    public void Init(int id)
    {
        m_Id = id;
    }

    private void Draw()
    {
        int segDelta = m_Links.Count - m_SegmentHolder.childCount;
        if(segDelta>0)
        {
            for (int i = 0; i < segDelta; i++)
            {
                Segment segment = Instantiate(m_SegmentPrefab, m_SegmentHolder);
                segment.Init(this);
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(segDelta); i++)
            {
                DestroyImmediate(m_SegmentHolder.GetChild(0).gameObject);
            }
        }

        for (int i = 0; i < m_Links.Count; i++)
        {
            m_Links[i].UpdatePositions();
            Segment segment = m_SegmentHolder.GetChild(i).GetComponent<Segment>();
            segment.Draw(color, color, m_Links[i].Positions);
        }
        if(m_TempLink != null)
        {
            Segment segment = Instantiate(m_SegmentPrefab, m_SegmentHolder);
            segment.Init(this);
            segment.Draw(color, color, m_TempLink.Positions);
        }

    }

    #region Nodes Methods

    public bool Contains(Node node)
    {
        return m_Nodes.Contains(node);
    }

    public Node NextNode(Node node)
    {
        int index = m_Nodes.IndexOf(node);
        if (index < 0)
            return null;
        else if (index == (m_Nodes.Count - 1))
            return node;
        else
            return m_Nodes[index + 1];
    }

    public Node PrevNode(Node node)
    {
        int index = m_Nodes.IndexOf(node);
        if (index < 0)
            return null;
        else if (index == 0)
            return node;
        else
            return m_Nodes[index - 1];
    }

    public void InsertTempBranch(Node from, Node temp)
    {
        m_TempLink = new Link(from, temp);
    }

    public void RemoveTempBranch()
    {
        m_TempLink = null;
    }

    public void InsertNodeBetween(Node node, Node n1, Node n2)
    {
        int index1 = m_Nodes.IndexOf(n1);
        int index2 = m_Nodes.IndexOf(n2);
        if (index1 < 0 || index2 < 0 || index1 == index2)
            return;

        int l = index1 < index2 ? index1 : index2;
        int r = index1 > index2 ? index1 : index2;

        m_Nodes.Insert(r, node);

        m_Links.RemoveAt(l);
        m_Links.Insert(l, new Link(m_Nodes[l], node));
        m_Links.Insert(l+1, new Link(node, m_Nodes[r + 1]));
        print("insert");
    }

    public void Replace(Node start, Node end)
    {
        int index1 = m_Nodes.IndexOf(start);
        int index2 = m_Nodes.IndexOf(end);

        if (Mathf.Abs(index1 - index2) > 1)
            return;

        int index = index1 < index2 ? index1 : index2;
        m_Links.RemoveAt(index);
        m_Links.Insert(index, new Link(start, end));

    }

    public void ExtendHead(Node node,bool start = false)
    {
        Link link;
        if (start)
            link = new Link(node, m_Nodes[0]);
        else
            link = new Link(m_Nodes[0], node);
        m_Nodes.Insert(0, node);
        m_Links.Insert(0, link);
        print("extend");
    }

    public void RemoveNode(Node node)
    {
        int index = m_Nodes.IndexOf(node);
        if (index < 0)
            return;

        if (m_Nodes.Count > 1)
        {
            if (index == 0)
            {
                m_Links.RemoveAt(0);
            }
            else if (index == (m_Nodes.Count - 1))
            {
                m_Links.RemoveAt(index - 1);
            }
            else
            {
                m_Links.RemoveAt(index);
                m_Links.RemoveAt(index - 1);
                m_Links.Insert(index - 1, new Link(m_Nodes[index - 1], m_Nodes[index + 1]));
            }
        }
        m_Nodes.RemoveAt(index);
        print("remove");
    }

    public void AppendTail(Node node, bool start = false)
    {
        if(m_Nodes.Count>=1)
        {
            Link link;
            if (start)
                link = new Link(node, m_Nodes[m_Nodes.Count - 1]);
            else
                link = new Link(m_Nodes[m_Nodes.Count - 1], node);
            m_Links.Add(link);
        }
        m_Nodes.Add(node);
        print("Append");
    }

    #endregion

    #region Station Methods and Handlers


    private void TrainEnterStationHandler(Train train)
    {
        if(!train.spawnFlag)
        {
            Link location = train.Location;
            int dir = train.Direction;
            Link next;
            int linkIndex = m_Links.IndexOf(location);
            if(dir == 1)
            {
                if (linkIndex == m_Links.Count - 1)
                {
                    next = location;
                    train.Direction = -1;
                }
                else
                    next = m_Links[linkIndex + 1];
            }
            else
            {
                if (linkIndex == 0)
                {
                    next = location;
                    train.Direction = 1;
                }
                else
                    next = m_Links[linkIndex - 1];
            }
            train.SetWayPoint(next, OrderedPositions(next, train.Direction));
        }
    }

    public void AddStation(Station station)
    {
        if (!m_Stations.Contains(station))
        {
            m_Stations.Add(station);
            station.GetEnterStaionEvent(this).AddListener(TrainEnterStationHandler);
        }
    }

    public void RemoveStation(Station station)
    {
        if (m_Trains.Count > 0)
            return;

        if (m_Stations.Contains(station))
        {
            m_Stations.Remove(station);
            RemoveNode(station.Node);
            station.Lines.Remove(this);
            station.GetEnterStaionEvent(this).RemoveListener(TrainEnterStationHandler);
            if (station.Lines.Count == 0)
                Destroy(station.gameObject);
        }
        //if(m_Stations.Count<2 && m_Stations.Count>0)
        //{
        //    m_Stations[0].GetEnterStaionEvent(this).RemoveListener(TrainEnterStationHandler);
        //    m_Stations[0].Lines.Remove(this);
        //    for (int i = 0; i < m_Trains.Count; i++)
        //    {
        //        Destroy(m_Trains[0].gameObject);
        //    }
        //    m_Trains.Clear();
        //}

    }

    public Station GetStation(Node node)
    {
        for (int i = 0; i < m_Stations.Count; i++)
        {
            if (m_Stations[i].Node == node)
                return m_Stations[i];
        }
        return null;
    }

    #endregion

    #region Train

    public Train SpawnTrain()
    {
        for (int i = 0; i < m_Trains.Count; i++)
        {
            if (m_Trains[i].Location == m_Links[0])
                return null;
        }
        Train train = Instantiate(m_TrainPrefab);
        train.Init(this);

        Vector3[] pos;
        if (m_Links[0].Nodes[0] == m_Nodes[0])
            pos = m_Links[0].Positions;
        else
        {
            int length = m_Links[0].Positions.Length;
            pos = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                pos[i] = m_Links[0].Positions[length - 1 - i];
            }
        }
        m_Trains.Add(train);
        train.SetWayPoint(m_Links[0],pos);
        train.transform.position = pos[0];
        train.spawnFlag = true;
        return train;
    }

    public void RemoveTrain(Train train)
    {
        m_Trains.Remove(train);
        Destroy(train.gameObject);
    }

    #endregion

    #region Helpers


    private Vector3[] OrderedPositions(Link link, int direction)
    {
        Vector3[] pos = link.Positions;
        Vector3[] orderedPos = new Vector3[pos.Length];

        int i1 = m_Nodes.IndexOf(link.Nodes[0]);
        int i2 = m_Nodes.IndexOf(link.Nodes[1]);

        if (i2 - i1 == direction)
            orderedPos = link.Positions;
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                orderedPos[i] = link.Positions[pos.Length - 1 - i];
            }
        }

        return orderedPos;
    }
    #endregion
}
