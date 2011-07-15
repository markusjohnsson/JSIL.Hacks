
namespace JSIL.Dom
{
    public class RgbStyle : Style
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public RgbStyle()
        {
        }

        public RgbStyle(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return string.Format("rgb({0},{1},{2})", Red, Green, Blue);
        }
    }
}
