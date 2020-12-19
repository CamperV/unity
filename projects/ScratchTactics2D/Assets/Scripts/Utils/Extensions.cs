using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Extensions {
    public static class Extensions {

        // extend Phase enum to link enums a la LL
        public static Enum.Phase NextPhase(this Enum.Phase p) {
            switch (p) {
                case Enum.Phase.player:
                    return Enum.Phase.enemy;
                case Enum.Phase.enemy:
                    return Enum.Phase.player;
                default:
                    return Enum.Phase.none;
            }
        }

        // extend Vector3Int to include Radiate
        public static IEnumerable<Vector3Int> Radiate(this Vector3Int v, int range) {
            HashSet<Vector3Int> toEnumerate = new HashSet<Vector3Int>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();

            queue.Enqueue(v);
            int currentDepth = 0;

            // let's avoid recursion in C#, at least in relatively-unknown-extension-space, yeah?
            while(queue.Count > 0) {
                Vector3Int curr = queue.Dequeue();
                if (curr != v) toEnumerate.Add(curr);

                // add all surrounding tiles to the retval, but only do this $range times
                if (currentDepth < range) {
                    queue.Enqueue(curr + Vector3Int.up);
                    queue.Enqueue(curr + Vector3Int.right);
                    queue.Enqueue(curr + Vector3Int.down);
                    queue.Enqueue(curr + Vector3Int.left);
                    currentDepth++;
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                yield return e; 
            }
        }
    }
}