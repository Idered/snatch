using System.Collections.Generic;

namespace Snatch
{
  class Utils
  {
    public static void RemoveAll<T>(IList<T> iList, IEnumerable<T> itemsToRemove)
    {
      HashSet<T> set = new HashSet<T>(itemsToRemove);
      List<T> list = iList as List<T>;

      if (list == null)
      {
        int i = 0;
        while (i < iList.Count)
        {
          if (set.Contains(iList[i]))
          {
            iList.RemoveAt(i);
          }
          else
          {
            i++;
          }
        }
      }
      else
      {
        list.RemoveAll(set.Contains);
      }
    }
  }
}
