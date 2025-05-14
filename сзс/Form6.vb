Imports OfficeOpenXml
Imports System.IO

Public Class Form6
    Public Property ExcelFilePath As String


    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBoxCurrency.Items.AddRange(New String() {"USD", "EUR", "KZT", "KGS"})
        ComboBoxCurrency.SelectedItem = "USD"

        If Not String.IsNullOrEmpty(ExcelFilePath) AndAlso File.Exists(ExcelFilePath) Then
            LoadExcelToDataGridView(ExcelFilePath)
        Else
            MessageBox.Show("Файл не найден.")
            Me.Close()
        End If
    End Sub

    Private Sub LoadExcelToDataGridView(filePath As String)
        ExcelPackage.License.SetNonCommercialPersonal("Your Name")

        Using package As New ExcelPackage(New FileInfo(filePath))
            Dim worksheet = package.Workbook.Worksheets.FirstOrDefault()
            If worksheet Is Nothing Then
                MessageBox.Show("Лист Excel не найден.")
                Return
            End If

            Dim dt As New DataTable()

            Dim startRow = worksheet.Dimension.Start.Row
            Dim endRow = worksheet.Dimension.End.Row
            Dim startCol = worksheet.Dimension.Start.Column
            Dim endCol = worksheet.Dimension.End.Column

            ' Заголовки
            For col = startCol To endCol
                dt.Columns.Add(worksheet.Cells(startRow, col).Text)
            Next

            ' Данные
            For row = startRow + 1 To endRow
                Dim dr = dt.NewRow()
                For col = startCol To endCol
                    dr(col - startCol) = worksheet.Cells(row, col).Text
                Next
                dt.Rows.Add(dr)
            Next

            DataGridView1.DataSource = dt
        End Using
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("Нет данных для конвертации.", "Ошибка")
            Return
        End If

        If Not dt.Columns.Contains("Сумма") OrElse Not dt.Columns.Contains("Валюта") Then
            MessageBox.Show("Таблица должна содержать столбцы 'Сумма' и 'Валюта'", "Ошибка")
            Return
        End If

        If ComboBoxCurrency.SelectedItem Is Nothing Then
            MessageBox.Show("Пожалуйста, выберите валюту для конвертации.")
            Return
        End If

        Dim targetCurrency As String = ComboBoxCurrency.SelectedItem.ToString()

        Dim rates As New Dictionary(Of String, Decimal) From {
        {"USD", 89.5D},
        {"EUR", 97.2D},
        {"KZT", 0.17D},
        {"KGS", 1D}
    }

        If Not dt.Columns.Contains("Сумма (" & targetCurrency & ")") Then
            dt.Columns.Add("Сумма (" & targetCurrency & ")", GetType(Decimal))
        End If

        For Each row As DataRow In dt.Rows
            Dim amount As Decimal
            Dim currency As String = row("Валюта").ToString().ToUpper()

            If Decimal.TryParse(row("Сумма").ToString(), amount) AndAlso
            rates.ContainsKey(currency) AndAlso rates.ContainsKey(targetCurrency) Then

                Dim convertedAmount = amount * rates(currency) / rates(targetCurrency)
                row("Сумма (" & targetCurrency & ")") = Math.Round(convertedAmount, 2)
            Else
                row("Сумма (" & targetCurrency & ")") = 0
            End If
        Next

        dt.AcceptChanges()
        MessageBox.Show("Конвертация завершена в " & targetCurrency & ".")
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing OrElse ComboBoxCurrency.SelectedItem Is Nothing Then
            MessageBox.Show("Сначала выполните конвертацию.", "Ошибка")
            Return
        End If

        Dim selectedCurrency As String = ComboBoxCurrency.SelectedItem.ToString()
        Dim columnName As String = "Сумма (" & selectedCurrency & ")"

        If Not dt.Columns.Contains(columnName) Then
            MessageBox.Show("Нет данных по валюте " & selectedCurrency)
            Return
        End If

        Dim total As Decimal = 0
        For Each row As DataRow In dt.Rows
            total += Convert.ToDecimal(row(columnName))
        Next

        Label1.Text = total.ToString("N2") & " " & selectedCurrency
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("Нет данных для сохранения.", "Ошибка")
            Return
        End If

        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "Excel Files|*.xlsx"
        saveDialog.Title = "Сохранить как Excel-файл"
        saveDialog.FileName = "converted_data.xlsx"

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Dim newFilePath As String = saveDialog.FileName

            ExcelPackage.License.SetNonCommercialPersonal("Your Name")

            Using package As New ExcelPackage()
                Dim worksheet = package.Workbook.Worksheets.Add("Sheet1")

                ' Заголовки
                For col = 0 To dt.Columns.Count - 1
                    worksheet.Cells(1, col + 1).Value = dt.Columns(col).ColumnName
                Next

                ' Данные
                For row = 0 To dt.Rows.Count - 1
                    For col = 0 To dt.Columns.Count - 1
                        worksheet.Cells(row + 2, col + 1).Value = dt.Rows(row)(col)
                    Next
                Next

                ' Сохраняем файл
                Dim folderPath = Path.Combine(Application.StartupPath, "SavedReports")
                If Not Directory.Exists(folderPath) Then Directory.CreateDirectory(folderPath)

                newFilePath = Path.Combine(folderPath, "converted_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".xlsx")
                Try

                    package.SaveAs(New FileInfo(newFilePath))
                    MessageBox.Show("Файл успешно сохранён: " & newFilePath)
                Catch ex As Exception
                    MessageBox.Show("Ошибка при сохранении: " & ex.Message)
                End Try
            End Using
        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim prevForm As New Form2()
        prevForm.Show()
        Me.Hide()
    End Sub
End Class

