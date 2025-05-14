Imports Microsoft.Office.Interop.Excel

Public Class Form10
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub FormRegister_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.Items.Add("Администратор")
        ComboBox1.Items.Add("Бухгалтер")
        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub ButtonRegister_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim login As String = TextBox1.Text.Trim()
        Dim password As String = TextBox2.Text.Trim()
        Dim role As String = ComboBox1.SelectedItem.ToString()

        If login = "" OrElse password = "" Then
            MessageBox.Show("Пожалуйста, заполните все поля.")
            Return
        End If

        Dim xlApp As New Application
        Dim xlWorkbook As Workbook = Nothing
        Dim xlSheet As Worksheet = Nothing
        Dim path As String = "C:\Users\user\Книга1.xlsx"

        Try
            xlWorkbook = xlApp.Workbooks.Open(path)
            xlSheet = xlWorkbook.Sheets(1)

            ' Поиск первой пустой строки
            Dim row As Integer = 2
            Do While xlSheet.Cells(row, 1).Value IsNot Nothing
                row += 1
            Loop

            xlSheet.Cells(row, 1).Value = login
            xlSheet.Cells(row, 2).Value = password
            xlSheet.Cells(row, 3).Value = role

            xlWorkbook.Save()

            MessageBox.Show("Сотрудник успешно зарегистрирован!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim prevForm As New Form9()
            prevForm.Show()
            Me.Hide()
        Catch ex As Exception
            MessageBox.Show("Ошибка при записи в Excel: " & ex.Message)
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

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim prevForm As New Form9()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class

