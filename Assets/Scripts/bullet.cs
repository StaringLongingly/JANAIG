using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float CurrentSP { get; private set; }
    public float CurrentHP { get; private set; }

    public bool IsFrozen { get; private set; } = true;
    public float FrozenTime = 1.0f;

    private string previousName = "";
    private string currentName = "";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsFrozen)
        {
            StartCoroutine(Waiter());
            previousName = GetParentName(other);
        }
        else
        {
            currentName = GetParentName(other);
            if (previousName != currentName)
            {
                previousName = currentName;
                currentName = GetParentName(other);

                (float hp, float sp) = GetHPAndSP(other);
                CurrentHP += hp;
                CurrentSP += sp;
            }
            else
            {
                IsFrozen = false;
                previousName = "";
                currentName = "";
                Throw();
            }
        }
    }

    private IEnumerator Waiter()
    {
        yield return new WaitForSeconds(FrozenTime);
        IsFrozen = false;
        Throw();
    }

    private void Throw()
    {
        Debug.Log("Throw!");
    }

    private string GetParentName(Collider other)
    {
        Transform parentTransform = other.transform.parent;
        return parentTransform != null ? parentTransform.gameObject.name : "";
    }

    private (float, float) GetHPAndSP(Collider other)
    {
        WeaponSelector wp = transform.parent.GetComponent<WeaponSelector>();
        return wp != null ? (wp.HitHP, wp.HitSP) : (0f, 0f);
    }
}
