using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DanceContactTracker.DynamicObjects;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

//This application was created by D James Martin.
namespace DanceContactTracker
{
    
    
    public partial class Form1 : Form
    {
        //Version Number
        private string versionNum = "0.1.2.5";

        //declarations
        private Contact contact;
        private List<Contact> contactList, contactListOld;
        private List<string> dtNameList;
        //private SomethingIsHappening somethingsHappening;
        private int punchInstanceHeight, purchaseInstanceHeight, contactIndex;
        private string punInstNumName, punInstDateName, punInstDelName, purInstNumName, purInstDateName, purInstDelName;
        private int punInstNumIdx, punInstDateIdx, punInstDelIdx, purInstNumIdx, purInstDateIdx, purInstDelIdx;
        private bool punchDateInputClean, purchaseDateInputClean;
        //private bool doHalf;
        private string punchDateBoxHolding, purchaseDateBoxHolding;
        private BinaryFormatter bf;
        private Stream fstream, contactStream;
        private string savePath, savePathDate, savePathTime, updatePath, saveFile, contactsOnOpen;
        private string fNameTxt, mNameTxt, lNameTxt, addressTxt, address2Txt, cityTxt, stateTxt, zipTxt, phoneTxt, emailTxt;
        //private bool waveBoxChecked;
        private BackgroundWorker worker;

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            //Clear Placeholders
            ClearDynaPanels();

            //Create Contact List [This will need to load the old list OR load new list]
            contactList = new List<Contact>();            
            dtNameList = new List<string>();
            bf = new BinaryFormatter();

            //Starting Contact Index Number.
            contactIndex = -1;
            //doHalf = false;
            punchDateInputClean = true;
            purchaseDateInputClean = true;
            savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DCT";
            savePathDate = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DCT\Archive\Date";
            savePathTime = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DCT\Archive\Time";
            //updatePath = savePath + @"\Update";
            saveFile = "contactInfo.dct";
            //contactsOnOpen = "contactListOld.txt";
            buildLbl.Text = "Build: " + versionNum;

            //remove old update files
            //if (Directory.Exists(updatePath))
            //Directory.Delete(updatePath, true);

            //Create needed folders
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            if (!Directory.Exists(savePathDate))
                Directory.CreateDirectory(savePathDate);
            if (!Directory.Exists(savePathTime))
                Directory.CreateDirectory(savePathTime);

            //if (!Directory.Exists(updatePath))
            //    Directory.CreateDirectory(updatePath);

            //Creating Heights of Punch Cards
            punchInstanceHeight = 26;
            purchaseInstanceHeight = punchInstanceHeight;

            //Index info in Instance Panels
            InstanceIndexes();

            //Refresh Screen
            RefreshScreen();
            
            //deserialize
            try
            {
                fstream = File.OpenRead(savePath+@"\"+saveFile);
                contactList = (List<Contact>)bf.Deserialize(fstream);
                fstream.Close();
                contactIndex = Math.Min(0, contactList.Count);//This doesn't make any sense.
                UpdateNameDT();
            }
            catch { }
            
            //Copy Contacts at start.
            //CreateContactSavePoint();
            

            //Stress Tester
            //int tempnum = 0;
            //string abc = "abcdefghijklmnopqrstuvwxyz";
            //for (int i = 0; i < 1000; i++)
            //{
            //    NewContact();
            //    fNameTxtBox.Text = abc;
            //    fNameTxtBox_Leave(null, null);

            //}

            FolderCleanup();

        }

        
        

        //Index the Cards. This will help later when you need to change stuff.
        private void InstanceIndexes()
        {
            //Name
            punInstNumName = "punchInstanceNumber";
            punInstDateName = "punchInstanceDateBox";
            punInstDelName = "punchInstanceDelButton";
            purInstNumName = "purchaseInstanceNumber";
            purInstDateName = "purchaseInstanceDateBox";
            purInstDelName = "purchaseInstanceDelButton";

            AddPunchInstancePanel(0);
            AddPurchaseInstancePanel(0);

            //Index Punch Instance
            int punchInstancePanelCtrlCount = datePanel.Controls[0].Controls.Count;
            for (int i = 0; i < punchInstancePanelCtrlCount; i++)
            {
                if (datePanel.Controls[0].Controls[i].Name == punInstNumName)
                    punInstNumIdx = i;
                else if (datePanel.Controls[0].Controls[i].Name == punInstDateName)
                    punInstDateIdx = i;
                else if (datePanel.Controls[0].Controls[i].Name == punInstDelName)
                    punInstDelIdx = i;
            }

            //Index Purchase Index
            int purchaseInstancePanelCtrlCount = purchasePanel.Controls[0].Controls.Count;
            for (int i = 0; i < purchaseInstancePanelCtrlCount; i++)
            {
                if (purchasePanel.Controls[0].Controls[i].Name == purInstNumName)
                    purInstNumIdx = i;
                else if (purchasePanel.Controls[0].Controls[i].Name == purInstDateName)
                    purInstDateIdx = i;
                else if (purchasePanel.Controls[0].Controls[i].Name == purInstDelName)
                    purInstDelIdx = i;
            }
            //Clear Entry
            ClearDynaPanels();
        }

