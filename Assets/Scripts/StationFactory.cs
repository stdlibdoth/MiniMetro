using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationFactory : MonoBehaviour
{
    [SerializeField] private Station m_stationPrefab;
    [SerializeField] private Image m_PassengerImagePrefab;
    [SerializeField] private List<Sprite> m_Sprites;

    private static StationFactory m_Singleton;

    private void Awake()
    {
        if (m_Singleton == null)
            m_Singleton = this;
        else
            DestroyImmediate(this);
    }

    public static int StationTypeCount
    {
        get { return m_Singleton.m_Sprites.Count; }
    }

    public static Station InstatiateStation(int type)
    {
        Station station = Instantiate(m_Singleton.m_stationPrefab);
        station.Init(type);
        if (type >= 0 && type <= m_Singleton.m_Sprites.Count - 1)
            station.GetComponent<SpriteRenderer>().sprite = m_Singleton.m_Sprites[type];
        return station;
    }

    public static Image GeneratePassengerImage(int type)
    {
        Image image = Instantiate(m_Singleton.m_PassengerImagePrefab);
        image.sprite = m_Singleton.m_Sprites[type];
        return image;
    }

    public static Sprite GetPassengerSprite(int type)
    {
        return m_Singleton.m_Sprites[type];
    }
}
