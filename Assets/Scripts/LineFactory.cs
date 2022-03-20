using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFactory : MonoBehaviour
{
    [SerializeField] private Line m_LinePrefab;

    private List<Color> m_LineColors;
    private int m_LineCounter;
    private Dictionary<int,Line> m_Lines;

    private void Awake()
    {
        m_LineColors = new List<Color>();
        m_Lines = new Dictionary<int, Line>();
    }

    public void RegistorLineColor(Color color)
    {
        m_LineColors.Add(color);
    }

    public Line InstatiateLine(Color color)
    {
        Line line = Instantiate(m_LinePrefab);
        line.Init(m_LineCounter);
        m_Lines[m_LineCounter] = line;
        m_LineCounter++;
        if (m_LineColors.Contains(color))
            line.color = color;
        return line;
    }

    public Line GetLine(int id)
    {
        if(m_Lines.ContainsKey(id))
            return m_Lines[id];
        return null;
    }
}
