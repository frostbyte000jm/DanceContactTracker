using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DanceContactTracker.DynamicObjects
{
    public partial class DynaLabel : Label
    {
        //decaration
        private int index;

        //public Names
        public int Index { get { return index; } set { if (value >= 0) index = value; } }

        public DynaLabel(int idx)
        {
            InitializeComponent();
            index = idx;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
