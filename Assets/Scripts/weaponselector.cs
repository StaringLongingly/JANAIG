using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    // Index of the currently selected weapon
    [Header("Weapon Resize")]
    [SerializeField] public float previousWeaponSize;
    [SerializeField] public float maxWeaponSize = 4;
    [SerializeField] public float desiredWeaponSize, lerpedWeaponSize, resizeDuration = 0.5f;
    [SerializeField] public bool isLeftController, startResize;


    // Health and Speed Scalars
    [Header("Scalars")]
    [SerializeField] private float HPScalar = 0.25f;
    [SerializeField] private float SPScalar = 0.25f;

    [Header("Objects")]
    public GameObject weaponBlade;
    public GameObject weaponTip;
    public CapsuleCollider hurtBox;


    // Current Hit HP and SP properties accessible from other scripts
    public float HitHP { get; private set; }
    public float HitSP { get; private set; }
    public float unscaledHitHP = 0;
    public float unscaledHitSP = 0;

    private void Update() { if (startResize) { startResize = false; SwitchWeapon(); } }

    private void Start()
    {
        // Activate the GameObject for the selected weapon3
        SwitchWeapon();
    }

    public void SwitchWeapon()
    {
        if (isLeftController) { desiredWeaponSize = PlayerPrefs.GetFloat("weaponSizeLeft"); }
        else { desiredWeaponSize = PlayerPrefs.GetFloat("weaponSizeRight"); }

        if ( desiredWeaponSize > maxWeaponSize ) { desiredWeaponSize = maxWeaponSize; }
        else if ( desiredWeaponSize < 0 ) { desiredWeaponSize = 0; }

        SetHitValues();
        StopAllCoroutines();
        StartCoroutine(Resize());
    }

    // Set Hit HP and SP values based on the weapon index
    private void SetHitValues()
    {
        // Vector X is HP, Y is SP
        Vector2 minSize = new Vector2(100f, 25f);
        Vector2 maxSize = new Vector2(25f, 100f);

        Vector2 lerpedHPSP = Vector2.Lerp(minSize, maxSize, desiredWeaponSize / maxWeaponSize);
        (HitHP, HitSP) = (lerpedHPSP.x, lerpedHPSP.y);

        unscaledHitHP = HitHP;
        unscaledHitSP = HitSP;

        HitHP *= HPScalar;
        HitSP *= SPScalar;
    }

    private IEnumerator Resize()
    {
        Debug.Log("Weapons Resizing!");

        float previousWeaponSize = lerpedWeaponSize;
        float lerpProgress = 0f;

        while ( lerpProgress < 1 )
        {
            lerpProgress += Time.deltaTime * (1 / resizeDuration);
            lerpedWeaponSize = Mathf.Lerp(previousWeaponSize, desiredWeaponSize, lerpProgress);

            hurtBox.height = 4.6f + lerpedWeaponSize * 2;
            hurtBox.center = new Vector3(0, 3.6f + lerpedWeaponSize, 0);

            weaponTip.transform.localPosition =  new Vector3(0, 5.6f + lerpedWeaponSize * 2, 0);

            weaponBlade.transform.localScale = new Vector3(0.45f, 2f + lerpedWeaponSize, 0.45f);
            weaponBlade.transform.localPosition = new Vector3(0, 3.6f + lerpedWeaponSize, 0);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
