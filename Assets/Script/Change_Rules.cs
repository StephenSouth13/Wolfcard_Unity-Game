using UnityEngine;
using UnityEngine.UI;

public class Change_Rules : MonoBehaviour
{
    public GameObject Rules_Image;
    public GameObject RulesWin_Image;

    public void RulesWin()
    {
        Rules_Image.SetActive(false);
        RulesWin_Image.SetActive(true);
    }
    public void backRules()
    {
        Rules_Image.SetActive(true);
        RulesWin_Image.SetActive(false);
    }

}
