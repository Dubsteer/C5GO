using LogicLayer.Models;

namespace Website.Models
{
    public sealed class PostCommentsViewModel
    {
        public int PostId { get; init; }
        public IReadOnlyList<Comment> Comments { get; init; } = [];
        public User? CurrentUser { get; init; }
        public bool IsAuthenticated { get; init; }
    }
}
