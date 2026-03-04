using System;
using System.Windows.Forms;
using TeklaApp.ViewModels;

namespace TeklaApp.Views
{
    public class MainForm : Form
    {
        private TextBox txtInfo;
        private MainViewModel _viewModel;

        public MainForm()
        {
            _viewModel = new MainViewModel();

            Text = "Công cụ Tekla 2025";
            Size = new System.Drawing.Size(350, 450);
            StartPosition = FormStartPosition.CenterScreen;

            Button btnReadParams = new Button();
            btnReadParams.Text = "Lệnh 1: Đọc TS Cấu Kiện";
            btnReadParams.Location = new System.Drawing.Point(60, 20);
            btnReadParams.Size = new System.Drawing.Size(200, 40);
            btnReadParams.Click += BtnReadParams_Click;

            Button btnDeleteCut = new Button();
            btnDeleteCut.Text = "Lệnh 2: Xóa PartCut";
            btnDeleteCut.Location = new System.Drawing.Point(60, 70);
            btnDeleteCut.Size = new System.Drawing.Size(200, 40);
            btnDeleteCut.Click += BtnDeleteCut_Click;

            Button btnAddAssembly = new Button();
            btnAddAssembly.Text = "Lệnh 3: Gộp Assembly";
            btnAddAssembly.Location = new System.Drawing.Point(60, 120);
            btnAddAssembly.Size = new System.Drawing.Size(200, 40);
            btnAddAssembly.Click += BtnAddAssembly_Click;

            Button btnRemoveDuplicates = new Button();
            btnRemoveDuplicates.Text = "Lệnh 4: Xóa PT Trùng";
            btnRemoveDuplicates.Location = new System.Drawing.Point(60, 170);
            btnRemoveDuplicates.Size = new System.Drawing.Size(200, 40);
            btnRemoveDuplicates.Click += BtnRemoveDuplicates_Click;

            txtInfo = new TextBox();
            txtInfo.Multiline = true;
            txtInfo.Location = new System.Drawing.Point(20, 230);
            txtInfo.Size = new System.Drawing.Size(290, 150);
            txtInfo.ReadOnly = true;
            txtInfo.ScrollBars = ScrollBars.Vertical;

            Controls.Add(btnReadParams);
            Controls.Add(btnDeleteCut);
            Controls.Add(btnAddAssembly);
            Controls.Add(btnRemoveDuplicates);
            Controls.Add(txtInfo);
        }

        private void BtnRemoveDuplicates_Click(object sender, EventArgs e)
        {
            string result = _viewModel.RemoveDuplicateCuts();
            UpdateInfo(result);
        }


        private void BtnReadParams_Click(object sender, EventArgs e)
        {
            string result = _viewModel.ReadParameters();
            UpdateInfo(result);
        }

        private void BtnDeleteCut_Click(object sender, EventArgs e)
        {
            string result = _viewModel.DeletePartCuts();
            UpdateInfo(result);
        }

        private void BtnAddAssembly_Click(object sender, EventArgs e)
        {
            string result = _viewModel.JoinAssembly();
            UpdateInfo(result);
        }

        private void UpdateInfo(string message)
        {
            txtInfo.Text = message;
        }
    }
}
