////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) GHI Electronics, LLC.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
//using Microsoft.SPOT;
using GHI.Glide.Display;
using nanoFramework.UI;

namespace GHI.Glide.UI
{
    /// <summary>
    /// The ListItem class holds information relevant to a specific option in a list-based component.
    /// </summary>
    public class ListItem : DisplayObject, IListItem
    {
        private Font _font = FontManager.GetFont(FontManager.FontType.droid_reg12);

        /// <summary>
        /// Creates a new ListItem.
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="value">Value</param>
        public ListItem(string label, object value)
        {
            Label = label;
            Value = value;
            Height = 32;
        }

        /// <summary>
        /// Renders the ListItem onto the provided bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap this item will be drawn on.</param>
        public override void Render(Bitmap bitmap)
        {
            Width = Parent.Width;
            bitmap.DrawTextInRect(Label, X, Y + (Height - _font.Height) / 2, Width, _font.Height, Bitmap.DT_AlignmentCenter, Colors.Black, _font);
            bitmap.DrawLine(Colors.LightGray, 1, 0, Y + Height, Width, Y + Height);
        }

        /// <summary>
        /// A string of text that describes this item.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
    }
}
