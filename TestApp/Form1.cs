using Groenteboer.Technova.Devices.Scales;

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

        private void Scale_WeightChanged(object? sender, ScaleWeightChangedEventArgs e)
        {
            Invoke(() =>
            {
                lblValue.Text = e.ToString();
            });
        }

        private void Scale_StatusChanged(object? sender, ScaleStatusChangedEventArgs e)
        {
            Invoke(() =>
            {
                SetStatusLabel(e.Status);
            });
        }

        private void Scale_ErrorOccurred(object? sender, ScaleErrorEventArgs e)
        {
            Invoke(() =>
            {
                lblStatus.Text = e.Message;
                lblStatus.ForeColor = Color.Firebrick;
            });
        }

        private void SetStatusLabel(ScaleStatus status)
        {
            lblStatus.Text = status switch
            {
                ScaleStatus.Disconnected => "Niet verbonden",
                ScaleStatus.Standby => "Stand-by",
                ScaleStatus.Ready => "Klaar voor gebruik",
                _ => status.ToString()
            };

            lblStatus.ForeColor = status switch
            {
                ScaleStatus.Disconnected => Color.Firebrick,
                ScaleStatus.Standby => Color.Goldenrod,
                ScaleStatus.Ready => Color.ForestGreen,
                _ => SystemColors.ControlText
            };
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scale.Dispose();
        }
    }
}
