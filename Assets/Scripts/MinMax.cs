using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{

    private float m_fMax;
    private float m_fMin;


    public float Min
    {
        get { return m_fMin; }
    }

    public float Max
    {
        get { return m_fMax; }
    }

    public MinMax(Vector2 v)
    {
        m_fMax = v.x > v.y ? v.x : v.y;
        m_fMin = v.x < v.y ? v.x : v.y;
    }

    public MinMax(float f1, float f2)
    {
        m_fMax = f1 > f2 ? f1 : f2;
        m_fMin = f1 < f2 ? f1 : f2;
    }
}
