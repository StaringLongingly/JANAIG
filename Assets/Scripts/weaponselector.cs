using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponselector : MonoBehaviour
{
    public GameObject[] objectsArray;
    public int SelectedWeapon = 0;

    public float HP = 0f;
    public float SP = 0f;

    public float DaggerHP = 100f;
    public float DaggerSP = 25f;

    public float KatanaHP = 50f;
    public float KatanaSP = 75f;

    public float LongswordHP = 75f;
    public float LongswordSP = 50f;

    public float GreatswordHP = 25f;
    public float GreatswordSP = 100f;

    void Start()
    {
        objectsArray[SelectedWeapon].SetActive(true);
        switch (SelectedWeapon)
        {   
            // dagger
            case 0: HP = DaggerHP; SP = DaggerSP; break;
            // katana
            case 1: HP = KatanaHP; SP = KatanaSP; break;
            // Longsword
            case 2: HP = LongswordHP; SP = LongswordSP; break;
            // Greatsword
            case 3: HP = GreatswordHP; SP = GreatswordSP; break;
        }
    }
}