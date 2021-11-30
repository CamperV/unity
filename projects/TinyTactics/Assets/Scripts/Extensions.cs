using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class Extensions
    {
        // Vector2Int
        public static IEnumerable<Vector2Int> Radiate(this Vector2Int v, int range) {
            List<Vector2Int> toEnumerate = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(v);

            while(queue.Count > 0) {
                Vector2Int curr = queue.Dequeue();
                if (toEnumerate.Contains(curr)) continue; 
                if (curr != v) toEnumerate.Add(curr);

                // add all surrounding tiles to the retval, but only do this $range times
                if (curr.ManhattanDistance(v) < range) {
                    queue.Enqueue(curr + Vector2Int.up);
                    queue.Enqueue(curr + Vector2Int.right);
                    queue.Enqueue(curr + Vector2Int.down);
                    queue.Enqueue(curr + Vector2Int.left);
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                yield return e; 
            }
        }

        // Vector3Int
        public static IEnumerable<Vector3Int> Radiate(this Vector3Int v, int range) {
            List<Vector3Int> toEnumerate = new List<Vector3Int>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(v);

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
        // public static IEnumerable<Vector3Int> GridRadiate(this Vector3Int v, TacticsGrid grid, int range) {
        //     List<Vector2Int> toEnumerate = new List<Vector2Int>();
        //     Queue<Vector2Int> queue = new Queue<Vector2Int>();
        //     Vector2Int _v = new Vector2Int(v.x, v.y);
        //     queue.Enqueue(_v);

        //     while(queue.Count > 0) {
        //         Vector2Int curr = queue.Dequeue();
        //         if (toEnumerate.Contains(curr)) continue; 
        //         if (curr != _v && grid.IsInBounds(grid.To3D(curr))) {
        //             toEnumerate.Add(curr);
        //         }

        //         if (curr.ManhattanDistance(_v) < range) {
        //             queue.Enqueue(curr + Vector2Int.up);
        //             queue.Enqueue(curr + Vector2Int.right);
        //             queue.Enqueue(curr + Vector2Int.down);
        //             queue.Enqueue(curr + Vector2Int.left);
        //         }
        //     }

        //     // now spill the beans
        //     foreach (var e in toEnumerate) {
        //         yield return grid.To3D(e);
        //     }
        // }
        // public static IEnumerable<Vector3Int> GridRadiate(this Vector3Int v, TacticsGrid grid, int range) {
        //     // quick scalable approach: iterate through a virtual cube and simply check the bounds and yield if within
        //     Vector3Int _lower = v - new Vector3Int(range, range, range);
        //     Vector3Int _upper = v + new Vector3Int(range, range, range);

        //     for (int x = _lower.x; x <= _upper.x; x++) {
        //         for (int y = _lower.y; y <= _upper.y; y++) {
        //             for (int z = _lower.z; z <= _upper.z; z++) {
        //                 Vector3Int _v = new Vector3Int(x, y, z);
        //                 if (_v != v && grid.IsInBounds(_v) && v.ManhattanDistance(_v) <= range)
        //                     yield return _v;
        //             }
        //         }
        //     }
        // }

        // Vector3Int
        public static IEnumerable<Vector3Int> RadiateSquare(this Vector3Int v, int range) {
            HashSet<Vector3Int> toEnumerate = new HashSet<Vector3Int> {v};

            // start with the first ring, skip the 0th
            for(int r = 1; r < range+1; r++) {
                int xMin = v.x - r;
                int yMin = v.y - r;
                int xMax = v.x + r;
                int yMax = v.y + r;

                // top and bottom
                for(int xm = xMin; xm <= xMax; xm++) {
                    toEnumerate.Add( new Vector3Int(xm, yMin, 0) );
                    toEnumerate.Add( new Vector3Int(xm, yMax, 0) );
                }

                // both sides
                for(int ym = yMin; ym <= yMax; ym++) {
                    toEnumerate.Add( new Vector3Int(xMin, ym, 0) );
                    toEnumerate.Add( new Vector3Int(xMax, ym, 0) );
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                yield return e; 
            }
        }

        // Vector3Int
        public static IEnumerable<Vector3Int> RadiateCircle(this Vector3Int v, int range) {
            HashSet<Vector3Int> toEnumerate = new HashSet<Vector3Int> {v};

            // start with the first ring, skip the 0th
            for(int r = 1; r < range+1; r++) {
                int xMin = v.x - r;
                int yMin = v.y - r;
                int xMax = v.x + r;
                int yMax = v.y + r;

                // top and bottom
                for(int xm = xMin; xm <= xMax; xm++) {
                    toEnumerate.Add( new Vector3Int(xm, yMin, 0) );
                    toEnumerate.Add( new Vector3Int(xm, yMax, 0) );
                }

                // both sides
                for(int ym = yMin; ym <= yMax; ym++) {
                    toEnumerate.Add( new Vector3Int(xMin, ym, 0) );
                    toEnumerate.Add( new Vector3Int(xMax, ym, 0) );
                }
            }

            // now spill the beans
            foreach (var e in toEnumerate) {
                Vector3Int ev = e - v;
		        if ((ev.x*ev.x) + (ev.y*ev.y) <= (range*range)) {
                    yield return e;
                }
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
        public static Vector3Int Unit(this Vector3Int v) {
            return new Vector3Int(Mathf.Clamp(v.x, -1, 1), Mathf.Clamp(v.y, -1, 1), Mathf.Clamp(v.z, -1, 1));
        }

        public static Vector3Int X(this Vector3Int v) {
            return new Vector3Int(v.x, 0, 0);
        }

        public static Vector3Int Y(this Vector3Int v) {
            return new Vector3Int(0, v.y, 0);
        }

        public static Vector3Int Z(this Vector3Int v) {
            return new Vector3Int(0, 0, v.z);
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

        // Vector3
        public static Vector3 Clamp(this Vector3 v, float clampVal) {
            return new Vector3(Mathf.Clamp(v.x,  -clampVal, clampVal),
                               Mathf.Clamp(v.y,  -clampVal, clampVal),
                               Mathf.Clamp(v.z,  -clampVal, clampVal));
        }

        // Vector3
        public static Vector2 Trim(this Vector3 v, string ax) {
            switch (ax) {
                case "x":
                case "X":
                    return new Vector2(v.y, v.z);
                case "y":
                case "Y":
                    return new Vector2(v.x, v.z);
                case "z":
                case "Z":
                    return new Vector2(v.x, v.y);
                default:
                    return new Vector2(v.x, v.y);
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

        // Type
        public static bool MatchesType(this Type c, Type t) {
            return c == t || c.IsSubclassOf(t);
        }

        // float[,]
        public static float[,] Add(this float[,] f, float[,] o) {
            float[,] resultant = new float[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    resultant[x, y] = f[x, y] + o[x, y];
                }
            }
            return resultant;
        }

        // float[,]
        public static float[,] Subtract(this float[,] f, float[,] o) {
            float[,] resultant = new float[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    resultant[x, y] = f[x, y] - o[x, y];
                }
            }
            return resultant;
        }

        // float[,]
        // subtraction that only allows a certain amount of depression
        public static float[,] SubtractGated(this float[,] f, float[,] o, float gateValue) {
            float[,] resultant = new float[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    float diff = f[x, y] - o[x, y];
                    resultant[x, y] = Mathf.Min(f[x, y] - gateValue, diff);
                }
            }
            return resultant;
        }

        // float[,]
        // gauge the depression by this instances's value
        public static float[,] SubtractProportionally(this float[,] f, float[,] o) {
            float[,] resultant = new float[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    resultant[x, y] = f[x, y] - (f[x, y] * o[x, y]);
                }
            }
            return resultant;
        }

        // float[,]
        public static float Min(this float[,] f) {
            float currMin = float.MaxValue;

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    currMin = Mathf.Min(currMin, f[x, y]);
                }
            }
            return currMin;
        }

        // float[,]
        public static float Max(this float[,] f) {
            float currMax = float.MinValue;

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    currMax = Mathf.Max(currMax, f[x, y]);
                }
            }
            return currMax;
        }

        // float[,]
        public static float[,] Normalize(this float[,] f) {
            float[,] r = new float[f.GetLength(0), f.GetLength(1)];
            float min = f.Min();
            float max = f.Max();
            if (min == max) {
                min = 0f;
                max = 1f;
            }

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    r[x, y] = Mathf.InverseLerp(min, max, f[x, y]);
                }
            }
            return r;
        }

        // int[,]
        public static int[,] Subtract(this int[,] f, int[,] o) {
            int[,] resultant = new int[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    resultant[x, y] = f[x, y] - o[x, y];
                }
            }
            return resultant;
        }

        // int[,]
        public static float[,] ToFloat(this int[,] i) {
            float[,] f = new float[i.GetLength(0), i.GetLength(1)];

            for (int x = 0; x < i.GetLength(0); x++) {
                for (int y = 0; y < i.GetLength(1); y++) {
                    f[x, y] = (float)i[x, y];
                }
            }
            return f;
        }
    }
}