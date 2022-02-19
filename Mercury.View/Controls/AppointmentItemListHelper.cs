using System.Collections.Generic;

namespace Mercury.View;

internal static class AppointmentItemListHelper
{
    public static void ApplyIdentation(IList<AppointmentItem> list)
    {
        var indents = new List<List<AppointmentItem>>();

        foreach (var item in list)
        {
            var index = GetIndex(indents, item);
            List<AppointmentItem> indent;
            if (index >= 0)
            {
                indent = indents[index];
            }
            else
            {
                indent = new List<AppointmentItem>();
                indents.Add(indent);
            }
            indent.Add(item);
        }

        for (var i = 0; i < indents.Count; i++)
        {
            foreach (var item in indents[i])
            {
                item.Indent = i;
            }
        }
    }

    static int GetIndex(List<List<AppointmentItem>> indents, AppointmentItem item)
    {
       for (var i = 0; i < indents.Count; i++)
       {
           if (FitsIn(indents[i], item)) return i;
       }

       return -1;
    }

    static bool FitsIn(List<AppointmentItem> items, AppointmentItem item)
    {
        foreach (var i in items)
        {
            if (i.HasOverlap(item)) return false;
        }

        return true;
    }
}