using System;
using System.Collections.Generic;

namespace SocialManagerPro
{
    /// <summary>
    /// Doubly LinkedList quản lý danh sách bài đăng mạng xã hội.
    ///   Head → bài đăng đầu tiên
    ///   Tail → bài đăng cuối cùng
    /// </summary>
    public class PostLinkedList
    {
        // ── Thuộc tính ─────────────────────────────────────────────────────────
        public Post Head { get; private set; }
        public Post Tail { get; private set; }
        public int Count { get; private set; }

        private int _nextId = 1;

        // ══════════════════════════════════════════════════════════════════════
        //  THÊM
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Thêm bài đăng vào cuối danh sách — O(1).</summary>
        public Post ThemCuoi(string title, string content,
                             Platform platform, DateTime scheduledTime, PostStatus status)
        {
            var post = new Post(_nextId++, title, content, platform, scheduledTime, status);

            if (Head == null)
                Head = Tail = post;
            else
            {
                post.Prev = Tail;
                Tail.Next = post;
                Tail = post;
            }

            Count++;
            return post;
        }

        /// <summary>Nhân đôi bài đăng (Duplicate) — chèn ngay sau node gốc.</summary>
        public Post NhanDoi(int id)
        {
            Post goc = TimTheoId(id);
            if (goc == null) return null;

            var ban = new Post(_nextId++,
                               goc.Title + " (copy)",
                               goc.Content,
                               goc.Platform,
                               goc.ScheduledTime,
                               PostStatus.Draft);

            // Chèn sau node gốc
            ban.Prev = goc;
            ban.Next = goc.Next;

            if (goc.Next != null) goc.Next.Prev = ban;
            else Tail = ban;

            goc.Next = ban;
            Count++;
            return ban;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  XOÁ
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Xoá bài đăng theo ID — O(n). Trả về true nếu thành công.</summary>
        public bool Xoa(int id)
        {
            Post cur = TimTheoId(id);
            if (cur == null) return false;

            if (cur.Prev != null) cur.Prev.Next = cur.Next;
            else Head = cur.Next;

            if (cur.Next != null) cur.Next.Prev = cur.Prev;
            else Tail = cur.Prev;

            cur.Next = cur.Prev = null;
            Count--;
            return true;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SỬA
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Cập nhật bài đăng theo ID — O(n).</summary>
        public bool Sua(int id, string title, string content,
                        Platform platform, DateTime scheduledTime, PostStatus status)
        {
            Post post = TimTheoId(id);
            if (post == null) return false;

            post.Title = title;
            post.Content = content;
            post.Platform = platform;
            post.ScheduledTime = scheduledTime;
            post.Status = status;
            return true;
        }

        /// <summary>Đặt trạng thái Published cho bài đăng — O(n).</summary>
        public bool DangBai(int id)
        {
            Post post = TimTheoId(id);
            if (post == null) return false;
            post.Status = PostStatus.Published;
            return true;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  TÌM KIẾM
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Tìm bài đăng theo ID — O(n).</summary>
        public Post TimTheoId(int id)
        {
            Post cur = Head;
            while (cur != null)
            {
                if (cur.Id == id) return cur;
                cur = cur.Next;
            }
            return null;
        }

        /// <summary>Lọc bài đăng theo từ khoá, trạng thái, nền tảng — O(n).</summary>
        public List<Post> Loc(string tuKhoa, PostStatus? trangThai, Platform? nenTang)
        {
            var kq = new List<Post>();
            Post cur = Head;

            while (cur != null)
            {
                bool khop = true;

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                    khop &= cur.Title.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                         || cur.Content.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase);

                if (trangThai.HasValue) khop &= cur.Status == trangThai.Value;
                if (nenTang.HasValue) khop &= cur.Platform == nenTang.Value;

                if (khop) kq.Add(cur);
                cur = cur.Next;
            }

            return kq;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DUYỆT
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Trả về toàn bộ danh sách Head → Tail.</summary>
        public List<Post> LayTatCa()
        {
            var ds = new List<Post>();
            Post cur = Head;
            while (cur != null) { ds.Add(cur); cur = cur.Next; }
            return ds;
        }

        /// <summary>Sinh ID tiếp theo (dùng để hiển thị preview).</summary>
        public int SinhIdMoi() => _nextId;

        /// <summary>
        /// Dựng lại LinkedList từ mảng đã sắp xếp.
        /// Dùng bởi SortHelper sau mỗi lần sắp xếp.
        /// </summary>
        public void RebuildFromArray(Post[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Head = Tail = null;
                Count = 0;
                return;
            }

            // Gán lại con trỏ Next / Prev cho từng node
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Prev = (i > 0) ? arr[i - 1] : null;
                arr[i].Next = (i < arr.Length - 1) ? arr[i + 1] : null;
            }

            Head = arr[0];
            Tail = arr[arr.Length - 1];
            Count = arr.Length;
        }
    }
}