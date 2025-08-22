param (
    [string]$TempDir = "$env:TEMP\TaskManagementPortal"
)

# Route file
$summaryFile = Join-Path $TempDir "task_summary.txt"

if (Test-Path $summaryFile) {
    $content = Get-Content $summaryFile -Raw

    # Load Windows Runtime Toast API
    [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null
    $template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)

    # Insert title and content with truncation
    $textNodes = $template.GetElementsByTagName("text")
    $textNodes.Item(0).AppendChild($template.CreateTextNode("📝 Task Reminder")) > $null  # Add emoji for visual
    $textNodes.Item(1).AppendChild($template.CreateTextNode($content.Substring(0, [Math]::Min(300, $content.Length)))) > $null

    # Add sound
    $audio = $template.CreateElement("audio")
    $audio.SetAttribute("src", "ms-winsoundevent:Notification.IM")
    $template.DocumentElement.AppendChild($audio) > $null

    # Show notification
    $toast = [Windows.UI.Notifications.ToastNotification]::new($template)
    $notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier("TaskManagementPortal")
    $notifier.Show($toast)
}
else {
    Write-Output "No summary file found at $summaryFile"
}
