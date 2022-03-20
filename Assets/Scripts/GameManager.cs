using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Singleton;
    [SerializeField] private int m_TargetFPS;
    [SerializeField] private int m_FramesPerTimeUnit;

    [SerializeField] private GridManager m_GridManager;

    public static int thumbsUp;
    public static int thumbsDown;
    public static MetroGrid Grid
    {
        get { return m_Singleton.m_GridManager.CurrentGrid; }
    }

    public static int FramesPerTimeUnit { get { return m_Singleton.m_FramesPerTimeUnit; } }

    private void Awake()
    {
        if (m_Singleton == null)
        {
            Application.targetFrameRate = m_TargetFPS;
            m_Singleton = this;
        }
        else
            DestroyImmediate(this);
    }
}
