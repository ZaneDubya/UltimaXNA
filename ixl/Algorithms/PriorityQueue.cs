using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Algorithms
{
    public class PriorityQueue<TValue> where TValue : class, IComparable<TValue>
    {
        private TValue[] m_elements;
        private int m_size;

        public PriorityQueue(int n)
        {
            m_elements = new TValue[n];
            m_size = 0;
        }

        public void Insert(TValue x)
        {
            if (m_size == m_elements.Length - 1) Resize(m_elements.Length * 2);

            m_elements[++m_size] = x;
            Swim(m_size);
        }

        public TValue DelMin()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Priority queue underflow");
            }

            Exch(1, m_size);

            var min = m_elements[m_size--];

            Sink(1);
            m_elements[m_size + 1] = null;

            if (m_size > 0 && m_size == (m_elements.Length - 1) / 4) Resize(m_elements.Length / 2);

            return min;
        }

        public TValue Min()
        {
            return m_elements[0];
        }

        public bool IsEmpty()
        {
            return m_size == 0;
        }

        private void Resize(int capacity)
        {
            var temp = new TValue[capacity];
            for (int i = 1; i <= m_size; i++) temp[i] = m_elements[i];
            m_elements = temp;
        }

        private void Exch(int n, int m)
        {
            TValue x = m_elements[n];
            m_elements[n] = m_elements[m];
            m_elements[m] = x;
        }

        private void Swim(int k)
        {
            while (k > 1 && Greater(k / 2, k))
            {
                Exch(k, k / 2);
                k = k / 2;
            }
        }

        private void Sink(int k)
        {
            while (2 * k <= m_size)
            {
                int j = 2 * k;
                if (j < m_size && Greater(j, j + 1)) j++;
                if (!Greater(k, j)) break;
                Exch(k, j);
                k = j;
            }
        }

        private bool Greater(int i, int j)
        {
            if (j > m_size) return false;
            return (m_elements[i]).CompareTo(m_elements[j]) > 0;
        }
    }
}
