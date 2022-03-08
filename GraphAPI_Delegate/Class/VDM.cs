using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphAPI_Delegate.Class
{
    public class VDM
    {
        private static VDM _VDM = new VDM();
        public static VDM Sgt { get { return _VDM; } }

        public Dictionary<string, string> TeamsToken = new Dictionary<string, string>();
    }
}
