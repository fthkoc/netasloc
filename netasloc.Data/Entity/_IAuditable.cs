using System;

namespace netasloc.Data.Entity
{
    public interface _IAuditable
    {
        uint id { get; set; }
        DateTime created_at { get; set; }
        DateTime updated_at { get; set; }
    }
}
