using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuContolScript : MonoBehaviour {

    public GameObject[] gameMenus;
    public GameObject statusMenu;
    public GameObject inventory;
    public GameObject listMenu;
    
    public bool[] isMenuOpen = new bool[3] { false, false, false };
    public bool isAnyMenuOpen;

    public GameObject Inventory;
    
    public GameObject[] listMenus = new GameObject[2];


    public void Start()
    {
        gameMenus = new GameObject[] { statusMenu, inventory, listMenu };
        DefaultState();       
    }

    public void DefaultState()
    {
        for(int i = 0; i < gameMenus.Length; i++)
        {
            gameMenus[i].SetActive(false);
            isMenuOpen[i] = false;
        }
        isAnyMenuOpen = false;  
    }


    public void MenuOpen(int index)
    {
        if (!isAnyMenuOpen)
        {
            gameMenus[index].SetActive(true);
            isAnyMenuOpen = true;
            isMenuOpen[index] = true;
        }
        else
        {
            if (isMenuOpen[index])
            {
                DefaultState();
            }
            else
                ;
        }
    }


    public void OpenListMenu(int index)
    {
        DefaultState();
        listMenus[index].SetActive(true);
    }




}
