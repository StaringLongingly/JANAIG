using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public bool isFrozen = true;
    public float FrozenTime = 1.0f;

    private string previousname = "";
    private string currentname = "";

    private void OnTriggerEnter(Collider other)
    {
        if (!isFrozen)
        {
            StartCoroutine(waiter()); 
            previousname = GetParentName(other);
        }
        else
        {
            currentname = GetParentName(other);
            if (previousname != currentname)
            {
                previousname = currentname;
                currentname = GetParentName(other);
            }
            else 
            {
                isFrozen = false;
                previousname = "";
                currentname = "";
                Throw();
            }
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(FrozenTime);
        isFrozen = false; 
        Throw();
    }

    void Throw()
    {
        Debug.Log("Throw!");
    }

    string GetParentName(Collider other)
    {
        Transform parentTransform = other.transform.parent;
        if (parentTransform != null)
        {
            return parentTransform.gameObject.name;
        }
        return "";
    }
}
