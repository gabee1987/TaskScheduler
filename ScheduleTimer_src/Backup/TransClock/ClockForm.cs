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
using System.Configuration;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using Schedule;

namespace TransClock
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private ScheduleTimer _TickTimer = new ScheduleTimer();
		private ScheduleTimer _AlarmTimer = new ScheduleTimer();
		private System.Windows.Forms.ContextMenu CxtMenu;
		private System.Windows.Forms.MenuItem itmClose;
		private System.Windows.Forms.MenuItem ItmAlarm;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			InitializeComponent();

			_Config = new Config("..\\..\\Config.xml");
			BackColor = NormalBackColor;
			_LastBackColor = AlarmColor;
			_TickTimer.Error += new ExceptionEventHandler(_TickTimer_Error);
			_AlarmTimer.Error += new ExceptionEventHandler(_TickTimer_Error);

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
			this.CxtMenu = new System.Windows.Forms.ContextMenu();
			this.itmClose = new System.Windows.Forms.MenuItem();
			this.ItmAlarm = new System.Windows.Forms.MenuItem();
			// 
			// CxtMenu
			// 
			this.CxtMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.itmClose,
																					this.ItmAlarm});
			// 
			// itmClose
			// 
			this.itmClose.Index = 0;
			this.itmClose.Text = "Close";
			this.itmClose.Click += new System.EventHandler(this.itmClose_Click);
			// 
			// ItmAlarm
			// 
			this.ItmAlarm.Index = 1;
			this.ItmAlarm.Text = "Alarm";
			this.ItmAlarm.Click += new System.EventHandler(this.ItmAlarm_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(13, 29);
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(178, 40);
			this.ContextMenu = this.CxtMenu;
			this.Font = new System.Drawing.Font("Digital Readout Upright", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Opacity = 0.5;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Transparent Clock";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.WhiteSmoke;
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);

		}
		#endregion


		private void Form1_Load(object sender, System.EventArgs e)
		{
			try 
			{
				SetLocation();
				SetFont();
				SetTickTimer();
				SetAlarmTimer();
			} 
			catch (Exception ex)
			{
				Startup.HandleException(ex);
			}
		}

		public string StrTime
		{
			get { return _StrTime; }
			set { _StrTime = value; }
		}

		public Color NormalBackColor
		{
			get	{ return Color.FromName(GetSettingDefault("back-color", "White")); }
		}

		public Color AlarmColor
		{
			get	{ return Color.FromName(GetSettingDefault("alarm-color", "Red")); }
		}

		public ScheduledTime AlarmTime
		{
			get 
			{
				string StrAlarm = GetSettingDefault("alarm", "Daily|4:30 PM");
				string[] ArrAlarm = StrAlarm.Split('|');
				if (ArrAlarm.Length != 2)
					throw new Exception("Invalid alarm format.");
				return new ScheduledTime(ArrAlarm[0], ArrAlarm[1]);
			}
		}

		private void SetTickTimer()
		{
			StrTime = DateTime.Now.ToString("T");
			_TickTimer.AddJob(
				new Schedule.ScheduledTime(EventTimeBase.BySecond, TimeSpan.Zero),
				new TickHandler(_TickTimer_Elapsed)
			);
			_TickTimer.Start();
		}

		private void _TickTimer_Elapsed(DateTime EventTime)
		{
			lock (this)
			{
				StrTime = EventTime.ToString("T");
				if (_Flashing)
				{
					Color Temp = BackColor;
					BackColor = _LastBackColor;
					_LastBackColor = Temp;
				}

			}
			this.Invoke(new System.Threading.ThreadStart(this.Invalidate));
		}

		delegate void TickHandler(DateTime tick);
		private void SetAlarmTimer()
		{
			_AlarmTimer.Stop();
			_AlarmTimer.ClearJobs();
			_AlarmTimer.AddJob(AlarmTime, new TickHandler(_AlarmTimer_Elapsed));
			_AlarmTimer.Start();
		}

		private void _AlarmTimer_Elapsed(DateTime time)
		{
			_Flashing = true;
			BackColor = NormalBackColor;
			_LastBackColor = AlarmColor;
		}

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			lock (this)
			{
				SizeF sizeF = e.Graphics.MeasureString(StrTime, _Font);
				sizeF.Height += 6; sizeF.Width += 6;
				Size S = new Size((int)sizeF.Width, (int)sizeF.Height);
				if (S != this.ClientSize)
					this.ClientSize = S;

				e.Graphics.DrawString(StrTime, _Font, new SolidBrush(_Color), 3, 3);
			}
		}

		bool _Flashing = false;
		bool _Drag = false;
		int _X, _Y;
		private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Flashing)
			{
				_Flashing = false;
				BackColor = NormalBackColor;
				_LastBackColor = AlarmColor;
			}
			_Drag = true;
			_X = e.X;
			_Y = e.Y;
		}

		private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_Drag = false;
		}

		private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_Drag == false)
				return;
			Point pCurent = Location;
			Location = new Point(Location.X + e.X-_X, Location.Y + e.Y-_Y);
		}

		private void itmClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		AlarmDialog _AlarmDialog = new AlarmDialog();
		private void ItmAlarm_Click(object sender, System.EventArgs e)
		{
			try 
			{
				_AlarmDialog.SetSchedule(GetSettingDefault("alarm", "Daily|4:30 PM"));
				if (_AlarmDialog.ShowDialog(this) == DialogResult.OK)
				{
					IScheduledItem Item = _AlarmDialog.GetSchedule();
					SetSetting("alarm", _AlarmDialog.GetScheduleString());
					_AlarmTimer.Stop();
					_AlarmTimer.ClearJobs();
					_AlarmTimer.AddJob(Item, new TickHandler(_AlarmTimer_Elapsed));
					_AlarmTimer.Start();
				}
			} 
			catch (Exception ex)
			{
				Startup.HandleException(ex);
			}
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SetSetting("location", string.Format("{0},{1}", Location.X, Location.Y));
		}

		private string GetSettingDefault(string StrKey, string StrDefault)
		{
			return _Config.GetSettingDefault(StrKey, StrDefault);
		}

		private void SetSetting(string StrKey, string StrValue)
		{
			_Config.SetSetting(StrKey, StrValue);
		}

		private void SetLocation()
		{
			string StrLocation = GetSettingDefault("location", "10,10");
			string[] ArrLocation = StrLocation.Split(',');
			if (ArrLocation.Length != 2)
				return;
			Location = new Point(int.Parse(ArrLocation[0]), int.Parse(ArrLocation[1]));
		}

		private void SetFont()
		{
			string StrFont = GetSettingDefault("font", "Digital Readout Upright");
			float Size = float.Parse(GetSettingDefault("font-size", "28"));
			_Font = new Font(StrFont, Size);
			
			_Color = Color.FromName(GetSettingDefault("color", "DarkGreen"));
		}

		private Config _Config;
		Color _Color;
		Color _LastBackColor;
		Font _Font;
		string _StrTime;

		private void _TickTimer_Error(object sender, ExceptionEventArgs e)
		{
			MessageBox.Show(e.Error.Message + "\r\n" + e.Error.StackTrace);
			Close();
		}
	}

	class Config
	{
		public Config(string StrFile)
		{
			_StrFile = StrFile;
			_Doc.Load(_StrFile);
		}
		public string GetSettingDefault(string StrKey, string StrDefault)
		{
			XmlNode Node = _Doc.SelectSingleNode("configuration/appSettings/add[@key='" + StrKey + "']");
			if (Node == null)
				return StrDefault;
			return ReadWithDefault(Node.Attributes["value"].Value, StrDefault);
		}
		public void SetSetting(string StrKey, string StrValue)
		{
			XmlNode Node = _Doc.SelectSingleNode("configuration/appSettings/add[@key='" + StrKey + "']");
			Node.Attributes["value"].Value = StrValue;
			_Doc.Save(_StrFile);
		}
		string _StrFile;
		XmlDocument _Doc = new XmlDocument();

		public static string ReadWithDefault(string StrValue, string StrDefault)
		{
			return (StrValue == null) ? StrDefault : StrValue;
		}
	}
}
