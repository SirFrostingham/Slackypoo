using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slackypoo
{
    public class Globals
    {
        public Globals()
        {
            CurrentEmailIds = new List<string>();
        }

        private static Globals _globals;

        public static Globals Current
        {
            get
            {
                if (_globals == null)
                    _globals = new Globals();

                return _globals;
            }
        }

        private List<string> _currentEmailIds;

        public List<string> CurrentEmailIds
        {
            get { return _currentEmailIds; }
            set
            {
                _currentEmailIds = value;
            }
        }
    }
}   
