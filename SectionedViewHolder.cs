using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SectionedRecyclerView
{
    public class SectionedViewHolder : RecyclerView.ViewHolder
    {
        private IPositionDelegate _positionDelegate;

        public SectionedViewHolder(View itemView)
            : base(itemView)
        {
        }

        public void SetPositionDelegate(IPositionDelegate positionDelegate)
        {
            this._positionDelegate = positionDelegate;
        }

        protected ItemCoord GetRelativePosition()
        {
            return _positionDelegate.RelativePosition(AdapterPosition);
        }

        protected bool IsHeader()
        {
            return _positionDelegate.IsHeader(AdapterPosition);
        }

        protected bool IsFooter()
        {
            return _positionDelegate.IsFooter(AdapterPosition);
        }

        public interface IPositionDelegate
        {
            ItemCoord RelativePosition(int absolutePosition);

            bool IsHeader(int absolutePosition);

            bool IsFooter(int absolutePosition);
        }
    }
    public enum SectionedRecyclerViewAdapterViewType
    {
        Footer = -3,
        Header = -2,
        Item = -1
    }
}