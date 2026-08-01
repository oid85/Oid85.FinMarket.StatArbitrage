using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oid85.PrintTemplate.Infrastructure.Database.Entities.Base;

public class BaseEntity
{
    [Column("id")]
    [Key]
    public Guid Id { get; set; }
}