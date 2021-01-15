using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessComments.Entities
{
    [Table(name: "FilesContent")]
    public class FileContent
    {
         public Guid Id { get; set; }
         public byte[] Content { get; set; }
         public Guid? DataStorageId { get; set; }
    }
}
