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

            // 3 nút icon top bar
            btnIconMenu.Click += (s, e) =>
            {
                MessageBox.Show("Menu chức năng đang phát triển!", "Menu",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnIconLightning.Click += (s, e) =>
            {
                // Đăng tất cả bài Scheduled → Published
                var ds = _danhSach.LayTatCa();
                int count = 0;
                foreach (var p in ds)
                {
                    if (p.Status == PostStatus.Scheduled)
                    {
                        _danhSach.DangBai(p.Id);
                        count++;
                    }
                }
                ApDungLoc();
                MessageBox.Show("Đã đăng " + count + " bài Scheduled!", "Publish All",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnIconSettings.Click += (s, e) =>
            {
                MessageBox.Show(
                    "⚙  CÀI ĐẶT\n\n" +
                    "Phiên bản: 1.0.0\n" +
                    "Ngôn ngữ: Tiếng Việt\n" +
                    "Nền tảng: .NET WinForms\n" +
                    "Cấu trúc dữ liệu: Doubly LinkedList",
                    "Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            btnBenchmark.Click += (s, e) =>
            {
                var frm = new FrmSortBenchmark(_danhSach);
                frm.ShowDialog(this);
                ApDungLoc();  // Cập nhật lại feed sau khi sắp xếp
            };
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
            _danhSach.ThemCuoi("Hôm nay trời đẹp quá! ☀️", "Buổi sáng thức dậy nhìn ra cửa sổ, bầu trời xanh trong và nắng vàng rực rỡ. Một ngày tuyệt vời để ra ngoài dạo chơi!", Platform.Facebook, new DateTime(2023, 12, 12, 0, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Ảnh du lịch Đà Lạt 🌿", "Cuối tuần vừa rồi mình có dịp ghé thăm thành phố ngàn hoa. Khí hậu mát mẻ, cảnh đẹp như tranh vẽ!", Platform.Instagram, new DateTime(2023, 12, 12, 3, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi("Chia sẻ kinh nghiệm học lập trình C#", "Sau 3 tháng học WinForms và cấu trúc dữ liệu, mình đúc kết được nhiều bài học quý giá. Hãy cùng tham khảo nhé!", Platform.LinkedIn, new DateTime(2023, 12, 12, 13, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Chúc mừng sinh nhật bạn thân! 🎂🎉", "Chúc bạn luôn vui vẻ, mạnh khỏe và thành công trên con đường phía trước. Tình bạn mãi bền vững!", Platform.Facebook, new DateTime(2022, 12, 18, 20, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Review quán cà phê mới mở 🍵", "Vừa khám phá một quán cà phê siêu xinh ở quận 1. Không gian ấm cúng, đồ uống ngon, giá cả hợp lý!", Platform.Instagram, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Tuyển dụng thực tập sinh IT 💻", "Công ty chúng tôi đang tìm kiếm thực tập sinh lập trình .NET, có kiến thức về WinForms hoặc ASP.NET.", Platform.LinkedIn, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Video nấu phở bò truyền thống 🍜", "Hướng dẫn chi tiết cách nấu phở bò chuẩn vị Hà Nội ngay tại nhà. Công thức gia truyền không thể bỏ qua!", Platform.Facebook, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Kỷ niệm 2 năm thành lập nhóm 🥳", "Hai năm đồng hành cùng nhau, chúng ta đã vượt qua bao thử thách. Cảm ơn tất cả mọi người đã luôn ủng hộ!", Platform.Instagram, new DateTime(2022, 12, 12, 19, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Sự kiện workshop lập trình miễn phí", "Tham gia workshop miễn phí về LinkedList và cấu trúc dữ liệu vào cuối tuần này. Đăng ký ngay hôm nay!", Platform.Instagram, new DateTime(2022, 12, 12, 19, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi("Mẹo tăng năng suất làm việc 📈", "5 thói quen giúp mình tăng gấp đôi năng suất mỗi ngày: dậy sớm, lập kế hoạch, tập trung, nghỉ ngơi đúng cách.", Platform.LinkedIn, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Flash sale cuối năm giảm đến 50% 🛍️", "Đừng bỏ lỡ đợt sale lớn nhất năm! Hàng nghìn sản phẩm giảm giá sâu, số lượng có hạn.", Platform.Facebook, new DateTime(2022, 12, 12, 16, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Bộ ảnh check-in Hội An cổ kính 🏮", "Phố cổ Hội An về đêm đẹp mê hồn với những chiếc đèn lồng rực rỡ. Nhất định phải ghé một lần!", Platform.Instagram, new DateTime(2022, 12, 12, 9, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Chào buổi sáng! ☕", "Một tách cà phê thơm ngon để bắt đầu ngày mới đầy năng lượng. Chúc mọi người có ngày làm việc thật hiệu quả!", Platform.Facebook, new DateTime(2023, 1, 5, 7, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Khám phá Phú Quốc 🌊", "Biển xanh, cát trắng và những buổi hoàng hôn tuyệt đẹp tại đảo ngọc Phú Quốc. Chuyến đi đáng nhớ nhất năm!", Platform.Instagram, new DateTime(2023, 1, 10, 10, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Kinh nghiệm học tiếng Anh online 📚", "Sau 6 tháng luyện tập mỗi ngày, mình đã đạt IELTS 7.0. Chia sẻ lộ trình và tài liệu miễn phí cho mọi người!", Platform.LinkedIn, new DateTime(2023, 1, 15, 9, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Tết này về quê ăn bánh chưng 🎑", "Không khí Tết cổ truyền thật ấm áp bên gia đình. Cùng nhau gói bánh chưng, đón giao thừa thật ý nghĩa!", Platform.Facebook, new DateTime(2023, 1, 20, 18, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi("Review sách 'Đắc Nhân Tâm' 📖", "Cuốn sách thay đổi cách mình nhìn nhận các mối quan hệ xã hội. Ai chưa đọc thì nên bắt đầu ngay hôm nay!", Platform.Facebook, new DateTime(2023, 2, 1, 20, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Ảnh street food Sài Gòn 🥢", "Hành trình khám phá ẩm thực đường phố Sài Gòn từ bún bò, bánh mì đến hủ tiếu. Thiên đường ăn uống!", Platform.Instagram, new DateTime(2023, 2, 5, 12, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Tuyển dụng lập trình viên .NET Senior", "Chúng tôi đang tìm kiếm Senior Developer có kinh nghiệm ASP.NET Core, SQL Server và microservices. Apply ngay!", Platform.LinkedIn, new DateTime(2023, 2, 10, 8, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Mẹo chụp ảnh đẹp bằng điện thoại 📸", "Không cần máy ảnh đắt tiền, chỉ cần nắm vững ánh sáng, góc chụp và bố cục là đã có ảnh đẹp rồi!", Platform.Instagram, new DateTime(2023, 2, 14, 17, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Workshop thiết kế UI/UX miễn phí 🎨", "Đăng ký tham gia buổi workshop hướng dẫn thiết kế giao diện người dùng với Figma vào thứ 7 tuần này!", Platform.Facebook, new DateTime(2023, 3, 1, 9, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi("Kỷ niệm ngày Quốc tế Phụ nữ 💐", "Gửi lời chúc yêu thương đến tất cả những người phụ nữ tuyệt vời! Chúc các chị em luôn rạng rỡ và hạnh phúc.", Platform.Facebook, new DateTime(2023, 3, 8, 6, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Hành trình chạy bộ buổi sáng 🏃", "Mỗi sáng 5km quanh công viên giúp mình tỉnh táo và tràn đầy năng lượng cả ngày. Hãy thử ngay hôm nay!", Platform.Instagram, new DateTime(2023, 3, 15, 6, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi("Xu hướng công nghệ AI năm 2023 🤖", "Trí tuệ nhân tạo đang thay đổi mọi ngành nghề. Cùng tìm hiểu những ứng dụng AI nổi bật nhất hiện nay!", Platform.LinkedIn, new DateTime(2023, 3, 20, 14, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi("Buổi tối chill cùng nhạc lo-fi 🎵", "Không có gì tuyệt hơn là ngồi nghe nhạc lo-fi, uống trà và đọc sách vào buổi tối. Cuộc sống thật bình yên!", Platform.Facebook, new DateTime(2023, 4, 2, 21, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Check-in cánh đồng hoa tam giác mạch 🌸", "Mùa hoa tam giác mạch Hà Giang năm nay đẹp hơn bao giờ hết. Nhất định phải đến một lần trong đời!", Platform.Instagram, new DateTime(2023, 4, 5, 8, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "5 kỹ năng mềm cần có trong năm 2023 💼", "Giao tiếp, tư duy phản biện, quản lý thời gian, làm việc nhóm và thích nghi với thay đổi – bạn đã có đủ chưa?", Platform.LinkedIn, new DateTime(2023, 4, 10, 9, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Sinh nhật mẹ yêu 🎂❤️", "Chúc mẹ luôn mạnh khỏe, vui vẻ và hạnh phúc. Cảm ơn mẹ đã luôn là chỗ dựa vững chắc nhất của con!", Platform.Facebook, new DateTime(2023, 4, 18, 7, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Công thức làm bánh flan tại nhà 🍮", "Chỉ cần trứng, sữa và đường là bạn đã có ngay món bánh flan mịn màng, thơm ngon không kém ngoài tiệm!", Platform.Facebook, new DateTime(2023, 4, 22, 15, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi( "Outfit mùa hè siêu mát 👗☀️", "Tổng hợp các set đồ nhẹ nhàng, thoáng mát cho mùa hè năm nay. Vừa cool vừa phong cách, phù hợp mọi dịp!", Platform.Instagram, new DateTime(2023, 5, 1, 10, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi( "Hội thảo khởi nghiệp công nghệ 2023 🚀", "Tham gia sự kiện kết nối startup và nhà đầu tư lớn nhất miền Nam. Cơ hội vàng để networking và học hỏi!", Platform.LinkedIn, new DateTime(2023, 5, 8, 8, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi( "Vlog một ngày ở Đà Nẵng 🌅", "Từ bãi biển Mỹ Khê buổi sáng đến cầu Rồng về đêm – Đà Nẵng luôn có cách làm mình yêu thêm mỗi lần ghé thăm!", Platform.Instagram, new DateTime(2023, 5, 15, 19, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Thử thách 30 ngày không dùng mạng xã hội 📵", "Sau 30 ngày detox digital, mình ngủ ngon hơn, tập trung hơn và có nhiều thời gian cho những điều thực sự quan trọng.", Platform.Facebook, new DateTime(2023, 5, 20, 20, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Top 10 công cụ hỗ trợ lập trình viên 🛠️", "VS Code, Git, Postman, Docker, Figma... Đây là bộ công cụ không thể thiếu giúp mình làm việc hiệu quả mỗi ngày.", Platform.LinkedIn, new DateTime(2023, 6, 1, 11, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi( "Mùa mưa Sài Gòn 🌧️", "Cơn mưa chiều Sài Gòn ập đến thật nhanh nhưng cũng tan thật chóng. Yêu cái nhịp sống bất ngờ của thành phố này!", Platform.Facebook, new DateTime(2023, 6, 10, 17, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Bộ sưu tập ảnh hoàng hôn 🌇", "365 ngày – 365 hoàng hôn khác nhau. Mỗi buổi chiều tà đều mang một vẻ đẹp riêng không thể lẫn vào đâu được.", Platform.Instagram, new DateTime(2023, 6, 15, 18, 30, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Chia sẻ giáo trình học SQL miễn phí 🗄️", "Tổng hợp tài liệu học SQL từ cơ bản đến nâng cao, kèm bài tập thực hành. Phù hợp cho người mới bắt đầu!", Platform.LinkedIn, new DateTime(2023, 6, 20, 10, 0, 0), PostStatus.Draft);
            _danhSach.ThemCuoi( "Cùng nhau trồng cây xanh 🌱", "Hôm nay mình trồng thêm một chậu cây nhỏ trên ban công. Mỗi người một hành động nhỏ góp phần bảo vệ môi trường!", Platform.Facebook, new DateTime(2023, 7, 5, 8, 0, 0), PostStatus.Published);
            _danhSach.ThemCuoi( "Góc học tập aesthetic 📐✏️", "Setup góc học tập gọn gàng, thoáng mát giúp mình tập trung hơn hẳn. Chia sẻ vài tips decor bàn học siêu dễ!", Platform.Instagram, new DateTime(2023, 7, 12, 14, 0, 0), PostStatus.Scheduled);
            _danhSach.ThemCuoi( "Webinar miễn phí: Nhập môn Data Science 📊", "Đăng ký tham gia buổi học trực tuyến giới thiệu về Python, pandas và machine learning cơ bản. Số lượng có hạn!", Platform.LinkedIn, new DateTime(2023, 7, 20, 9, 0, 0), PostStatus.Scheduled);
        }
    }
}