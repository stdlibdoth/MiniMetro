using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MetroGrid: MonoBehaviour
{
    [SerializeField] private GridData m_GridData;

    [SerializeField] private Transform m_Floor;
    [SerializeField] private DecalProjector m_DecalProjector;

    private Vector2 m_OriginPos;

    public GridData GridData
    {
        get { return m_GridData; }
    }


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_OriginPos = gameObject.transform.position;
        Vector2 size = new Vector2(m_GridData.halfDimension.x * 0.2f * m_GridData.cellSize,
            m_GridData.halfDimension.y * 0.2f * m_GridData.cellSize);
        float minDepth = Mathf.Abs(m_DecalProjector.transform.position.z - m_Floor.position.z);

        m_Floor.transform.localScale = new Vector3(size.x, 1, size.y);
        m_DecalProjector.uvScale = new Vector2(size.x * 10, size.y * 10);
        m_DecalProjector.size = new Vector3(size.x * 10, size.y * 10, minDepth + 2);
        m_DecalProjector.pivot = new Vector3(m_Floor.position.x, m_Floor.position.y, 0.5f * (minDepth + 2));
        m_Floor.gameObject.SetActive(true);
        m_DecalProjector.gameObject.SetActive(true);
    }


    public Vector2Int SnapNearest(Vector2 pos)
    {
        Vector2 position = pos - m_OriginPos;
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return new Vector2Int(x, y);
    }
}
