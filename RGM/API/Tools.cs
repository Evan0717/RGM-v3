using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RGM.API
{
    public class Tools
    {
        public static T GetRandomValue<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public static List<T> EnumToList<T>()
        {
            Array items = Enum.GetValues(typeof(T));
            List<T> itemList = new List<T>();

            foreach (T item in items)
            {
                if (!item.ToString().Contains("None"))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static Color GetRandomColor(bool Transparency = false)
        {
            if (!Transparency)
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            else
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
    }
}
