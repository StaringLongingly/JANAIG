using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageSetter : MonoBehaviour
{
    [Range(0, 1)]
    public float lengthIncrease = 0f;

    [Header("Texts:")]
    public TextMeshPro damageText;
    public TextMeshPro speedText;

    [Header("Sign Identifier")]
    public bool isNegativeLength = false;

    public void OnTriggerEnter(Collider swordCollider)
    {
        if (swordCollider.gameObject.CompareTag("Weapon"))
        {
            float signedLengthIncrease = lengthIncrease;
            if (isNegativeLength) { signedLengthIncrease *= -1; }

            WeaponSelector weaponSelector = swordCollider.gameObject.transform.parent.GetComponent<WeaponSelector>();

            string weaponToChange = "weaponSize";
            if (weaponSelector.isLeftController) 
            { 
                weaponToChange += "Left";
            }
            else
            {
                weaponToChange += "Right";
            }

            float newWeaponLength = PlayerPrefs.GetFloat(weaponToChange) + signedLengthIncrease;

            if ( newWeaponLength > weaponSelector.maxWeaponSize ) { newWeaponLength = weaponSelector.maxWeaponSize; }
            else if ( newWeaponLength < 0 ) { newWeaponLength = 0; }
            
            PlayerPrefs.SetFloat(weaponToChange, newWeaponLength);

            weaponSelector.SwitchWeapon();

            Debug.Log("HP and SP: " + weaponSelector.HitHP + " " + weaponSelector.HitSP);
            damageText.text = "DAMAGE         " + weaponSelector.HitHP;
            speedText.text = "SPEED          " + weaponSelector.HitSP;

            //gameObject.GetComponent<MoveAndGrowBullet>().GrowBulletFunction(true);
        }
    }
}
