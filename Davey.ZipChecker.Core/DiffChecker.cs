using Davey.ZipChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker
{
    /// <summary>
    /// Utility to compute the difference between two collections of entries by a supplied key.
    /// Useful for comparing ZipEntryInfo collections by their path/name.
    /// </summary>
    public static class DiffChecker
    {
        /// <summary>
        /// Result of a diff operation: items present in A but not B, and items present in B but not A.
        /// </summary>
        public sealed record DiffResult<T>(IReadOnlyList<T> OnlyInA, IReadOnlyList<T> OnlyInB);

        /*
         Pseudocode / Plan (detailed):
         - Provide a ComputeDiff method specialized for ZipEntryInfo collections (not generic).
         - Parameters:
           - a: first collection of ZipEntryInfo (IReadOnlyCollection to reflect a concrete collection type)
           - b: second collection of ZipEntryInfo
           - keySelector: function to extract the identifying key from a ZipEntryInfo (e.g., FullName)
           - keyComparer: optional string comparer (defaults to Ordinal)
         - Validate arguments (throw ArgumentNullException for null a, b, or keySelector).
         - Ensure keyComparer has a default (StringComparer.Ordinal) if null.
         - Build a lookup dictionary for B:
           - Key: selected key (use empty string if selector returns null)
           - Value: List<ZipEntryInfo> preserving duplicates for that key
         - Iterate A:
           - Track all keys seen in A using a HashSet with the provided comparer.
           - For each item in A, if its key is not in the lookup for B, add the item to onlyInA.
         - After scanning A, iterate the lookup for B:
           - For each key in lookupB, if the key was not seen in A (keysInA doesn't contain it), add all stored items for that key to onlyInB.
         - Return a DiffResult<ZipEntryInfo> with the two result lists.
         - Keep behavior identical to original generic implementation, but typed to ZipEntryInfo to allow direct referencing in calling code.
        */

        /// <summary>
        /// Compute the difference between two ZipEntryInfo collections.
        /// Presence is determined solely by key equality (provided by keySelector).
        /// </summary>
        /// <param name="a">First collection (A) of ZipEntryInfo.</param>
        /// <param name="b">Second collection (B) of ZipEntryInfo.</param>
        /// <param name="keySelector">Function that returns the key identifying a ZipEntryInfo (e.g., FullName).</param>
        /// <param name="keyComparer">Comparer for keys (defaults to Ordinal).</param>
        /// <returns>DiffResult containing lists OnlyInA and OnlyInB.</returns>
        public static DiffResult<ZipEntryInfo> ComputeDiff(
            IReadOnlyCollection<ZipEntryInfo> a,
            IReadOnlyCollection<ZipEntryInfo> b,
            IEqualityComparer<string>? keyComparer = null)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));

            keyComparer ??= StringComparer.Ordinal;

            // Build lookup for B: key -> list of items with that key (preserve duplicates)
            var lookupB = new Dictionary<string, List<ZipEntryInfo>>(keyComparer);
            foreach (var item in b)
            {
                if (!lookupB.TryGetValue(item.Path, out var list))
                {
                    list = new List<ZipEntryInfo>();
                    lookupB[item.Path] = list;
                }
                list.Add(item);
            }

            var onlyInA = new List<ZipEntryInfo>();
            var keysInA = new HashSet<string>(keyComparer);

            // Iterate A: collect keys and items whose key is not present in B
            foreach (var item in a)
            {
                keysInA.Add(item.Path);
                if (!lookupB.ContainsKey(item.Path))
                {
                    onlyInA.Add(item);
                }
            }

            // Items in B whose key did not appear in A
            var onlyInB = new List<ZipEntryInfo>();
            foreach (var kv in lookupB)
            {
                if (!keysInA.Contains(kv.Key))
                {
                    // add all items that share this key
                    onlyInB.AddRange(kv.Value);
                }
            }

            return new DiffResult<ZipEntryInfo>(onlyInA, onlyInB);
        }
    }
}
