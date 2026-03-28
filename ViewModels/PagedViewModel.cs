using TechStore.Models;

namespace TechStore.ViewModels
{
    public class PagedViewModel<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public string? Search { get; set; }
        public string? CategoryFilter { get; set; }

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
        public int PreviousPage => Page - 1;
        public int NextPage => Page + 1;
    }
}

