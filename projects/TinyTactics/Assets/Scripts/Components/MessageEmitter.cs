using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Extensions;

public class MessageEmitter : MonoBehaviour
{
    // ENSURE THESE ARE IN ORDER IN THE INSPECTOR
    public enum MessageType {
        Damage,
        CritDamage,
        NoDamage,
        Miss,
        Heal,
        Buff,
        Debuff
    }

    public Transform origin;  // assign in inspector
    public bool randomSaltFlag;

    [SerializeField] private Message[] messagePrefabs;

    public void Emit(MessageType messageType, string message) {
        if (randomSaltFlag) {
            Vector3 finalOffset = origin.position;

            float radius = 0.475f;

            // on the edge of the unit circle
            finalOffset = radius * (Vector3)Random.insideUnitCircle.normalized;

            // now only on the top-half
            finalOffset = new Vector3(
                Mathf.Clamp(finalOffset.x, -.3f, .3f),
                Mathf.Clamp(Mathf.Abs(finalOffset.y), .4f, radius), 
                0
            );
        }

        Message emission = Instantiate(messagePrefabs[(int)messageType], origin.position, Quaternion.identity);
        emission.textMesh.SetText(message);
    }

    // this emits towards a source, using the attached Unit's transform as a pivot
    public void EmitTowards(MessageType messageType, string message, Vector3 source) {
        float originMag = (origin.position - transform.position).magnitude;
        Vector3 direction = source - transform.position;
        Vector3 offset = originMag*direction.normalized;

        // and clamp it, finally
        float clampedY = (offset.y >= 0f) ? 0.75f*(originMag + 0.10f) : -0.75f*(originMag + 0.10f);

        Message emission = Instantiate(messagePrefabs[(int)messageType], transform.position + new Vector3(offset.x, clampedY, offset.z), Quaternion.identity);
        emission.textMesh.SetText(message);
    }
}
