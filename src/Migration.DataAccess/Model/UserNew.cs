namespace Migration.DataAccess.Model;

public class UserNew
{
    public Guid UserId { get; set; } // New primary key
    public string FullName { get; set; } // First and last name combined
    public string StandardizedEmail { get; set; } // Normalized field
    public bool IsActive { get; set; }
    public string PreferredDocumentType { get; set; } // Mapping from OLD DocumentType
}
