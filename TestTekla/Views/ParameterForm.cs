using System;
using System.Drawing;
using System.Windows.Forms;
using TeklaApp.ViewModels;

namespace TeklaApp.Views
{
    public class ParameterForm : Form
    {
        private ParameterViewModel _viewModel;
        private TextBox txtInfo;
        private Button btnRead;

        public ParameterForm()
        {
            _viewModel = new ParameterViewModel();

            this.Text = "Đọc Parameter Cấu Kiện";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            btnRead = new Button();
            btnRead.Text = "Chọn & Đọc Thông Số";
            btnRead.Location = new Point(20, 20);
            btnRead.Size = new Size(340, 45);
            btnRead.BackColor = Color.LightBlue;
            btnRead.Click += BtnRead_Click;

            txtInfo = new TextBox();
            txtInfo.Multiline = true;
            txtInfo.ReadOnly = true;
            txtInfo.ScrollBars = ScrollBars.Vertical;
            txtInfo.Location = new Point(20, 80);
            txtInfo.Size = new Size(340, 360);
            txtInfo.Font = new Font("Consolas", 9F);

            this.Controls.Add(btnRead);
            this.Controls.Add(txtInfo);
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            // Ẩn form tạm thời để không che khuất màn hình Tekla khi pick
            this.Hide();
            try
            {
                string result = _viewModel.ReadParameters();
                txtInfo.Text = result;
            }
            finally
            {
                this.Show();
            }
        }
    }
}
