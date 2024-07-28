namespace ExternalDriveBackupCreator;

public class BackupSettings
{
    public required List<string> DirectoriesToBackup { get; set; }
    public required string BackupFolderName { get; set; }
    public required string DateFormat { get; set; }
    public string? DefaultDriveName { get; set; }
}
