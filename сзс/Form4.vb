Imports System.ComponentModel
Imports System.Data.OleDb
Imports System.IO
Imports OfficeOpenXml

Public Class Form4
    Private data As DataTable
    Private excelData As DataTable

    Public Sub New(dt As DataTable)
        InitializeComponent()
        data = dt
    End Sub

    Private Sub FormExcelViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView1.DataSource = data
    End Sub

    ' Объяви переменную на уровне формы (вверху):
    Private filteredDataTable As DataTable

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                MessageBox.Show("Нет данных в таблице.", "Ошибка")
                Return
            End If

            If Not dt.Columns.Contains("ID") Then
                MessageBox.Show("В таблице нет столбца 'ID'", "Ошибка")
                Return
            End If

            Dim accessFile As String = "D:\бд банка.accdb"
            If Not File.Exists(accessFile) Then
                MessageBox.Show("Файл Access не найден.", "Ошибка")
                Return
            End If

            ' Загружаем допустимые ID из Access
            Dim allowedIDs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & accessFile
            Using conn As New OleDbConnection(connStr)
                conn.Open()
                Using cmd As New OleDbCommand("SELECT [ID] FROM Таблица1", conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            If Not IsDBNull(reader("ID")) Then
                                allowedIDs.Add(reader("ID").ToString().Trim())
                            End If
                        End While
                    End Using
                End Using
            End Using

            ' Создаём таблицу для отфильтрованных данных
            filteredDataTable = dt.Clone()

            Dim removedCount As Integer = 0

            For Each row As DataRow In dt.Rows
                Dim idVal = row("ID")
                If idVal IsNot DBNull.Value Then
                    Dim idStr = idVal.ToString().Trim()
                    If allowedIDs.Contains(idStr) Then
                        filteredDataTable.ImportRow(row)
                    Else
                        removedCount += 1
                    End If
                Else
                    removedCount += 1
                End If
            Next

            DataGridView1.DataSource = filteredDataTable
            MessageBox.Show($"Фильтрация завершена. Удалено строк: {removedCount}", "Готово")

        Catch ex As Exception
            MessageBox.Show("Ошибка при фильтрации: " & ex.Message)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If filteredDataTable Is Nothing OrElse filteredDataTable.Rows.Count = 0 Then
                MessageBox.Show("Нет отфильтрованных данных для сохранения.", "Ошибка")
                Return
            End If

            Dim sfd As New SaveFileDialog()
            sfd.Title = "Сохранить Excel-файл"
            sfd.Filter = "Excel файлы (*.xlsx)|*.xlsx"
            sfd.FileName = "Filtered_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".xlsx"

            If sfd.ShowDialog() = DialogResult.OK Then
                ExcelPackage.License.SetNonCommercialPersonal("Your Name")
                Using package As New ExcelPackage()
                    Dim sheet = package.Workbook.Worksheets.Add("Результаты")
                    sheet.Cells("A1").LoadFromDataTable(filteredDataTable, True)
                    package.SaveAs(New FileInfo(sfd.FileName))
                End Using

                ' Копируем в папку SavedFiles
                Dim savedDir As String = Path.Combine(Application.StartupPath, "SavedFiles")
                If Not Directory.Exists(savedDir) Then Directory.CreateDirectory(savedDir)
                Dim savedPath As String = Path.Combine(savedDir, Path.GetFileName(sfd.FileName))
                File.Copy(sfd.FileName, savedPath, True)

                ' Открываем Form5
                Dim f5 As New Form5()
                f5.Show()
                f5.LoadExcelFileList()
            End If

        Catch ex As Exception
            MessageBox.Show("Ошибка при сохранении: " & ex.Message)
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim prevForm As New Form3()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class