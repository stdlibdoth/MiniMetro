using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] private MeshCollider m_MeshCollider;
    [SerializeField] private LineRenderer m_LineRenderer;


    private Mesh m_Mesh;
    private Line m_Line;

    public Line Line { get { return m_Line; } }

    private void Awake()
    {
        m_Mesh = new Mesh();
    }
    public void Init(Line line)
    {
        m_Line = line;
    }

    public void Draw(Color start_color, Color end_color, Vector3[] pos)
    {
        m_LineRenderer.positionCount = pos.Length;
        m_LineRenderer.SetPositions(pos);
        m_LineRenderer.startColor = start_color;
        m_LineRenderer.endColor = end_color;
        UpdateCollider();
    }


    private void UpdateCollider()
    {
        m_LineRenderer.BakeMesh(m_Mesh);
        m_MeshCollider.sharedMesh = m_Mesh;
    }
}
