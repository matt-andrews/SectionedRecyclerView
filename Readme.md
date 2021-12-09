# SectionedRecyclerView

This is a port of the java library originally written by https://github.com/afollestad (the original library has since been taken down).

While porting this library I made some changes to increase the efficiency. I have not measured the exact performance, but there is no degredation with thousands of entries and hundreds of sections.

To use this library your adapter should inherit `SectionedRecyclerViewAdapter` instead of `RecyclerView.Adapter` and `SectionedViewHolder` instead of `RecyclerView.ViewHolder`. Override the abstract methods, and attach the adapter!

### Example

```c#
public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = null;
            switch ((SectionedRecyclerViewAdapterViewType)viewType)
            {
                case SectionedRecyclerViewAdapterViewType.Header:
                    v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_photo_header, parent, false);
                    return new HeaderView(v, this);
                case SectionedRecyclerViewAdapterViewType.Footer:
                    v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_photo_footer, parent, false);
                    return new FooterView(v, this);
                default:
                    v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_photo_recycler, parent, false);
                    return new ItemView(v, this);
            }

        }
public override void OnBindHeaderViewHolder(Java.Lang.Object holder, int section, bool expanded)
        {
            var view = holder as HeaderView;
            ...
        }
public override void OnBindFooterViewHolder(Java.Lang.Object holder, int section) 
        {
            var view = holder as FooterView;
            ...
        }
public override void OnBindViewHolder(Java.Lang.Object holder, int section, int relativePosition, int absolutePosition)
        {
            var view = holder as ItemView;
            ...
        }
```