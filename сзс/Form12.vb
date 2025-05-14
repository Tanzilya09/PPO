Imports Microsoft.Office.Interop.Excel

Public Class Form12
    Private path As String = "C:\Users\user\Книга1.xlsx"

    Private Sub FormDeleteUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadUsers()
    End Sub

    Private Sub LoadUsers()
        Dim xlApp As New Application
        Dim xlWorkbook As Workbook = Nothing
        Dim xlSheet As Worksheet = Nothing

        Try
            xlWorkbook = xlApp.Workbooks.Open(path)
            xlSheet = xlWorkbook.Sheets(1)

            ComboBox1.Items.Clear()
            Dim row As Integer = 2

            Do While xlSheet.Cells(row, 1).Value IsNot Nothing
                ComboBox1.Items.Add(xlSheet.Cells(row, 1).Value.ToString())
                row += 1
            Loop

            If ComboBox1.Items.Count > 0 Then
                ComboBox1.SelectedIndex = 0
            End If

        Catch ex As Exception
            MessageBox.Show("Ошибка при загрузке пользователей: " & ex.Message)
        Finally
            xlWorkbook?.Close(False)
            xlApp.Quit()
            ReleaseObject(xlSheet)
            ReleaseObject(xlWorkbook)
            ReleaseObject(xlApp)
        End Try
    End Sub

    Private Sub ButtonDelete_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim selectedUser As String = ComboBox1.SelectedItem?.ToString()

        If String.IsNullOrEmpty(selectedUser) Then
            MessageBox.Show("Выберите пользователя для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim xlApp As New Application
        Dim xlWorkbook As Workbook = Nothing
        Dim xlSheet As Worksheet = Nothing

        Try
            xlWorkbook = xlApp.Workbooks.Open(path)
            xlSheet = xlWorkbook.Sheets(1)

            Dim row As Integer = 2
            Dim deleted As Boolean = False

            Do While xlSheet.Cells(row, 1).Value IsNot Nothing
                If xlSheet.Cells(row, 1).Value.ToString() = selectedUser Then
                    xlSheet.Rows(row).Delete()
                    deleted = True
                    Exit Do
                End If
                row += 1
            Loop

            If deleted Then
                xlWorkbook.Save()
                MessageBox.Show("Пользователь удалён.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadUsers()
            Else
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Ошибка при удалении: " & ex.Message)
        Finally
            xlWorkbook?.Close(True)
            xlApp.Quit()
            ReleaseObject(xlSheet)
            ReleaseObject(xlWorkbook)
            ReleaseObject(xlApp)
        End Try
    End Sub

    Private Sub ReleaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub

    Private Sub ButtonChangePass_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim selectedUser As String = ComboBox1.SelectedItem?.ToString()
        Dim newPassword As String = TextBox1.Text.Trim()

        If String.IsNullOrEmpty(selectedUser) OrElse String.IsNullOrEmpty(newPassword) Then
            MessageBox.Show("Выберите пользователя и введите новый пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim xlApp As New Application
        Dim xlWorkbook As Workbook = Nothing
        Dim xlSheet As Worksheet = Nothing

        Try
            xlWorkbook = xlApp.Workbooks.Open(path)
            xlSheet = xlWorkbook.Sheets(1)

            Dim row As Integer = 2
            Dim updated As Boolean = False

            Do While xlSheet.Cells(row, 1).Value IsNot Nothing
                If xlSheet.Cells(row, 1).Value.ToString() = selectedUser Then
                    xlSheet.Cells(row, 2).Value = newPassword
                    updated = True
                    Exit Do
                End If
                row += 1
            Loop

            If updated Then
                xlWorkbook.Save()
                MessageBox.Show("Пароль обновлён.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information)
                TextBox1.Clear()
            Else
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Ошибка при обновлении пароля: " & ex.Message)
        Finally
            xlWorkbook?.Close(True)
            xlApp.Quit()
            ReleaseObject(xlSheet)
            ReleaseObject(xlWorkbook)
            ReleaseObject(xlApp)
        End Try
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim prevForm As New Form9()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class
