using Fiddler;

namespace BinaryMessageFiddlerExtension
{
    public class RawBinaryRequestInspector : RawBinaryInspector, IRequestInspector2
    {
        public HTTPRequestHeaders headers
        {
            get { return null; }
            set {  }
        }
    }
}
