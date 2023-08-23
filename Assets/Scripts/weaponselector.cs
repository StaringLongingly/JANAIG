using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    // Array of weapon objects to be selected
    [SerializeField] private GameObject[] objectsArray;

    // Index of the currently selected weapon
    [SerializeField] private int selectedWeapon = 0;

    // Health and Speed Scalars
    [SerializeField] private float HPScalar = 0.25f;
    [SerializeField] private float SPScalar = 0.25f;

    // Health and Speed Points (HP and SP) for different weapons
    [SerializeField] private float daggerHP = 100f;
    [SerializeField] private float daggerSP = 25f;

    [SerializeField] private float katanaHP = 50f;
    [SerializeField] private float katanaSP = 75f;

    [SerializeField] private float longswordHP = 75f;
    [SerializeField] private float longswordSP = 50f;

    [SerializeField] private float greatswordHP = 25f;
    [SerializeField] private float greatswordSP = 100f;

    // Current Hit HP and SP properties accessible from other scripts
    public float HitHP { get; private set; }
    public float HitSP { get; private set; }

    private void Start()
    {
        // Activate the GameObject for the selected weapon
        objectsArray[selectedWeapon].SetActive(true);

        // Set Hit HP and SP values for the selected weapon
        SetHitValues(selectedWeapon);
    }

    // Set Hit HP and SP values based on the weapon index
    private void SetHitValues(int weaponIndex)
    {
        switch (weaponIndex)
        {
            case 0: HitHP = daggerHP; HitSP = daggerSP; break;
            case 1: HitHP = katanaHP; HitSP = katanaSP; break;
            case 2: HitHP = longswordHP; HitSP = longswordSP; break;
            case 3: HitHP = greatswordHP; HitSP = greatswordSP; break;
            default: HitHP = 0f; HitSP = 0f; break; // Handle unexpected index
        }

        HitHP *= HPScalar;
        HitSP *= SPScalar;
    }
}
