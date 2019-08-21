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
	public partial class Progress : Form
	{
		public Progress()
		{
			InitializeComponent();
		}
		
		public void ClearLog()
		{
			StatusBox.BeginInvoke(new Action<string>(_ClearLog),null);
		}
		void _ClearLog(string dummy) { StatusBox.Text = "";Application.DoEvents(); }
		public void WriteLog(string @string)
		{
			if(null!=@string)
				StatusBox.BeginInvoke(new Action<string>(StatusBox.AppendText),@string);
			Application.DoEvents();
		}
		
	}
}
