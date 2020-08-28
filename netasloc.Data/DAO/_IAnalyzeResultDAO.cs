using netasloc.Data.Entity;
using System;
using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public interface _IAnalyzeResultDAO : __IOperationsDAO<AnalyzeResult>
    {
        IEnumerable<AnalyzeResult> GetAnalyzeResultsForRelease(DateTime release_start, DateTime release_end);
    }
}
