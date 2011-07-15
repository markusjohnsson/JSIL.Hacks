
namespace JSIL.Dom
{
    public abstract class Style
    {
        public abstract override string ToString();

        public static Style Parse(string s)
        {
            return new StringStyle(s);
        }
    }
}