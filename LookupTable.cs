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
    internal class LookupTable
    {
        private Dictionary<int, int> _dict;
        private List<int> _listList;
        private int[] _list;

        public int this[int i] => _dict[i];
        public LookupTable()
        {
            _dict = new Dictionary<int, int>();
            _listList = new List<int>();
        }
        public void Clear()
        {
            _listList = new List<int>();
            _dict = new Dictionary<int, int>();
        }
        public void Add(int k, int v)
        {
            _dict.Add(k, v);
            _listList.Add(k);
        }

        public int Count()
        {
            return _list.Length;
        }
        public int AbsOfSection(int section)
        {
            return _list[section];
        }
        public bool ContainsKey(int k)
        {
            return _dict.ContainsKey(k);
        }
        public void Lock()
        {
            _list = _listList.ToArray();
        }
        public int[] Keys()
        {
            return _list;
        }

    }
}