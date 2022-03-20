using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link
{

    [SerializeField] private Vector3[] m_Pos;
    [SerializeField] private Node m_Start;
    [SerializeField] private Node m_End;

    public Vector3[] Positions
    {
        get { return m_Pos; }
    }

    public Node[] Nodes
    {
        get
        {
            return new Node[] { m_Start, m_End };
        }
    }

    public Link(Node startNode, Node endNode)
    {
        m_Start = startNode;
        m_End = endNode;
        Vector3 start = new Vector3(startNode.Coord.x, startNode.Coord.y, 0);
        Vector3 end = new Vector3(endNode.Coord.x, endNode.Coord.y, 0);
        CalculatePositions(start, end);
    }

    private void CalculatePositions(Vector3 start,Vector3 end)
    {
        if (start.x == end.x || start.y == end.y || Mathf.Abs(start.x - end.x) == Mathf.Abs(start.y- end.y))
            m_Pos = new Vector3[] { start, end };
        else
        {
            float xDist = Mathf.Abs(end.x - start.x);
            float yDist = Mathf.Abs(end.y - start.y);
            float xDir = (end.x - start.x) / xDist;
            float yDir = (end.y - start.y) / yDist;

            Vector3 increment = xDist >= yDist ? new Vector3(xDir * yDist, yDir * yDist, start.z) :
                new Vector3(xDir * xDist, yDir * xDist, start.z);

            Vector3 midNode = start + increment;

            m_Pos = new Vector3[] { start, midNode, end };
        }
    }

    public Vector3[] UpdatePositions()
    {
        Vector3 start = new Vector3(m_Start.Coord.x, m_Start.Coord.y, 0);
        Vector3 end = new Vector3(m_End.Coord.x, m_End.Coord.y, 0);
        CalculatePositions(start, end);
        return m_Pos;
    }

}
