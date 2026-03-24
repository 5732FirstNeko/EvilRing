using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(GameManager).Name);
                Instance = Object.AddComponent<GameManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


}
