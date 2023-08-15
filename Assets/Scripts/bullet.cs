using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Debug")]
    // Debug
    public bool debug = false;
    public bool debugSwitch = false;
    public bool startthrow = false; void Update() { if (startthrow) { startthrow = false; StartCoroutine(Throw()); } }

    [Header("Throw Settings")]
    // Throw variation settings
    public float throwVariationX = 5f;
    public float throwVariationY = 10f;
    public float throwVariationZ = 15f;

    [Header("Health and Speed")]
    // Scalar for adjusting health value
    public float hpScalar = 1f;
    // Current speed and health of the bullet
    public float currentSP = 1f;
    public float currentHP = 1f;

    [Header("Freeze Settings")]
    // Flag to determine if the bullet is frozen
    public bool isFrozen = true;
    // Duration of freezing time
    public float frozenTime = 1.0f;

    // Store the name of the previously collided object
    private string previousCollidedObjectName = "";

    private Rigidbody rb;

    private void Awake()
    {
        // Get the Rigidbody component of the bullet
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFrozen)
        {
            // Wait for frozen time before throwing again
            StartCoroutine(WaitThenThrow());
            // Store the name of the collided object
            previousCollidedObjectName = GetParentName(other);
        }
        else
        {
            // Get the name of the currently collided object
            string currentCollidedObjectName = GetParentName(other);
            if (previousCollidedObjectName != currentCollidedObjectName)
            {
                // Update previous object name and adjust health and speed
                previousCollidedObjectName = currentCollidedObjectName;

                (float hp, float speed) = GetHPAndSpeed(other);
                currentHP += hp;
                currentSP += speed;
            }
            else
            {
                // Reset previous object name and initiate throw
                previousCollidedObjectName = "";
                StartCoroutine(Throw());
            }
        }
    }

    private IEnumerator WaitThenThrow()
    {
        // Wait for frozen time before initiating throw
        yield return new WaitForSeconds(frozenTime);
        StartCoroutine(Throw());
    }

    private IEnumerator Throw()
    {
        isFrozen = false;

        // Limit the maximum speed
        if (currentSP > 400) { currentSP = 400; }
        // Calculate throw duration based on current speed
        float throwDuration = -0.005f * currentSP + 3f;

        // Define throw trajectory points
        Vector3 startPos = new Vector3(-10f, 0f, 0f);
        Vector3 endPos = new Vector3(10f, 0f, 0f);

        if (debug)
        {
            // Modify trajectory points for debugging
            debugSwitch = !debugSwitch;
            if (debugSwitch) { startPos *= -1; endPos *= -1; }
        }

        // Define control point for Bezier curve
        Vector3 controlPos = new Vector3(
            Random.Range(-throwVariationX, throwVariationX),
            Random.Range(0, throwVariationY),
            Random.Range(-throwVariationZ, throwVariationZ)
        );

        Debug.Log("Throw at " + controlPos);

        float elapsedTime = 0.0f;
        while (elapsedTime < throwDuration && !isFrozen)
        {
            float t = elapsedTime / throwDuration;

            // Calculate new position using Quadratic Bezier curve formula
            Vector3 newPos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPos + Mathf.Pow(t, 2) * endPos;

            // Move the bullet to the new position
            rb.MovePosition(newPos);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Miss");
        if (debug)
        {
            // Initiate throw for debugging
            StartCoroutine(Throw());
        }
    }

    private string GetParentName(Collider other)
    {
        // Get the name of the parent object if it exists
        Transform parentTransform = other.transform.parent;
        return parentTransform != null ? parentTransform.gameObject.name : "";
    }

    private (float, float) GetHPAndSpeed(Collider other)
    {
        // Get health and speed values from the collided object's WeaponSelector component
        WeaponSelector wp = transform.parent.GetComponent<WeaponSelector>();
        return wp != null ? (wp.HitHP, wp.HitSP) : (0f, 0f);
    }
}
