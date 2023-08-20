using System.Collections;
using System.Collections.Generic;
using EzySlice;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SliceObject : MonoBehaviour
{
    [Header("Bullet Reset")]
    public (float, float) resetVariationX = (-0.2f, -0.4f); 
    public (float, float) resetVariationY = (-0.6f, 0.6f);
    public (float, float) resetVariationZ = (-0.3f, 0.3f);

    [Header("Haptics")]
    public bool isLeftController;
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

    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit) 
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        WeaponSelector wp = gameObject.transform.parent.GetComponent<WeaponSelector>();

        hapticIntensity = wp.HitHP / 100;
        hapticDuration = wp.HitSP / 100;

        parentController = gameObject.transform.parent.GetComponent<ActionBasedController>();
        parentController.SendHapticImpulse(hapticIntensity,  hapticDuration);

        Vector3 velocity = velocityEstimator.GetVelocityEstimate(); 
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crosssectionMaterial);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crosssectionMaterial);
            SetupSlicedComponent(lowerHull);

            Vector3 randomPosition = new Vector3
            (
                Random.Range(resetVariationX.Item1, resetVariationX.Item2),
                Random.Range(resetVariationY.Item1, resetVariationY.Item2),
                Random.Range(resetVariationZ.Item1, resetVariationZ.Item2)
            );
            target.transform.position = randomPosition.normalized * 0.8f + new Vector3(10f, 1.5f, 0f);
        }
    }

    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        //rb.useGravity = false;
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

}
