using System;
using System.ComponentModel.DataAnnotations;

namespace Studio1BTask.Models
{
    public class CustomerTransaction
    {
        [Key] public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime Date { get; set; }
    }
}