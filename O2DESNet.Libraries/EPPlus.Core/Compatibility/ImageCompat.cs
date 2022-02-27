using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;



namespace OfficeOpenXml.Compatibility
{
    internal class ImageCompat
    {
        internal static byte[] GetImageAsByteArray(Image image)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, new PngEncoder());
                bytes = ms.ToArray();
            }

            return bytes;
        }

        internal static byte[] GetImageAsByteArray(Image image, IImageFormat format)
        {
            IImageEncoder encoder;
            byte[] bytes;

            switch (format.Name)
            {
                case "GIF": encoder = new GifEncoder(); break;
                case "BMP": encoder = new BmpEncoder(); break;
                case "PNG": encoder = new PngEncoder(); break;
                case "JPEG": encoder = new JpegEncoder { Quality = 75 }; break;

                default:
                    throw new Exception("No image format");

            }

            using (var ms = new MemoryStream())
            {
                image.Save(ms, encoder);
                bytes = ms.ToArray();
            }

            return bytes;
        }
    }
}
