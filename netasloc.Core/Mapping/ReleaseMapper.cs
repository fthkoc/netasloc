using netasloc.Core.DTO;
using netasloc.Data.Entity;

namespace netasloc.Core.Mapping
{
    public class ReleaseMapper : _IMapper<Release, ReleaseDTO>
    {
        public ReleaseDTO DataToCore(Release item)
        {
            return new ReleaseDTO()
            {
                ID = item.id,
                CreatedAt = item.created_at,
                UpdatedAt = item.updated_at,
                ReleaseCode = item.release_code,
                TotalLineCount = item.total_line_count,
                CodeLineCount = item.code_line_count,
                CommentLineCount = item.comment_line_count,
                EmptyLineCount = item.empty_line_count,
                DifferenceSLOC = item.difference_sloc,
                DifferenceLOC = item.difference_loc
            };
        }

        public Release CoreToData(ReleaseDTO item)
        {
            return new Release()
            {
                id = item.ID,
                created_at = item.CreatedAt,
                updated_at = item.UpdatedAt,
                release_code = item.ReleaseCode,
                total_line_count = item.TotalLineCount,
                code_line_count = item.CodeLineCount,
                comment_line_count = item.CommentLineCount,
                empty_line_count = item.EmptyLineCount,
                difference_sloc = item.DifferenceSLOC,
                difference_loc = item.DifferenceLOC
            };
        }
    }
}
