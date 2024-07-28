
using ExternalDriveBackupCreator;
using Microsoft.Extensions.Configuration;

var drives = DriveInfo.GetDrives()
    .Where(d => d.IsReady)
    .ToList();

if (!drives.Any()) 
{
    Console.WriteLine("No drives are available for backup.");
}

var hasLocalConfig = File.Exists("appsettings.local.json");
var configName = hasLocalConfig ? "appsettings.local.json" : "appsettings.json";
var configPath = Path.Combine(Directory.GetCurrentDirectory(), configName);

var config = new ConfigurationBuilder()
    .AddJsonFile(configPath, optional: false)
    .Build() ?? throw new Exception("Config is null");

var backupSettings = config.GetSection("BackupSettings").Get<BackupSettings>()
    ?? throw new Exception("BackupSettings could not be binded.");

_printDrives(drives!);
var selectedDrive = _getSelectedDriveFromUser(drives);

Console.WriteLine($"\nSelected drive: {selectedDrive.Name} - {selectedDrive.VolumeLabel} ({selectedDrive})");

// TODO: make error handling more robust
try
{
    foreach (var dir in backupSettings.DirectoriesToBackup)
    {
        Console.WriteLine($"\nBacking up {dir} to {selectedDrive.Name}...");
        _backupDirectory(dir, selectedDrive, backupSettings);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

void _backupDirectory(string sourceDir, DriveInfo targetDrive, BackupSettings settings)
{
    string backupRoot = Path.Combine(targetDrive.Name, settings.BackupFolderName);
    string backupDir = Path.Combine(backupRoot, Path.GetFileName(sourceDir), DateTime.Now.ToString(settings.DateFormat));

    Directory.CreateDirectory(backupDir);

    foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(sourceDir, backupDir));
    }

    foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(sourceDir, backupDir), true);
    }

    Console.WriteLine($"Backed up {sourceDir} to {backupDir}");
}

void _printDrives(List<DriveInfo> drives)
{
    Console.WriteLine("Available drives:");

    for (var i = 0; i < drives.Count; i++)
    {
        var drive = drives[i];
        Console.WriteLine($"[{i}] {drive.Name} - {drive.VolumeLabel} ({drive})");
    }
}

DriveInfo _getSelectedDriveFromUser(List<DriveInfo> drives)
{
    DriveInfo selectedDrive;
    while (true)
    {
        Console.Write("\nEnter the number of the drive to use for backup: ");
        var input = Console.ReadLine();

        if (!int.TryParse(input, out int driveNumber))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        if (driveNumber < 0 || driveNumber >= drives.Count)
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            continue;
        }

        selectedDrive = drives[driveNumber];
        break;
    }

    return selectedDrive;
}