Imports System.IO
Public Class Form7

    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBox1.SelectionMode = SelectionMode.MultiExtended
        LoadSavedReports()
    End Sub

    Private Sub LoadSavedReports()
        Dim folderPath As String = Path.Combine(Application.StartupPath, "SavedReports")

        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If

        Dim files = Directory.GetFiles(folderPath, "*.xlsx")
        ListBox1.Items.Clear()
        For Each file In files
            ListBox1.Items.Add(Path.GetFileName(file))
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ListBox1.SelectedItems.Count = 0 Then
            MessageBox.Show("Выберите хотя бы один файл.")
            Return
        End If

        Dim selectedFiles As New List(Of String)

        For Each selectedItem As String In ListBox1.SelectedItems
            Dim fullPath = Path.Combine(Application.StartupPath, "SavedReports", selectedItem)
            selectedFiles.Add(fullPath)
        Next

        Dim form8 As New Form8()
        form8.SelectedFilePaths = selectedFiles
        form8.ShowDialog()
    End Sub
End Class
