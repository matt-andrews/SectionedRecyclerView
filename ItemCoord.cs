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
    public class ItemCoord
    {
        private readonly int _section;
        private readonly int _relativePos;

        public ItemCoord(int section, int relativePos)
        {
            this._section = section;
            this._relativePos = relativePos;
        }

        public int Section()
        {
            return _section;
        }

        public int RelativePos()
        {
            return _relativePos;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemCoord coord &&
                   _section == coord._section &&
                   _relativePos == coord._relativePos;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_section, _relativePos);
        }

        public override string ToString()
        {
            return _section + ":" + _relativePos;
        }
    }
}