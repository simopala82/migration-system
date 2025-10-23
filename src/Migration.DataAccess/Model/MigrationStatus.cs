namespace Migration.DataAccess.Model;

public class MigrationStatus
{
    public int Id { get; set; }
    public int LegacyUserId { get; set; }
    public Guid? NewUserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public MigrationState Status { get; set; }
    public string? ErrorDetails { get; set; }
    public string? AdminActionBy { get; set; }
}

public enum MigrationState
{
    PENDING = 0,
    IN_PROGRESS = 10,
    SUCCESS = 20,
    FAILED = 30
}
