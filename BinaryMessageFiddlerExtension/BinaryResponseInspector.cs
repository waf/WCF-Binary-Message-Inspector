using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;

namespace BinaryMessageFiddlerExtension
{
    public class BinaryResponseInspector : BinaryInspector, IResponseInspector2
    {
        public HTTPResponseHeaders headers
        {
            get { return null; }
            set {  }
        }
    }
}
