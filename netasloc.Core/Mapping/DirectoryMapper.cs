using netasloc.Core.DTO;
using netasloc.Data.Entity;

namespace netasloc.Core.Mapping
{
    public class DirectoryMapper : _IMapper<Directory, DirectoryDTO>
    {
        public DirectoryDTO DataToCore(Directory item)
        {
            return new DirectoryDTO()
            {
                ID = item.id,
                CreatedAt = item.created_at,
                UpdatedAt = item.updated_at,
                ProjectName = item.project_name,
                FullPath = item.full_path,
                FileCount = item.file_count,
                TotalLineCount = item.total_line_count,
                CodeLineCount = item.code_line_count,
                CommentLineCount = item.comment_line_count,
                EmptyLineCount = item.empty_line_count
            };
        }

        public Directory CoreToData(DirectoryDTO item)
        {
            return new Directory()
            {
                id = item.ID,
                created_at = item.CreatedAt,
                updated_at = item.UpdatedAt,
                project_name = item.ProjectName,
                full_path = item.FullPath,
                file_count = item.FileCount,
                total_line_count = item.TotalLineCount,
                code_line_count = item.CodeLineCount,
                comment_line_count = item.CommentLineCount,
                empty_line_count = item.EmptyLineCount
            };
        }
    }
}
