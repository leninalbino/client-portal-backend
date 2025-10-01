using ClientPortal.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClientPortal.Application.DTOs
{
    public class CreateClientDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public DocumentType DocumentType { get; set; }
        
        [Required]
        [StringLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;
        
        [Required]
        public IFormFile CurriculumVitae { get; set; } = null!;
        
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}