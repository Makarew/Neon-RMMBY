using System.Collections.Generic;
using UnityEngine;

namespace RMMBY.Helpers
{
    public class ObjectFinders
    {
        public static GameObject[] FindAllObjectsWithName(string name)
        {
            List<GameObject> list = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if(obj.name == name)
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }

        public static GameObject[] FindAllObjectsEndsWithName(string name)
        {
            List<GameObject> list = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.name.EndsWith(name))
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }

        public static GameObject[] FindAllObjectsStartsWithName(string name)
        {
            List<GameObject> list = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.name.StartsWith(name))
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }

        public static GameObject[] FindAllObjectsContainingName(string name)
        {
            List<GameObject> list = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains(name))
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }

        public static List<GameObject> FindAllObjectsContainingNameInList(string name, List<GameObject> objects)
        {
            List<GameObject> list = new List<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains(name))
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        public static GameObject[] FindAllObjectsOnLayer(int layer)
        {
            List<GameObject> list = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.layer == layer)
                {
                    list.Add(obj);
                }
            }

            return list.ToArray();
        }
    }
}
