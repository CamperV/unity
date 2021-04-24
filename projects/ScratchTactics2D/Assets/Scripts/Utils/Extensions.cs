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

        // Vector3Int
        public static IEnumerable<Vector3Int> Radiate(this Vector3Int v, int range) {
            List<Vector3Int> toEnumerate = new List<Vector3Int>();
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
        public static IEnumerable<Vector3Int> GridRadiate(this Vector3Int v, GameGrid grid, int range) {
            List<Vector2Int> toEnumerate = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Vector2Int _v = new Vector2Int(v.x, v.y);
            queue.Enqueue(_v);

            while(queue.Count > 0) {
                Vector2Int curr = queue.Dequeue();
                if (toEnumerate.Contains(curr)) continue; 
                if (curr != _v) toEnumerate.Add(curr);

                if (curr.ManhattanDistance(_v) < range) {
                    queue.Enqueue(curr + Vector2Int.up);
                    queue.Enqueue(curr + Vector2Int.right);
                    queue.Enqueue(curr + Vector2Int.down);
                    queue.Enqueue(curr + Vector2Int.left);
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                yield return grid.To3D(e);
            }
        }

        // Vector3Int
        // NOTE: This is not true MH distance. But this is what we need
        public static int ManhattanDistance(this Vector3Int v, Vector3Int o) {
            return Mathf.Abs(v.x-o.x) + Mathf.Abs(v.y-o.y);
        }

        // Vector2Int
        public static int ManhattanDistance(this Vector2Int v, Vector2Int o) {
            return Mathf.Abs(v.x-o.x) + Mathf.Abs(v.y-o.y);
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

        // List
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

        // List
        public static T PopRandom<T>(this List<T> list) {
            int rand = Random.Range(0, list.Count);
            T r = list[rand];
            list.RemoveAt(rand);
            return r;
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