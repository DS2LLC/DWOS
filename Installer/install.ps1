# Installer
# Assumption - version numbers & GUIDs were previously updated

$sourcePath = if ($Env:BUILD_SOURCESDIRECTORY -And $Env:InstallerFolder) {
    Join-Path $Env:BUILD_SOURCESDIRECTORY $Env:InstallerFolder
} else {
    "C:\Projects\DWOS\Source Code\Development"
}

Write-Host `$sourcePath = `"$sourcePath`"

$installerPath = Join-Path $sourcePath Installer
$clientInstaller = Join-Path $installerPath "DWOS Client.aip"
$migrationInstaller = Join-Path $installerPath "DWOS Migration.aip"
$serverInstaller = Join-Path $installerPath "DWOS Server.aip"
$archiverInstaller = Join-Path $installerPath "DWOS Data Archiver.aip"

if ($Env:AdvancedInstallerPath) {
    $advInstaller = $Env:AdvancedInstallerPath
} else {
    $hkeyLocalMachine = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, [Microsoft.Win32.RegistryView]::Registry64)
    $advInstallerKey = $hkeyLocalMachine.OpenSubKey("SOFTWARE\Wow6432Node\Caphyon\Advanced Installer")
    $advInstallerPath =  $advInstallerKey.GetValue("Advanced Installer Path")
    $advInstaller = [io.path]::combine($advInstallerPath, "bin", "x86", "AdvancedInstaller.com")
}

Write-Host `$advInstaller = `"$advInstaller`"

if ($Env:BuildClient -eq $null) {
    Write-Host `$Env.BuildClient` was unset
    $buildClient = $true;
} else {
    $buildClient = [System.Convert]::ToBoolean($Env:BuildClient);
}

if ($Env:BuildMigration -eq $null) {
    Write-Host `$Env.BuildMigration` was unset
    $buildMigration = $true;
} else {
    $buildMigration = [System.Convert]::ToBoolean($Env:BuildMigration);
}

if ($env:BuildServer -eq $null) {
    Write-Host `$Env.BuildServer` was unset
    $buildServer = $true;
} else {
    $buildServer = [System.Convert]::ToBoolean($Env:BuildServer);
}

if ($env:BuildArchiver -eq $null) {
    Write-Host `$Env.BuildArchiver` was unset
    $buildArchiver = $true;
} else {
    $buildArchiver = [System.Convert]::ToBoolean($Env:BuildArchiver);
}

if (Test-Path -LiteralPath $advInstaller) {
    if ((Test-Path $clientInstaller) -And $buildClient) {
        Write-Host ***Building $clientInstaller***

        $proc = Start-Process -NoNewWindow -FilePath $advInstaller -ArgumentList "/build `"$clientInstaller`"" -Passthru
        Wait-Process -InputObject $proc
        Write-Host Exit Code: $proc.ExitCode
        if ($proc.ExitCode -ne 0) {
            Write-Host Could not build installer.
            exit 1
        }
    } else {
        Write-Host Skipping client installer.
    }

    if ((Test-Path $migrationInstaller) -And $buildMigration) {
        Write-Host ***Building $migrationInstaller***

        $proc = Start-Process -NoNewWindow -FilePath $advInstaller -ArgumentList "/build `"$migrationInstaller`"" -Passthru
        Wait-Process -InputObject $proc
        Write-Host Exit Code: $proc.ExitCode
        if ($proc.ExitCode -ne 0) {
            Write-Host Could not build installer.
            exit 1
        }
    } else {
        Write-Host Skipping migration installer.
    }

    if ((Test-Path $serverInstaller) -And $buildServer) {
        Write-Host ***Building $serverInstaller***
    
        $proc = Start-Process -NoNewWindow -FilePath $advInstaller -ArgumentList "/build `"$serverInstaller`"" -Passthru
        Wait-Process -InputObject $proc
        Write-Host Exit Code: $proc.ExitCode
        if ($proc.ExitCode -ne 0) {
            Write-Host Could not build installer.
            exit 1
        }
    } else {
        Write-Host Skipping server installer.
    }
    
    if ((Test-Path $archiverInstaller) -And $buildArchiver) {
        Write-Host ***Building $archiverInstaller***

        $proc = Start-Process -NoNewWindow -FilePath $advInstaller -ArgumentList "/build `"$archiverInstaller`"" -Passthru
        Wait-Process -InputObject $proc
        Write-Host Exit Code: $proc.ExitCode
        if ($proc.ExitCode -ne 0) {
            Write-Host Could not build installer.
            exit 1
        }
    } else {
        Write-Host Skipping Archiver installer.
    }
} else {
    Write-Host Installer application not found
}
