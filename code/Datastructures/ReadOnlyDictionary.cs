namespace RJCP.Core.Datastructures
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// A wrapper class for a Read Only Dictionary
    /// </summary>
    /// <remarks>
    /// Originally from: 
    /// <para>Updates from</para>
    /// <list type="bullet">
    /// <item>http://stackoverflow.com/questions/35002/does-c-sharp-have-a-way-of-giving-me-an-immutable-dictionary</item>
    /// <item>http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=29</item>
    /// <item>http://www.blackwasp.co.uk/ReadOnlyDictionary_5.aspx</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TKey">Key type for the dictionary</typeparam>
    /// <typeparam name="TValue">Value type for the dictionary</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    [DebuggerTypeProxy(typeof(ReadOnlyDictionaryDebugView<,>))]
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> m_Dict;

        /// <summary>
        /// Constructor. Provide the dictionary to instantiate as read-only
        /// </summary>
        /// <param name="dict">Dictionary to export as read only</param>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dict) { m_Dict = dict; }

        /// <summary>
        /// Add a value to the dictionary - NOT SUPPORTED (Read Only)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value) { throw new NotSupportedException("Dictionary is read only"); }

        /// <summary>
        /// Tests if the dictionary contains a specified key
        /// </summary>
        /// <param name="key">Key to test for</param>
        /// <returns><b>true</b> if the dictionary contains the key</returns>
        public bool ContainsKey(TKey key) { return m_Dict.ContainsKey(key); }

        /// <summary>
        /// Get the collection of keys in the dictionary
        /// </summary>
        public ICollection<TKey> Keys { get { return m_Dict.Keys; } }
        
        /// <summary>
        /// Remove a key from the dictionary - NOT SUPPORTED (Read Only)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) { throw new NotSupportedException("Dictionary is read only"); }

        /// <summary>
        /// Test if the dictionary contains a value
        /// </summary>
        /// <param name="key">Key to obtain</param>
        /// <param name="value">Value to contain the results if key is present</param>
        /// <returns><b>true</b> if the key was present</returns>
        public bool TryGetValue(TKey key, out TValue value) { return m_Dict.TryGetValue(key, out value); }

        /// <summary>
        /// Get the collection of values in the dictionary
        /// </summary>
        public ICollection<TValue> Values { get { return m_Dict.Values; } }

        /// <summary>
        /// Map a key to the corresponding value
        /// </summary>
        /// <param name="key">Key to map from</param>
        /// <returns>The value belonging to the specified key</returns>
        public TValue this[TKey key]
        {
            get { return m_Dict[key]; }
            set { throw new NotSupportedException("Dictionary is read only"); }
        }

        /// <summary>
        /// Add a KeyValuePair to the dictionary - NOT SUPPORTED (Read Only)
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException("Dictionary is read only"); }

        /// <summary>
        /// Clear the contents of the dictionary - NOT SUPPORTED (Read Only)
        /// </summary>
        public void Clear() { throw new NotSupportedException("Dictionary is read only"); }

        /// <summary>
        /// Checks if the dictionary contains a KeyValue pair
        /// </summary>
        /// <param name="item">The KeyValue pair to check for</param>
        /// <returns><b>true</b> if the dictionary contains the KeyValue pair</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) { return m_Dict.Contains(item); }

        /// <summary>
        /// Copy all the Dictionary's KeyValue pairs into an array
        /// </summary>
        /// <param name="array">Array to hold all the KeyValue pairs</param>
        /// <param name="arrayIndex">Index offset to copy into</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { m_Dict.CopyTo(array, arrayIndex); }

        /// <summary>
        /// Return the number of elements in the dictionary
        /// </summary>
        public int Count { get { return m_Dict.Count; } }

        /// <summary>
        /// If the Dictionary is read only or not (always true)
        /// </summary>
        public bool IsReadOnly { get { return true; } }

        /// <summary>
        /// Remove a KeyValue pair from the dictionary - NOT SUPPORTED (Read Only)
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>If the key/value pair was in the dictionary</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException("Dictionary is read only"); }

        /// <summary>
        /// Get the enumerator for KeyValue pairs
        /// </summary>
        /// <returns>The enumerator for the KeyValue pair</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return m_Dict.GetEnumerator(); }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_Dict).GetEnumerator();
        }
    }

    internal sealed class ReadOnlyDictionaryDebugView<TKey, TValue>
    {
        private IDictionary<TKey, TValue> dict;

        public ReadOnlyDictionaryDebugView(ReadOnlyDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) {
                throw new ArgumentNullException("dictionary");
            }

            this.dict = dictionary;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[this.dict.Count];
                this.dict.CopyTo(array, 0);
                return array;
            }
        }
    }
}
