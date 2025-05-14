Imports System.IO
Imports OfficeOpenXml

Public Class Form5
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadExcelFileList()
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        If ListBox1.SelectedItem IsNot Nothing Then
            Dim fileName As String = ListBox1.SelectedItem.ToString()
            Dim fullPath As String = Path.Combine(Application.StartupPath, "SavedFiles", fileName)

            If File.Exists(fullPath) Then
                ' Открыть Form6 и передать путь
                Dim f6 As New Form6()
                f6.ExcelFilePath = fullPath
                f6.Show()
            End If
        End If
    End Sub

    Public Sub LoadExcelFileList()
        Dim saveDir As String = Path.Combine(Application.StartupPath, "SavedFiles")

        If Not Directory.Exists(saveDir) Then
            Directory.CreateDirectory(saveDir)
        End If

        Dim excelFiles As String() = Directory.GetFiles(saveDir, "*.xlsx")
        ListBox1.Items.Clear()
        For Each filePath As String In excelFiles
            ListBox1.Items.Add(Path.GetFileName(filePath))
        Next
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim prevForm As New Form3()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class
