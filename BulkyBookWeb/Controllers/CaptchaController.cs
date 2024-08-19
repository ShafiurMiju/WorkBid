using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Drawing;

namespace WorkBid.Controllers
{
    public class CaptchaController : Controller
    {
        [HttpGet]
        public IActionResult GenerateCaptcha(string text)
        {
            using var bitmap = new Bitmap(200, 60);
            using var graphics = Graphics.FromImage(bitmap);
            var font = new Font("Arial", 24, FontStyle.Bold);
            var brush = new SolidBrush(Color.Black);
            graphics.Clear(Color.White);
            graphics.DrawString(text, font, brush, 10, 10);

            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            return File(memoryStream.ToArray(), "image/png");
        }
    }
}
