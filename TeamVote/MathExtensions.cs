using System.Collections.Generic;

namespace TeamVote
{
   public static class MathExtensions
   {
      public static double Median<TSource>( this IEnumerable<TSource> source, Func<TSource, int> selector )
      {
         if ( source == null )
         {
            throw new ArgumentNullException( "The source is null" );
         }

         if ( source.Count() == 0 )
         {
            return 0;
         }

         var nums = new List<int>();
         using ( IEnumerator<TSource> e = source.GetEnumerator() )
         {
            while ( e.MoveNext() )
            {
               nums.Add( selector( e.Current ) );
            }
         }

         var data = nums.OrderBy( n => n ).ToArray();

         if ( data.Length % 2 == 0 )
         {
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
         }

         return data[data.Length / 2];
      }

      public static int ModeWithMaxTiebreaker<TSource>( this IEnumerable<TSource> source, Func<TSource, int> selector )
      {
         if ( source == null )
         {
            throw new ArgumentNullException( "The source is null" );
         }

         if ( source.Count() == 0 )
         {
            return 0;
         }

         var nums = new List<int>();
         using ( IEnumerator<TSource> e = source.GetEnumerator() )
         {
            while ( e.MoveNext() )
            {
               nums.Add( selector( e.Current ) );
            }
         }

         // Store the number of occurences for each element
         Dictionary<int, int> counts = new Dictionary<int, int>();

         // Add one to the count for the occurence of a character
         foreach ( var element in nums )
         {
            if ( counts.ContainsKey( element ) )
            {
               counts[element]++;
            }
            else
            {
               counts.Add( element, 1 );
            }
         }

         // Loop through the counts of each element and find the 
         // element that occurred most often
         int mode = 0;
         int max = 0;
         foreach ( KeyValuePair<int, int> count in counts )
         {
            if ( count.Value > max )
            {
               // Update the mode
               mode = count.Key;
               max = count.Value;
            }

            // Tiebreaker
            if ( count.Value == max && count.Key > mode )
            {
               // Update the mode
               mode = count.Key;
               max = count.Value;
            }
         }

         return mode;
      }
   }
}
