using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

// these tools are used for maps of type T[,]
namespace MapTools
{
    public static class MapTools
    {
        // apply arbitary functions to this map
        public static T[,] Map<T>(this T[,] t, Func<T, T> Fn) {
            T[,] r = new T[t.GetLength(0), t.GetLength(1)];

            for (int x = 0; x < t.GetLength(0); x++) {
                for (int y = 0; y < t.GetLength(1); y++) {
                    r[x, y] = Fn(t[x, y]);
                }
            }
            return r;
        }

        public static T[,] Mask<T>(this T[,] t, int[,] mask) {
            T[,] r = new T[t.GetLength(0), t.GetLength(1)];

            for (int x = 0; x < t.GetLength(0); x++) {
                for (int y = 0; y < t.GetLength(1); y++) {
                    if (mask[x, y] == 1) {
                        r[x, y] = t[x, y];
                    }
                }
            }
            return r;
        }

        public static bool Contains<T>(this T[,] t, int x, int y) {
            return (x >= 0) && (x < t.GetLength(0)) && (y >= 0) && (y < t.GetLength(1));
        }

        public static int[,] BinaryThreshold(this float[,] f, float thresh) {
            int[,] mask = new int[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    mask[x, y] = (f[x, y] >= thresh) ? 1 : 0;
                }
            }
            return mask;
        }

        public static int[,] ClampBinaryThreshold(this float[,] f, float lowerThresh, float upperThresh) {
            int[,] mask = new int[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    mask[x, y] = (f[x, y] >= lowerThresh && f[x, y] <= upperThresh) ? 1 : 0;
                }
            }
            return mask;
        }

        public static int[,] Inverse(this int[,] i) {
            int[,] mask = new int[i.GetLength(0), i.GetLength(1)];

            for (int x = 0; x < i.GetLength(0); x++) {
                for (int y = 0; y < i.GetLength(1); y++) {
                    mask[x, y] = (i[x, y] == 0) ? 1 : 0;
                }
            }
            return mask;
        }

        public static List<Vector2Int> ToList(this int[,] i) {
            List<Vector2Int> retVal = new List<Vector2Int>();

            for (int x = 0; x < i.GetLength(0); x++) {
                for (int y = 0; y < i.GetLength(1); y++) {
                    retVal.Add( new Vector2Int(x, y) );
                }
            }
            return retVal;
        }

        public static IEnumerable<Vector2Int> Where(this int[,] i, Func<int, bool> Filter) {
            for (int x = 0; x < i.GetLength(0); x++) {
                for (int y = 0; y < i.GetLength(1); y++) {
                    if (Filter(i[x, y])) {
                        yield return new Vector2Int(x, y);
                    }
                }
            }
        }

        public static int[,] LocationsOf<T>(this T[,] m, params T[] matches) {
            int[,] mask = new int[m.GetLength(0), m.GetLength(1)];

            for (int x = 0; x < m.GetLength(0); x++) {
                for (int y = 0; y < m.GetLength(1); y++) {

                    // for each matcher - don't use ternary b/c we want an OR relation given the params
                    // by default the mask equals 0 anyway
                    foreach(T e in matches) {
                        if (m[x, y].Equals(e)) mask[x, y] = 1;
                    }
                }
            }
            return mask;
        }

        // similar to IPathable
        public static IEnumerable<Vector3Int> GetNeighbors<T>(this T[,] m, Vector3Int origin) {
            Vector3Int up = origin + Vector3Int.up;
            Vector3Int right = origin + Vector3Int.right;
            Vector3Int down = origin + Vector3Int.down;
            Vector3Int left = origin + Vector3Int.left;
            if (m.Contains<T>(up.x, up.y)) yield return up;
            if (m.Contains<T>(right.x, right.y)) yield return right;
            if (m.Contains<T>(down.x, down.y)) yield return down;
            if (m.Contains<T>(left.x, left.y)) yield return left;
        }
    }
}