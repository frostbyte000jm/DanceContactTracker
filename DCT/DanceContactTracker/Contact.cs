using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceContactTracker
{   
    [Serializable()]
    class Contact : IComparable<Contact>
    {
        //declarations
        private string nameFirst, nameMiddle, nameLast, address, address2, city, state, zip, phone, email;
        private bool doSignedWaiver, doEmailOwners, doSendWarningEmail;
        private List<DateTime> dateList; //, purchaseList;
        private List<PurchaseInfo> purchaseList;
        private List<PunchInfo> punchInfoList;
        private Guid uniqueID;
        private double punchesLeft;

        //public Names
        public string Name {get { return nameLast + ", " + nameFirst + " " + nameMiddle;}}
        public string NameFirst { get { return nameFirst; } set { if (value.Length >= 0) nameFirst = value; } }
        public string NameMiddle { get { return nameMiddle; } set { if (value.Length >= 0) nameMiddle = value; } }
        public string NameLast { get { return nameLast; } set { if (value.Length >= 0) nameLast = value; } }
        public string Address { get { return address; } set { if (value.Length >= 0) address = value; } }
        public string Address2 { get { return address2; } set { if (value.Length >= 0) address2 = value; } }
        public string City { get { return city; } set { if (value.Length >= 0) city = value; } }
        public string State { get { return state; } set { if (value.Length >= 0) state = value; } }
        public string Zip { get { return zip; } set { if (value.Length >= 0) zip = value; } }
        public string Phone { get { return phone; } set { if (value.Length >= 0) phone = value; } }
        public string Email { get { return email; } set { if (value.Length >= 0) email = value; } }
        public bool DoSignedWaiver { get { return doSignedWaiver; } set { if (value == true) doSignedWaiver = true; } }
        public List<DateTime> DateList { get { return dateList; } }//Retiring 
        public List<PunchInfo> PunchInfoList { get { return punchInfoList; } }
        public List<PurchaseInfo> PurchaseList { get { return purchaseList; } }
        //public List<DateTime> PurchaseList { get { return purchaseList; } }
        public double PunchesLeft { get { return punchesLeft; } }
        public Guid UniqueID { get { return uniqueID; } }
        public string AllData { get { return nameLast + nameFirst + nameMiddle + address + address2 + city + state + zip + phone + email; } }
        public bool DoEmailOwners { get { return doEmailOwners; } set { doEmailOwners = value; } }
        public bool DoSendWarningEmail { get { return doSendWarningEmail; } set { doSendWarningEmail = value; } }

        public Contact(Guid uID) 
        {
            nameFirst = "";
            nameMiddle = "";
            nameLast = "";
            address = "";
            address2 = "";
            city = "";
            state = "";
            zip = "";
            phone = "";
            email = "";
            doSignedWaiver = false;
            dateList = new List<DateTime>();
            punchInfoList = new List<PunchInfo>();
            purchaseList = new List<PurchaseInfo>();
            uniqueID = uID;
            punchesLeft = 0;
            doEmailOwners = true;
            doSendWarningEmail = false;

            //for (int i = 0; i < 20;i++ )
            //{
            //    dateList.Add(DateTime.Today);
            //}
                
        }

        public void StoreDatePunch(DateTime datePunched, double used)
        {
            bool isHalf = false;
            if(used<1)
                isHalf = true;
            PunchInfo punch = new PunchInfo(datePunched,isHalf);
            punchInfoList.Insert(0, punch);
            
            //dateList.Insert(0,datePunched);
            punchesLeft -= used;

            
        }
        public void ChangeDatePunch(DateTime datePunched, bool isHalf, int idx)
        {
            //Because we are just changing the date, we need to give back it current value.
            if (punchInfoList[idx].IsHalf)
                punchesLeft += 0.5;
            else
                punchesLeft += 1.0;

            punchInfoList[idx].DatePunched = datePunched;
            punchInfoList[idx].IsHalf = isHalf;

            //Once the date is changed we need to take away it's new value. 
            if (isHalf)
                punchesLeft -= 0.5;
            else
                punchesLeft -= 1.0;
                
        }

        public void StoreDatePurchase(DateTime datePunched)
        {
            PurchaseInfo purchase = new PurchaseInfo(datePunched);
            purchaseList.Insert(0,purchase);
            punchesLeft += 10;
        }
        public void UpdateDatePurchase(DateTime datePunched, int idx)
        {
            purchaseList[idx].Date = datePunched;
        }
        public void SortDatePurchased()
        {
            purchaseList.Sort();
        }

        public void DeleteDatePurchase(int idx)
        {
            purchaseList.RemoveAt(idx);
            punchesLeft -= 10;
        }

        public void AddCredits(double num)
        {
            punchesLeft += num;
        }

        public int CompareTo(Contact cont)
        {
            int result = this.AllData.CompareTo(cont.AllData);
            return result;
        }
        
        public void SortDatePunch()
        {
            punchInfoList.Sort();
        }

        public void SetNewDate(int idx, DateTime dt, bool dh)
        {
            punchInfoList[idx].DatePunched = dt;
            punchInfoList[idx].IsHalf = dh;
        }

        public string CustomerInfo ()
        {
            string c = nameFirst + " " + nameMiddle + " " + nameLast + ". " + address + ", " + address2 + ", " + city + ", " + state + " " + zip + ". " + phone + " " + email;
            return c;
        }
        

        //public List<DateTime> getDates()
        //{
        //    return dateList;
        //}

    }
}
