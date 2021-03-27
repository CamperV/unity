using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

            // let's avoid recursion in C#, at least in relatively-unknown-extension-space, yeah?
            while(queue.Count > 0) {
                Vector3Int curr = queue.Dequeue();
                if (toEnumerate.Contains(curr)) continue; 
                if (curr != v) toEnumerate.Add(curr);

                // add all surrounding tiles to the retval, but only do this $range times
                if (curr.ManhattanDistance(v) < range) {
                    queue.Enqueue(curr + Vector3Int.up);
                    queue.Enqueue(curr + Vector3Int.right);
                    queue.Enqueue(curr + Vector3Int.down);
                    queue.Enqueue(curr + Vector3Int.left);
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                yield return e; 
            }
        }

        // Vector3Int
        public static int ManhattanDistance(this Vector3Int v, Vector3Int o) {
            return Mathf.Abs(v.x-o.x) + Mathf.Abs(v.y-o.y) + Mathf.Abs(v.z-o.z);
        }

        // Vector3Int
        public static bool AdjacentTo(this Vector3Int v, Vector3Int o) {
            return v.ManhattanDistance(o) == 1;
        }

        // Vector3Int
        public static Vector3Int DivBy(this Vector3Int v, int i) {
            float s = (float)i;
            return new Vector3Int((int)(v.x/s), (int)(v.y/s), (int)(v.z/s));
        }

        // Vector3Int
        public static Vector3Int GridPosInDirection(this Vector3Int v, GameGrid grid, Vector2Int dir) {
            return grid.To3D(new Vector2Int(v.x, v.y) + dir);
        }

        // Vector3
        public static Vector3 SafeScale(this Vector3 v, Vector3 o) {
            return new Vector3((o.x > 0) ? v.x/o.x : 1,
                               (o.y > 0) ? v.y/o.y : 1,
                               (o.z > 0) ? v.z/o.z : 1);
        }

        // Vector3
        public static IEnumerable<Vector3> SteppedInterpEx(this Vector3 v, Vector3 f, int numSteps) {
            Vector3 interpStep = (1.0f / numSteps) * (f - v);
            for (int i = 1; i <= numSteps; i++) {
                yield return v + (i * interpStep);
            }
        }

        // Vector3
        public static IEnumerable<Vector3> SteppedInterpInc(this Vector3 v, Vector3 f, int numSteps) {
            Vector3 interpStep = (1.0f / numSteps) * (f - v);
            for (int i = 0; i <= numSteps; i++) {
                yield return v + (i * interpStep);
            }
        }

        // List
        public static T PopAt<T>(this List<T> list, int index) {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }

        public static List<T> RandomSelections<T>(this List<T> l, int numSelections) {
            List<T> retVal = new List<T>();
            List<int> available = Enumerable.Range(0, l.Count).ToList();
            
            while (retVal.Count < numSelections) {
                var avaIndex = Random.Range(0, available.Count);
                var selIndex = available[avaIndex];
                
                retVal.Add(l[selIndex]);
                available.Remove(selIndex);
            }
            return retVal;
        }

        // Color
        public static Color WithAlpha(this Color c, float alpha) {
            return new Color(c.r, c.g, c.b, alpha);
        }

        public static Color WithTint(this Color c, float tint) {
            return new Color(tint, tint, tint, 1.0f);
        }

        // Dict
        public static TValue GetValueOtherwise<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey k, TValue tv) {
            if (d.ContainsKey(k)) {
                return d[k];
            } else {
                return tv;
            }
        }

        // Component
        public static bool MatchesType(this Component c, Type t) {
            return c.GetType() == t || c.GetType().IsSubclassOf(t);
        }
    }
}