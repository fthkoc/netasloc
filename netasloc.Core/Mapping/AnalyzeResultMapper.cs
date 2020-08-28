using netasloc.Core.DTO;
using netasloc.Data.Entity;

namespace netasloc.Core.Mapping
{
    public class AnalyzeResultMapper : _IMapper<AnalyzeResult, AnalyzeResultDTO>
    {
        public AnalyzeResultDTO DataToCore(AnalyzeResult item)
        {
            return new AnalyzeResultDTO()
            {
                ID = item.id,
                CreatedAt = item.created_at,
                UpdatedAt = item.updated_at,
                DirectoryCount = item.directory_count,
                DirectoryIDList = item.directory_id_list,
                TotalLineCount = item.total_line_count,
                CodeLineCount = item.code_line_count,
                CommentLineCount = item.comment_line_count,
                EmptyLineCount = item.empty_line_count,
                DifferenceSLOC = item.difference_sloc,
                DifferenceLOC = item.difference_loc
            };
        }

        public AnalyzeResult CoreToData(AnalyzeResultDTO item)
        {
            return new AnalyzeResult()
            {
                id = item.ID,
                created_at = item.CreatedAt,
                updated_at = item.UpdatedAt,
                directory_count = item.DirectoryCount,
                directory_id_list = item.DirectoryIDList,
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
