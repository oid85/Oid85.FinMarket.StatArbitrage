using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities.Base;

public class BaseEntity
{
    [Column("id")]
    [Key]
    public Guid Id { get; set; }
}