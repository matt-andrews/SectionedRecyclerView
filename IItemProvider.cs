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
    internal interface IItemProvider
    {
        int SectionCount { get; }

        int GetItemCount(int sectionIndex);

        bool ShowHeadersForEmptySections();

        bool ShowFooters();
    }
}