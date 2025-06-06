using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCard : MonoBehaviour
{

    public GameObject GroupInfor;

    HashSet<GameObject> AllCard;
    void Start()
    {
        AllCard = new HashSet<GameObject>();
        foreach (Transform Child in GroupInfor.transform)
        {
            AllCard.Add(Child.gameObject);
        }
    }
    void DeactivateAllCards()
    {
        foreach(GameObject card in AllCard)
        {
            if(card != null)
            {
                card.SetActive(false);
            }
        }
    }
    void ShowCard(GameObject Card)
    {
        DeactivateAllCards();
        GroupInfor.SetActive(true);
        Card.SetActive(true);
    }
    #region villager
    public GameObject Villager;
    public GameObject VillagerChief;
    public GameObject Guardian;
    public GameObject Witch;
    public GameObject Prophet;
    public GameObject Hunter;
    public GameObject CupidGod;
    public GameObject Patients;
    public GameObject Thievies;
    public GameObject Substitutes;
    public GameObject TraitorVillager;

    public void OpenVillager()
    {
        ShowCard(Villager);
    }
    public void OpenVillagerChief()
    {
        ShowCard(VillagerChief);
    }
    public void OpenGuardian()
    {
        ShowCard(Guardian);
    }
    public void OpenWitch()
    {
        ShowCard(Witch);
    }
    public void OpenProphet()
    {
        ShowCard(Prophet);
    }
    public void OpenHunter()
    {
        ShowCard(Hunter);
    }
    public void OpenCupidGod()
    {
        ShowCard(CupidGod);
    }
    public void OpenPatients()
    {
        ShowCard(Patients);
    }
    public void OpenThieves()
    {
        ShowCard(Thievies);
    }
    public void OpenSubstitutes()
    {
        ShowCard(Substitutes);
    }
    public void OpenTraitorVillager()
    {
        ShowCard(TraitorVillager);
    }
    #endregion
    #region Wolf
    public GameObject NormalWolf;
    public GameObject WolfLeader;
    public GameObject WolfCup;
    public GameObject WolfDisease;
    public GameObject TraitorWolf;
    public void OpenNormalWolf()
    {
        ShowCard(NormalWolf);
    }
    public void OpenWolfLeader()
    {
        ShowCard (WolfLeader);
    }
    public void OpenWolfCup()
    {
        ShowCard(WolfCup);
    }
    public void OpenWolfDisease()
    {
        ShowCard(WolfDisease);
    }
    public void OpenTraitorWolf()
    {
        ShowCard(TraitorWolf);
    }


    #endregion
    #region SoLo
    public GameObject TraitorSolo;
    public GameObject SoloFlutePlayer;
    public GameObject SoloFrustratedPerson;
    public GameObject SoloCloneMirror;
    public void OpenTraitorSolo()
    {
        ShowCard(TraitorSolo);
    }
    public void OpenSoloFlutePlayer()
    {
        ShowCard(SoloFlutePlayer);
    }
    public void OpenSoloFrustratedPerson()
    {
        ShowCard(SoloFrustratedPerson);
    }
    public void OpenSoloCloneMirror()
    {
        ShowCard(SoloCloneMirror);
    }

    #endregion

}
