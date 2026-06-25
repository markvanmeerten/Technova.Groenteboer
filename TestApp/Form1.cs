using Groenteboer.Technova.Devices.Scales;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private readonly IScale _scale = ScaleFactory.CreateDefault();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _scale.WeightChanged += Scale_WeightChanged;
            _scale.StatusChanged += Scale_StatusChanged;
            _scale.ErrorOccurred += Scale_ErrorOccurred;

            _scale.Start();

            lblType.Text = _scale.ToString();
        }

        private void Scale_WeightChanged(object sender, ScaleWeightChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(delegate
                {
                    lblValue.Text = e.ToString();
                }));
            }
            else
            {
                lblValue.Text = e.ToString();
            }
        }

        private void Scale_StatusChanged(object sender, ScaleStatusChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(delegate
                {
                    SetStatusLabel(e.Status);
                }));
            }
            else
            {
                SetStatusLabel(e.Status);
            }
        }

        private void Scale_ErrorOccurred(object sender, ScaleErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(delegate
                {
                    lblStatus.Text = e.Message;
                    lblStatus.ForeColor = Color.Firebrick;
                }));
            }
            else
            {
                lblStatus.Text = e.Message;
                lblStatus.ForeColor = Color.Firebrick;
            }
        }

        private void SetStatusLabel(ScaleStatus status)
        {
            switch (status)
            {
                case ScaleStatus.Disconnected:
                    lblStatus.Text = "Niet verbonden";
                    lblStatus.ForeColor = Color.Firebrick;
                    break;

                case ScaleStatus.Standby:
                    lblStatus.Text = "Stand-by";
                    lblStatus.ForeColor = Color.Goldenrod;
                    break;

                case ScaleStatus.Ready:
                    lblStatus.Text = "Klaar voor gebruik";
                    lblStatus.ForeColor = Color.ForestGreen;
                    break;

                default:
                    lblStatus.Text = status.ToString();
                    lblStatus.ForeColor = SystemColors.ControlText;
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scale.Dispose();
        }
    }
}
