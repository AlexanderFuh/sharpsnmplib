/*
 * Created by SharpDevelop.
 * User: lexli
 * Date: 2008-12-14
 * Time: 14:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Lextm.SharpSnmpLib.Agent
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
	    private SnmpDemon _demon = new SnmpDemon();
	    
		public MainForm()
		{
			InitializeComponent();            
            Application.Idle += Application_Idle;
		}

        private void Application_Idle(object sender, EventArgs e)
        {
            btnStart.Enabled = !_demon.Active;
            btnStop.Enabled = _demon.Active;
        }

		private void BtnStartClick(object sender, EventArgs e)
		{
		    _demon.Start(int.Parse(txtAgentPort.Text));
		}

        private void BtnStopClick(object sender, EventArgs e)
		{
			_demon.Stop();
		}
			
		private void BtnTrapClick(object sender, EventArgs e)
		{
		    IPAddress ip = IPAddress.Parse(txtIP.Text);
		    if (ip == null)
		    {
		        return;
		    }

		    Messenger.SendTrapV1(new IPEndPoint(ip, int.Parse(txtPort.Text)),
		                         IPAddress.Loopback, // here should be IP of the current machine.
		                         new OctetString("public"),
		                         new ObjectIdentifier(new uint[] { 1, 3, 6 }),
		                         GenericCode.ColdStart,
		                         0,
		                         0,
		                         new List<Variable>());
		}

	    private void BtnTrap2Click(object sender, EventArgs e)
		{
            IPAddress ip = IPAddress.Parse(txtIP.Text);
            if (ip == null)
            {
                return;
            }

			Messenger.SendTrapV2(0, VersionCode.V2,
			                 new IPEndPoint(ip, int.Parse(txtPort.Text)),
			                 new OctetString("public"),
			                 new ObjectIdentifier(new uint[] { 1, 3, 6 }),
			                 0,
			                 new List<Variable>());
		}
		
		private void BtnInformClick(object sender, EventArgs e)
		{
            IPAddress ip = IPAddress.Parse(txtIP.Text);
            if (ip == null)
            {
                return;
            }

			try
			{
				Messenger.SendInform(0, VersionCode.V2, 
				                 new IPEndPoint(ip, int.Parse(txtPort.Text)),
				                 new OctetString("public"),
				                 new ObjectIdentifier(new uint[] { 1, 3, 6 }),
				                 0,
				                 new List<Variable>(), 2000);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
