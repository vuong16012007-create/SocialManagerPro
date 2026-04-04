using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SocialManagerPro
{
    public partial class FrmMain : Form
    {
        private readonly PostLinkedList _danhSach = new PostLinkedList();
        private PostStatus? _filterStatus = null;
        private Platform? _filterPlatform = null;
        private int _idDangChon = -1;

        public FrmMain()
        {
            InitializeComponent();
            ThemDuLieuMau();
            HienThiDanhSach(_danhSach.LayTatCa());
            DangKySuKien();
        }

        private void DangKySuKien()
        {
            txtSearch.TextChanged += (s, e) => ApDungLoc();

            btnAll.Click += (s, e) => DatLocTrangThai(null, btnAll);
            btnDraft.Click += (s, e) => DatLocTrangThai(PostStatus.Draft, btnDraft);
            btnScheduled.Click += (s, e) => DatLocTrangThai(PostStatus.Scheduled, btnScheduled);
            btnPublished.Click += (s, e) => DatLocTrangThai(PostStatus.Published, btnPublished);
            btnFailed.Click += (s, e) => DatLocTrangThai(PostStatus.Failed, btnFailed);

            cboPlatform.SelectedIndexChanged += (s, e) =>
            {
                _filterPlatform = cboPlatform.SelectedIndex == 0
                    ? (Platform?)null
                    : (Platform)(cboPlatform.SelectedIndex - 1);
                ApDungLoc();
            };

            dgvPosts.CellPainting += DgvPosts_CellPainting;
            dgvPosts.SelectionChanged += DgvPosts_SelectionChanged;
            dgvPosts.CellClick += DgvPosts_CellClick;
            dgvPosts.CellContentClick += DgvPosts_CellContentClick;

            btnSaveDraft.Click += BtnSaveDraft_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSchedulePost.Click += BtnSchedulePost_Click;

            foreach (Control c in pnlSidebar.Controls)
                if (c is Button btn)
                    btn.Click += SidebarBtn_Click;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HIỂN THỊ DANH SÁCH
        // ══════════════════════════════════════════════════════════════════════
        private void HienThiDanhSach(List<Post> posts)
        {
            dgvPosts.Rows.Clear();

            foreach (Post p in posts)
            {
                int idx = dgvPosts.Rows.Add();
                var row = dgvPosts.Rows[idx];

                row.Cells["colCheck"].Value = p.IsSelected;
                row.Cells["colPlatform"].Value = p.Platform.ToString();
                row.Cells["colTitle"].Value = p.Title;
                row.Cells["colContent"].Value = p.Content.Length > 50
                                                  ? p.Content.Substring(0, 50) + "…"
                                                  : p.Content;
                row.Cells["colTime"].Value = p.ScheduledTime.ToString("MM/dd/yyyy HH:mm");
                row.Cells["colStatus"].Value = p.Status.ToString();
                row.Cells["colActions"].Value = "•••";
                row.Tag = p.Id;
            }

            lblPostCount.Text = "Your Social Media Posts  (" + posts.Count + ")";

            // ── Bỏ chọn tất cả dòng sau khi load → panel phải giữ trống ──
            dgvPosts.ClearSelection();
            _idDangChon = -1;
        }
        // ══════════════════════════════════════════════════════════════════════
        //  LỌC
        // ══════════════════════════════════════════════════════════════════════
        private void ApDungLoc()
        {
            string kw = txtSearch.Text.Trim();
            if (kw == "Search") kw = "";
            HienThiDanhSach(_danhSach.Loc(kw, _filterStatus, _filterPlatform));
        }

        private void DatLocTrangThai(PostStatus? status, Button active)
        {
            _filterStatus = status;
            foreach (Control c in pnlStatusFilter.Controls)
                if (c is Button b) { b.BackColor = Color.FromArgb(240, 240, 240); b.ForeColor = Color.FromArgb(80, 80, 80); }
            active.BackColor = Color.FromArgb(45, 74, 140);
            active.ForeColor = Color.White;
            ApDungLoc();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CUSTOM PAINT
        // ══════════════════════════════════════════════════════════════════════
        private void DgvPosts_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Icon nền tảng hình tròn
            if (e.ColumnIndex == dgvPosts.Columns["colPlatform"].Index)
            {
                e.PaintBackground(e.ClipBounds, true);
                if (e.Value == null) { e.Handled = true; return; }

                string pName = e.Value.ToString();
                Color fill; string label;
                switch (pName)
                {
                    case "Facebook": fill = Color.FromArgb(24, 119, 242); label = "f"; break;
                    case "Instagram": fill = Color.FromArgb(193, 53, 132); label = "IG"; break;
                    case "LinkedIn": fill = Color.FromArgb(10, 102, 194); label = "in"; break;
                    case "TikTok": fill = Color.Black; label = "TT"; break;
                    default: fill = Color.Gray; label = "?"; break;
                }
                int d = 28;
                int x = e.CellBounds.X + (e.CellBounds.Width - d) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - d) / 2;
                var rc = new Rectangle(x, y, d, d);
                using (var br = new SolidBrush(fill)) e.Graphics.FillEllipse(br, rc);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var brT = new SolidBrush(Color.White);
                e.Graphics.DrawString(label, new Font("Segoe UI", 7.5f, FontStyle.Bold), brT, rc, fmt);
                e.Handled = true;
                return;
            }

            // Badge trạng thái bo góc
            if (e.ColumnIndex == dgvPosts.Columns["colStatus"].Index)
            {
                e.PaintBackground(e.ClipBounds, true);
                if (e.Value == null) { e.Handled = true; return; }

                string st = e.Value.ToString();
                Color badgeColor = st switch
                {
                    "Published" => Color.FromArgb(40, 167, 69),
                    "Scheduled" => Color.FromArgb(255, 149, 0),
                    "Draft" => Color.FromArgb(0, 123, 255),
                    "Failed" => Color.FromArgb(220, 53, 69),
                    _ => Color.Gray
                };
                int bw = 85, bh = 24;
                int bx = e.CellBounds.X + (e.CellBounds.Width - bw) / 2;
                int by = e.CellBounds.Y + (e.CellBounds.Height - bh) / 2;
                var rc = new Rectangle(bx, by, bw, bh);
                using (var path = RoundedRect(rc, 5))
                using (var br = new SolidBrush(badgeColor))
                    e.Graphics.FillPath(br, path);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var brT = new SolidBrush(Color.White);
                e.Graphics.DrawString(st, new Font("Segoe UI", 8.5f, FontStyle.Bold), brT, rc, fmt);
                e.Handled = true;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CHỌN DÒNG → PANEL CHI TIẾT
        // ══════════════════════════════════════════════════════════════════════
        private void DgvPosts_SelectionChanged(object sender, EventArgs e)
        {
            // Không tự điền khi không có dòng được chọn
            if (dgvPosts.SelectedRows.Count == 0)
            {
                _idDangChon = -1;
                return;
            }

            var row = dgvPosts.SelectedRows[0];
            if (row.Tag == null) return;

            int id = (int)row.Tag;
            Post post = _danhSach.TimTheoId(id);
            if (post == null) return;

            _idDangChon = id;
            DienPanelChiTiet(post);
        }

        private void DienPanelChiTiet(Post post)
        {
            txtTitle.Text = post.Title;
            txtContent.Text = post.Content;
            chkFacebook.Checked = post.Platform == Platform.Facebook;
            chkInstagram.Checked = post.Platform == Platform.Instagram;
            chkLinkedIn.Checked = post.Platform == Platform.LinkedIn;
            chkTikTok.Checked = post.Platform == Platform.TikTok;
            dtpScheduled.Value = post.ScheduledTime < dtpScheduled.MinDate ? DateTime.Now : post.ScheduledTime;
            cboStatus.SelectedIndex = (int)post.Status;
            lblPreviewPlatform.Text = post.Platform.ToString();
            lblPreviewPlatform.BackColor = PlatformColor(post.Platform);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONTEXT MENU (•••)
        // ══════════════════════════════════════════════════════════════════════
        private void DgvPosts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex != dgvPosts.Columns["colActions"].Index) return;
            var row = dgvPosts.Rows[e.RowIndex];
            if (row.Tag == null) return;
            int id = (int)row.Tag;

            var menu = new ContextMenuStrip();
            menu.Items.Add("Edit", null, (s, ev) => ChinhSua(id));
            menu.Items.Add("Duplicate", null, (s, ev) => NhanDoi(id));
            menu.Items.Add("Delete", null, (s, ev) => Xoa(id));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Publish Now", null, (s, ev) => DangBaiNgay(id));
            menu.Show(dgvPosts, dgvPosts.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true).Location);
        }

        private void DgvPosts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == dgvPosts.Columns["colCheck"].Index)
                dgvPosts.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CRUD
        // ══════════════════════════════════════════════════════════════════════
        private void ChinhSua(int id)
        {
            Post post = _danhSach.TimTheoId(id);
            if (post == null) return;
            _idDangChon = id;
            DienPanelChiTiet(post);
        }

        private void NhanDoi(int id)
        {
            Post ban = _danhSach.NhanDoi(id);
            if (ban != null) { ApDungLoc(); MessageBox.Show($"Đã tạo bản sao: \"{ban.Title}\"", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private void Xoa(int id)
        {
            Post post = _danhSach.TimTheoId(id);
            string ten = post?.Title ?? $"#{id}";
            if (MessageBox.Show($"Xoá bài đăng \"{ten}\"?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _danhSach.Xoa(id);
                if (_idDangChon == id) { _idDangChon = -1; XoaTrangPanel(); }
                ApDungLoc();
            }
        }

        private void DangBaiNgay(int id) { _danhSach.DangBai(id); ApDungLoc(); }

        // ══════════════════════════════════════════════════════════════════════
        //  PANEL BUTTONS
        // ══════════════════════════════════════════════════════════════════════
        private void BtnSaveDraft_Click(object sender, EventArgs e)
        {
            Platform nP = LayNenTangDuocChon();
            if (_idDangChon == -1)
                _danhSach.ThemCuoi(txtTitle.Text.Trim(), txtContent.Text.Trim(), nP, dtpScheduled.Value, PostStatus.Draft);
            else
                _danhSach.Sua(_idDangChon, txtTitle.Text.Trim(), txtContent.Text.Trim(), nP, dtpScheduled.Value, (PostStatus)cboStatus.SelectedIndex);
            ApDungLoc();
            MessageBox.Show("Đã lưu!", "Save Draft", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e) { if (_idDangChon != -1) Xoa(_idDangChon); }

        private void BtnSchedulePost_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("Vui lòng nhập tiêu đề!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            Platform nP = LayNenTangDuocChon();
            if (_idDangChon == -1)
                _danhSach.ThemCuoi(txtTitle.Text.Trim(), txtContent.Text.Trim(), nP, dtpScheduled.Value, PostStatus.Scheduled);
            else
                _danhSach.Sua(_idDangChon, txtTitle.Text.Trim(), txtContent.Text.Trim(), nP, dtpScheduled.Value, PostStatus.Scheduled);
            _idDangChon = -1; XoaTrangPanel(); ApDungLoc();
            MessageBox.Show("Đã lên lịch!", "Schedule Post", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SidebarBtn_Click(object sender, EventArgs e)
        {
            // Tắt highlight tất cả nút sidebar
            foreach (Control c in pnlSidebar.Controls)
                if (c is Button b)
                {
                    b.BackColor = Color.Transparent;
                    b.ForeColor = Color.FromArgb(200, 200, 210);
                }

            // Highlight nút được click
            var btn = (Button)sender;
            btn.BackColor = Color.FromArgb(45, 55, 75);
            btn.ForeColor = Color.White;

            // ── Xử lý từng nút ────────────────────────────────────────────
            if (btn == btnCreatePost)
            {
                // Tạo bài mới → xoá trắng panel phải
                _idDangChon = -1;
                XoaTrangPanel();
                _filterStatus = null;
                _filterPlatform = null;
                ApDungLoc();
            }
            else if (btn == btnNavScheduled)
            {
                // Lọc chỉ hiện bài Scheduled
                _filterStatus = PostStatus.Scheduled;
                _filterPlatform = null;

                // Đồng bộ highlight nút filter bên trên
                DatLocTrangThai(PostStatus.Scheduled, btnScheduled);
                ApDungLoc();
            }
            else if (btn == btnList)
            {
                // Hiện toàn bộ danh sách
                _filterStatus = null;
                _filterPlatform = null;

                // Đồng bộ highlight nút filter "All"
                DatLocTrangThai(null, btnAll);
                ApDungLoc();
            }
            else if (btn == btnDashboard)
            {
                // Hiện thống kê nhanh
                var ds = _danhSach.LayTatCa();
                int published = 0, scheduled = 0, draft = 0, failed = 0;
                foreach (var p in ds)
                {
                    switch (p.Status)
                    {
                        case PostStatus.Published: published++; break;
                        case PostStatus.Scheduled: scheduled++; break;
                        case PostStatus.Draft: draft++; break;
                        case PostStatus.Failed: failed++; break;
                    }
                }

                MessageBox.Show(
                    "📊  THỐNG KÊ BÀI ĐĂNG\n\n" +
                    "✅  Published  : " + published + " bài\n" +
                    "🟠  Scheduled  : " + scheduled + " bài\n" +
                    "🔵  Draft      : " + draft + " bài\n" +
                    "🔴  Failed     : " + failed + " bài\n" +
                    "─────────────────────\n" +
                    "📝  Tổng cộng  : " + ds.Count + " bài",
                    "Dashboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (btn == btnAccounts)
            {
                // Hiện danh sách nền tảng đang dùng
                MessageBox.Show(
                    "🌐  TÀI KHOẢN ĐÃ KẾT NỐI\n\n" +
                    "  f   Facebook\n" +
                    "  IG  Instagram\n" +
                    "  in  LinkedIn\n" +
                    "  TT  TikTok",
                    "Accounts",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        // ══════════════════════════════════════════════════════════════════════
        //  TIỆN ÍCH
        // ══════════════════════════════════════════════════════════════════════
        private Platform LayNenTangDuocChon()
        {
            if (chkFacebook.Checked) return Platform.Facebook;
            if (chkInstagram.Checked) return Platform.Instagram;
            if (chkLinkedIn.Checked) return Platform.LinkedIn;
            if (chkTikTok.Checked) return Platform.TikTok;
            return Platform.Facebook;
        }

        private void XoaTrangPanel()
        {
            txtTitle.Clear(); txtContent.Clear();
            chkFacebook.Checked = chkInstagram.Checked = chkLinkedIn.Checked = chkTikTok.Checked = false;
            cboStatus.SelectedIndex = 0;
            dtpScheduled.Value = DateTime.Now.AddDays(1);
        }

        private static Color PlatformColor(Platform p) => p switch
        {
            Platform.Facebook => Color.FromArgb(24, 119, 242),
            Platform.Instagram => Color.FromArgb(193, 53, 132),
            Platform.LinkedIn => Color.FromArgb(10, 102, 194),
            Platform.TikTok => Color.Black,
            _ => Color.Gray
        };

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void ThemDuLieuMau()
        {
            _danhSach.ThemCuoi(
                "Hôm nay trời đẹp quá! ☀️",
                "Buổi sáng thức dậy nhìn ra cửa sổ, bầu trời xanh trong và nắng vàng rực rỡ. Một ngày tuyệt vời để ra ngoài dạo chơi!",
                Platform.Facebook, new DateTime(2023, 12, 12, 0, 0, 0), PostStatus.Published);

            _danhSach.ThemCuoi(
                "Ảnh du lịch Đà Lạt 🌿",
                "Cuối tuần vừa rồi mình có dịp ghé thăm thành phố ngàn hoa. Khí hậu mát mẻ, cảnh đẹp như tranh vẽ!",
                Platform.Instagram, new DateTime(2023, 12, 12, 3, 0, 0), PostStatus.Scheduled);

            _danhSach.ThemCuoi(
                "Chia sẻ kinh nghiệm học lập trình C#",
                "Sau 3 tháng học WinForms và cấu trúc dữ liệu, mình đúc kết được nhiều bài học quý giá. Hãy cùng tham khảo nhé!",
                Platform.LinkedIn, new DateTime(2023, 12, 12, 13, 0, 0), PostStatus.Draft);

            _danhSach.ThemCuoi(
                "Chúc mừng sinh nhật bạn thân! 🎂🎉",
                "Chúc bạn luôn vui vẻ, mạnh khỏe và thành công trên con đường phía trước. Tình bạn mãi bền vững!",
                Platform.Facebook, new DateTime(2022, 12, 18, 20, 0, 0), PostStatus.Published);

            _danhSach.ThemCuoi(
                "Review quán cà phê mới mở 🍵",
                "Vừa khám phá một quán cà phê siêu xinh ở quận 1. Không gian ấm cúng, đồ uống ngon, giá cả hợp lý!",
                Platform.Instagram, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);

            _danhSach.ThemCuoi(
                "Tuyển dụng thực tập sinh IT 💻",
                "Công ty chúng tôi đang tìm kiếm thực tập sinh lập trình .NET, có kiến thức về WinForms hoặc ASP.NET.",
                Platform.LinkedIn, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);

            _danhSach.ThemCuoi(
                "Video nấu phở bò truyền thống 🍜",
                "Hướng dẫn chi tiết cách nấu phở bò chuẩn vị Hà Nội ngay tại nhà. Công thức gia truyền không thể bỏ qua!",
                Platform.Facebook, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Published);

            _danhSach.ThemCuoi(
                "Kỷ niệm 2 năm thành lập nhóm 🥳",
                "Hai năm đồng hành cùng nhau, chúng ta đã vượt qua bao thử thách. Cảm ơn tất cả mọi người đã luôn ủng hộ!",
                Platform.Instagram, new DateTime(2022, 12, 12, 19, 30, 0), PostStatus.Published);

            _danhSach.ThemCuoi(
                "Sự kiện workshop lập trình miễn phí",
                "Tham gia workshop miễn phí về LinkedList và cấu trúc dữ liệu vào cuối tuần này. Đăng ký ngay hôm nay!",
                Platform.Instagram, new DateTime(2022, 12, 12, 19, 0, 0), PostStatus.Scheduled);

            _danhSach.ThemCuoi(
                "Mẹo tăng năng suất làm việc 📈",
                "5 thói quen giúp mình tăng gấp đôi năng suất mỗi ngày: dậy sớm, lập kế hoạch, tập trung, nghỉ ngơi đúng cách.",
                Platform.LinkedIn, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);

            _danhSach.ThemCuoi(
                "Flash sale cuối năm giảm đến 50% 🛍️",
                "Đừng bỏ lỡ đợt sale lớn nhất năm! Hàng nghìn sản phẩm giảm giá sâu, số lượng có hạn.",
                Platform.Facebook, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Published);

            _danhSach.ThemCuoi(
                "Bộ ảnh check-in Hội An cổ kính 🏮",
                "Phố cổ Hội An về đêm đẹp mê hồn với những chiếc đèn lồng rực rỡ. Nhất định phải ghé một lần!",
                Platform.Instagram, new DateTime(2022, 12, 12, 9, 0, 0), PostStatus.Published);
        }
    }
}