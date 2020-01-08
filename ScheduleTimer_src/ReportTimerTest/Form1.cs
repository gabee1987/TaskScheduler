/***************************************************************************
 * Copyright Andy Brummer 2004-2005
 * 
 * This code is provided "as is", with absolutely no warranty expressed
 * or implied. Any use is at your own risk.
 *
 * This code may be used in compiled form in any way you desire. This
 * file may be redistributed unmodified by any means provided it is
 * not sold for profit without the authors written consent, and
 * providing that this notice and the authors name is included. If
 * the source code in  this file is used in any commercial application
 * then a simple email would be nice.
 * 
 **************************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Schedule;

namespace ReportTimerTest
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 16);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "Start";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(96, 16);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "Stop";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 23);
			this.label1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(48, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 23);
			this.label2.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 102);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		ReportTimer _Timer = new ReportTimer();

		private void button1_Click(object sender, System.EventArgs e)
		{
			_Timer.Start();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			_Timer.Stop();
		}

		bool _Sync = true;

		private void Form1_Load(object sender, System.EventArgs e)
		{
			_LabelHandler = new setLabelHandler(this.setLabel);
			_Timer.Elapsed += new ReportEventHandler(_Timer_Elapsed);
			_Timer.Error +=new ExceptionEventHandler(Error);

			if (_Sync)
			{
				for(int i=0; i<60; ++i)
				{
					_Timer.AddReportEvent(new ScheduledTime("ByMinute", "60,0"), 120-i);
					_Timer.AddReportEvent(new ScheduledTime("ByMinute", "60,0"), i);
				}
			} else 
			{
				for(int i=0; i<60; ++i)
				{
					_Timer.AddAsyncReportEvent(new ScheduledTime("ByMinute", "60,0"), 120-i);
					_Timer.AddAsyncReportEvent(new ScheduledTime("ByMinute", "60,0"), i);
				}
			}
			_Timer.AddReportEvent(new ScheduledTime("ByMinute", "30,0"), 60);
			_Timer.AddReportEvent(new ScheduledTime("ByMinute", "30,0"), 0);
		}
		private void _Timer_Elapsed(object sender, ReportEventArgs e)
		{
			label1.Invoke(_LabelHandler, new object[] { e.ReportNo });
		}

		delegate void setLabelHandler(int ReportNo);
		setLabelHandler _LabelHandler;
		private void setLabel(int ReportNo)
		{
			if (ReportNo < 60)
				label1.Text = ReportNo.ToString();
			else
				label2.Text = (ReportNo-60).ToString();
		}

		private void Error(object Sender, ExceptionEventArgs Args)
		{
			string StrError = "";
			Exception e = Args.Error;
			while (e != null)
			{
				StrError += e.Message + "\r\n" + e.StackTrace + "\r\n-----------------------------\r\n";
				e = e.InnerException;
			}
			MessageBox.Show(StrError);
			Close();
		}
	}
}
