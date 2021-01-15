using System;

namespace ProcessComments.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public int Code { get; set; }
    }
}
