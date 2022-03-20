using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LineToggleBtnScript : MonoBehaviour
{
    [SerializeField] private Toggle m_Toggle;
    [SerializeField] private Image m_BGImage;
    [SerializeField] private Image m_CheckMarkImage;

    [SerializeField]private int m_Line;


    public int LineId { get { return m_Line; } }

    public void Init(int line, Color color)
    {
        m_BGImage.color = color;
        m_CheckMarkImage.color = color;
        m_Line = line;
    }
}
