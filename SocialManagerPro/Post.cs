using System;

namespace SocialManagerPro
{
    // ── Trạng thái bài đăng ────────────────────────────────────────────────────
    public enum PostStatus { Draft, Scheduled, Published, Failed }

    // ── Nền tảng mạng xã hội ──────────────────────────────────────────────────
    public enum Platform { Facebook, Instagram, LinkedIn, TikTok }

    /// <summary>
    /// Node của Doubly LinkedList — đại diện một bài đăng mạng xã hội.
    /// </summary>
    public class Post
    {
        // ── Dữ liệu bài đăng ──────────────────────────────────────────────────
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Platform Platform { get; set; }
        public DateTime ScheduledTime { get; set; }
        public PostStatus Status { get; set; }
        public bool IsSelected { get; set; }

        // ── Con trỏ LinkedList ─────────────────────────────────────────────────
        public Post Next { get; set; }
        public Post Prev { get; set; }

        public Post(int id, string title, string content,
                    Platform platform, DateTime scheduledTime, PostStatus status)
        {
            Id = id;
            Title = title;
            Content = content;
            Platform = platform;
            ScheduledTime = scheduledTime;
            Status = status;
            IsSelected = false;
            Next = null;
            Prev = null;
        }
    }
}