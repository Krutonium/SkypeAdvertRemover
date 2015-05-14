Imports System.IO
Imports Microsoft.Win32
Module Module1
    'Created 5/14/2015 By Kenton Boadway out of Sheer Annoyance with how Slow Skype was
    'When Adverts were enabled in Skype.
    Sub Main()
        Console.Title = "Skype Advert Remover"
        Dim Skype As Process() = Process.GetProcessesByName("skype")
        For Each runningSkype In Skype
            runningSkype.Kill()         'Skype can not be running.
        Next
        Dim Folders As New List(Of String)  'This will be our list of Skype Accounts to Patch.
        Dim SkypeDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        SkypeDir += "/Skype"    'Setting up the main Skype Dir.
        For Each folderSource In Directory.GetDirectories(SkypeDir)
            If File.Exists(folderSource & "\config.xml") Then
                Folders.Add(folderSource)       'If the folder contains a config.xml, it is a Skype Account.
            End If
        Next
        For Each folder In Folders
            File.Copy(folder & "\config.xml", folder & "config.xml.bak", True)  'Backup!
            Dim Config() As String = File.ReadAllLines(folder & "\config.xml")  'Read the Original XML
            Dim NewConfig As New List(Of String)
            For Each line In Config
                If line.ToUpper.Contains("ADVERT") Then                         'Any Line containing the word ADVERT is deleted.
                    line = Nothing                                              'No Exceptions.
                End If
                NewConfig.Add(line)                                             'Add the line back to the new config file, sans adverts
            Next
            File.WriteAllLines(folder & "\config.xml", NewConfig)               'Write out the new config file.
        Next
        '--------------------------------
        ' This Section of Code adds Apps.Skype.Com to the Internet Explorer "Deny" list.
        '--------------------------------
        Dim NetPatch As RegistryKey
        NetPatch = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains", True)
        NetPatch.CreateSubKey("skype.com")
        NetPatch = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\skype.com", True)
        NetPatch.CreateSubKey("apps")
        NetPatch = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\skype.com\apps", True)
        NetPatch.SetValue("*", "4", RegistryValueKind.DWord)
        NetPatch.Close()
        'Done!
        Console.WriteLine("Patching Complete!")
        Console.ReadKey()
    End Sub
End Module
