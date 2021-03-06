﻿using System;
using System.Drawing;
using Screna;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class WebcamOverlay : IOverlay
    {
        public void Dispose() { }

        static Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(Settings.Instance.Webcam_X, Settings.Instance.Webcam_Y);

            switch (Settings.Instance.Webcam_XAlign)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (Settings.Instance.Webcam_YAlign)
            {
                case Alignment.Center:
                    point.Y = Bounds.Height / 2 - ImageSize.Height / 2 + point.Y;
                    break;

                case Alignment.End:
                    point.Y = Bounds.Height - ImageSize.Height - point.Y;
                    break;
            }

            return point;
        }

        public void Draw(Graphics g, Func<Point, Point> PointTransform = null)
        {
            var webcam = ServiceProvider.WebCamProvider;

            // No Webcam
            if (webcam.SelectedCam == webcam.AvailableCams[0])
                return;

            var img = webcam.Capture();

            if (img == null)
                return;

            if (Settings.Instance.Webcam_DoResize)
                img = img.Resize(new Size(Settings.Instance.Webcam_ResizeWidth, Settings.Instance.Webcam_ResizeHeight), false);

            using (img)
            {
                try
                {
                    var point = GetPosition(new Size((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height), img.Size);

                    if (Settings.Instance.Webcam_Opacity < 100)
                    {
                        var colormatrix = new ColorMatrix
                        {
                            Matrix33 = Settings.Instance.Webcam_Opacity / 100.0f
                        };

                        var imgAttribute = new ImageAttributes();
                        imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        g.DrawImage(img, new Rectangle(point, img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
                    }
                    else g.DrawImage(img, point);
                }
                catch { }
            }
        }
    }
}