using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCamController : MonoBehaviour
{
    [SerializeField] private Camera m_Cam;
    [SerializeField] private MetroGrid m_Grid;


    [SerializeField] private float m_ZoomSpeed;
    [SerializeField] private float m_PanSpeed;

    private Vector3 m_MousePos;
    private bool m_PanFlag;

    private void Update()
    {
        

        if(Input.mouseScrollDelta!= Vector2.zero)
        {
            UpdateZoom();
        }


        if (Input.GetMouseButtonDown(2) && !m_PanFlag)
        {
            m_PanFlag = true;
            m_MousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2) && m_PanFlag)
        {
            UpdatePan();
        }
        else if (Input.GetMouseButtonUp(2) && m_PanFlag)
            m_PanFlag = false;

        ClampCam();
    }




    private void UpdateZoom()
    {
        float targetSize = m_Cam.orthographicSize;

        float camSizeDelta = -m_ZoomSpeed * Input.mouseScrollDelta.y;
        m_Cam.orthographicSize = m_Cam.orthographicSize + camSizeDelta;

    }

    private void UpdatePan()
    {

        Vector3 dir = Input.mousePosition - m_MousePos;
        dir = new Vector3(dir.x, dir.y).normalized;

        Vector3 targetPos = m_Cam.transform.position - dir * m_PanSpeed* Time.deltaTime;

        m_Cam.transform.position = targetPos;

        m_MousePos = Input.mousePosition;
    }

    private void ClampCam()
    {
        float minGridExtent = new MinMax(m_Grid.GridData.halfDimension).Min;
        m_Cam.orthographicSize = Mathf.Clamp(m_Cam.orthographicSize, 0.5f * minGridExtent, minGridExtent);


        float upper = m_Grid.GridData.halfDimension.y - m_Cam.orthographicSize;
        float lower = -m_Grid.GridData.halfDimension.y + m_Cam.orthographicSize;
        float left = -m_Grid.GridData.halfDimension.x + m_Cam.orthographicSize * m_Cam.aspect;
        float right = m_Grid.GridData.halfDimension.x - m_Cam.orthographicSize * m_Cam.aspect;

        float x = Mathf.Clamp(m_Cam.transform.position.x, left, right);
        float y = Mathf.Clamp(m_Cam.transform.position.y, lower, upper);
        m_Cam.transform.position = new Vector3(x, y, m_Cam.transform.position.z);

    }
}
