using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AutoGenerateSnellenVisionChart
{
    public class ImageWrapper
    {
        public BitmapImage Bitmap { get; set; }
        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }
        public string CorrectAnswer { get; set; }

    }

}
