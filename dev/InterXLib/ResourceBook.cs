using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib
{
    /// <summary>
    /// Manages a list of available chunked resources ("pages").
    /// </summary>
    public class ResourceBook
    {
        private int m_NumPages;
        private int m_Offset;

        private List<PagesAvailable> m_EmptyPages;
        private List<PagesInUse> m_InUsePages;
        public delegate void EventPagesReleased(PagesInUse pages);

        public ResourceBook(int num_pages, int first_page)
        {
            m_NumPages = num_pages;
            m_Offset = first_page;

            m_InUsePages = new List<PagesInUse>();

            m_EmptyPages = new List<PagesAvailable>();
            m_EmptyPages.Add(new PagesAvailable(0, m_NumPages));
        }

        public Bookmark RequestContiguousFreePages(int count)
        {
            for (int i = 0; i < m_EmptyPages.Count; i++)
            {
                if (m_EmptyPages[i].PageLength >= count)
                {
                    PagesAvailable available = m_EmptyPages[i];
                    PagesInUse new_pages = new PagesInUse(available.FirstPage, m_Offset, count, InternalEventPagesReleased);
                    available.FirstPage += count;
                    available.PageLength -= count;
                    m_InUsePages.Add(new_pages);
                    return new_pages.GetBookmark();
                }
            }

            Logging.Fatal(string.Format("RequestContiguousFreePages failed with page count of {0}.", count));
            return null;
        }

        private void InternalEventPagesReleased(PagesInUse pages)
        {
            // if these previously in use pages are contiguous with a free block of pages, add them to that block.
            bool block_found = false;
            for (int i = 0; i < m_EmptyPages.Count; i++)
            {
                if (m_EmptyPages[i].FirstPage == pages.FollowingPage)
                {
                    m_EmptyPages[i].FirstPage = pages.FirstPage;
                    m_EmptyPages[i].PageLength += pages.PageLength;
                    m_InUsePages.Remove(pages);
                    block_found = true;
                    break;
                }
                if (m_EmptyPages[i].FollowingPage == pages.FirstPage)
                {
                    m_EmptyPages[i].PageLength += pages.PageLength;
                    m_InUsePages.Remove(pages);
                    block_found = true;
                    break;
                }
            }
            // otherwise add a new free block of pages.
            if (!block_found)
            {
                m_EmptyPages.Add(new PagesAvailable(pages.FirstPage, pages.PageLength));
                m_InUsePages.Remove(pages);
            }

            if (m_ContiguousMonitorCount++ >= 32)
                MergeContiguousEmptyBlocks();
        }

        private int m_ContiguousMonitorCount = 0;
        public void MergeContiguousEmptyBlocks()
        {
            for (int i = 0; i < m_EmptyPages.Count; i++)
            {
                PagesAvailable parent = m_EmptyPages[i];
                for (int j = 0; j < m_EmptyPages.Count; j++)
                {
                    if (m_EmptyPages[j].FirstPage == parent.FirstPage)
                        continue;
                    if (m_EmptyPages[j].FirstPage == parent.FollowingPage)
                    {
                        parent.PageLength += m_EmptyPages[j].PageLength;
                        m_EmptyPages.RemoveAt(j);
                        j--;
                        continue;
                    }
                    if (m_EmptyPages[j].FollowingPage == parent.FirstPage)
                    {
                        parent.FirstPage = m_EmptyPages[j].FirstPage;
                        parent.PageLength += m_EmptyPages[j].PageLength;
                        m_EmptyPages.RemoveAt(j);
                        j--;
                        continue;
                    }
                }
            }
        }

        public int PagesFreeCount
        {
            get
            {
                int value = 0;
                for (int i = 0; i < m_EmptyPages.Count; i++)
                    value += m_EmptyPages[i].PageLength;
                return value;
            }
        }

        public int PagesFreeFragmentation
        {
            get
            {
                return m_EmptyPages.Count;
            }
        }


        private class PagesAvailable
        {
            public int FirstPage;
            public int PageLength;
            public int FollowingPage
            {
                get
                {
                    return FirstPage + PageLength;
                }
            }

            public PagesAvailable(int first, int length)
            {
                FirstPage = first;
                PageLength = length;
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", FirstPage, FirstPage + PageLength - 1);
            }
        }

        public class PagesInUse
        {
            private int m_UseCount;

            public int FirstPage;
            public int PageLength;
            public int FollowingPage
            {
                get
                {
                    return FirstPage + PageLength;
                }
            }

            private Bookmark m_Bookmark;
            EventPagesReleased m_OnReleased;

            public PagesInUse(int index, int offset, int length, EventPagesReleased on_released)
            {
                m_UseCount = 0;
                FirstPage = index;
                PageLength = length;
                m_OnReleased = on_released;
                m_Bookmark = new Bookmark(this, offset);
            }

            public Bookmark GetBookmark()
            {
                m_UseCount++;
                return m_Bookmark;
            }

            public void Release()
            {
                m_UseCount--;
                if (m_UseCount <= 0)
                {
                    m_Bookmark = null;
                    m_OnReleased(this);
                }
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", FirstPage, FirstPage + PageLength - 1);
            }
        }
    }



    public class Bookmark
    {
        private ResourceBook.PagesInUse m_Pages;
        private int m_Offset;

        public Bookmark(ResourceBook.PagesInUse index, int offset)
        {
            m_Pages = index;
            m_Offset = offset;
        }

        public int ExternalFirstPage
        {
            get
            {
                return m_Pages.FirstPage + m_Offset;
            }
        }

        public int PageLength
        {
            get
            {
                return m_Pages.PageLength;
            }
        }

        public void Release()
        {
            m_Pages.Release();
        }
    }
}
