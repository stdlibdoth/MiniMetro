using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Node
{
    [SerializeField] private Vector2Int m_Coord;


    public Vector2Int Coord
    {
        get { return m_Coord; }
        set { m_Coord = value; }
    }

    public Node(Vector2Int coord)
    {
        m_Coord = coord;
    }
}
