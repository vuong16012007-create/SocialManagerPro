using System;
using System.Drawing;
using System.Windows.Forms;

namespace SocialManagerPro
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        // ── Sidebar ────────────────────────────────────────────────────────────
        private Panel pnlSidebar;
        private Label lblAppName;
        private Button btnDashboard, btnCreatePost, btnNavScheduled, btnList, btnAccounts, btnSettingsSide;

        // ── Main area ──────────────────────────────────────────────────────────
        private Panel pnlMain;

        // TopBar
        private Panel pnlTopBar;
        private TextBox txtSearch;
        private Label lblSearchIcon;

        // Filter bar
        private Panel pnlFilters;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblStatus;
        private Panel pnlStatusFilter;
        private Button btnAll, btnDraft, btnScheduled, btnPublished, btnFailed;
        private Label lblPlatformLbl;
        private ComboBox cboPlatform;

        // Post list
        private Label lblPostCount;
        private DataGridView dgvPosts;
        private DataGridViewCheckBoxColumn colCheck;
        private DataGridViewTextBoxColumn colPlatform, colTitle, colContent, colTime, colStatus, colActions;

        // ── Right panel ────────────────────────────────────────────────────────
        private Panel pnlRight;
        private Label lblDetailsTitle;
        private Label lblPostDetails;

        // Preview
        private Panel pnlPreview;
        private Label lblPreviewPlatform;
        private Label lblPreviewContent;
        private Panel pnlThumbs;

        // Form fields
        private Label lblTitleField;
        private TextBox txtTitle;
        private Label lblContentField;
        private TextBox txtContent;
        private Label lblPlatformSel;
        private Panel pnlPlatformChecks;
        private CheckBox chkFacebook, chkInstagram, chkLinkedIn, chkTikTok;
        private Label lblScheduledLbl;
        private DateTimePicker dtpScheduled;
        private Label lblStatusField;
        private ComboBox cboStatus;
        private Button btnBenchmark;

        // Action buttons
        private Button btnSaveDraft, btnDelete, btnSchedulePost;

        // Icon buttons top bar
        private Button btnIconMenu, btnIconLightning, btnIconSettings;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── Form ──────────────────────────────────────────────────────────
            Text = "Social Manager Pro";
            ClientSize = new Size(1280, 760);
            MinimumSize = new Size(1100, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 9.5f);

            // ══════════════════════════════════════════════════════════════════
            //  SIDEBAR (left, 195px, dark)
            // ══════════════════════════════════════════════════════════════════
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 195,
                BackColor = Color.FromArgb(30, 35, 48),
                Padding = new Padding(0, 0, 0, 0)
            };

            lblAppName = new Label
            {
                Text = "Social Manager Pro",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Bounds = new Rectangle(10, 18, 175, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var sepLine = new Panel
            {
                Bounds = new Rectangle(0, 56, 195, 1),
                BackColor = Color.FromArgb(55, 65, 85)
            };

            btnDashboard = MakeSidebarBtn("  ⊞  Dashboard", 70);
            btnCreatePost = MakeSidebarBtn("  ✎  Create Post", 112);
            btnNavScheduled = MakeSidebarBtn("  ⏰  Scheduled", 154);
            btnList = MakeSidebarBtn("  ≡  List", 196);
            btnAccounts = MakeSidebarBtn("  ○  Accounts", 238);

            btnSettingsSide = new Button
            {
                Text = "  ⚙  Settings",
                ForeColor = Color.FromArgb(200, 200, 210),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5f),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Bounds = new Rectangle(0, 700, 195, 42),
                Cursor = Cursors.Hand
            };
            btnSettingsSide.FlatAppearance.BorderSize = 0;
            btnSettingsSide.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 55, 75);


            btnList.BackColor = Color.FromArgb(45, 55, 75);
            btnList.ForeColor = Color.White;

            pnlSidebar.Controls.AddRange(new Control[]
                { lblAppName, sepLine, btnDashboard, btnCreatePost, btnNavScheduled,
                  btnList, btnAccounts, btnSettingsSide });

            // ══════════════════════════════════════════════════════════════════
            //  RIGHT PANEL (Details & Preview, 320px)
            // ══════════════════════════════════════════════════════════════════
            pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 320,
                BackColor = Color.White,
                Padding = new Padding(16, 14, 16, 14),
                AutoScroll = true
            };

            lblDetailsTitle = new Label
            {
                Text = "Details & Preview",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 35, 48),
                Bounds = new Rectangle(16, 14, 280, 28),
                AutoSize = false
            };

            lblPostDetails = new Label
            {
                Text = "POST DETAILS",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 130, 150),
                Bounds = new Rectangle(16, 50, 280, 18)
            };

            pnlPreview = new Panel
            {
                Bounds = new Rectangle(16, 72, 284, 145),
                BackColor = Color.FromArgb(248, 249, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPreviewPlatform = new Label
            {
                Text = "Facebook",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(24, 119, 242),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                Bounds = new Rectangle(8, 8, 70, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblPreviewContent = new Label
            {
                Text = "Nội dung bài viết của bạn sẽ hiển thị tại đây. Bạn có thể kiểm tra định dạng, hình ảnh và cách trình bày trước khi quyết định đăng lên các nền tảng mạng xã hội...",
                ForeColor = Color.FromArgb(60, 70, 85),
                Font = new Font("Segoe UI", 8f),
                Bounds = new Rectangle(8, 36, 268, 60),
                AutoSize = false
            };

            pnlThumbs = new Panel { Bounds = new Rectangle(8, 100, 268, 36), BackColor = Color.Transparent };
            Color[] thumbColors = { Color.SteelBlue, Color.SeaGreen, Color.DimGray, Color.CadetBlue };
            for (int i = 0; i < 4; i++)
            {
                pnlThumbs.Controls.Add(new Panel
                {
                    Bounds = new Rectangle(i * 68, 0, 64, 34),
                    BackColor = thumbColors[i]
                });
            }

            pnlPreview.Controls.AddRange(new Control[] { lblPreviewPlatform, lblPreviewContent, pnlThumbs });

            int ry = 226;

            lblTitleField = RightLabel("Post Title", ry); ry += 20;
            txtTitle = new TextBox { Bounds = new Rectangle(16, ry, 284, 26), BorderStyle = BorderStyle.FixedSingle }; ry += 34;

            lblContentField = RightLabel("Content", ry); ry += 20;
            txtContent = new TextBox
            {
                Bounds = new Rectangle(16, ry, 284, 72),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical
            };
            ry += 80;

            lblPlatformSel = RightLabel("Platform Selection", ry); ry += 22;
            pnlPlatformChecks = new Panel { Bounds = new Rectangle(16, ry, 284, 86), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            chkFacebook = MakeCheckBox("Facebook", 4);
            chkInstagram = MakeCheckBox("Instagram", 26);
            chkLinkedIn = MakeCheckBox("LinkedIn", 48);
            chkTikTok = MakeCheckBox("TikTok", 70);
            pnlPlatformChecks.Controls.AddRange(new Control[] { chkFacebook, chkInstagram, chkLinkedIn, chkTikTok });
            ry += 94;

            lblScheduledLbl = RightLabel("Scheduled Date/Time", ry); ry += 20;
            dtpScheduled = new DateTimePicker
            {
                Bounds = new Rectangle(16, ry, 284, 26),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/dd/yyyy",
                Value = DateTime.Now.AddDays(1)
            };
            ry += 34;

            lblStatusField = RightLabel("Status", ry); ry += 20;
            cboStatus = new ComboBox
            {
                Bounds = new Rectangle(16, ry, 284, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "Draft", "Scheduled", "Published", "Failed" });
            cboStatus.SelectedIndex = 0;
            ry += 36;

            btnSaveDraft = new Button
            {
                Text = "SAVE DRAFT",
                Bounds = new Rectangle(16, ry, 132, 32),
                BackColor = Color.FromArgb(230, 232, 236),
                ForeColor = Color.FromArgb(40, 45, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveDraft.FlatAppearance.BorderColor = Color.FromArgb(200, 202, 210);

            btnDelete = new Button
            {
                Text = "DELETE",
                Bounds = new Rectangle(152, ry, 148, 32),
                BackColor = Color.FromArgb(230, 232, 236),
                ForeColor = Color.FromArgb(40, 45, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderColor = Color.FromArgb(200, 202, 210);
            ry += 40;

            btnSchedulePost = new Button
            {
                Text = "SCHEDULE POST",
                Bounds = new Rectangle(16, ry, 284, 36),
                BackColor = Color.FromArgb(45, 74, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSchedulePost.FlatAppearance.BorderSize = 0;

            pnlRight.Controls.AddRange(new Control[]
            {
                lblDetailsTitle, lblPostDetails, pnlPreview,
                lblTitleField, txtTitle,
                lblContentField, txtContent,
                lblPlatformSel, pnlPlatformChecks,
                lblScheduledLbl, dtpScheduled,
                lblStatusField, cboStatus,
                btnSaveDraft, btnDelete, btnSchedulePost
            });

            // ══════════════════════════════════════════════════════════════════
            //  MAIN PANEL (center)
            // ══════════════════════════════════════════════════════════════════
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 246, 248),
                Padding = new Padding(16, 10, 16, 10)
            };

            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.Transparent };

            var pnlSearchWrap = new Panel
            {
                Location = new Point(0, 10),
                Size = new Size(280, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Location = new Point(8, 6),
                Size = new Size(230, 20),
                Text = "Search",
                ForeColor = Color.Gray,
                BackColor = Color.White
            };
            txtSearch.GotFocus += (s, e) => { if (txtSearch.Text == "Search") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; } };
            txtSearch.LostFocus += (s, e) => { if (txtSearch.Text == "") { txtSearch.Text = "Search"; txtSearch.ForeColor = Color.Gray; } };
            lblSearchIcon = new Label { Text = "🔍", Location = new Point(245, 6), AutoSize = true };
            pnlSearchWrap.Controls.AddRange(new Control[] { txtSearch, lblSearchIcon });

            btnIconMenu = MakeIconBtn("≡", new Rectangle(300, 12, 32, 28));
            btnIconLightning = MakeIconBtn("⚡", new Rectangle(336, 12, 32, 28));
            btnIconSettings = MakeIconBtn("⚙", new Rectangle(372, 12, 32, 28));
            pnlTopBar.Controls.AddRange(new Control[] { pnlSearchWrap, btnIconMenu, btnIconLightning, btnIconSettings });

            pnlFilters = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 0) };

            lblDate = new Label { Text = "Date", AutoSize = true, Location = new Point(0, 8), ForeColor = Color.FromArgb(60, 70, 85), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            dtpDate = new DateTimePicker { Location = new Point(0, 28), Width = 155, Format = DateTimePickerFormat.Custom, CustomFormat = "MM/dd/yyyy" };

            lblStatus = new Label { Text = "Status", AutoSize = true, Location = new Point(170, 8), ForeColor = Color.FromArgb(60, 70, 85), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            pnlStatusFilter = new Panel { Location = new Point(170, 24), Size = new Size(385, 30), BackColor = Color.Transparent };
            btnAll = MakeFilterBtn("All", 0, true);
            btnDraft = MakeFilterBtn("Draft", 60, false);
            btnScheduled = MakeFilterBtn("Scheduled", 110, false);
            btnPublished = MakeFilterBtn("Published", 193, false);
            btnFailed = MakeFilterBtn("Failed", 276, false);
            pnlStatusFilter.Controls.AddRange(new Control[] { btnAll, btnDraft, btnScheduled, btnPublished, btnFailed });

            lblPlatformLbl = new Label { Text = "Platform", AutoSize = true, Location = new Point(565, 8), ForeColor = Color.FromArgb(60, 70, 85), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            cboPlatform = new ComboBox { Location = new Point(565, 24), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPlatform.Items.AddRange(new object[] { "All Platforms", "Facebook", "Instagram", "LinkedIn", "TikTok" });
            cboPlatform.SelectedIndex = 0;

            btnBenchmark = new Button
            {
                Text = "⏱ So sánh Sort",
                Bounds = new Rectangle(570, 24, 130, 28),
                BackColor = Color.FromArgb(255, 149, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBenchmark.FlatAppearance.BorderSize = 0;

            pnlFilters.Controls.AddRange(new Control[]
                { lblDate, dtpDate, lblStatus, pnlStatusFilter, lblPlatformLbl, cboPlatform, btnBenchmark });

            lblPostCount = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Your Social Media Posts",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 35, 48),
                BackColor = Color.Transparent
            };

            // ── DataGridView ──────────────────────────────────────────────────
            dgvPosts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(230, 232, 236),
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 48 },
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            dgvPosts.DefaultCellStyle.BackColor = Color.White;
            dgvPosts.DefaultCellStyle.ForeColor = Color.FromArgb(40, 50, 65);
            dgvPosts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 240, 255);
            dgvPosts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 35, 48);
            dgvPosts.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgvPosts.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvPosts.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(90, 100, 115);
            dgvPosts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvPosts.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;

            colCheck = new DataGridViewCheckBoxColumn { Name = "colCheck", HeaderText = "Select", Width = 55 };
            colPlatform = new DataGridViewTextBoxColumn { Name = "colPlatform", HeaderText = "Platform", Width = 80, ReadOnly = true };
            colTitle = new DataGridViewTextBoxColumn { Name = "colTitle", HeaderText = "Title", Width = 180, ReadOnly = true };
            colContent = new DataGridViewTextBoxColumn { Name = "colContent", HeaderText = "Content Snippet", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true };
            colTime = new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Scheduled Time", Width = 140, ReadOnly = true };
            colStatus = new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", Width = 110, ReadOnly = true };
            colActions = new DataGridViewTextBoxColumn { Name = "colActions", HeaderText = "", Width = 45, ReadOnly = true };
            colActions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colActions.DefaultCellStyle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);

            dgvPosts.Columns.AddRange(new DataGridViewColumn[]
                { colCheck, colPlatform, colTitle, colContent, colTime, colStatus, colActions });

            pnlMain.Controls.Add(dgvPosts);
            pnlMain.Controls.Add(lblPostCount);
            pnlMain.Controls.Add(pnlFilters);
            pnlMain.Controls.Add(pnlTopBar);

            Controls.Add(pnlMain);
            Controls.Add(pnlRight);
            Controls.Add(pnlSidebar);
        }

        private Button MakeSidebarBtn(string text, int top)
        {
            var btn = new Button
            {
                Text = text,
                ForeColor = Color.FromArgb(180, 185, 200),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5f),
                Bounds = new Rectangle(0, top, 195, 40),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(42, 50, 68);
            return btn;
        }

        private Button MakeIconBtn(string text, Rectangle bounds)
        {
            var btn = new Button
            {
                Text = text,
                Bounds = bounds,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(80, 90, 110),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(210, 212, 220);
            return btn;
        }

        private Button MakeFilterBtn(string text, int x, bool active)
        {
            int w = text == "All" ? 55 : (text.Length * 8 + 16);
            var btn = new Button
            {
                Text = text,
                Bounds = new Rectangle(x, 0, w, 28),
                BackColor = active ? Color.FromArgb(45, 74, 140) : Color.FromArgb(240, 241, 244),
                ForeColor = active ? Color.White : Color.FromArgb(80, 90, 110),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(210, 212, 220);
            return btn;
        }

        private Label RightLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Bounds = new Rectangle(16, y, 284, 18),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 90)
            };
        }

        private CheckBox MakeCheckBox(string text, int top)
        {
            return new CheckBox
            {
                Text = text,
                Bounds = new Rectangle(8, top, 140, 20),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(40, 50, 65)
            };
        }
    }
}
