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

        public static int[,] BinaryThreshold(this float[,] f, float thresh) {
            int[,] mask = new int[f.GetLength(0), f.GetLength(1)];

            for (int x = 0; x < f.GetLength(0); x++) {
                for (int y = 0; y < f.GetLength(1); y++) {
                    mask[x, y] = (f[x, y] >= thresh) ? 1 : 0;
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

        public static List<Vector2Int> FilterToList(this int[,] i, Func<int, bool> Filter) {
            List<Vector2Int> retVal = new List<Vector2Int>();

            for (int x = 0; x < i.GetLength(0); x++) {
                for (int y = 0; y < i.GetLength(1); y++) {
                    if (Filter(i[x, y])) retVal.Add( new Vector2Int(x, y) );
                }
            }
            return retVal;
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
    }
}
