
namespace netasloc.Core.Models
{
    public class Language
    {
        public string[] Aliases { get; set; }
        public string[] FileExtensions { get; set; }
        public string[] LineCommentCharacters { get; set; }
        public string[][] BlockCommentCharacters { get; set; }
    }
}
