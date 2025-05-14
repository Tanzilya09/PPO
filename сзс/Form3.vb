Imports Microsoft.Office.Interop
Imports System.Data.OleDb
Imports System.Runtime.InteropServices

Public Class Form3

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Excel Files|*.xlsx;*.xls"
        If ofd.ShowDialog() = DialogResult.OK Then
            Dim filePath As String = ofd.FileName
            Dim excelData As DataTable = ImportExcelData(filePath)
            If excelData IsNot Nothing Then
                Dim viewer As New Form4(excelData)
                viewer.Show()
            Else
                MessageBox.Show("Не удалось загрузить Excel.")
            End If
        End If
    End Sub

    ' Импорт Excel с помощью OleDb
    Private Function ImportExcelData(filePath As String) As DataTable
        Try
            Dim connStr As String = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;'"
            Using conn As New OleDbConnection(connStr)
                conn.Open()
                Dim schemaTable As DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
                Dim sheetName As String = schemaTable.Rows(0)("TABLE_NAME").ToString()

                Dim cmd As New OleDbCommand($"SELECT * FROM [{sheetName}]", conn)
                Dim adapter As New OleDbDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        Catch ex As Exception
            MessageBox.Show("Ошибка импорта Excel: " & ex.Message)
            Return Nothing
        End Try
    End Function
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim f5 As New Form5()
        f5.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Dim f6 As New Form6()
        f6.Show()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Dim prevForm As New Form2()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class