using System;

namespace ProcessComments.Entities
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public int Code { get; set; }
    }
}
