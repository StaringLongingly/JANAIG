using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsArray;
    [SerializeField] private int selectedWeapon = 0;

    [SerializeField] private float daggerHP = 100f;
    [SerializeField] private float daggerSP = 25f;

    [SerializeField] private float katanaHP = 50f;
    [SerializeField] private float katanaSP = 75f;

    [SerializeField] private float longswordHP = 75f;
    [SerializeField] private float longswordSP = 50f;

    [SerializeField] private float greatswordHP = 25f;
    [SerializeField] private float greatswordSP = 100f;

    public float HitHP { get; private set; }
    public float HitSP { get; private set; }

    private void Start()
    {
        objectsArray[selectedWeapon].SetActive(true);
        SetHitValues(selectedWeapon);
    }

    private void SetHitValues(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0: HitHP = daggerHP; HitSP = daggerSP; break;
            case 1: HitHP = katanaHP; HitSP = katanaSP; break;
            case 2: HitHP = longswordHP; HitSP = longswordSP; break;
            case 3: HitHP = greatswordHP; HitSP = greatswordSP; break;
        }
    }
}