using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SocialManagerPro
{
    public static class SortHelper
    {
        // Số lần lặp để đo trung bình
        private const int SO_LAN_LAP = 100000;

        // ══════════════════════════════════════════════════════════════════
        //  TIỆN ÍCH
        // ══════════════════════════════════════════════════════════════════

        private static Post[] ToArray(PostLinkedList list)
        {
            var arr = new Post[list.Count];
            Post cur = list.Head;
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = cur;
                cur = cur.Next;
            }
            return arr;
        }

        /// <summary>Copy dữ liệu (Id, Title, ScheduledTime...) sang mảng mới
        /// để không ảnh hưởng đến các node gốc trong LinkedList.</summary>
        private static Post[] CloneData(Post[] src)
        {
            var dst = new Post[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                dst[i] = new Post(
                    src[i].Id,
                    src[i].Title,
                    src[i].Content,
                    src[i].Platform,
                    src[i].ScheduledTime,
                    src[i].Status);
            }
            return dst;
        }

        // ══════════════════════════════════════════════════════════════════
        //  1. BUBBLE SORT — O(n²)
        // ══════════════════════════════════════════════════════════════════
        public static double BubbleSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < arr.Length - 1; i++)
                    for (int j = 0; j < arr.Length - 1 - i; j++)
                        if (arr[j].ScheduledTime > arr[j + 1].ScheduledTime)
                        {
                            Post tmp = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = tmp;
                        }

                sw.Stop();
                total += sw.ElapsedTicks;
            }

            // Áp kết quả sắp xếp cuối vào LinkedList
            Post[] sorted = CloneData(original);
            ApplyBubble(sorted);
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void ApplyBubble(Post[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
                for (int j = 0; j < arr.Length - 1 - i; j++)
                    if (arr[j].ScheduledTime > arr[j + 1].ScheduledTime)
                    { Post t = arr[j]; arr[j] = arr[j + 1]; arr[j + 1] = t; }
        }

        // ══════════════════════════════════════════════════════════════════
        //  2. SELECTION SORT — O(n²)
        // ══════════════════════════════════════════════════════════════════
        public static double SelectionSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();

                for (int i = 0; i < arr.Length - 1; i++)
                {
                    int minIdx = i;
                    for (int j = i + 1; j < arr.Length; j++)
                        if (arr[j].ScheduledTime < arr[minIdx].ScheduledTime)
                            minIdx = j;
                    if (minIdx != i)
                    { Post t = arr[i]; arr[i] = arr[minIdx]; arr[minIdx] = t; }
                }

                sw.Stop();
                total += sw.ElapsedTicks;
            }

            Post[] sorted = CloneData(original);
            ApplySelection(sorted);
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void ApplySelection(Post[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int m = i;
                for (int j = i + 1; j < arr.Length; j++)
                    if (arr[j].ScheduledTime < arr[m].ScheduledTime) m = j;
                if (m != i) { Post t = arr[i]; arr[i] = arr[m]; arr[m] = t; }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  3. INSERTION SORT — O(n²)
        // ══════════════════════════════════════════════════════════════════
        public static double InsertionSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();

                for (int i = 1; i < arr.Length; i++)
                {
                    Post key = arr[i];
                    int j = i - 1;
                    while (j >= 0 && arr[j].ScheduledTime > key.ScheduledTime)
                    { arr[j + 1] = arr[j]; j--; }
                    arr[j + 1] = key;
                }

                sw.Stop();
                total += sw.ElapsedTicks;
            }

            Post[] sorted = CloneData(original);
            ApplyInsertion(sorted);
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void ApplyInsertion(Post[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                Post key = arr[i]; int j = i - 1;
                while (j >= 0 && arr[j].ScheduledTime > key.ScheduledTime)
                { arr[j + 1] = arr[j]; j--; }
                arr[j + 1] = key;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  4. QUICK SORT — O(n log n) trung bình
        // ══════════════════════════════════════════════════════════════════
        public static double QuickSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();
                QuickSortRec(arr, 0, arr.Length - 1);
                sw.Stop();
                total += sw.ElapsedTicks;
            }

            Post[] sorted = CloneData(original);
            QuickSortRec(sorted, 0, sorted.Length - 1);
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void QuickSortRec(Post[] arr, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(arr, low, high);
                QuickSortRec(arr, low, pi - 1);
                QuickSortRec(arr, pi + 1, high);
            }
        }

        private static int Partition(Post[] arr, int low, int high)
        {
            DateTime pivot = arr[high].ScheduledTime;
            int i = low - 1;
            for (int j = low; j < high; j++)
                if (arr[j].ScheduledTime <= pivot)
                { i++; Post t = arr[i]; arr[i] = arr[j]; arr[j] = t; }
            Post tmp = arr[i + 1]; arr[i + 1] = arr[high]; arr[high] = tmp;
            return i + 1;
        }

        // ══════════════════════════════════════════════════════════════════
        //  5. MERGE SORT — O(n log n)
        // ══════════════════════════════════════════════════════════════════
        public static double MergeSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();
                MergeSortRec(arr, 0, arr.Length - 1);
                sw.Stop();
                total += sw.ElapsedTicks;
            }

            Post[] sorted = CloneData(original);
            MergeSortRec(sorted, 0, sorted.Length - 1);
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void MergeSortRec(Post[] arr, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSortRec(arr, left, mid);
                MergeSortRec(arr, mid + 1, right);
                Merge(arr, left, mid, right);
            }
        }

        private static void Merge(Post[] arr, int left, int mid, int right)
        {
            int n1 = mid - left + 1, n2 = right - mid;
            Post[] L = new Post[n1], R = new Post[n2];
            Array.Copy(arr, left, L, 0, n1);
            Array.Copy(arr, mid + 1, R, 0, n2);
            int i = 0, j = 0, k = left;
            while (i < n1 && j < n2)
                arr[k++] = L[i].ScheduledTime <= R[j].ScheduledTime ? L[i++] : R[j++];
            while (i < n1) arr[k++] = L[i++];
            while (j < n2) arr[k++] = R[j++];
        }

        // ══════════════════════════════════════════════════════════════════
        //  6. HEAP SORT — O(n log n)
        // ══════════════════════════════════════════════════════════════════
        public static double HeapSort(PostLinkedList list)
        {
            Post[] original = ToArray(list);
            long total = 0;

            for (int lap = 0; lap < SO_LAN_LAP; lap++)
            {
                Post[] arr = CloneData(original);
                var sw = Stopwatch.StartNew();

                int n = arr.Length;
                for (int i = n / 2 - 1; i >= 0; i--) Heapify(arr, n, i);
                for (int i = n - 1; i > 0; i--)
                { Post t = arr[0]; arr[0] = arr[i]; arr[i] = t; Heapify(arr, i, 0); }

                sw.Stop();
                total += sw.ElapsedTicks;
            }

            Post[] sorted = CloneData(original);
            int sn = sorted.Length;
            for (int i = sn / 2 - 1; i >= 0; i--) Heapify(sorted, sn, i);
            for (int i = sn - 1; i > 0; i--)
            { Post t = sorted[0]; sorted[0] = sorted[i]; sorted[i] = t; Heapify(sorted, i, 0); }
            list.RebuildFromArray(sorted);

            return (double)total / SO_LAN_LAP / Stopwatch.Frequency * 1000.0;
        }

        private static void Heapify(Post[] arr, int n, int i)
        {
            int largest = i, l = 2 * i + 1, r = 2 * i + 2;
            if (l < n && arr[l].ScheduledTime > arr[largest].ScheduledTime) largest = l;
            if (r < n && arr[r].ScheduledTime > arr[largest].ScheduledTime) largest = r;
            if (largest != i)
            { Post t = arr[i]; arr[i] = arr[largest]; arr[largest] = t; Heapify(arr, n, largest); }
        }

        // ══════════════════════════════════════════════════════════════════
        //  BENCHMARK TẤT CẢ 6 — chạy 100.000 lần mỗi thuật toán
        // ══════════════════════════════════════════════════════════════════
        public static List<SortResult> BenchmarkAll(PostLinkedList list)
        {
            var results = new List<SortResult>();

            results.Add(new SortResult { TenThuatToan = "Bubble Sort", ThoiGian_ms = BubbleSort(list) });
            results.Add(new SortResult { TenThuatToan = "Selection Sort", ThoiGian_ms = SelectionSort(list) });
            results.Add(new SortResult { TenThuatToan = "Insertion Sort", ThoiGian_ms = InsertionSort(list) });
            results.Add(new SortResult { TenThuatToan = "Quick Sort", ThoiGian_ms = QuickSort(list) });
            results.Add(new SortResult { TenThuatToan = "Merge Sort", ThoiGian_ms = MergeSort(list) });
            results.Add(new SortResult { TenThuatToan = "Heap Sort", ThoiGian_ms = HeapSort(list) });

            return results;
        }
    }

    /// <summary>Kết quả đo thời gian — dùng double để hiện số thập phân.</summary>
    public class SortResult
    {
        public string TenThuatToan { get; set; }
        public double ThoiGian_ms { get; set; }   // đổi từ long → double
    }
}