using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.API
{
    public class Tools
    {
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
    }
}
