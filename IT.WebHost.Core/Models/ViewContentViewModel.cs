using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Content;

namespace IT.WebHost.Core.Models
{
    public class ViewContentViewModel
    {
        public string ContentId { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public ContentPublicRecord? Record { get; set; }
        public IEnumerable<CommentResponseRecord>? Comments { get; set; } = new List<CommentResponseRecord>();

    }
}
