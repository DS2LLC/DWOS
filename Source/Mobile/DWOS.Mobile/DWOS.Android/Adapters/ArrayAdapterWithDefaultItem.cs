using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace DWOS.Android
{
    /// <summary>
    /// <see cref="ArrayAdapterWithDefaultItem"/> inherits from <see cref="ArrayAdapter<T>"/> while providing the capability
    /// of providing a "Hint" item for Spinners
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayAdapterWithDefaultItem<T> : ArrayAdapter<T>
    {
        #region Methods

        public ArrayAdapterWithDefaultItem(Context context, int textViewResourceId, IList<T> objects)
            : base(context, textViewResourceId, objects)
        {

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            
            if (view == null)
                view = base.GetView(position, convertView, parent);

            if (position == (Count - 1))
            {
                var textView = view.FindViewById<TextView>(global::Android.Resource.Id.Text1);
                textView.Text = string.Empty;
                textView.Hint = GetItem(position).ToString();
            }

            return view;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            const string hintTag = "HINT_VIEW";

            if (position == (Count - 1))
            {
                // Re-use or create 0-height TextView
                if (convertView?.Tag?.ToString() == hintTag)
                {
                    return convertView;
                }
                else
                {
                    var textView = new TextView(Context);
                    textView.LayoutParameters = new AbsListView.LayoutParams(0, 0);
                    textView.Tag = hintTag;

                    return textView;
                }
            }
            else
            {
                // Do not re-use the hint's drop down view
                if (convertView?.Tag?.ToString() == hintTag)
                {
                    return base.GetDropDownView(position, null, parent);
                }
                else
                {
                    return base.GetDropDownView(position, convertView, parent);
                }
            }
        }

        #endregion
    }
}