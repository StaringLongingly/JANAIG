using System.Collections;
using EzySlice;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SliceObject : MonoBehaviour
{

    [Header("Haptics")]
    public ActionBasedController parentController;
    public float hapticIntensity, hapticDuration;

    [Header("Slicing")]
    public Transform startSlicePoint;
    public Transform endSlicePoint; 
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    public Material crosssectionMaterial;
    public float cutForce = 100;
    public bool isColliding = false;

    void OnTriggerEnter(Collider other)
    {
        Slice(other.transform.gameObject);
        if ( other.gameObject.GetComponent<Bullet>() != null ) { other.gameObject.GetComponent<Bullet>().CollideWithSword(gameObject.GetComponent<Collider>()); }
    }

    public void Slice(GameObject target)
    {
        Debug.Log("Sliced!");

        if (target.GetComponent<SceneChanger>() != null)
        {
            //if the target is a menu option adjust haptics
            hapticIntensity  = 1f;
            hapticDuration = 2f;
        }
        else
        {
            WeaponSelector wp = gameObject.transform.parent.GetComponent<WeaponSelector>();

            hapticIntensity = 0.005f * wp.HitHP + 0.5f;
            hapticDuration = 0.015f * wp.HitSP + 0.5f;
        }

        parentController = gameObject.transform.parent.GetComponent<ActionBasedController>();
        Debug.Log("Sending Haptics with Intensity: " + hapticIntensity + " and Duration: " + hapticDuration);
        parentController.SendHapticImpulse(hapticIntensity,  hapticDuration * 2);

        Vector3 velocity = velocityEstimator.GetVelocityEstimate(); 
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            Debug.Log("Hull is not null!");
            GameObject upperHull = hull.CreateUpperHull(target, crosssectionMaterial);
            SetupSlicedComponent(upperHull, target);

            GameObject lowerHull = hull.CreateLowerHull(target, crosssectionMaterial);
            SetupSlicedComponent(lowerHull, target);

            if (target.GetComponent<SceneChanger>() != null)
            {

                MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
                SphereCollider sphereCollider = target.GetComponent<SphereCollider>();

                (meshRenderer.enabled, sphereCollider.enabled) = (false, false);
            }

        }
    }

    public void SetupSlicedComponent(GameObject slicedObject, GameObject target)
    {
        StartCoroutine(DestroySlicedObject(slicedObject, target));
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        //rb.useGravity = false;
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

    public IEnumerator DestroySlicedObject(GameObject slicedObject, GameObject target)
    {
        yield return new WaitForSeconds(1);
        if ( target.GetComponent<SceneChanger>() != null ) { StartCoroutine(target.GetComponent<SceneChanger>().SwitchScene()); }
        while ( slicedObject.transform.localScale.x >= 0.01 ) 
        {
            yield return new WaitForSeconds(0.16f);
            slicedObject.transform.localScale = slicedObject.transform.localScale * 0.8f;
        }
        Destroy(slicedObject);
    }

}
