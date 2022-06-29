using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

public class Message : MonoBehaviour
{
    public TextMeshPro textMesh;
    public bool shakeOnStart;

    void Start() {
        StartCoroutine( AnimateThenDestroy() );
        
        if (shakeOnStart) StartCoroutine( Shake(0.10f, 3) );
    }

    private IEnumerator AnimateThenDestroy() {
        yield return new WaitForSeconds(1f);
        //

        Color originalColor = textMesh.color;
        Vector3 originalPosition = transform.position;
        Vector3 finalPosition = transform.position + new Vector3(0, 0.035f, 0);
        //
        float fixedTime = 0.5f;
		float timeRatio = 0.0f;

		while (timeRatio < 1.0f) {
			timeRatio += (Time.deltaTime / fixedTime);
            textMesh.color = originalColor.WithAlpha(1f - timeRatio);

            transform.position = Vector3.Lerp(originalPosition, finalPosition, timeRatio);
			yield return null;
		}

        Destroy(gameObject);
	}

	private IEnumerator Shake(float radius, int numberOfShakes) {
		var ogPosition = transform.position;

		for (int i = 0; i < numberOfShakes; i++) {
			Vector3 offset = (Vector3)Random.insideUnitCircle*radius;
			transform.position = ogPosition + offset;
			radius /= 2f;
			yield return new WaitForSeconds(0.05f);
		}

		transform.position = ogPosition;
	}
}
