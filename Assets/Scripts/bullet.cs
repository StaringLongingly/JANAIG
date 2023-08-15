using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public bool debug = false;
    public bool debug_switch = false;

    public Rigidbody rb;
    
    public bool startthrow = false; void Update() { if (startthrow) { startthrow = false; StartCoroutine(Throw());
 } }

    public float ThrowVariationX = 10f;
    public float ThrowVariationY = 20f;
    public float ThrowVariationZ = 40f;

    public float HPScalar = 1f;

    public float CurrentSP = 1f;
    public float CurrentHP = 1f;

    public bool IsFrozen = true;
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
                previousName = "";
                currentName = "";
                StartCoroutine(Throw());

            }
        }
    }

    private IEnumerator Waiter()
    {
        yield return new WaitForSeconds(FrozenTime);
        StartCoroutine(Throw());

    }

    private IEnumerator Throw()
    {
        IsFrozen = false;

        if (CurrentSP > 400) { CurrentSP = 400; }
        float ThrowDuration = -0.005f * CurrentSP + 3f;

        float elapsedTime = 0.0f;

        Vector3 startPos = new Vector3 (-10f, 0f, 0f);
        Vector3 endPos = new Vector3 (10f, 0f, 0f);
        if (debug)
        {
            debug_switch = !debug_switch;

            if (debug_switch) { startPos *= -1; endPos *= -1; }
        }
        //startPos = gameObject.transform.position;
        Vector3 controlPos = new Vector3
        (
            Random.Range(-ThrowVariationX, ThrowVariationX),
            Random.Range(0, ThrowVariationY),
            Random.Range(-ThrowVariationZ, ThrowVariationZ)
        );

        Debug.Log("Throw at " + controlPos);

        while (elapsedTime < ThrowDuration && !IsFrozen)
        {
            float t = elapsedTime / ThrowDuration;

            // Quadratic Bezier curve formula
            Vector3 newPos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPos + Mathf.Pow(t, 2) * endPos;

            GetComponent<Rigidbody>().MovePosition(newPos);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("Miss");
        if (debug) { StartCoroutine(Throw()); }
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
