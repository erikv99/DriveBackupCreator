namespace ExternalDriveBackupCreator;

internal class UserInputHandler
{
    public static DriveInfo GetDriveToBackup(List<DriveInfo> drives) 
    {
        while (true) 
        {
            Console.Write("Enter the number of the drive to use for backup: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int driveNumber))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            if (driveNumber < 0 || driveNumber >=  drives.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                continue;
            }

            var selectedDrive = drives[driveNumber];
            return selectedDrive;  
        }
    }
}
