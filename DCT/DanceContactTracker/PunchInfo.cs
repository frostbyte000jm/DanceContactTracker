using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceContactTracker
{
    [Serializable()]
    class PunchInfo: IComparable<PunchInfo>
    {
        //declaration
        private DateTime datePunched;
        private bool isHalf;

        public DateTime DatePunched { get { return datePunched; } set { datePunched = value; } }
        public bool IsHalf { get { return isHalf; } set { isHalf = value; } }

        public PunchInfo(DateTime dt, bool h)
        {
            datePunched = dt;
            isHalf = h;
        }

        public int CompareTo(PunchInfo cont)
        {
            int result = -this.datePunched.CompareTo(cont.datePunched);
            return result;
        }
    }
}
