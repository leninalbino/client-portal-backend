using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClientPortal.Application.DTOs
{
    public class UpdateClientDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        public IFormFile? CurriculumVitae { get; set; }
        
        public IFormFile? Photo { get; set; }
    }
}