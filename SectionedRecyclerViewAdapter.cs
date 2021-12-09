using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SectionedRecyclerView
{
    public abstract class SectionedRecyclerViewAdapter : RecyclerView.Adapter, IItemProvider
    {
        private const string TAG = "SectionedRVAdapter";
        private readonly PositionManager positionManager;
        private GridLayoutManager layoutManager;
        private bool showHeadersForEmptySections;
        private bool showFooters;
        /// <summary>
        /// If this is false, IsFooter() will always return false
        /// </summary>
        public bool AllowFooters { get; set; } = false;
        /// <summary>
        /// If this is false, IsSectionExpanded() will always return true
        /// </summary>
        public bool IsCollapsable { get; set; } = false;

        public SectionedRecyclerViewAdapter()
        {
            positionManager = new PositionManager();
        }

        public void NotifySectionChanged(int section)
        {
            if (section < 0 || section > SectionCount - 1)
            {
                throw new ArgumentOutOfRangeException(
                    "Section " + section + " is out of range of existing sections.");
            }
            int sectionHeaderIndex = positionManager.SectionHeaderIndex(section);
            if (sectionHeaderIndex == -1)
            {
                throw new ArgumentOutOfRangeException("No header position mapped for section " + section);
            }
            int sectionItemCount = GetItemCount(section);
            if (sectionItemCount == 0)
            {
                Log.Debug(TAG, "There are no items in section " + section + " to notify.");
                return;
            }
            Log.Debug(TAG, "Invalidating " + sectionItemCount + " items starting at index " + sectionHeaderIndex);
            NotifyItemRangeChanged(sectionHeaderIndex, sectionItemCount);
        }

        public void ExpandSection(int section)
        {
            positionManager.ExpandSection(section);
            NotifyDataSetChanged();
        }

        public void CollapseSection(int section)
        {
            positionManager.CollapseSection(section);
            NotifyDataSetChanged();
        }

        public void ExpandAllSections()
        {
            if (!positionManager.HasInvalidated())
            {
                positionManager.Invalidate(this);
            }
            positionManager.ExpandAllSections();
            NotifyDataSetChanged();
        }

        public void CollapseAllSections()
        {
            if (!positionManager.HasInvalidated())
            {
                positionManager.Invalidate(this);
            }
            positionManager.CollapseAllSections();
            NotifyDataSetChanged();
        }

        public void ToggleSectionExpanded(int section)
        {
            positionManager.ToggleSectionExpanded(section);
            NotifyDataSetChanged();
        }

        public abstract int SectionCount { get; }

        public abstract int GetItemCount(int section);

        public abstract void OnBindHeaderViewHolder(Java.Lang.Object holder, int section, bool expanded);

        public abstract void OnBindFooterViewHolder(Java.Lang.Object holder, int section);

        public abstract void OnBindViewHolder(
            Java.Lang.Object holder, int section, int relativePosition, int absolutePosition);

        public bool IsHeader(int position)
        {
            return positionManager.IsHeader(position);
        }

        public bool IsFooter(int position)
        {
            if (!AllowFooters)
                return false;
            return positionManager.IsFooter(position);
        }

        public bool IsSectionExpanded(int section)
        {
            if (!IsCollapsable)
                return true;
            return positionManager.IsSectionExpanded(section);
        }

        public int GetSectionHeaderIndex(int section)
        {
            return positionManager.SectionHeaderIndex(section);
        }

        public int GetSectionFooterIndex(int section)
        {
            return positionManager.SectionFooterIndex(section);
        }


        /// <summary>
        /// Toggle whether or not section headers are shown when a section has no items. Makes a call to NotifyDataSetChanged()
        /// </summary>
        /// <param name="show"></param>
        public void ShouldShowHeadersForEmptySections(bool show)
        {
            showHeadersForEmptySections = show;
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Toggle whether or not section footers are shown at the bottom of each section. Makes a call to NotifyDataSetChanged()
        /// </summary>
        /// <param name="show"></param>
        public void ShouldShowFooters(bool show)
        {
            showFooters = show;
            NotifyDataSetChanged();
        }

        public void SetLayoutManager(GridLayoutManager lm)
        {
            layoutManager = lm;
            if (lm == null)
            {
                return;
            }
            layoutManager.SetSpanSizeLookup(new SpanSizeLookup(this));
        }
        private class SpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            private SectionedRecyclerViewAdapter _adapter;
            public SpanSizeLookup(SectionedRecyclerViewAdapter adapter)
            {
                this.SpanIndexCacheEnabled = true;
                _adapter = adapter;
            }
            public override int GetSpanSize(int position)
            {
                if (_adapter.IsHeader(position))// || _adapter.IsFooter(position))
                {
                    return _adapter.layoutManager.SpanCount;
                }
                return 1;
            }
        }
        /// <summary>
        /// This was being used in SpanSizeLookup class, but was being called uselessly causing tons of lag.
        /// </summary>
        /// <param name="fullSpanSize"></param>
        /// <param name="section"></param>
        /// <param name="relativePosition"></param>
        /// <param name="absolutePosition"></param>
        /// <returns></returns>
        protected int GetRowSpan(
            int fullSpanSize, int section, int relativePosition, int absolutePosition)
        {
            return 1;
        }

        /// <summary>
        /// Converts an absolute position to a relative position and section.
        /// </summary>
        /// <param name="absolutePosition"></param>
        /// <returns></returns>
        public ItemCoord GetRelativePosition(int absolutePosition)
        {
            return positionManager.RelativePosition(absolutePosition);
        }

        /// <summary>
        /// Converts a relative position (index inside of a section) to an absolute position (index out of all items and headers).
        /// </summary>
        /// <param name="sectionIndex"></param>
        /// <param name="relativeIndex"></param>
        /// <returns></returns>
        public int GetAbsolutePosition(int sectionIndex, int relativeIndex)
        {
            return positionManager.AbsolutePosition(sectionIndex, relativeIndex);
        }

        /// <summary>
        /// Converts a relative position (index inside of a section) to an absolute position (index out of all items and headers).
        /// </summary>
        /// <param name="relativePosition"></param>
        /// <returns></returns>
        public int GetAbsolutePosition(ItemCoord relativePosition)
        {
            return positionManager.AbsolutePosition(relativePosition);
        }
        public override int ItemCount => positionManager.Invalidate(this);

        public bool ShowHeadersForEmptySections()
        {
            return showHeadersForEmptySections;
        }
        public bool ShowFooters()
        {
            return showFooters;
        }

        public sealed override long GetItemId(int position)
        {
            if (IsHeader(position))
            {
                int pos = positionManager.SectionId(position);
                return GetHeaderId(pos);
            }
            else if (IsFooter(position))
            {
                int pos = positionManager.FooterId(position);
                return GetFooterId(pos);
            }
            else
            {
                ItemCoord sectionAndPos = GetRelativePosition(position);
                return GetItemId(sectionAndPos.Section(), sectionAndPos.RelativePos());
            }
        }

        public long GetHeaderId(int section)
        {
            return base.GetItemId(section);
        }

        public long GetFooterId(int section)
        {
            return base.GetItemId(section) + GetItemCount(section);
        }

        public long GetItemId(int section, int position)
        {
            return base.GetItemId(position);
        }

        
        public sealed override int GetItemViewType(int position)
        {
            if (IsHeader(position))
            {
                return (int)SectionedRecyclerViewAdapterViewType.Header;
            }
            else if (IsFooter(position))
            {
                return (int)SectionedRecyclerViewAdapterViewType.Footer;
            }
            else
            {
                return (int)SectionedRecyclerViewAdapterViewType.Item;
            }
        }

        public int GetHeaderViewType(int section)
        {
            return (int)SectionedRecyclerViewAdapterViewType.Header;
        }

        public int GetFooterViewType(int section)
        {
            return (int)SectionedRecyclerViewAdapterViewType.Footer;
        }

        public virtual int GetItemViewType(int section, int relativePosition, int absolutePosition)
        {
            return (int)SectionedRecyclerViewAdapterViewType.Item;
        }

        [Obsolete("Use the abstract OnBindViewHolder() instead")]
        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((SectionedViewHolder)holder).SetPositionDelegate(positionManager);
            if (IsHeader(position))
            {
                StaggeredGridLayoutManager.LayoutParams layoutParams = null;
                if (holder.ItemView.LayoutParameters is GridLayoutManager.LayoutParams)
                {
                    layoutParams =
                        new StaggeredGridLayoutManager.LayoutParams(
                            ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                }
                else if (holder.ItemView.LayoutParameters is StaggeredGridLayoutManager.LayoutParams)
                {
                    layoutParams = (StaggeredGridLayoutManager.LayoutParams)holder.ItemView.LayoutParameters;
                }
                else
                    return;
                layoutParams.FullSpan = true;
                int sectionIndex = positionManager.SectionId(position);
                OnBindHeaderViewHolder(holder, sectionIndex, IsSectionExpanded(sectionIndex));

                holder.ItemView.LayoutParameters = layoutParams;
            }

            else
            {
                ItemCoord sectionAndPos = GetRelativePosition(position);

                OnBindViewHolder(
                    holder,
                    sectionAndPos.Section(),
                    // offset section view positions
                    sectionAndPos.RelativePos(),
                    position);
            }
        }

        public void OnBindViewHolder(Java.Lang.Object holder, int position, IList<Java.Lang.Object> payloads)
        {
            base.OnBindViewHolder((RecyclerView.ViewHolder)holder, position, payloads);
        }
    }
}