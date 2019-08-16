using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pck
{
	public partial class ProgressForm : Form
	{
		public ProgressForm()
		{
			InitializeComponent();
		}
		public void ClearLog()
		{
			StatusBox.Text = "";
		}
		public void WriteLog(string @string)
		{
			if(null!=@string)
				StatusBox.AppendText(@string);
		}
	}
}