        //Build Punch Card
        private void AddPunchInstancePanel(int index)
        {
            //Create new stuff.
            DynaPanel punchInstancePanel = new DynaPanel(index);
            DynaLabel punchInstanceNumber = new DynaLabel(index);
            DynaTextBox punchInstanceDateBox = new DynaTextBox(index);
            DynaButton punchInstanceDelButton = new DynaButton(index);

            //Index Number
            punchInstanceNumber.AutoSize = true;
            punchInstanceNumber.Location = new System.Drawing.Point(3, 6);
            punchInstanceNumber.Name = punInstNumName;
            punchInstanceNumber.Size = new System.Drawing.Size(16, 13);
            //punchInstanceNumber.TabIndex = index + 1;
            punchInstanceNumber.Text = index + 1 + ":";

            //Date Box
            punchInstanceDateBox.Location = new System.Drawing.Point(38, 3);
            punchInstanceDateBox.Name = punInstDateName;
            punchInstanceDateBox.TextAlign = HorizontalAlignment.Center;
            punchInstanceDateBox.Size = new System.Drawing.Size(72, 20);
            punchInstanceDateBox.TabIndex = index * 3 + 1;
            punchInstanceDateBox.Text = "";
            punchInstanceDateBox.BackColor = Color.White; //Leaving this here so I can edit later. I want it to turn Yellow when half punch.
            punchInstanceDateBox.Enter += new EventHandler(punchInstanceDateBox_Enter);
            punchInstanceDateBox.Leave += new EventHandler(punchInstanceDateBox_Leave);
            punchInstanceDateBox.KeyPress += new KeyPressEventHandler(punchInstanceDateBox_KeyPress);

            //Delete Button
            punchInstanceDelButton.Location = new System.Drawing.Point(116, 1);
            punchInstanceDelButton.Name = punInstDelName;
            punchInstanceDelButton.Size = new System.Drawing.Size(32, 23);
            punchInstanceDelButton.TabIndex = index * 3 + 2;
            punchInstanceDelButton.Text = "Del";
            punchInstanceDelButton.UseVisualStyleBackColor = true;
            punchInstanceDelButton.Click += new EventHandler(PunchInstanceDelButton_Click);

            //Instance Panel
            punchInstancePanel.Controls.Add(punchInstanceDelButton);
            punchInstancePanel.Controls.Add(punchInstanceNumber);
            punchInstancePanel.Controls.Add(punchInstanceDateBox);
            punchInstancePanel.Location = new System.Drawing.Point(3, 2 + (punchInstanceHeight * index));
            punchInstancePanel.Name = "punchInstancePanel";
            punchInstancePanel.Size = new System.Drawing.Size(153, 26);
            //punchInstancePanel.TabIndex = index + 1;

            datePanel.Controls.Add(punchInstancePanel);
        }
        //Build Purchase Card
        private void AddPurchaseInstancePanel(int index)
        {
            //Create new stuff.
            DynaPanel purchaseInstancePanel = new DynaPanel(index);
            DynaLabel purchaseInstanceNumber = new DynaLabel(index);
            DynaTextBox purchaseInstanceDateBox = new DynaTextBox(index);
            DynaButton purchaseInstanceDelButton = new DynaButton(index);

            //Index Number
            purchaseInstanceNumber.AutoSize = true;
            purchaseInstanceNumber.Location = new System.Drawing.Point(3, 6);
            purchaseInstanceNumber.Name = purInstNumName;
            purchaseInstanceNumber.Size = new System.Drawing.Size(16, 13);
            //purchaseInstanceNumber.TabIndex = index + 1;
            purchaseInstanceNumber.Text = 1+":";

            //Date Box
            purchaseInstanceDateBox.Location = new System.Drawing.Point(38, 3);
            purchaseInstanceDateBox.Name = purInstDateName;
            purchaseInstanceDateBox.TextAlign = HorizontalAlignment.Center;
            purchaseInstanceDateBox.Size = new System.Drawing.Size(72, 20);
            purchaseInstanceDateBox.TabIndex = index * 3 + 1;
            purchaseInstanceDateBox.Text = "";
            purchaseInstanceDateBox.Enter += new EventHandler(purchaseInstanceDateBox_Enter);
            purchaseInstanceDateBox.Leave += new EventHandler(purchaseInstanceDateBox_Leave);
            purchaseInstanceDateBox.KeyPress += new KeyPressEventHandler(purchaseInstanceDateBox_KeyPress);
            

            //Delete Button
            purchaseInstanceDelButton.Location = new System.Drawing.Point(116, 1);
            purchaseInstanceDelButton.Name = purInstDelName;
            purchaseInstanceDelButton.Size = new System.Drawing.Size(32, 23);
            purchaseInstanceDelButton.TabIndex = index *3 + 2;
            purchaseInstanceDelButton.Text = "Del";
            purchaseInstanceDelButton.UseVisualStyleBackColor = true;
            purchaseInstanceDelButton.Click += new EventHandler(PurchaseInstanceDelButton_Click);

            //Instance Panel
            purchaseInstancePanel.Controls.Add(purchaseInstanceDelButton);
            purchaseInstancePanel.Controls.Add(purchaseInstanceNumber);
            purchaseInstancePanel.Controls.Add(purchaseInstanceDateBox);
            purchaseInstancePanel.Location = new System.Drawing.Point(3, 2 + (punchInstanceHeight * index));
            purchaseInstancePanel.Name = "purchaseInstancePanel";
            purchaseInstancePanel.Size = new System.Drawing.Size(153, 26);
            //purchaseInstancePanel.TabIndex = index + 1;

            purchasePanel.Controls.Add(purchaseInstancePanel);
        }
        //Add Credits
        //private void AddPurchases()
        //{
        //    contactList[contactIndex].AddCredits();
        //}
        //Build New Contact
        private void NewContact()
        {
            Guid uid = Guid.NewGuid();
            contact = new Contact(uid);
            contactList.Add(contact);
            contactIndex = contactList.Count - 1;

            SaveFile();
        }

