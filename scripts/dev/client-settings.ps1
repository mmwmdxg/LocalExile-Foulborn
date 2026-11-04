# Writes the client options that matter for this server into the client's settings file, before the launcher starts it:
# the window (mode and size) and the network mode. The client keeps these in
# Documents\My Games\Path of Exile\production_Config.ini ([DISPLAY] resolution_width/height, fullscreen,
# borderless_windowed_fullscreen; [NETWORKING] networking_type, Agents/docs/protocol/07-latency-visibility-movement.md §6).
# The patched client's own default name, poeoffline_Config.ini, is written too, so either name works.
# Only these keys change; everything else in the file is kept, and the previous file is copied once to *.bak.
#   -Width <px> -Height <px> [-Fullscreen] [-Borderless] [-NetworkMode predictive|lockstep|auto]
param(
    [int]$Width = 2560,
    [int]$Height = 1440,
    [switch]$Fullscreen,
    [switch]$Borderless,
    [ValidateSet('predictive', 'lockstep', 'auto')][string]$NetworkMode = 'predictive',
    [string]$Folder
)

$ErrorActionPreference = 'Stop'

$dir = if ($Folder) { $Folder } else { Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'My Games\Path of Exile' }
New-Item -ItemType Directory -Force $dir | Out-Null

# The values to set, by section and key.
$wanted = [ordered]@{
    'DISPLAY' = [ordered]@{
        resolution_width = "$Width"
        resolution_height = "$Height"
        fullscreen = if ($Fullscreen) { 'true' } else { 'false' }
        borderless_windowed_fullscreen = if ($Borderless) { 'true' } else { 'false' }
        maximize_window = 'false'
    }
    'NETWORKING' = [ordered]@{ networking_type = $NetworkMode }
}

function Set-ClientSettings([string]$path) {
    $sections = [ordered]@{}
    if (Test-Path $path) {
        $current = ''
        foreach ($line in Get-Content -LiteralPath $path) {
            $trimmed = $line.Trim()
            if ($trimmed -match '^\[(.+)\]$') { $current = $Matches[1]; if (-not $sections.Contains($current)) { $sections[$current] = [ordered]@{} }; continue }
            if ($trimmed -match '^([^=]+)=(.*)$') {
                if (-not $sections.Contains($current)) { $sections[$current] = [ordered]@{} }
                $sections[$current][$Matches[1].Trim()] = $Matches[2].Trim()
            }
        }
        Copy-Item -LiteralPath $path -Destination "$path.bak" -Force
    }
    foreach ($section in $wanted.Keys) {
        if (-not $sections.Contains($section)) { $sections[$section] = [ordered]@{} }
        foreach ($key in $wanted[$section].Keys) { $sections[$section][$key] = $wanted[$section][$key] }
    }
    $out = [System.Collections.Generic.List[string]]::new()
    foreach ($section in $sections.Keys) {
        $out.Add("[$section]")
        foreach ($key in $sections[$section].Keys) { $out.Add("$key=$($sections[$section][$key])") }
        $out.Add('')
    }
    Set-Content -LiteralPath $path -Value $out -Encoding ascii
    Write-Output "client settings: $path"
}

foreach ($name in 'production_Config.ini', 'poeoffline_Config.ini') { Set-ClientSettings (Join-Path $dir $name) }
Write-Output "  display: $(if ($Fullscreen) { 'fullscreen' } elseif ($Borderless) { 'borderless' } else { 'windowed' }) ${Width}x${Height}"
Write-Output "  networking_type: $NetworkMode"
