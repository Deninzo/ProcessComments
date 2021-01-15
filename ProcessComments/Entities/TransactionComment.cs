using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessComments.Entities
{
    public class TransactionComment
    {
        [Key]
        public Guid TransactionId { get; set; }
        public string Comment { get; set; }
    }
}