        private void RefreshScreen()
        {
            //CreateNameDT();
            //Clear Screen
            ClearDynaPanels();

            //showScreen?
            if (contactIndex < 0)
                contactPanel.Visible = false;
            else
            {
                contactPanel.Visible = true;

                //linkup all the boxes
                fNameTxtBox.Text = contactList[contactIndex].NameFirst;
                mNameTxtBox.Text = contactList[contactIndex].NameMiddle;
                lNameTxtBox.Text = contactList[contactIndex].NameLast;
                addressTxtBox.Text = contactList[contactIndex].Address;
                address2TxtBox.Text = contactList[contactIndex].Address2;
                cityTxtBox.Text = contactList[contactIndex].City;
                stateTxtBox.Text = contactList[contactIndex].State;
                zipTxtBox.Text = contactList[contactIndex].Zip;
                emailTxtBox.Text = contactList[contactIndex].Email;
                phoneTxtBox.Text = contactList[contactIndex].Phone;
                waiverChkBox.Checked = contactList[contactIndex].DoSignedWaiver;

                //Layout Purchases
                int purchaseCount = contactList[contactIndex].PurchaseList.Count;
                for (int i = 0; i < purchaseCount; i++)
                {
                    AddPurchaseInstancePanel(i);
                }
                //Layout Punches
                int punchCount = contactList[contactIndex].PunchInfoList.Count;
                for (int i = 0; i < punchCount; i++)
                {
                    AddPunchInstancePanel(i);
                }
                punchesLeftLbl.Text = Convert.ToString(contactList[contactIndex].PunchesLeft);
            }
        }
        private void RefreshPurchasePanel()
        {
            int purchasePanelCount = contactList[contactIndex].PurchaseList.Count;
            for (int i = 0; i < purchasePanelCount; i++)
            {
                purchasePanel.Controls[i].Controls[purInstNumIdx].Text = Convert.ToString(purchasePanelCount - i);
                purchasePanel.Controls[i].Controls[purInstDateIdx].Text = contactList[contactIndex].PurchaseList[i].Date.ToString("MM/dd/yyyy");
            }
            punchesLeftLbl.Text = Convert.ToString(contactList[contactIndex].PunchesLeft);

            
        }
        private void RefreshPunchPanel()
        {
            int punchPanelCount = contactList[contactIndex].PunchInfoList.Count;
            for (int i = 0; i < punchPanelCount; i++)
            {
                datePanel.Controls[i].Controls[punInstNumIdx].Text = Convert.ToString(punchPanelCount - i);
                datePanel.Controls[i].Controls[punInstDateIdx].Text = contactList[contactIndex].PunchInfoList[i].DatePunched.ToString("MM/dd/yyyy");

                if (contactList[contactIndex].PunchInfoList[i].IsHalf)
                    datePanel.Controls[i].Controls[punInstDateIdx].BackColor = Color.Yellow;
                else
                    datePanel.Controls[i].Controls[punInstDateIdx].BackColor = Color.White;
                    
                    //.DateList[i].ToString("MM/dd/yyyy");
            }
            punchesLeftLbl.Text = Convert.ToString(contactList[contactIndex].PunchesLeft);
        }
        private void ClearDynaPanels()
        {
            datePanel.Controls.Clear();
            purchasePanel.Controls.Clear();
        }
        private void UpdateNameDT()
        {
            if (contactIndex < 0)
                return;

            Guid tempGuid = contactList[contactIndex].UniqueID;
            comboBoxNames.Items.Clear();
            dtNameList.Clear();
            contactList.Sort();
            for (int i = 0; i < contactList.Count; i++)
            {
                dtNameList.Add(contactList[i].Name);
            }
            comboBoxNames.Items.AddRange(dtNameList.ToArray());
            for (int i = 0; i < contactList.Count; i++)
            {
                if (contactList[i].UniqueID == tempGuid)
                {
                    contactIndex = i;
                    break;
                }
            }
        }
        //private void UpdateContactInfo()
        //{
        //    //linkup all the boxes
        
        //}
        //private void CheckForDirt()
        //{
        //    if(contactIndex>=0)
        //    {
        //        bool nameDirty = false;

