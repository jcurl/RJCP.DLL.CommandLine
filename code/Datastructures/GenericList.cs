namespace RJCP.Core.Datastructures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class GenericList<T> : IList<T> where T:class
    {
        private IList<T> m_ListGeneric;
        private IList m_List;

        public GenericList(IList list)
        {
            if (list == null) throw new ArgumentNullException("list");
            m_List = list;
        }

        public GenericList(IList<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");
            m_ListGeneric = list;
        }

        public int IndexOf(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.IndexOf(item);
            }
            return m_List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Insert(index, item);
                return;
            }
            m_List.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.RemoveAt(index);
                return;
            }
            m_List.RemoveAt(index);
        }

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

        public void Add(T argument)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Add(argument);
                return;
            }
            m_List.Add(argument);
        }

        public void Clear()
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.Clear();
                return;
            }
            m_List.Clear();
        }

        public bool Contains(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.Contains(item);
            }
            return m_List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (m_ListGeneric != null) {
                m_ListGeneric.CopyTo(array, arrayIndex);
                return;
            }
            m_List.CopyTo(array, arrayIndex);
        }

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

        public bool Remove(T item)
        {
            if (m_ListGeneric != null) {
                return m_ListGeneric.Remove(item);
            }
            m_List.Remove(item);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class ListEnumerator<T> : IEnumerator<T> where T:class
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

            public void Dispose()
            {
                if (m_EnumeratorGen != null) m_EnumeratorGen.Dispose();
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
        }
    }
}
