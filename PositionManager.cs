using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SectionedRecyclerView
{
    internal class PositionManager : SectionedViewHolder.IPositionDelegate
    {
        private readonly LookupTable headerLocationMap;
        private readonly Dictionary<int, int> footerLocationMap;
        private readonly Dictionary<int, bool> collapsedSectionMap;
        private IItemProvider itemProvider;
        private bool hasInvalidated;
        /// <summary>
        /// If this is false, IsFooter() will always return false
        /// </summary>
        public bool AllowFooters { get; set; } = false;
        /// <summary>
        /// If this is false, IsSectionExpanded() will always return true
        /// </summary>
        public bool IsCollapsable { get; set; } = false;

        public PositionManager()
        {
            this.headerLocationMap = new LookupTable();
            this.footerLocationMap = new Dictionary<int, int>();
            this.collapsedSectionMap = new Dictionary<int, bool>();
        }

        public bool HasInvalidated()
        {
            return hasInvalidated;
        }

        public int Invalidate(IItemProvider itemProvider)
        {
            this.hasInvalidated = true;
            this.itemProvider = itemProvider;
            int count = 0;
            headerLocationMap.Clear();
            footerLocationMap.Clear();
            for (int s = 0; s < itemProvider.SectionCount; s++)
            {
                int itemCount = itemProvider.GetItemCount(s);
                if (collapsedSectionMap.ContainsKey(s))
                {
                    headerLocationMap.Add(count, s);
                    count += 1;
                    continue;
                }
                if (itemProvider.ShowHeadersForEmptySections() || (itemCount > 0))
                {
                    headerLocationMap.Add(count, s);
                    count += itemCount + 1;
                    if (itemProvider.ShowFooters())
                    {
                        footerLocationMap.Add(count, s);
                        count += 1;
                    }
                }
            }
            headerLocationMap.Lock();
            return count;
        }

        public bool IsHeader(int absolutePosition)
        {
            return headerLocationMap.ContainsKey(absolutePosition);
        }

        public bool IsFooter(int absolutePosition)
        {
            if (!AllowFooters)
                return false;
            return footerLocationMap.ContainsKey(absolutePosition);
        }

        public int SectionId(int absolutePosition)
        {
            if (!headerLocationMap.ContainsKey(absolutePosition))
            {
                return -1;
            }
            return headerLocationMap[absolutePosition];
        }

        public int FooterId(int absolutePosition)
        {
            if (!footerLocationMap.ContainsKey(absolutePosition))
            {
                return -1;
            }
            return footerLocationMap[absolutePosition];
        }

        public int SectionHeaderIndex(int section)
        {
            if (headerLocationMap.Count() >= section)
            {
                return headerLocationMap.AbsOfSection(section);
            }
            else
            {
                return -1;
            }
        }

        public int SectionFooterIndex(int section)
        {
            foreach (int key in footerLocationMap.Keys)
            {
                if (footerLocationMap[key] == section)
                {
                    return key;
                }
            }
            return -1;
        }

        /// <summary>
        /// Converts an absolute position to a relative position and section.
        /// </summary>
        /// <param name="absolutePosition"></param>
        /// <returns></returns>
        public ItemCoord RelativePosition(int absolutePosition)
        {
            if (headerLocationMap.ContainsKey(absolutePosition))
            {
                return new ItemCoord(headerLocationMap[absolutePosition], -1);
            }
            int lastSectionIndex = -1;
            foreach (int sectionIndex in headerLocationMap.Keys())
            {
                if (absolutePosition > sectionIndex)
                {
                    lastSectionIndex = sectionIndex;
                }
                else
                {
                    break;
                }
            }
            return new ItemCoord(
                headerLocationMap[lastSectionIndex], absolutePosition - lastSectionIndex - 1);
        }

        /// <summary>
        /// Converts a relative position (index inside of a section) to an absolute position (index out of all items and headers).
        /// </summary>
        /// <param name="sectionIndex"></param>
        /// <param name="relativeIndex"></param>
        /// <returns></returns>
        public int AbsolutePosition(int sectionIndex, int relativeIndex)
        {
            if (sectionIndex < 0 || sectionIndex > itemProvider.SectionCount - 1)
            {
                return -1;
            }
            int sectionHeaderIndex = SectionHeaderIndex(sectionIndex);
            if (relativeIndex > itemProvider.GetItemCount(sectionIndex) - 1)
            {
                return -1;
            }
            return sectionHeaderIndex + (relativeIndex + 1);
        }

        /// <summary>
        /// Converts a relative position (index inside of a section) to an absolute position (index out of all items and headers).
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <returns></returns>
        public int AbsolutePosition(ItemCoord relativePosition)
        {
            return AbsolutePosition(relativePosition.Section(), relativePosition.RelativePos());
        }

        public void ExpandSection(int section)
        {
            if (section < 0 || section > itemProvider.SectionCount - 1)
            {
                throw new ArgumentOutOfRangeException("Section " + section + " is out of bounds.");
            }
            collapsedSectionMap.Remove(section);
        }

        public void CollapseSection(int section)
        {
            if (section < 0 || section > itemProvider.SectionCount - 1)
            {
                throw new ArgumentOutOfRangeException("Section " + section + " is out of bounds.");
            }
            collapsedSectionMap.Add(section, true);
        }

        public void ToggleSectionExpanded(int section)
        {
            if (collapsedSectionMap.ContainsKey(section))
            {
                ExpandSection(section);
            }
            else
            {
                CollapseSection(section);
            }
        }

        public void ExpandAllSections()
        {
            for (int i = 0; i < itemProvider.SectionCount; i++)
            {
                ExpandSection(i);
            }
        }

        public void CollapseAllSections()
        {
            for (int i = 0; i < itemProvider.SectionCount; i++)
            {
                CollapseSection(i);
            }
        }

        public bool IsSectionExpanded(int section)
        {
            if (!IsCollapsable)
                return true;
            if (section < 0 || section > itemProvider.SectionCount - 1)
            {
                throw new ArgumentOutOfRangeException("Section " + section + " is out of bounds.");
            }
            return !collapsedSectionMap.ContainsKey(section);
        }
    }
}