
namespace JSIL.Dom
{
    public class StringStyle: Style
    {
        private string s;

        public StringStyle(string s)
        {
            this.s = s;
        }

        public override string ToString()
        {
            return s;
        }
    }
}
