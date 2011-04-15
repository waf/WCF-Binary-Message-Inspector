using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace BinaryMessageFiddlerExtension
{
    public class BinaryRequestInspector : BinaryInspector, IRequestInspector2
    {
        public HTTPRequestHeaders headers
        {
            get { return null; }
            set {  }
        }
    }
}
