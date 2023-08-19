using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Debug")]
    // Debug
    public bool debug = false;
    public bool debugSwitch = false;

    [Header("Function Debug")]
    // Function Debugging
    public bool startthrow = false;
    public bool startcolorchange = false;
    void Update() 
    {
        if (debug)
        {
            if (startthrow) { startthrow = false; Debug.Log("Debug Throw Start!"); RestartThrow(); } 
            if (startcolorchange) { startcolorchange = false; Debug.Log("Debug Color Change Start!"); ChangeColor(); }
        }
    }

    [Header("Dummy Mode")]
    public bool dummy_mode = true;
    public bool dummy_turn = true;
    public float dummy_wait_time = 0f;


    [Header("Throw Settings")]
    // Throw variation settings
    public float throwVariationX = 5f;
    public float throwVariationY = 10f;
    public float throwVariationZ = 15f;

    [Header("Health and Speed")]
    // Current speed and health of the bullet
    public float currentSP = 0f;
    public float currentHP = 0f;
    public Material bulletMaterial;
    public Material WatchMaterial;
    public bool SeparateColorMode = false;

    [Header("Freeze Settings")]
    // Flag to determine if the bullet is frozen
    public bool isFrozen = false;
    // Duration of freezing time
    public float frozenTime = 1.0f;

    // Store the name of the previously collided object
    private string previousCollidedObjectName = "";

    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        // Get the Rigidbody component of the bullet
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        ChangeColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        ChangeColor();

        if (!isFrozen)
        {
            isFrozen = true;
            if (dummy_mode) { dummy_turn = false; }
            // Wait for frozen time before throwing again
            Debug.Log("First Hit1");
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
                //Debug.Log("Speed Add!");
                currentSP += speed;
            }
            else
            {
                // Reset previous object name and initiate throw
                previousCollidedObjectName = "";
                RestartThrow();
            }
        }
    }

    private IEnumerator WaitThenThrow()
    {
        Debug.Log("Waiter Activated!");
        // Wait for frozen time before initiating throw
        yield return new WaitForSeconds(frozenTime);
        Debug.Log("Waiter Finished");
        if (dummy_mode) { dummy_turn = false; }
        RestartThrow();
    }

    private IEnumerator Throw()
    {
        if ( dummy_turn && dummy_mode )
        { 
            trailRenderer.enabled = false;
            rb.MovePosition(new Vector3(-10f,1f,0f));
            yield return new WaitForSeconds(dummy_wait_time); 
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }

        isFrozen = false;

        // Calculate throw duration based on current speed
        float throwDuration = -0.005f * currentSP + 3f;

        // Define throw trajectory points
        Vector3 PlayerPos = new Vector3(10f, 1.5f, 0f);
        Vector3 DummyPos = new Vector3(-10f, 1f, 0f);

        Vector3 startPos = gameObject.transform.position;
        Vector3 endPos = DummyPos;

        if (dummy_turn) //if its dummy's turn the bullet goes from the dummy to the player
        {
            startPos = DummyPos;
            endPos = PlayerPos;
        }

        // Define control point for Bezier curve
        Vector3 controlPos = new Vector3(
            Random.Range(-throwVariationX, throwVariationX),
            Random.Range(-throwVariationY, throwVariationY),
            Random.Range(-throwVariationZ, throwVariationZ)
        );

        //Debug.Log("Throw at " + controlPos);

        float elapsedTime = 0.0f;
        while (elapsedTime < throwDuration)
        {
            if (isFrozen) { StopAllCoroutines(); }

            float t = elapsedTime / throwDuration;

            // Calculate new position using Quadratic Bezier curve formula
            Vector3 newPos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPos + Mathf.Pow(t, 2) * endPos;

            // Move the bullet to the new position
            rb.MovePosition(newPos);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("Miss");

        if (dummy_mode)
        {
            currentHP = 0;
            if ( dummy_turn) { currentSP = 0; }
            dummy_turn = true;
        }
        if (dummy_mode && dummy_turn) { RestartThrow(); }

        ChangeColor();
    
    }

    private void RestartThrow()
    {
        StopAllCoroutines();
        StartCoroutine(Throw());
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
        WeaponSelector wp = other.transform.parent.GetComponent<WeaponSelector>();
        return wp != null ? (wp.HitHP, wp.HitSP) : (0f, 0f);
    }

    private void ChangeColor()
    {   
        if (currentHP > 400) { currentHP = 400; }
        if (currentSP > 400) { currentSP = 400; }
 
        WatchMaterial.SetFloat("_SP", currentSP/400);
        WatchMaterial.SetFloat("_HP", currentHP/400);

        if (currentSP == 0 && currentHP == 0)
        {
            Color White = new Color(1.0f, 1.0f, 1.0f);
            bulletMaterial.SetColor("_NoiseColor", White);
            bulletMaterial.SetColor("_OutlineColor", White);
        }
        else
        {
            if (SeparateColorMode)
            {
                Color NoiseColor = new Color(currentHP/400, 0.0f, 0.0f, 1.0f);
                Color OutlineColor = new Color(0.0f, 0.0f, currentSP/400, 1.0f);

                bulletMaterial.SetColor("_NoiseColor", NoiseColor);
                bulletMaterial.SetColor("_OutlineColor", OutlineColor);
            }
            else
            {
                Color Universalcolor = new Color(currentHP/400, 0.0f, currentSP/400, 1.0f);
                bulletMaterial.SetColor("_NoiseColor", Universalcolor);
                bulletMaterial.SetColor("_OutlineColor", new Color(1.0f, 1.0f, 1.0f, 1.0f));
            }
        }
    }
}
