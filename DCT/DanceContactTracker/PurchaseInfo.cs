using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceContactTracker
{
    [Serializable()]
    class PurchaseInfo: IComparable<PurchaseInfo>
    {
        //declaration
        private DateTime date;

        public DateTime Date { get { return date; } set { date = value; } }

        public PurchaseInfo(DateTime dt)
        {
            date = dt;
        }

        public int CompareTo(PurchaseInfo cont)
        {
            int result = -this.date.CompareTo(cont.date);
            return result;
        }
    }
}