        //        if(fNameDirty)
        //        {
        //            contactList[contactIndex].NameFirst = fNameTxtBox.Text;
        //            nameDirty = true;
        //        }
        //        if(mNameDirty)
        //        {
        //            contactList[contactIndex].NameMiddle = mNameTxtBox.Text;
        //            nameDirty = true;
        //        }
        //        if(lNameDirty)
        //        {
        //            contactList[contactIndex].NameLast = lNameTxtBox.Text;
        //            nameDirty = true;
        //        }
        //        if(add1Dirty)
        //        {
        //            contactList[contactIndex].Address = addressTxtBox.Text;
        //        }
        //        if(add2Dirty)
        //        {
        //            contactList[contactIndex].Address2 = address2TxtBox.Text;
        //        }
        //        if(cityDirty)
        //        {
        //            contactList[contactIndex].City = cityTxtBox.Text;
        //        }
        //        if(stateDirty)
        //        {
        //            contactList[contactIndex].State = stateTxtBox.Text;
        //        }
        //        if(zipDirty)
        //        {
        //            contactList[contactIndex].Zip = zipTxtBox.Text;
        //        }
        //        if(emailDirty)
        //        {
        //            contactList[contactIndex].Email = emailTxtBox.Text;
        //        }
        //        if(phoneDirty)
        //        {
        //            contactList[contactIndex].Phone = phoneTxtBox.Text;
        //        }
        //        if(nameDirty)
        //        {
        //            Guid tempGuid = contactList[contactIndex].UniqueID;
        //            UpdateNameDT();
        //            for (int i = 0; i < contactList.Count; i++)
        //            {
        //                if (contactList[i].UniqueID == tempGuid)
        //                {
        //                    contactIndex = i;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
        //Move this above events when done.
        private void UpdatePunchDate(int idx, DateTime date)
        {

            bool halfPunch = false;
            int count = contactList[contactIndex].PunchInfoList.Count;
            for (int i = 0; i < count; i++)
            {
                if (contactList[contactIndex].PunchInfoList[i].DatePunched == date)
                {
                    DialogResult daig = MessageBox.Show("Date has been changed to a date that is already in the list. Should this date be considered a half punch?", "Half Punch?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (daig == DialogResult.Yes)
                    {
                        halfPunch = true;
                    }
                    break;
                }
            }
            contactList[contactIndex].ChangeDatePunch(date, halfPunch, idx);

            //sort and refresh.
            punchDateInputClean = true;
            contactList[contactIndex].SortDatePunch();
            RefreshPunchPanel();


        }
        private void UpdatePurchaseDate(int idx, DateTime date)
        {
            contactList[contactIndex].UpdateDatePurchase(date, idx);
            contactList[contactIndex].SortDatePurchased();
            RefreshPurchasePanel();
        }
        //send owners e-mail about updates
        private void CreateEmailToOwners()
        {
            List<String> contactInformation = new List<string>();
            for (int i = 0; i<contactList.Count;i++)
            {
                if(contactList[i].DoEmailOwners)
                {
                    string s = contactList[i].CustomerInfo();
                    contactInformation.Add(s);
                }
            }
            if (contactInformation.Count == 0)
                return;

            string emailAddy = "djanddancepro@gmail.com";
            string emailName = "DCTBot";
            string subject = "Customer Contact Information Update!";
            string emailBody = "The Dance Contact Tracker has some user updates, the updated information is below:<br /><br />";
            for(int i =0;i<contactInformation.Count;i++)
            {
                emailBody += (i + 1) + ") " + contactInformation[i] + "<br /><br />";
            }
            emailBody += "<br /><br />Please update your contact book.";

            Email email = new Email(emailAddy, emailName, subject, emailBody);
            try
            {
                email.Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show("System does not seem to be connected to the internet. If your system is connected to the internet try hitting it. If that doesn't work contact the idiot who made this application for you and send him a screen shot of this error.\n\n" + ex);
            }

            for (int i = 0; i<contactList.Count; i++)
            {
                contactList[i].DoEmailOwners = false;
            }
        }

        //send email to clients
        private void CreateEmailToContacts()
        {

        }

        private void UpdateAfterEntry()
        {
            contactList[contactIndex].DoEmailOwners = true;
        }
        
        
        //private void CreateContactSavePoint()
        //{
        //    contactStream = File.Open(contactsOnOpen, FileMode.Create);
        //    bf.Serialize(contactStream, contactList);
        //    contactStream.Close();
        //}
        ////I needs lots of work. THis is going to be tough.
        //private void CompareContacts()
        //{
        //    //load contact list
        //    contactStream = File.OpenRead(contactsOnOpen);
        //    contactListOld = (List<Contact>)bf.Deserialize(contactStream);
        //    contactStream.Close();

        //    //Run Test
        //    List<Contact> newContacts = new List<Contact>();
        //    List<Contact> oldContact = new List<Contact>();
        //    for (int i = 0; i < contactListOld.Count; i++)
        //    {
        //        Guid g = contactListOld[i].UniqueID;
        //        for (int j = 0; j < contactList.Count; j++)
        //        {
        //            if(contactList[j].UniqueID == g)
        //            {
        //                bool isSame = contactList[j].AllData == contactListOld[i].AllData;
        //            }
        //        }
        //    }



        //        //Create New SavePoint
        //        CreateContactSavePoint();
        //}

        ////I needs to create an e-mail for the customers
        //private void CreateCustomerEmail()
        //{
        //    //Code Goes here.
        //}

        private void SaveFile()
        {
            string endhr = DateTime.Now.ToString("yyyy.MM.dd.HH.mm");
            string enddt = DateTime.Now.ToString("yyyy.MM.dd");

            try
            {
                fstream.Close();
            }
            catch { }
            //Main
            fstream = File.Open(savePath + @"\" + saveFile, FileMode.Create);
            bf.Serialize(fstream, contactList);
            fstream.Close();
            //Hour Backup
            fstream = File.Open(savePathTime + @"\" + saveFile + endhr, FileMode.Create);
            bf.Serialize(fstream, contactList);
            fstream.Close();
            //Date Backup
            fstream = File.Open(savePathDate + @"\" + saveFile + enddt, FileMode.Create);
            bf.Serialize(fstream, contactList);
            fstream.Close();
        }

        private void FolderCleanup()
        {
            
            int daysSaving = 30;
            bool doCleanupHours = false;
            bool doCleanupDates = false;
            List<string> saveFilesList = new List<string>();
            //DirectoryInfo files = new DirectoryInfo(savePath);

            string[] arrSaveFilesDates = Directory.GetFiles(savePathDate, "*.*", SearchOption.TopDirectoryOnly);
            if (arrSaveFilesDates.GetLength(0) >= daysSaving)
                doCleanupDates = true;

            string[] arrSaveFileHours = Directory.GetFiles(savePathTime, "*.*", SearchOption.TopDirectoryOnly);
            if (arrSaveFileHours.GetLength(0) >= daysSaving)
                doCleanupHours = true;

            if(doCleanupDates)
            {
                List<string> listSaveFilesDates = new List<string>();
                for (int i = 0; i < arrSaveFilesDates.GetLength(0); i++)
                {
                    listSaveFilesDates.Add(arrSaveFilesDates[i]);
                }
                listSaveFilesDates.Sort();
                listSaveFilesDates.Reverse();

                listSaveFilesDates.RemoveRange(0, daysSaving);

                for (int i = 0; i<listSaveFilesDates.Count; i++)
                {
                    File.Delete(listSaveFilesDates[i]);
                }

            }

            if (doCleanupHours)
            {
                List<string> listSaveFilesHours = new List<string>();
                for (int i = 0; i < arrSaveFileHours.GetLength(0); i++)
                {
                    listSaveFilesHours.Add(arrSaveFileHours[i]);
                }
                listSaveFilesHours.Sort();
                listSaveFilesHours.Reverse();

                listSaveFilesHours.RemoveRange(0, daysSaving);

                for (int i = 0; i < listSaveFilesHours.Count; i++)
                {
                    File.Delete(listSaveFilesHours[i]);
                }

            }

        }
        

        //***Events***************************************************************************

        private void PunchInstanceDelButton_Click(object sender, EventArgs e)
        {
            //Set focus
            datePanel.Focus();

            DynaButton button = (DynaButton)sender;
            int idx = button.Index;
            double addBack = 1;
            if(contactList[contactIndex].PunchInfoList[idx].IsHalf)
                addBack = 0.5;
            contactList[contactIndex].AddCredits(addBack);
            contactList[contactIndex].PunchInfoList.RemoveAt(idx);
            int totCtrls = datePanel.Controls.Count;
            datePanel.Controls.RemoveAt(totCtrls-1);
            RefreshPunchPanel();

            
            
            //

            //for (int i = totCtrls-1; i >= 0; i--)
            //{
            //    int test1 = (totCtrls-i-1);
            //    datePanel.Controls[i].Location = new Point(3, 2 + punchInstanceHeight*(totCtrls-i-1));

            //    string test = datePanel.Controls[0].Controls[0].Name;
            //}
            
        }
        private void PurchaseInstanceDelButton_Click(object sender, EventArgs e)
        {
            //Set focus
            purchasePanel.Focus();

            DynaButton button = (DynaButton)sender;
            int idx = button.Index;
            contactList[contactIndex].DeleteDatePurchase(idx);
            int totCtrls = purchasePanel.Controls.Count;
            purchasePanel.Controls.RemoveAt(totCtrls - 1);
            RefreshPurchasePanel();

            //DynaButton button = (DynaButton)sender;
            //int index = button.Index;
            //datePanel.Controls.RemoveAt(index);

            //int totCtrls = datePanel.Controls.Count;

            //for (int i = totCtrls-1; i >= 0; i--)
            //{
            //    int test1 = (totCtrls-i-1);
            //    datePanel.Controls[i].Location = new Point(3, 2 + punchInstanceHeight*(totCtrls-i-1));

            //    string test = datePanel.Controls[0].Controls[0].Name;
            //}

        }

        private void addNewContact_Click(object sender, EventArgs e)
        {
            controlsToolStrip.Focus();
            //CheckForDirt();
            NewContact();
            RefreshScreen();
        }
        
        private void deleteContactBtn_Click(object sender, EventArgs e)
        {
            controlsToolStrip.Focus();
            DialogResult diag = MessageBox.Show("This will remove the contact and all of his/her information. Are you sure you wish to do this. Deleting the contact there is no way to bring them back.\n\n Click 'Yes' to delete.", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if(diag == DialogResult.Yes)
            {
                contactList.RemoveAt(contactIndex);
                comboBoxNames.Items.RemoveAt(contactIndex);
                contactIndex = -1;
                RefreshScreen();
                UpdateNameDT();
                comboBoxNames.Text = "";
            }
        }

        private void purchaceVisitButton_Click(object sender, EventArgs e)
        {
            //Set focus
            controlsToolStrip.Focus();

            //Check to see if there are any visable contacts
            if (contactIndex < 0)
                return;

            //Check to see if they have the minimum info filled out.
            if(contactList[contactIndex].NameFirst.Length==0 || contactList[contactIndex].NameLast.Length==0)
            {
                MessageBox.Show("Please enter a first and last name for the customer before purchasing a punch.", "Must enter required fields.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //dialog to verify transaction is complete.
            DialogResult doBuy = MessageBox.Show("Press OK once transaction has been completed.", "Transaction in progress:", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            //Once complete Take todays date and place it in the contact. 
            if (doBuy == DialogResult.OK)
            {
                contactList[contactIndex].StoreDatePurchase(DateTime.Today);
                AddPurchaseInstancePanel(contactList[contactIndex].PurchaseList.Count - 1);
                RefreshPurchasePanel();
            }
            SaveFile();
        }

        private void punchVisitButton_Click(object sender, EventArgs e)
        {
            //Set focus
            controlsToolStrip.Focus();
            //Check to see if there are any visable contacts
            if (contactIndex < 0)
                return;

            //How much does the punch Cost [1 or 0.5].
            double punchNum = 1;
            try
            {
                if (contactList[contactIndex].PunchInfoList[0].DatePunched == DateTime.Today)
                {
                    DialogResult doHalfDlog = MessageBox.Show(fNameTxtBox.Text+" has already purchased a dance today, is customer a half punch?", "Half Punch?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                    if (doHalfDlog == DialogResult.Cancel)
                        return;

                    if (doHalfDlog == DialogResult.Yes)
                    {
                        punchNum = 0.5;
                        //doHalf = true;
                    }
                    else
                    {
                        punchNum = 1;
                    }
                }
            }
            catch { }

            //Check if they have enough credits and then create the punch
            if (contactList[contactIndex].PunchesLeft < punchNum)
            {
                MessageBox.Show(fNameTxtBox.Text+" does not have enough punches.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                contactList[contactIndex].StoreDatePunch(DateTime.Today,punchNum);
                AddPunchInstancePanel(contactList[contactIndex].PunchInfoList.Count - 1);
                RefreshPunchPanel();
                if (contactList[contactIndex].PunchesLeft <= 2)
                {
                    MessageBox.Show(fNameTxtBox.Text + " has " + punchesLeftLbl.Text + " punch(es) left.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void comboBoxNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set focus
            controlsToolStrip.Focus();

            int index = comboBoxNames.SelectedIndex;
            if (index >= 0)
            {
                //CheckForDirt();
                contactIndex = index;
                RefreshScreen();
                RefreshPurchasePanel();
                RefreshPunchPanel();
            }

            comboBoxNames.Text = contactList[contactIndex].Name;
        }

        private void waiverChkBox_CheckedChanged(object sender, EventArgs e)
        {
            contactList[contactIndex].DoSignedWaiver = waiverChkBox.Checked;
        }

        //private void fNameTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    fNameDirty = true;
        //}

        //private void mNameTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    mNameDirty = true;
        //}

        //private void lNameTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    lNameDirty = true;
        //}

        //private void addressTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    add1Dirty = true;
        //}

        //private void address2TxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    add2Dirty = true;
        //}

        //private void cityTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    cityDirty = true;
        //}

        //private void stateTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    stateDirty = true;
        //}

        //private void zipTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    zipDirty = true;
        //}

        //private void emailTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    emailDirty = true;
        //}

        //private void phoneTxtBox_TextChanged(object sender, EventArgs e)
        //{
        //    phoneDirty = true;
        //}

        //private void comboBoxNames_Click(object sender, EventArgs e)
        //{
        //    //comboBoxNames.Focus();
        //    //comboBoxNames.DroppedDown = true;
        //}

        

        //private void addressTxtBox_Validated(object sender, EventArgs e)
        //{
        //    MessageBox.Show("BOOM!");
        //}

        private void punchesLeftLbl_TextChanged(object sender, EventArgs e)
        {
            if (contactList[contactIndex].PunchesLeft <= 2)
            {
                punchesLeftLbl.ForeColor = Color.Red;
            }   
            else
                punchesLeftLbl.ForeColor = Color.Black;
        }

        private void punchInstanceDateBox_Leave(object sender, EventArgs e)
        {
            if (punchDateInputClean)
                return;

            DynaTextBox textbox = (DynaTextBox)sender;
            if (textbox.Text == punchDateBoxHolding)
            {
                punchDateInputClean = true;
                return;
            }

            //temp need idea.
            
            int idx = textbox.Index;
            try
            {
                DateTime date = Convert.ToDateTime(textbox.Text);
                UpdatePunchDate(idx, date);
            }
            catch
            {
                MessageBox.Show("Please enter a valid date in the following formats: 'mm/dd/yyyy' or 'm/d/yyyy'.", "Invalid Date Entered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                punchDateInputClean = true;
            }
            
        }

        private void punchInstanceDateBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            punchDateInputClean = false;
        }

        private void punchInstanceDateBox_Enter(object sender, EventArgs e)
        {
            DynaTextBox textbox = (DynaTextBox)sender;
            punchDateBoxHolding = textbox.Text;
        }

        private void purchaseInstanceDateBox_Leave(object sender, EventArgs e)
        {
            if (purchaseDateInputClean)
                return;
            DynaTextBox textbox = (DynaTextBox)sender;
            if (textbox.Text == purchaseDateBoxHolding)
            {
                purchaseDateInputClean = true;
                return;
            }
                

            int idx = textbox.Index;
            try
            {
                DateTime date = Convert.ToDateTime(textbox.Text);
                UpdatePurchaseDate(idx, date);
                
            }
            catch
            {
                MessageBox.Show("Please enter a valid date in the following formats: 'mm/dd/yyyy' or 'm/d/yyyy'.", "Invalid Date Entered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                purchaseDateInputClean = true;
                //punchDateInputClean = true;
            }
        }

        private void purchaseInstanceDateBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            purchaseDateInputClean = false;
        }

        private void purchaseInstanceDateBox_Enter(object sender, EventArgs e)
        {
            Form1.ActiveForm.Select();

            DynaTextBox textbox = (DynaTextBox)sender;
            purchaseDateBoxHolding = textbox.Text;
        }

        private void fNameTxtBox_Leave(object sender, EventArgs e)
        {
            if(fNameTxt != fNameTxtBox.Text)
            {
                fNameTxtBox.Text = fNameTxtBox.Text.Substring(0, 1).ToUpper() + fNameTxtBox.Text.Substring(1);
                contactList[contactIndex].NameFirst = fNameTxtBox.Text;
                UpdateNameDT();
                UpdateAfterEntry();
            }
        }

        private void mNameTxtBox_Leave(object sender, EventArgs e)
        {
            if(mNameTxt != mNameTxtBox.Text)
            {
                mNameTxtBox.Text = mNameTxtBox.Text.ToUpper();
                contactList[contactIndex].NameMiddle = mNameTxtBox.Text;
                UpdateNameDT();
                UpdateAfterEntry();
            }
        }

        private void lNameTxtBox_Leave(object sender, EventArgs e)
        {
            if(lNameTxt != lNameTxtBox.Text)
            {
                lNameTxtBox.Text = lNameTxtBox.Text.Substring(0, 1).ToUpper() + lNameTxtBox.Text.Substring(1);
                contactList[contactIndex].NameLast = lNameTxtBox.Text;
                UpdateNameDT();
                UpdateAfterEntry();
            }
        }

        private void addressTxtBox_Leave(object sender, EventArgs e)
        {
            if(addressTxt != addressTxtBox.Text)
            {
                contactList[contactIndex].Address = addressTxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void address2TxtBox_Leave(object sender, EventArgs e)
        {
            if(address2Txt != address2TxtBox.Text)
            {
                contactList[contactIndex].Address2 = address2TxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void cityTxtBox_Leave(object sender, EventArgs e)
        {
            if(cityTxt != cityTxtBox.Text)
            {
                contactList[contactIndex].City = cityTxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void stateTxtBox_Leave(object sender, EventArgs e)
        {
            if(stateTxt != stateTxtBox.Text)
            {
                stateTxtBox.Text = stateTxtBox.Text.ToUpper();
                contactList[contactIndex].State = stateTxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void zipTxtBox_Leave(object sender, EventArgs e)
        {
            if(zipTxt != zipTxtBox.Text)
            {
                //remove dashes before putting them back in. This was easier than checking.
                string zip = zipTxtBox.Text.Replace("-", "");
                if (zip.Length == 9)
                    zipTxtBox.Text = zip.Substring(0, 5) + "-" + zip.Substring(5);

                contactList[contactIndex].Zip = zipTxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void emailTxtBox_Leave(object sender, EventArgs e)
        {
            if(emailTxt != emailTxtBox.Text)
            {
                contactList[contactIndex].Email = emailTxtBox.Text;
                UpdateAfterEntry();
            }
        }

        private void phoneTxtBox_Leave(object sender, EventArgs e)
        {
            if(phoneTxt != phoneTxtBox.Text)
            {
                string phone = phoneTxtBox.Text;
                phone = phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                string ext = phone.Substring(10);
                phone = "(" + phone.Substring(0, 3) + ") " + phone.Substring(3, 3) + "-" + phone.Substring(6, 4);
                phoneTxtBox.Text = phone + " " + ext;
                contactList[contactIndex].Phone = phoneTxtBox.Text;
                UpdateAfterEntry();
            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveFile();
        }

        private void fNameTxtBox_Enter(object sender, EventArgs e)
        {
            fNameTxt = fNameTxtBox.Text;
        }

        private void mNameTxtBox_Enter(object sender, EventArgs e)
        {
            mNameTxt = mNameTxtBox.Text;
        }

        private void lNameTxtBox_Enter(object sender, EventArgs e)
        {
            lNameTxt = lNameTxtBox.Text;
        }

        private void addressTxtBox_Enter(object sender, EventArgs e)
        {
            addressTxt = addressTxtBox.Text;
        }

        private void address2TxtBox_Enter(object sender, EventArgs e)
        {
            address2Txt = address2TxtBox.Text;
        }

        private void cityTxtBox_Enter(object sender, EventArgs e)
        {
            cityTxt = cityTxtBox.Text;
        }

        private void stateTxtBox_Enter(object sender, EventArgs e)
        {
            stateTxt = stateTxtBox.Text;
        }

        private void zipTxtBox_Enter(object sender, EventArgs e)
        {
            zipTxt = zipTxtBox.Text;
        }

        private void emailTxtBox_Enter(object sender, EventArgs e)
        {
            emailTxt = emailTxtBox.Text;
        }

        private void phoneTxtBox_Enter(object sender, EventArgs e)
        {
            phoneTxt = phoneTxtBox.Text;
        }

        private void sendEmailsBtn_Click(object sender, EventArgs e)
        {
            CreateEmailToOwners();
        }

        private void saveContactsBtn_Click(object sender, EventArgs e)
        {
            SaveFile();
        }


        //******************************************
        //******************************************
        //TRASH
        //******************************************
        //******************************************

        //private void fNameTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].NameFirst = fNameTxtBox.Text;
        //    Guid tempGuid = contactList[contactIndex].UniqueID;
        //    CreateNameDT();
        //    for (int i = 0; i < contactList.Count; i++)
        //    {
        //        if (contactList[i].UniqueID == tempGuid)
        //        {
        //            contactIndex = i;
        //            break;
        //        }
        //    }
        //}

        //private void mNameTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].NameMiddle = mNameTxtBox.Text;
        //    Guid tempGuid = contactList[contactIndex].UniqueID;
        //    CreateNameDT();
        //    for (int i = 0; i < contactList.Count; i++)
        //    {
        //        if (contactList[i].UniqueID == tempGuid)
        //        {
        //            contactIndex = i;
        //            break;
        //        }
        //    }
        //}

        //private void lNameTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].NameLast = lNameTxtBox.Text;
        //    Guid tempGuid = contactList[contactIndex].UniqueID;
        //    CreateNameDT();
        //    for (int i = 0; i < contactList.Count; i++)
        //    {
        //        if (contactList[i].UniqueID == tempGuid)
        //        {
        //            contactIndex = i;
        //            break;
        //        }
        //    }
        //}

        //private void addressTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].Address = addressTxtBox.Text;
        //}

        //private void address2TxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].Address2 = address2TxtBox.Text;
        //}

        //private void cityTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].City = cityTxtBox.Text;
        //}

        //private void stateTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].State = stateTxtBox.Text;
        //}

        //private void zipTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].Zip = zipTxtBox.Text;
        //}

        //private void emailTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].Email = emailTxtBox.Text;
        //}

        //private void phoneTxtBox_Leave(object sender, EventArgs e)
        //{
        //    contactList[contactIndex].Phone = phoneTxtBox.Text;
        //}



        //private void Name_TextChanged(object sender, EventArgs e)
        //{
        //    dropBoxDirty = true;
        //}


        //private void Initialize()
        //{
        //    //remove placeholders
        //    datePanel.Controls.Clear();
        //    purchasePanel.Controls.Clear();

        //    //Create Stuff
        //    contactList = new List<Contact>();

        //    //Define Stuff
        //    punchInstanceHeight = 26;
        //    purchaseInstanceHeight = 26;
        //    contactIndex = -1;
        //    contactPanel.Visible = false;
        //    InstanceIndexes();//This will create an index for stuff in the panels.
        //}

        ////THis will index the locations.



        ////Add Ticket Punch.
        //private void AddEntryPunchInstance()
        //{
        //    int visitCount = contactList[contactIndex].DateList.Count;
        //    int purchaseCount = contactList[contactIndex].PurchaseList.Count;

        //    bool doStop = false;
        //    //if (visitCount >= (purchaseCount * 10))
        //    //    doStop = true;

        //    if (doStop)
        //    {
        //        MessageBox.Show("The customer must purchase more dances.", "No dance for you!");
        //    }
        //    else
        //    {
        //        contactList[contactIndex].DateList.Add(DateTime.Today);
        //    }

        //    PunchInstancePanelCreate(visitCount);

        //}

        ////This will create a new punch Instance Panel and will add a blank
        ////to the contact List.
        //private void PunchInstancePanelCreate(int index)
        //{
        //    //Create new stuff.
        //    DynaPanel punchInstancePanel = new DynaPanel(index);
        //    DynaLabel punchInstanceNumber = new DynaLabel(index);
        //    DynaTextBox punchInstanceDateBox = new DynaTextBox(index);
        //    DynaButton punchInstanceDelButton = new DynaButton(index);



        //    //Index Number
        //    punchInstanceNumber.AutoSize = true;
        //    punchInstanceNumber.Location = new System.Drawing.Point(3, 6);
        //    punchInstanceNumber.Name = punInstNumName;
        //    punchInstanceNumber.Size = new System.Drawing.Size(16, 13);
        //    punchInstanceNumber.TabIndex = index + 1;
        //    punchInstanceNumber.Text = index + 1 + ":";

        //    //Date Box
        //    punchInstanceDateBox.Location = new System.Drawing.Point(38, 3);
        //    punchInstanceDateBox.Name = punInstDateName;
        //    punchInstanceDateBox.TextAlign = HorizontalAlignment.Center;
        //    punchInstanceDateBox.Size = new System.Drawing.Size(72, 20);
        //    punchInstanceDateBox.TabIndex = index + 1;
        //    punchInstanceDateBox.Text = "";

        //    //Delete Button
        //    punchInstanceDelButton.Location = new System.Drawing.Point(116, 1);
        //    punchInstanceDelButton.Name = punInstDelName;
        //    punchInstanceDelButton.Size = new System.Drawing.Size(32, 23);
        //    punchInstanceDelButton.TabIndex = index + 1;
        //    punchInstanceDelButton.Text = "Del";
        //    punchInstanceDelButton.UseVisualStyleBackColor = true;
        //    punchInstanceDelButton.Click += new EventHandler(PunchInstanceDelButton_Click);

        //    //Instance Panel
        //    punchInstancePanel.Controls.Add(punchInstanceDelButton);
        //    punchInstancePanel.Controls.Add(punchInstanceNumber);
        //    punchInstancePanel.Controls.Add(punchInstanceDateBox);
        //    punchInstancePanel.Location = new System.Drawing.Point(3, 2 + (punchInstanceHeight*index));
        //    punchInstancePanel.Name = "punchInstancePanel";
        //    punchInstancePanel.Size = new System.Drawing.Size(153, 26);
        //    punchInstancePanel.TabIndex = index+1;

        //    datePanel.Controls.Add(punchInstancePanel);


        //}

        //private void refreshContact()
        //{

        //    fNameTxtBox.Text = contactList[contactIndex].NameFirst;

        //    int contactCount = contactList[contactIndex].DateList.Count;
        //    for (int i = 0; i < contactCount; i++)
        //    {
        //        AddEntryPunchInstance(i);
        //        //int punchInstancePanelCtrlCount = datePanel.Controls[i].Controls.Count;
        //        datePanel.Controls[i].Controls[punInstDateIdx].Text = contactList[contactIndex].DateList[i].ToString("M/d/yyyy");
        //    }
        //    int purchaseCount = contactList[contactIndex].PurchaseList.Count;
        //    for (int i = 0; i < purchaseCount; i++)
        //    {
        //        AddEntryPurchaseInstance(i);
        //        //int punchInstancePanelCtrlCount = datePanel.Controls[i].Controls.Count;
        //        purchasePanel.Controls[i].Controls[purInstDateIdx].Text = contactList[contactIndex].PurchaseList[i].ToString("M/d/yyyy");
        //    }


        //}

        //private void AddEntryPurchaseInstance(int index)
        //{
        //    //int punchInstanceHeight = 26;

        //    //Move current punches down
        //    //for (int i = 0; i < index; i++)
        //    //{
        //    //    purchasePanel.Controls[i].Location = new Point(3, 2 + (index - i) * purchaseInstanceHeight);
        //    //}

        //    PurchaseInstancePanelCreate(index);

        //}
        //private void PurchaseInstancePanelCreate(int index)
        //{
        //    //Create new stuff.
        //    DynaPanel purchaseInstancePanel = new DynaPanel(index);
        //    DynaLabel purchaseInstanceNumber = new DynaLabel(index);
        //    DynaTextBox purchaseInstanceDateBox = new DynaTextBox(index);
        //    DynaButton purchaseInstanceDelButton = new DynaButton(index);



        //    //Index Number
        //    purchaseInstanceNumber.AutoSize = true;
        //    purchaseInstanceNumber.Location = new System.Drawing.Point(3, 6);
        //    purchaseInstanceNumber.Name = purInstNumName;
        //    purchaseInstanceNumber.Size = new System.Drawing.Size(16, 13);
        //    purchaseInstanceNumber.TabIndex = index + 1;
        //    purchaseInstanceNumber.Text = index + 1 + ":";

        //    //Date Box
        //    purchaseInstanceDateBox.Location = new System.Drawing.Point(38, 3);
        //    purchaseInstanceDateBox.Name = purInstDateName;
        //    purchaseInstanceDateBox.TextAlign = HorizontalAlignment.Center;
        //    purchaseInstanceDateBox.Size = new System.Drawing.Size(72, 20);
        //    purchaseInstanceDateBox.TabIndex = index + 1;
        //    purchaseInstanceDateBox.Text = "";

        //    //Delete Button
        //    purchaseInstanceDelButton.Location = new System.Drawing.Point(116, 1);
        //    purchaseInstanceDelButton.Name = purInstDelName;
        //    purchaseInstanceDelButton.Size = new System.Drawing.Size(32, 23);
        //    purchaseInstanceDelButton.TabIndex = index + 1;
        //    purchaseInstanceDelButton.Text = "Del";
        //    purchaseInstanceDelButton.UseVisualStyleBackColor = true;
        //    purchaseInstanceDelButton.Click += new EventHandler(PurchaseInstanceDelButton_Click);

        //    //Instance Panel
        //    purchaseInstancePanel.Controls.Add(purchaseInstanceDelButton);
        //    purchaseInstancePanel.Controls.Add(purchaseInstanceNumber);
        //    purchaseInstancePanel.Controls.Add(purchaseInstanceDateBox);
        //    purchaseInstancePanel.Location = new System.Drawing.Point(3, 2);
        //    purchaseInstancePanel.Name = "purchaseInstancePanel";
        //    purchaseInstancePanel.Size = new System.Drawing.Size(153, 26);
        //    purchaseInstancePanel.TabIndex = index + 1;

        //    purchasePanel.Controls.Add(purchaseInstancePanel);

        //    purchasePanel.ResumeLayout();


        //}


        
    }
}
