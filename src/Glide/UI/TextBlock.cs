////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) GHI Electronics, LLC.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
//using Microsoft.SPOT;
//using Microsoft.SPOT.Presentation.Media;
using GHI.Glide.Display;
using GHI.Glide.Geom;
using nanoFramework.Presentation.Media;
using nanoFramework.UI;

namespace GHI.Glide.UI
{
    /// <summary>
    /// The TextBlock component is a multiline text field.
    /// </summary>
    public class TextBlock : DisplayObject
    {
        private bool _pressed = false;

        /// <summary>
        /// Creates a new TextBlock.
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="alpha">Alpha</param>
        /// <param name="x">X-axis position.</param>
        /// <param name="y">Y-axis position.</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public TextBlock(string name, ushort alpha, int x, int y, int width, int height)
        {
            Name = name;
            Alpha = alpha;
            X = x;
            Y = y;
            Width = width;
            Height = height;

            // Default
            Text = string.Empty;
            TextAlign = HorizontalAlignment.Left;
            TextVerticalAlign = VerticalAlignment.Top;
            Font = FontManager.GetFont(FontManager.FontType.droid_reg12);
            FontColor = Colors.Black;
        }

        /// <summary>
        /// Renders the TextBlock onto it's parent container's graphics.
        /// </summary>
        public override void Render()
        {
            int x = Parent.X + X;
            int y = Parent.Y + Y;

            uint flags = Bitmap.DT_WordWrap;
            if (TextAlign == HorizontalAlignment.Left)
                flags += Bitmap.DT_AlignmentLeft;
            else if (TextAlign == HorizontalAlignment.Center)
                flags += Bitmap.DT_AlignmentCenter;
            else if (TextAlign == HorizontalAlignment.Right)
                flags += Bitmap.DT_AlignmentRight;

            if (ShowBackColor)
                Parent.Graphics.DrawRectangle(Rect, BackColor, Alpha);

            Size size = FontManager.GetSize(Font, Text);
            if (TextVerticalAlign == VerticalAlignment.Middle)
                y += (Height - size.Height) / 2;
            else if (TextVerticalAlign == VerticalAlignment.Bottom)
                y += Height - size.Height;

            Parent.Graphics.DrawTextInRect(Text, x, y, Width, Height, flags, FontColor, Font);
        }

        /// <summary>
        /// Handles the touch down event.
        /// </summary>
        /// <param name="e">Touch event arguments.</param>
        /// <returns>Touch event arguments.</returns>
        public override TouchEventArgs OnTouchDown(TouchEventArgs e)
        {
            // Only register a press if our tap event has listeners.
            if (!TapEventIsNull() && Rect.Contains(e.Point))
            {
                _pressed = true;
                e.StopPropagation();
            }

            return e;
        }

        /// <summary>
        /// Handles the touch up event.
        /// </summary>
        /// <param name="e">Touch event arguments.</param>
        /// <returns>Touch event arguments.</returns>
        public override TouchEventArgs OnTouchUp(TouchEventArgs e)
        {
            if (_pressed && Rect.Contains(e.Point))
            {
                _pressed = false;

                if (For != null)
                {
                    if (For is CheckBox)
                    {
                        CheckBox checkBox = (CheckBox)For;
                        checkBox.Checked = !checkBox.Checked;
                        checkBox.TriggerTapEvent(checkBox);
                    }
                    else if (For is RadioButton)
                    {
                        // Radio buttons have rules for changing their selected
                        // value so we must call a toggle function.
                        ((RadioButton)For).Toggle();
                    }
                }
                else
                {
                    TriggerTapEvent(this);
                }

                e.StopPropagation();
            }

            return e;
        }

        /// <summary>
        /// A string containing the text.
        /// </summary>
        public string Text;

        /// <summary>
        /// Text horizonal alignement within the textfield.
        /// </summary>
        public HorizontalAlignment TextAlign;

        /// <summary>
        /// Textfield's vertical alignement within the TextBlock.
        /// </summary>
        public VerticalAlignment TextVerticalAlign;

        /// <summary>
        /// Font used by the text.
        /// </summary>
        public Font Font;

        /// <summary>
        /// Indicates the font color.
        /// </summary>
        public Color FontColor;

        /// <summary>
        /// Background color.
        /// </summary>
        public Color BackColor;

        /// <summary>
        /// Indicates whether or not a background color will be shown.
        /// </summary>
        public bool ShowBackColor = false;

        /// <summary>
        /// Indicates a CheckBox or RadioButton this label can toggle.
        /// </summary>
        public DisplayObject For;
    }
}
