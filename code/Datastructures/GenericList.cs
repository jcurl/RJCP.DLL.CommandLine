namespace RJCP.Core.Datastructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper class around a <see cref="IList"/> object.
    /// </summary>
    /// <typeparam name="T">The object type to wrap, which must be a reference type.</typeparam>
    /// <remarks>
    /// Allows an effective wrapper around a <see cref="IList"/> object to something
    /// requiring a generic list of a particular type.
    /// </remarks>
    public class GenericList<T> : IList<T> where T : class
    {
        private IList m_List;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericList{T}"/> class.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> may not be <see langword="null"/>.</exception>
        /// <remarks>
        /// Provide a list that should be wrapped. The list you provided remains the master. Modifying the
        /// collection using this object will also modify the list given in this constructor. You must be aware
        /// otherwise unintended side effects may occur.
        /// </remarks>
        public GenericList(IList list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            m_List = list;
        }

        /// <summary>
        /// Inserts an item to the <see cref="GenericList{T}" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="GenericList{T}" />.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index.</exception>
        /// <exception cref="NotSupportedException">
        /// The list is read-only.
        /// <para>- or -</para>
        /// The list has a fixed size.
        /// </exception>
        public void Insert(int index, T item)
        {
            m_List.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="GenericList{T}" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index.</exception>
        /// <exception cref="NotSupportedException">
        /// The list is read-only.
        /// <para>- or -</para>
        /// The list has a fixed size.
        /// </exception>
        public void RemoveAt(int index)
        {
            m_List.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index.</exception>
        /// <exception cref="NotSupportedException">The property is set and the list is read-only.</exception>
        public T this[int index]
        {
            get { return ConvertObject(m_List[index]); }
            set { m_List[index] = value; }
        }

        /// <summary>
        /// Adds an item to the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="GenericList{T}" />.</param>
        /// <exception cref="NotSupportedException">
        /// The list is read-only.
        /// <para>- or -</para>
        /// The list has a fixed size.
        /// </exception>
        public void Add(T item)
        {
            m_List.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="GenericList{T}" />.
        /// </summary>
        /// <exception cref="NotSupportedException">The list is read-only.</exception>
        public void Clear()
        {
            m_List.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="GenericList{T}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="GenericList{T}" />.</param>
        /// <returns><see langword="true"/> if <paramref name="item" /> is found in the <see cref="GenericList{T}" />;
        /// otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }

        /// <summary>
        /// Copies the content of the list to a specified array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of
        /// the elements copied from <see cref="GenericList{T}"/>. The <see cref="Array"/> must
        /// have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array"/> is multidimensional.
        /// <para>- or -</para>
        /// The number of elements in the source <see cref="GenericList{T}"/> is greater than the available space from
        /// <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex is less than zero");
            if (array.Rank != 1) throw new ArgumentException("array is multidimensional", nameof(array));
            if (m_List.Count + arrayIndex > array.Length) throw new ArgumentException("insufficient space to copy", nameof(array));

            int offset = 0;
            foreach (object element in m_List) {
                array[arrayIndex + offset] = ConvertObject(element);
                offset++;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="GenericList{T}" />.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="GenericList{T}" />.</value>
        public int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="GenericList{T}" /> is read-only.
        /// </summary>
        /// <value><see langword="true"/> if this instance is read only; otherwise, <see langword="false"/>.</value>
        public bool IsReadOnly
        {
            get { return m_List.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="GenericList{T}" />.</param>
        /// <returns><see langword="true"/> if <paramref name="item" /> was successfully removed
        /// from the <see cref="GenericList{T}" />; otherwise, <see langword="false"/>. This
        /// method also returns <see langword="false"/> if <paramref name="item" /> is not found
        /// in the original <see cref="GenericList{T}" />.</returns>
        /// <exception cref="NotSupportedException">
        /// The list is read-only.
        /// <para>- or -</para>
        /// The list has a fixed size.
        /// </exception>
        public bool Remove(T item)
        {
            if (m_List.Contains(item)) {
                m_List.Remove(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="GenericList{T}" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="GenericList{T}" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        /// <summary>
        /// Converts the object to the type of the list.
        /// </summary>
        /// <param name="item">The item that should be converted.</param>
        /// <returns>The converted object type T.</returns>
        /// <remarks>
        /// Override this object and provide an implementation for this method if the object shouldn't
        /// just be type casted using the <c>as</c> operator. For example, a <c>GenericList&lt;string&gt;</c>
        /// might override this method to call <c>item.ToString()</c>. Other implementations might want
        /// to create new object types.
        /// <para>Note, as the base implementation is the original list given in the constructor, this
        /// object is temporary and will be recreated every time when it is needed, it is the original
        /// object that is always stored as this is a wrapper.</para>
        /// </remarks>
        protected virtual T ConvertObject(object item)
        {
            return item as T;
        }

        private sealed class ListEnumerator : IEnumerator<T>
        {
            private GenericList<T> m_Parent;
            private IEnumerator m_EnumeratorObj;

            public ListEnumerator(GenericList<T> parent)
            {
                m_Parent = parent;
                m_EnumeratorObj = parent.m_List.GetEnumerator();
            }

            public T Current { get { return m_Parent.ConvertObject(m_EnumeratorObj.Current); } }

            object IEnumerator.Current { get { return Current; } }

            public bool MoveNext() { return m_EnumeratorObj.MoveNext(); }

            public void Reset() { m_EnumeratorObj.Reset(); }

            public void Dispose()
            {
                IDisposable disposable = m_EnumeratorObj as IDisposable;
                if (disposable != null) disposable.Dispose();
                m_EnumeratorObj = null;
            }
        }
    }
}
