using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<Players> AllPlayers = new List<Players>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
