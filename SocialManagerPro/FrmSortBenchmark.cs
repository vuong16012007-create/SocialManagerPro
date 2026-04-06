using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SocialManagerPro
{
    /// <summary>
    /// Form đo và so sánh thời gian 6 thuật toán sắp xếp.
    /// </summary>
    public class FrmSortBenchmark : Form
    {
        private readonly PostLinkedList _danhSach;

        // Controls
        private ComboBox cboThuatToan;
        private Button btnChay;
        private Button btnSoSanh;
        private DataGridView dgvKetQua;
        private Label lblTieuDe;
        private Label lblChon;
        private Label lblKetQua;
        private Panel pnlBottom;

        public FrmSortBenchmark(PostLinkedList danhSach)
        {
            _danhSach = danhSach;
            InitUI();
        }

        private void InitUI()
        {
            // ── Form ─────────────────────────────────────────────────────
            Text = "So sánh thuật toán sắp xếp";
            ClientSize = new Size(620, 480);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 9.5f);

            // ── Tiêu đề ──────────────────────────────────────────────────
            lblTieuDe = new Label
            {
                Text = "⏱  Đo thời gian sắp xếp bài đăng theo Scheduled Time",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 35, 48),
                Bounds = new Rectangle(16, 14, 580, 28),
                AutoSize = false
            };

            // ── Chọn thuật toán ───────────────────────────────────────────
            lblChon = new Label
            {
                Text = "Chọn thuật toán:",
                Bounds = new Rectangle(16, 54, 120, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cboThuatToan = new ComboBox
            {
                Bounds = new Rectangle(140, 54, 220, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboThuatToan.Items.AddRange(new object[]
            {
                "1. Bubble Sort    — O(n²)",
                "2. Selection Sort — O(n²)",
                "3. Insertion Sort — O(n²)",
                "4. Quick Sort     — O(n log n)",
                "5. Merge Sort     — O(n log n)",
                "6. Heap Sort      — O(n log n)"
            });
            cboThuatToan.SelectedIndex = 0;

            btnChay = new Button
            {
                Text = "▶  Chạy",
                Bounds = new Rectangle(370, 52, 90, 30),
                BackColor = Color.FromArgb(45, 74, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnChay.FlatAppearance.BorderSize = 0;
            btnChay.Click += BtnChay_Click;

            btnSoSanh = new Button
            {
                Text = "📊  So sánh tất cả",
                Bounds = new Rectangle(470, 52, 135, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSoSanh.FlatAppearance.BorderSize = 0;
            btnSoSanh.Click += BtnSoSanh_Click;

            // ── Label kết quả ─────────────────────────────────────────────
            lblKetQua = new Label
            {
                Text = "Kết quả:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 35, 48),
                Bounds = new Rectangle(16, 96, 200, 22)
            };

            // ── DataGridView kết quả ──────────────────────────────────────
            dgvKetQua = new DataGridView
            {
                Bounds = new Rectangle(16, 122, 585, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
                                         | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 222, 226),
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 36 },
                ColumnHeadersHeight = 34,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            dgvKetQua.DefaultCellStyle.Font = new Font("Cascadia Code", 9.5f);
            dgvKetQua.DefaultCellStyle.BackColor = Color.White;
            dgvKetQua.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 240, 255);
            dgvKetQua.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvKetQua.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvKetQua.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 74, 140);
            dgvKetQua.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Cột
            var colSTT = new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "#", Width = 40 };
            var colTen = new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Thuật toán", Width = 200 };
            var colDoPhc = new DataGridViewTextBoxColumn { Name = "colDoPhc", HeaderText = "Độ phức tạp", Width = 140 };
            var colMs = new DataGridViewTextBoxColumn { Name = "colMs", HeaderText = "Thời gian (ms)", Width = 120 };
            var colNhan = new DataGridViewTextBoxColumn { Name = "colNhan", HeaderText = "Đánh giá", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };

            colSTT.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMs.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvKetQua.Columns.AddRange(new DataGridViewColumn[]
                { colSTT, colTen, colDoPhc, colMs, colNhan });

            // ── Panel bottom ──────────────────────────────────────────────
            pnlBottom = new Panel
            {
                Bounds = new Rectangle(0, 435, 620, 45),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(230, 232, 236)
            };
            var lblNote = new Label
            {
                Text = "💡 Bài đăng được sắp xếp tăng dần theo Scheduled Time sau mỗi lần chạy.",
                ForeColor = Color.FromArgb(80, 90, 110),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                Bounds = new Rectangle(12, 12, 590, 20)
            };
            pnlBottom.Controls.Add(lblNote);

            // ── Thêm vào Form ─────────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                lblTieuDe, lblChon, cboThuatToan, btnChay, btnSoSanh,
                lblKetQua, dgvKetQua, pnlBottom
            });
        }

        // ══════════════════════════════════════════════════════════════════
        //  CHẠY 1 THUẬT TOÁN
        // ══════════════════════════════════════════════════════════════════
        private void BtnChay_Click(object sender, EventArgs e)
        {
            if (_danhSach.Count == 0)
            {
                MessageBox.Show("Danh sách đang trống!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvKetQua.Rows.Clear();

            string tenTT; double ms; string doPhc;

            switch (cboThuatToan.SelectedIndex)
            {
                case 0: tenTT = "Bubble Sort"; ms = SortHelper.BubbleSort(_danhSach); doPhc = "O(n²)"; break;
                case 1: tenTT = "Selection Sort"; ms = SortHelper.SelectionSort(_danhSach); doPhc = "O(n²)"; break;
                case 2: tenTT = "Insertion Sort"; ms = SortHelper.InsertionSort(_danhSach); doPhc = "O(n²)"; break;
                case 3: tenTT = "Quick Sort"; ms = SortHelper.QuickSort(_danhSach); doPhc = "O(n log n)"; break;
                case 4: tenTT = "Merge Sort"; ms = SortHelper.MergeSort(_danhSach); doPhc = "O(n log n)"; break;
                case 5: tenTT = "Heap Sort"; ms = SortHelper.HeapSort(_danhSach); doPhc = "O(n log n)"; break;
                default: return;
            }

            int idx = dgvKetQua.Rows.Add();
            var row = dgvKetQua.Rows[idx];
            row.Cells["colSTT"].Value = 1;
            row.Cells["colTen"].Value = tenTT;
            row.Cells["colDoPhc"].Value = doPhc;
            row.Cells["colMs"].Value = ms.ToString("F6") + " ms";
            row.Cells["colNhan"].Value = ms < 0.001 ? "⚡ Cực nhanh"
                                       : ms < 0.01 ? "🥇 Rất nhanh"
                                       : ms < 0.1 ? "✅ Nhanh"
                                       : "🔵 Trung bình";
            // Màu dòng
            row.DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255);

            MessageBox.Show(
              tenTT + " hoàn thành!\n\n" +
             "Số lần chạy   : 100.000 lần\n" +
            "Số bài đăng   : " + _danhSach.Count + "\n" +
             "TB mỗi lần    : " + ms.ToString("F6") + " ms\n" +
             "Độ phức tạp   : " + doPhc,
                "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ══════════════════════════════════════════════════════════════════
        //  SO SÁNH TẤT CẢ 6 THUẬT TOÁN
        // ══════════════════════════════════════════════════════════════════
        private void BtnSoSanh_Click(object sender, EventArgs e)
        {
            if (_danhSach.Count == 0)
            {
                MessageBox.Show("Danh sách đang trống!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvKetQua.Rows.Clear();

            List<SortResult> results = SortHelper.BenchmarkAll(_danhSach);

            // Tìm min/max để tô màu
            double minMs = double.MaxValue, maxMs = double.MinValue;
            foreach (var r in results)
            {
                if (r.ThoiGian_ms < minMs) minMs = r.ThoiGian_ms;
                if (r.ThoiGian_ms > maxMs) maxMs = r.ThoiGian_ms;
            }

            string[] doPhcArr = { "O(n²)", "O(n²)", "O(n²)", "O(n log n)", "O(n log n)", "O(n log n)" };

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                int idx = dgvKetQua.Rows.Add();
                var row = dgvKetQua.Rows[idx];

                string nhanXet;
                Color bgColor;

                if (r.ThoiGian_ms == minMs)
                {
                    nhanXet = "🥇 Nhanh nhất";
                    bgColor = Color.FromArgb(212, 237, 218);   // xanh lá nhạt
                }
                else if (r.ThoiGian_ms == maxMs)
                {
                    nhanXet = "🔴 Chậm nhất";
                    bgColor = Color.FromArgb(248, 215, 218);   // đỏ nhạt
                }
                else
                {
                    nhanXet = "✅ Trung bình";
                    bgColor = Color.White;
                }

                row.Cells["colSTT"].Value = i + 1;
                row.Cells["colTen"].Value = r.TenThuatToan;
                row.Cells["colDoPhc"].Value = doPhcArr[i];
                row.Cells["colMs"].Value = r.ThoiGian_ms.ToString("F6") + " ms";
                row.Cells["colNhan"].Value = nhanXet;
                row.DefaultCellStyle.BackColor = bgColor;
            }
        }
    }
}