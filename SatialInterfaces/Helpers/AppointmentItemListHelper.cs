using System.Collections.Generic;
using System.Linq;
using SatialInterfaces.Controls;

namespace SatialInterfaces.Helpers;

internal static class AppointmentItemListHelper
{
    /// <summary>
    /// Applies the indentation of the appointment list (based on avoiding overlap)
    /// </summary>
    /// <param name="list">Appointment list to process</param>
    public static void ApplyIndentation(IEnumerable<AppointmentItem> list)
    {
        var listOfLists = new List<List<AppointmentItem>>();

        foreach (var item in list)
        {
            var index = GetIndent(listOfLists, item);
            List<AppointmentItem> indentList;
            if (index >= 0)
            {
                indentList = listOfLists[index];
            }
            else
            {
                indentList = new List<AppointmentItem>();
                listOfLists.Add(indentList);
            }
            indentList.Add(item);
        }

        for (var i = 0; i < listOfLists.Count; i++)
        {
            foreach (var item in listOfLists[i])
            {
                item.Indent = i;
            }
        }
    }

    /// <summary>
    /// Gets the index of the list in which the given item fits
    /// </summary>
    /// <param name="listOfLists">List with indentations</param>
    /// <param name="item">Item to check</param>
    /// <returns>The index or -1 otherwise</returns>
    static int GetIndent(IReadOnlyList<List<AppointmentItem>> listOfLists, AppointmentItem item)
    {
       for (var i = 0; i < listOfLists.Count; i++)
       {
           if (FitsIn(listOfLists[i], item)) return i;
       }

       return -1;
    }

    /// <summary>
    /// Checks if the given item fits in the list (has no overlap)
    /// </summary>
    /// <param name="list">List to check in</param>
    /// <param name="item">Item to check</param>
    /// <returns>True if it fits and false otherwise</returns>
    static bool FitsIn(IEnumerable<AppointmentItem> list, AppointmentItem item)
    {
        return list.All(i => !i.HasOverlap(item));
    }
}
