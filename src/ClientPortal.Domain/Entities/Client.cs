using System;

namespace ClientPortal.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string? CurriculumVitaeFileName { get; set; }
        public string? CurriculumVitaeFilePath { get; set; }
        public string? PhotoFileName { get; set; }
        public string? PhotoFilePath { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}