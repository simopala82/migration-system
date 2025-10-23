namespace Migration.DataAccess.Model;

public class UserOld
{
    public int LegacyUserId { get; set; } // Key in the old DB
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LegacyEmailAddress { get; set; } // Field to normalize
    public string Status { get; set; } // E.g. "ACTIVE", "INACTIVE"
    public string DocumentType { get; set; } // Preferred document type
    public bool IsMigrated { get; set; } // Indicates if the user has been migrated
}
