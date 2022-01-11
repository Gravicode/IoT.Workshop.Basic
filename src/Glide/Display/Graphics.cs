////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) GHI Electronics, LLC.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
//using Microsoft.SPOT;
//using Microsoft.SPOT.Presentation.Media;
using GHI.Glide.Geom;
using nanoFramework.Presentation.Media;
using nanoFramework.UI;

namespace GHI.Glide.Display
{
    /// <summary>
    /// The Graphics class contains a set of methods you can use to draw on itself.
    /// </summary>
    public sealed class Graphics
    {
        private Bitmap _bitmap;

        /// <summary>
        /// Creates a new Graphics object.
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Graphics(int width, int height)
        {
            _bitmap = new Bitmap(width, height);
        }

        /// <summary>
        /// Clears the Graphics object.
        /// </summary>
        public void Clear()
        {
            _bitmap.Clear();
        }

        /// <summary>
        /// Disposes of the Graphics object.
        /// </summary>
        public void Dispose()
        {
            _bitmap.Dispose();
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="colorOutline">Color of the outline.</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="xRadius">X Radius</param>
        /// <param name="yRadius">Y Radius</param>
        public void DrawEllipse(Color colorOutline, int x, int y, int xRadius, int yRadius)
        {
            _bitmap.DrawEllipse(colorOutline, x, y, xRadius, yRadius);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="colorOutline">Color of the outline.</param>
        /// <param name="thicknessOutline">Thickness of the outline.</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="xRadius">X Radius</param>
        /// <param name="yRadius">Y Radius</param>
        /// <param name="colorGradientStart">Starting gradient color.</param>
        /// <param name="xGradientStart">Starting gradient X.</param>
        /// <param name="yGradientStart">Starting gradient Y.</param>
        /// <param name="colorGradientEnd">Ending gradient color.</param>
        /// <param name="xGradientEnd">Ending gradient X.</param>
        /// <param name="yGradientEnd">Ending gradient Y.</param>
        /// <param name="opacity">Opacity</param>
        public void DrawEllipse(Color colorOutline, int thicknessOutline, int x, int y, int xRadius, int yRadius, Color colorGradientStart, int xGradientStart, int yGradientStart, Color colorGradientEnd, int xGradientEnd, int yGradientEnd, ushort opacity)
        {
            _bitmap.DrawEllipse(colorOutline, thicknessOutline, x, y, xRadius, yRadius, colorGradientStart, xGradientStart, yGradientStart, colorGradientEnd, xGradientEnd, yGradientEnd, opacity);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="xDst">Destination X.</param>
        /// <param name="yDst">Destination Y.</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="xSrc">Source X.</param>
        /// <param name="ySrc">Source Y.</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void DrawImage(int xDst, int yDst, Bitmap bitmap, int xSrc, int ySrc, int width, int height)
        {
            _bitmap.DrawImage(xDst, yDst, bitmap, xSrc, ySrc, width, height);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="xDst">Destination X.</param>
        /// <param name="yDst">Destination Y.</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="xSrc">Source X.</param>
        /// <param name="ySrc">Source Y.</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="opacity">Opacity</param>
        public void DrawImage(int xDst, int yDst, Bitmap bitmap, int xSrc, int ySrc, int width, int height, ushort opacity)
        {
            _bitmap.DrawImage(xDst, yDst, bitmap, xSrc, ySrc, width, height, opacity);
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="thickness">Thickness</param>
        /// <param name="x0">Starting X.</param>
        /// <param name="y0">Starting Y.</param>
        /// <param name="x1">Ending X.</param>
        /// <param name="y1">Ending Y.</param>
        public void DrawLine(Color color, int thickness, int x0, int y0, int x1, int y1)
        {
            _bitmap.DrawLine(color, thickness, x0, y0, x1, y1);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="colorOutline">Color of the outline.</param>
        /// <param name="thicknessOutline">Thickness of the outline.</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="xCornerRadius">X corner radius.</param>
        /// <param name="yCornerRadius">Y corner radius.</param>
        /// <param name="colorGradientStart">Starting gradient color.</param>
        /// <param name="xGradientStart">Starting gradient X.</param>
        /// <param name="yGradientStart">Starting gradient Y.</param>
        /// <param name="colorGradientEnd">Ending gradient color.</param>
        /// <param name="xGradientEnd">Ending gradient X.</param>
        /// <param name="yGradientEnd">Ending gradient Y.</param>
        /// <param name="opacity">Opacity</param>
        public void DrawRectangle(Color colorOutline, int thicknessOutline, int x, int y, int width, int height, int xCornerRadius, int yCornerRadius, Color colorGradientStart, int xGradientStart, int yGradientStart, Color colorGradientEnd, int xGradientEnd, int yGradientEnd, ushort opacity)
        {
            _bitmap.DrawRectangle(colorOutline, thicknessOutline, x, y, width, height, xCornerRadius, yCornerRadius, colorGradientStart, xGradientStart, yGradientStart, colorGradientEnd, xGradientEnd, yGradientEnd, opacity);

        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="color">Color</param>
        /// <param name="opacity">Opacity</param>
        public void DrawRectangle(Rectangle rect, Color color, ushort opacity)
        {
            _bitmap.DrawRectangle(0, 0, rect.X, rect.Y, rect.Width, rect.Height, 0, 0, color, 0, 0, 0, 0, 0, opacity);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="font">Font</param>
        /// <param name="color">Color</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void DrawText(string text, Font font, Color color, int x, int y)
        {
            _bitmap.DrawText(text, font, color, x, y);
        }

        /// <summary>
        /// Draws text in a rectangle.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="dtFlags">Flags found in Bitmap.</param>
        /// <param name="color">Color</param>
        /// <param name="font">Font</param>
        public void DrawTextInRect(string text, int x, int y, int width, int height, uint dtFlags, Color color, Font font)
        {
            _bitmap.DrawTextInRect(text, x, y, width, height, dtFlags, color, font);
        }

        /// <summary>
        /// Draws text in a rectangle.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="xRelStart"></param>
        /// <param name="yRelStart"></param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="dtFlags">Flags found in Bitmap.</param>
        /// <param name="color">Color</param>
        /// <param name="font">Font</param>
        /// <returns></returns>
        public bool DrawTextInRect(ref string text, ref int xRelStart, ref int yRelStart, int x, int y, int width, int height, uint dtFlags, Color color, Font font)
        {
            return _bitmap.DrawTextInRect(ref text, ref xRelStart, ref yRelStart, x, y, width, height, dtFlags, color, font);
        }

        /// <summary>
        /// Draws text in a rectangle.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="dtFlags">Flags found in Bitmap.</param>
        /// <param name="color">Color</param>
        /// <param name="font">Font</param>
        public void DrawTextInRect(string text, Rectangle rect, uint dtFlags, Color color, Font font)
        {
            _bitmap.DrawTextInRect(text, rect.X, rect.Y, rect.Width, rect.Height, dtFlags, color, font);
        }

        /// <summary>
        /// Flushes the Graphics object to the screen.
        /// </summary>
        public void Flush()
        {
            _bitmap.Flush();
        }

        /// <summary>
        /// Flushes the Graphics object to the screen.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void Flush(int x, int y, int width, int height)
        {
            _bitmap.Flush(x, y, width, height);
        }

        /// <summary>
        /// Bitmap used by the Graphics object.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetBitmap()
        {
            return _bitmap;
        }

        //public Color GetPixel(int xPos, int yPos);
        //public void MakeTransparent(Color color);
        //public void RotateImage(int angle, int xDst, int yDst, Bitmap bitmap, int xSrc, int ySrc, int width, int height, ushort opacity);

        /// <summary>
        /// Resizes images without distortion.
        /// </summary>
        /// <param name="xDst">Destination X.</param>
        /// <param name="yDst">Destination Y.</param>
        /// <param name="widthDst">Width</param>
        /// <param name="heightDst">Height</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="leftBorder">Left border.</param>
        /// <param name="topBorder">Top border.</param>
        /// <param name="rightBorder">Right border.</param>
        /// <param name="bottomBorder">Bottom border.</param>
        /// <param name="opacity">Opacity</param>
        public void Scale9Image(int xDst, int yDst, int widthDst, int heightDst, Bitmap bitmap, int leftBorder, int topBorder, int rightBorder, int bottomBorder, ushort opacity)
        {
            _bitmap.Scale9Image(xDst, yDst, widthDst, heightDst, bitmap, leftBorder, topBorder, rightBorder, bottomBorder, opacity);
        }

        /// <summary>
        /// Resizes images without distortion.
        /// </summary>
        /// <param name="xDst">Destination X.</param>
        /// <param name="yDst">Destination Y.</param>
        /// <param name="widthDst">Width</param>
        /// <param name="heightDst">Height</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="border">Border</param>
        /// <param name="opacity">Opacity</param>
        public void Scale9Image(int xDst, int yDst, int widthDst, int heightDst, Bitmap bitmap, int border, ushort opacity)
        {
            _bitmap.Scale9Image(xDst, yDst, widthDst, heightDst, bitmap, border, border, border, border, opacity);
        }

        /// <summary>
        /// Sets the clipping rectangle.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void SetClippingRectangle(int x, int y, int width, int height)
        {
            _bitmap.SetClippingRectangle(x, y, width, height);
        }

        //public void SetPixel(int xPos, int yPos, Color color);

        /// <summary>
        /// Stretch an image.
        /// </summary>
        /// <param name="xDst">Destination X.</param>
        /// <param name="yDst">Destination Y.</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="opacity">Opacity</param>
        public void StretchImage(int xDst, int yDst, Bitmap bitmap, int width, int height, ushort opacity)
        {
            _bitmap.StretchImage(xDst, yDst, bitmap, width, height, opacity);
        }

        //public void StretchImage(int xDst, int yDst, int widthDst, int heightDst, Bitmap bitmap, int xSrc, int ySrc, int widthSrc, int heightSrc, ushort opacity);
        //public void TileImage(int xDst, int yDst, Bitmap bitmap, int width, int height, ushort opacity);
    }
}
