using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private MetroGrid m_Grid;

    public MetroGrid CurrentGrid
    {
        get { return m_Grid; }
    }
}
