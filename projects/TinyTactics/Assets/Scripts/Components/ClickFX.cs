using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFX : MonoBehaviour
{
    public AudioClip clickFX;

	// experimental protoype for click sounds
	public void PlayClickFX() {
        AudioSource.PlayClipAtPoint(clickFX, transform.position, 1f);
        // Debug.Break();
    }
}
