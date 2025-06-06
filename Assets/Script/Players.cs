using UnityEngine;
[System.Serializable]

public class Players
{
    public int Id;

    public string Name;
    public string Account;

    [HideInInspector]public string Password;
    public string AvatarPath;
    public string FramePath;
}
