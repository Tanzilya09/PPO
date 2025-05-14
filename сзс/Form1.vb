Imports Microsoft.Office.Interop.Excel

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Авторизация"
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        TextBox2.UseSystemPasswordChar = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim loginInput As String = TextBox1.Text
        Dim passwordInput As String = TextBox2.Text

        Dim userRole As String = CheckCredentials(loginInput, passwordInput)

        If userRole IsNot Nothing Then
            MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information)

            If userRole = "Администратор" Then
                Dim adminForm As New Form9()
                adminForm.Show()
            ElseIf userRole = "Бухгалтер" Then
                Dim accountantForm As New Form2()
                accountantForm.Show()
            Else
                MessageBox.Show("Неизвестная роль пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.Hide()
        Else
            MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Function CheckCredentials(login As String, password As String) As String
        Dim xlApp As New Application
        Dim xlWorkbook As Workbook = Nothing
        Dim xlSheet As Worksheet = Nothing
        Dim path As String = "C:\Users\user\Книга1.xlsx"
        Dim role As String = Nothing

        Try
            xlWorkbook = xlApp.Workbooks.Open(path)
            xlSheet = xlWorkbook.Sheets(1)

            Dim row As Integer = 2
            Do While xlSheet.Cells(row, 1).Value IsNot Nothing
                Dim fileLogin As String = xlSheet.Cells(row, 1).Value.ToString()
                Dim filePassword As String = xlSheet.Cells(row, 2).Value.ToString()

                If fileLogin = login AndAlso filePassword = password Then
                    role = xlSheet.Cells(row, 3).Value.ToString()
                    Exit Do
                End If
                row += 1
            Loop
        Catch ex As Exception
            MessageBox.Show("Ошибка при чтении Excel: " & ex.Message)
        Finally
            xlWorkbook?.Close(False)
            xlApp.Quit()
            ReleaseObject(xlSheet)
            ReleaseObject(xlWorkbook)
            ReleaseObject(xlApp)
        End Try

        Return role
    End Function

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
End Class



