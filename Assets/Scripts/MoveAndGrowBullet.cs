using System.Collections;
using UnityEngine;

public class MoveAndGrowBullet : MonoBehaviour
{
    [Header("Bullet Reset")]
    public (float, float) resetVariationX = (-0.2f, -0.4f); 
    public (float, float) resetVariationY = (-0.4f, 0.4f);
    public (float, float) resetVariationZ = (-0.3f, 0.3f);

    public void MoveAndGrowBulletFunction()
    {
        StopAllCoroutines();
        StartCoroutine(MoveBullet());
    }

    private IEnumerator MoveBullet()
    {
        Vector3 randomPosition = new Vector3
        (
            Random.Range(resetVariationX.Item1, resetVariationX.Item2),
            Random.Range(resetVariationY.Item1, resetVariationY.Item2),
            Random.Range(resetVariationZ.Item1, resetVariationZ.Item2)
        );
        Vector3 newBulletPosition = randomPosition.normalized * 0.8f + new Vector3(10f, 1.5f, 0f);
        Vector3 oldBulletPosition = gameObject.transform.position;

        float moveProgress = 0;
        while (moveProgress < 1)
        {
            gameObject.transform.position = Vector3.Lerp(oldBulletPosition, newBulletPosition, moveProgress);
            moveProgress += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        gameObject.transform.position = newBulletPosition;

        GrowBulletFunction(false);
    }

    public void GrowBulletFunction(bool wait) { StartCoroutine(GrowBulletCoroutine(wait)); }

    public IEnumerator GrowBulletCoroutine(bool wait)
    {
        float startingSize = 0.01f;
        float growRate = 1.5f;

        gameObject.transform.localScale = new Vector3(0,0,0);
        if (wait) { yield return new WaitForSeconds(0.5f); }
        gameObject.transform.localScale = new Vector3(startingSize, startingSize, startingSize);

        while (gameObject.transform.localScale.z < 0.25f)
        {
            //Debug.Log("Changing Size!");
            gameObject.transform.localScale *= growRate;

            yield return new WaitForSeconds(0.01f);
        }
    }

}