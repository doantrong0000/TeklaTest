using System;
using System.Windows.Forms;
using TeklaApp.ViewModels;

namespace TeklaApp.Views
{
    public class MainForm : Form
    {
        private MainViewModel _viewModel;

        public MainForm()
        {
            _viewModel = new MainViewModel();

            Text = "Công cụ Tekla 2025";
            Size = new System.Drawing.Size(350, 320);
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

            Button btnStepTag = new Button();
            btnStepTag.Text = "Lệnh 5: Ký hiệu giật cấp";
            btnStepTag.Location = new System.Drawing.Point(60, 220);
            btnStepTag.Size = new System.Drawing.Size(200, 40);
            btnStepTag.Click += BtnStepTag_Click;

            Controls.Add(btnReadParams);
            Controls.Add(btnDeleteCut);
            Controls.Add(btnAddAssembly);
            Controls.Add(btnRemoveDuplicates);
            Controls.Add(btnStepTag);
        }

        private void BtnStepTag_Click(object sender, EventArgs e)
        {
            StepTagViewModel tgViewModel = new StepTagViewModel();
            string result = tgViewModel.CreateStepTag();
            MessageBox.Show(result, "Thông báo");
        }

        private void BtnRemoveDuplicates_Click(object sender, EventArgs e)
        {
            string result = _viewModel.RemoveDuplicateCuts();
            MessageBox.Show(result, "Thông báo");
        }


        private void BtnReadParams_Click(object sender, EventArgs e)
        {
            ParameterForm paramForm = new ParameterForm();
            paramForm.ShowDialog(this);
        }

        private void BtnDeleteCut_Click(object sender, EventArgs e)
        {
            string result = _viewModel.DeletePartCuts();
            MessageBox.Show(result, "Thông báo");
        }

        private void BtnAddAssembly_Click(object sender, EventArgs e)
        {
            string result = _viewModel.JoinAssembly();
            MessageBox.Show(result, "Thông báo");
        }

    }
}
