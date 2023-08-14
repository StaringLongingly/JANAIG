using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public Rigidbody rb;
    public bool positivex = true;
    
    public bool startthrow = false; void Update() { if (startthrow) { startthrow = false; Throw(); } }

    public float SPScalar = 2f;
    public float HPScalar = 1f;

    public float CurrentSP = 1f;
    public float CurrentHP = 1f;

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
        rb.velocity = Vector3.zero;
        positivex = !positivex;
        Vector3 direction = new Vector3
        (
            positivex ? 1 : -1, // X component between -1 and 1
            Random.Range(0, 0.5f), // Y component between -1 and 1
            Random.Range(-4f, 4f)  // Z component between -1 and 1
        );
        rb.AddForce(direction * CurrentSP * SPScalar, ForceMode.Acceleration);
        Debug.Log("Throw to " + direction);
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
