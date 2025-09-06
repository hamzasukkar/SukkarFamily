# PowerShell script to add Date column to News table
$dbPath = ".\SukkarFamily\data\FamilyTree.db"

if (-not (Test-Path $dbPath)) {
    Write-Host "Database file not found: $dbPath" -ForegroundColor Red
    exit 1
}

try {
    # Load SQLite assembly
    Add-Type -Path "$env:USERPROFILE\.nuget\packages\system.data.sqlite.core\1.0.118\lib\netstandard2.0\System.Data.SQLite.dll" -ErrorAction SilentlyContinue
    
    if (-not ([System.Management.Automation.PSTypeName]'System.Data.SQLite.SQLiteConnection').Type) {
        Write-Host "SQLite assembly not found. Trying alternative approach..." -ForegroundColor Yellow
        
        # Try to use Microsoft.Data.Sqlite
        Add-Type -AssemblyName "Microsoft.Data.Sqlite" -ErrorAction SilentlyContinue
    }
    
    $connectionString = "Data Source=$dbPath"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Check if Date column exists
    $checkCommand = $connection.CreateCommand()
    $checkCommand.CommandText = "PRAGMA table_info(News)"
    $reader = $checkCommand.ExecuteReader()
    
    $dateColumnExists = $false
    while ($reader.Read()) {
        if ($reader["name"] -eq "Date") {
            $dateColumnExists = $true
            break
        }
    }
    $reader.Close()
    
    if ($dateColumnExists) {
        Write-Host "Date column already exists in News table." -ForegroundColor Green
    } else {
        # Add the Date column
        $alterCommand = $connection.CreateCommand()
        $alterCommand.CommandText = "ALTER TABLE News ADD COLUMN Date TEXT"
        $alterCommand.ExecuteNonQuery()
        
        Write-Host "Successfully added Date column to News table." -ForegroundColor Green
    }
    
    $connection.Close()
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack trace: $($_.Exception.StackTrace)" -ForegroundColor Red
}