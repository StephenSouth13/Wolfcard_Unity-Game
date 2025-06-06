using System.Collections.Generic;
using System;
using UnityEngine;
[System.Serializable]
public class Character
{
    public string Name;
    public Dictionary<Sprite, string> SpriteCharacter = new Dictionary<Sprite, string>();
    public Dictionary<string, Action> ActionCharacter = new Dictionary<string, Action>();
    
    public Character(string Name, Dictionary<Sprite, string> SpC , Dictionary<string ,Action> AcC)
    {
        this.Name = Name;
        this.SpriteCharacter = SpC;
        this.ActionCharacter = AcC;
    }
    // vẫn chưa dùng tới 
}
