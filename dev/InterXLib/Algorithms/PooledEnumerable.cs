/***************************************************************************
 *                            From Map.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Algorithms
{
    public interface IPooledEnumerable : IEnumerable
    {
        void Free();
    }

    public interface IPooledEnumerable<T> : IPooledEnumerable, IEnumerable<T>
    {

    }

    public interface IPooledEnumerator<T> : IEnumerator<T>
    {
        void Free();
    }

	class PooledEnumerable<T> : IPooledEnumerable<T>, IDisposable
	{
		private IPooledEnumerator<T> m_Enumerator;

		private static Queue<PooledEnumerable<T>> m_InstancePool = new Queue<PooledEnumerable<T>>();

		public static PooledEnumerable<T> Instantiate(IPooledEnumerator<T> etor)
		{
			PooledEnumerable<T> e = null;

			lock (m_InstancePool) {
				if ( m_InstancePool.Count > 0 ) {
					e = m_InstancePool.Dequeue();
					e.m_Enumerator = etor;
				}
			}

			if (e == null )
				e = new PooledEnumerable<T>( etor );

			return e;
		}

		private PooledEnumerable(IPooledEnumerator<T> etor)
		{
			m_Enumerator = etor;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (m_Enumerator == null)
				throw new ObjectDisposedException("PooledEnumerable", "GetEnumerator() called after Free()");

			return m_Enumerator;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if ( m_Enumerator == null )
				throw new ObjectDisposedException( "PooledEnumerable", "GetEnumerator() called after Free()" );

			return m_Enumerator;
		}

		public void Free()
		{
			if ( m_Enumerator != null) {
					m_Enumerator.Free();
					m_Enumerator = null;
			}

			lock (m_InstancePool) {
				if (m_InstancePool.Count < 200) // Arbitrary
					m_InstancePool.Enqueue( this );
			}
		}

		public void Dispose()
		{
			if (m_Enumerator != null) {
				m_Enumerator.Free();
				m_Enumerator = null;
			}
		}
	}
}
