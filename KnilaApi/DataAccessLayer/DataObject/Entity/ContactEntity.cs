using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Net;

namespace KnilaApi.DataAccessLayer.DataObject.Entity
{
    public class ContactEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId {get; set;}
        public string? FirstName {get; set;}
        public string? LastName {get; set;}
        public string? Email {get; set;}
        public string? Password {get; set;}
        public string? PhoneNumber {get; set;}
        public string? Address {get; set;}
        public string? City {get; set;}
        public string? State {get; set;}
        public string? Country {get; set;}
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
