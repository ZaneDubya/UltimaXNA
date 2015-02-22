using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Algorithms
{
    public class PriorityQueue<TValue> where TValue : class, IComparable<TValue>
    {
        private TValue[] _elements;
        private int _size;

        public PriorityQueue(int n)
        {
            _elements = new TValue[n];
            _size = 0;
        }

        public void Insert(TValue x)
        {
            if (_size == _elements.Length - 1) Resize(_elements.Length * 2);

            _elements[++_size] = x;
            Swim(_size);
        }

        public TValue DelMin()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Priority queue underflow");
            }

            Exch(1, _size);

            var min = _elements[_size--];

            Sink(1);
            _elements[_size + 1] = null;

            if (_size > 0 && _size == (_elements.Length - 1) / 4) Resize(_elements.Length / 2);

            return min;
        }

        public TValue Min()
        {
            return _elements[0];
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        private void Resize(int capacity)
        {
            var temp = new TValue[capacity];
            for (int i = 1; i <= _size; i++) temp[i] = _elements[i];
            _elements = temp;
        }

        private void Exch(int n, int m)
        {
            TValue x = _elements[n];
            _elements[n] = _elements[m];
            _elements[m] = x;
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
            while (2 * k <= _size)
            {
                int j = 2 * k;
                if (j < _size && Greater(j, j + 1)) j++;
                if (!Greater(k, j)) break;
                Exch(k, j);
                k = j;
            }
        }

        private bool Greater(int i, int j)
        {
            if (j > _size) return false;
            return (_elements[i]).CompareTo(_elements[j]) > 0;
        }
    }
}
