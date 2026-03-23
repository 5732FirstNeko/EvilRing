using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static UIManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(UIManager).Name);
                Instance = Object.AddComponent<UIManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    

    public FriendlyUnitUI choiceFriendlyUnitCard;

    private void Awake()
    {
        choiceFriendlyUnitCard = null;
    }

    //TODO : On UnitDataUI instance ,only ChangeData when UnitDataUI was dsplayed
    public void UnitDataDisplay(Unit unit)
    {
        //TODO : UnitDataUI Display Logic
    }

    public void UnitDataUnDisPlay()
    {
        //TODO : UnitDataUI UnDisPlay Logic
    }
}
