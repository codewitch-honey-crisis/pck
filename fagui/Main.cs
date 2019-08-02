using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Pck
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();
		}

		private void Regex_TextChanged(object sender, EventArgs e)
		{
			RenderTimer.Enabled = true;
		}

		private void Input_TextChanged(object sender, EventArgs e)
		{
			RenderTimer.Enabled = true;
		}

		private void RenderTimer_Tick(object sender, EventArgs e)
		{
			RenderTimer.Enabled = false;
			CharFA<string> fa = null;
			try
			{
				fa = CharFA<string>.Parse(Regex.Text,"Accept");
			}
			catch(Exception ex)
			{
				Status.Text = string.Format("Error Parsing Regex: {0}", ex.Message);
			}
			if(null!=fa)
			{
				// mark our states for displaying the DFA later
				foreach(var ffa in fa.FillClosure())
					ffa.Tag = ffa;
				
				try
				{
					var kvp=fa.Lex(Input.Text.GetEnumerator(),"#ERROR");
					if(kvp.Value.Length!=Input.Text.Length)
					{
						Status.Text = string.Format("Input error at {0}",kvp.Value.Length);
					} else
						Status.Text = "Successfully Lexed.";
				}
				catch(Exception eex)
				{
					Status.Text = string.Format("Input error: {0}", eex.Message);
				}
				var options = new CharFA<string>.DotGraphOptions();
				
				options.DebugString = Input.Text;
				var dfa = fa.ToDfa();
				dfa.TrimDuplicates();
				try
				{
					using (var stm = fa.RenderToStream("jpg", false, options))
						NfaGraph.Image = Image.FromStream(stm);
				} catch
				{
				}
				options.StatePrefix = @"Q";
				options.DebugSourceNfa = fa;
				try
				{
					using (var stm = dfa.RenderToStream("jpg", false, options))
						DfaGraph.Image = Image.FromStream(stm);

				}
				catch { }

			}
		}
	}
}
