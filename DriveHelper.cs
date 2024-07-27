namespace ExternalDriveBackupCreator;

internal class DriveHelper
{
    public static List<DriveInfo> GetUsableDrives()
    {
        return DriveInfo.GetDrives()
            .Where(d => d.IsReady && d.DriveType == DriveType.Removable)
            .ToList();
    }
}
