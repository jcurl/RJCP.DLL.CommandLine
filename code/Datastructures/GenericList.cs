namespace RJCP.Core.Datastructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper class around a List or List&lt;T&gt; object.
    /// </summary>
    /// <typeparam name="T">The object type to wrap.</typeparam>
    public class GenericList<T> : IList<T> where T:class
    {
        private IList<T> m_ListGeneric;
        private IList m_List;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericList{T}"/> class.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="System.ArgumentNullException"><c>list</c> may not be <see langword="null"/>.</exception>
        public GenericList(IList list)
        {
            if (list == null) throw new ArgumentNullException("list");
            m_List = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericList{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="System.ArgumentNullException"><c>list</c> may not be <see langword="null"/>.</exception>
        public GenericList(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");
            m_ListGeneric = list;
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="GenericList{T}" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.IndexOf(item);
            }
            return m_List.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="GenericList{T}" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="GenericList{T}" />.</param>
        public void Insert(int index, T item)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Insert(index, item);
                return;
            }
            m_List.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="GenericList{T}" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.RemoveAt(index);
                return;
            }
            m_List.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public T this[int index]
        {
            get
            {
                if (m_ListGeneric != null) {
                    return m_ListGeneric[index];
                }
                return m_List[index] as T;
            }
            set
            {
                if (m_ListGeneric != null) {
                    m_ListGeneric[index] = value;
                    return;
                }
                m_List[index] = value;
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="GenericList{T}" />.</param>
        public void Add(T item)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Add(item);
                return;
            }
            m_List.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="GenericList{T}" />.
        /// </summary>
        public void Clear()
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Clear();
                return;
            }
            m_List.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="GenericList{T}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="GenericList{T}" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="GenericList{T}" />; otherwise, false.</returns>
        public bool Contains(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.Contains(item);
            }
            return m_List.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.CopyTo(array, arrayIndex);
                return;
            }
            m_List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="GenericList{T}" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                if (m_ListGeneric != null) {
                    return m_ListGeneric.Count;
                }
                return m_List.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GenericList{T}" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get
            {
                if (m_ListGeneric != null) {
                    return m_ListGeneric.IsReadOnly;
                }
                return m_List.IsReadOnly;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="GenericList{T}" />.</param>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.Remove(item);
            }
            m_List.Remove(item);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="GenericList{T}" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class ListEnumerator : IEnumerator<T>
        {
            private GenericList<T> m_Parent;
            private IEnumerator m_EnumeratorObj;
            private IEnumerator<T> m_EnumeratorGen;

            public ListEnumerator(GenericList<T> parent)
            {
                m_Parent = parent;
                if (m_Parent.m_ListGeneric != null) {
                    m_EnumeratorGen = m_Parent.m_ListGeneric.GetEnumerator();
                } else if (m_Parent.m_List != null) {
                    m_EnumeratorObj = m_Parent.m_List.GetEnumerator();
                }
            }

            public T Current
            {
                get
                {
                    if (m_EnumeratorGen != null) return m_EnumeratorGen.Current;
                    if (m_EnumeratorObj != null) return m_EnumeratorObj.Current as T;
                    return null;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (m_EnumeratorGen != null) return m_EnumeratorGen.MoveNext();
                if (m_EnumeratorObj != null) return m_EnumeratorObj.MoveNext();
                return false;
            }

            public void Reset()
            {
                if (m_EnumeratorGen != null) m_EnumeratorGen.Reset();
                if (m_EnumeratorObj != null) m_EnumeratorObj.Reset();
            }

            public void Dispose()
            {
                if (m_EnumeratorGen != null) m_EnumeratorGen.Dispose();
            }
        }
    }
}
