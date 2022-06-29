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

    public Vector3 originOffset;
    public bool randomSaltFlag;
    [SerializeField] private Message[] messagePrefabs;

    public void Emit(MessageType messageType, string message) {
        Vector3 finalOffset = originOffset;

        if (randomSaltFlag) {
            float radius = 0.475f;

            // on the edge of the unit circle
            finalOffset = radius * (Vector3)Random.insideUnitCircle.normalized;

            // now only on the top-half
            finalOffset = new Vector3(
                Mathf.Clamp(finalOffset.x, -.3f, .3f),
                Mathf.Clamp(Mathf.Abs(finalOffset.y), .4f, radius), 
                0
            );

            // uncomment for fixed-offset damage numbers
            // finalOffset = new Vector3(
            //     0,
            //     radius, 
            //     0
            // );
        }

        Message emission = Instantiate(messagePrefabs[(int)messageType], transform.position + finalOffset, Quaternion.identity);
        emission.textMesh.SetText(message);
    }
}
