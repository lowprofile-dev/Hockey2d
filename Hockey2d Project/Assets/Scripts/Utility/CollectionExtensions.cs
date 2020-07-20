using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtensions
{
    /// <remarks>Origem: http://stackoverflow.com/questions/273313/randomize-a-listt</remarks>
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1)
        {  
            n--;  
            var k = Random.Range(0, n);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
