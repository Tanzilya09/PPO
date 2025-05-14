Imports System.IO
Imports OfficeOpenXml
Imports System.Data

Public Class Form8
    Public SelectedFilePaths As List(Of String)
    Private AllDataTable As DataTable

    Private Sub Form8_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ExcelPackage.License.SetNonCommercialPersonal("Your Name")
        LoadAllData()
        PopulateIdFilter()
    End Sub

    Private Sub LoadAllData()
        AllDataTable = New DataTable()

        For Each filePath In SelectedFilePaths
            If Not File.Exists(filePath) Then
                MessageBox.Show("Файл не найден: " & filePath)
                Continue For
            End If

            Using package As New ExcelPackage(New FileInfo(filePath))
                Dim ws = package.Workbook.Worksheets(0)

                Dim rows = ws.Dimension.Rows
                Dim cols = ws.Dimension.Columns

                If AllDataTable.Columns.Count = 0 Then
                    For col = 1 To cols
                        AllDataTable.Columns.Add(ws.Cells(1, col).Text)
                    Next
                End If

                For row = 2 To rows
                    Dim newRow = AllDataTable.NewRow()
                    For col = 1 To cols
                        newRow(col - 1) = ws.Cells(row, col).Text
                    Next
                    AllDataTable.Rows.Add(newRow)
                Next
            End Using
        Next

        DataGridView1.DataSource = AllDataTable
    End Sub

    Private Sub PopulateIdFilter()
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("Все ID")

        If AllDataTable IsNot Nothing AndAlso AllDataTable.Columns.Contains("ID") Then
            Dim uniqueIds = AllDataTable.AsEnumerable().
                            Select(Function(r) r("ID").ToString()).
                            Distinct().
                            OrderBy(Function(x) x)
            For Each id In uniqueIds
                ComboBox1.Items.Add(id)
            Next
        End If

        ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If AllDataTable Is Nothing Then Return

        Dim startDate As Date = DateTimePicker1.Value.Date
        Dim endDate As Date = DateTimePicker2.Value.Date
        Dim selectedId As String = ComboBox1.SelectedItem?.ToString()

        Dim filtered = AllDataTable.Clone()
        For Each row As DataRow In AllDataTable.Rows
            Dim dateValue As DateTime
            If Date.TryParse(row("Дата").ToString(), dateValue) Then
                If dateValue >= startDate AndAlso dateValue <= endDate Then
                    If selectedId = "Все ID" OrElse row("ID").ToString().Trim().ToLower() = selectedId.Trim().ToLower() Then
                        filtered.ImportRow(row)
                    End If
                End If
            End If
        Next

        DataGridView1.DataSource = filtered
        MessageBox.Show("Отчёт сформирован.")
    End Sub

    Private Sub ButtonExport_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If DataGridView1.DataSource Is Nothing Then
            MessageBox.Show("Нет данных для экспорта.")
            Return
        End If

        Dim dt As DataTable = CType(DataGridView1.DataSource, DataTable)

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Excel Files|*.xlsx"
        sfd.Title = "Сохранить отчёт"

        If sfd.ShowDialog() = DialogResult.OK Then
            Dim totalTransactions As Integer = dt.Rows.Count
            Dim totalAmount As Decimal = 0
            Dim currencySums As New Dictionary(Of String, Decimal)
            Dim summaryPerIDCurrency As New Dictionary(Of String, Dictionary(Of String, Decimal))

            Dim amountColumns = dt.Columns.Cast(Of DataColumn)().
                                Where(Function(c) c.ColumnName.StartsWith("Сумма (")).
                                ToList()

            For Each row As DataRow In dt.Rows
                Dim idStr = row("ID").ToString()
                For Each col In amountColumns
                    Dim raw = row(col.ColumnName).ToString().Replace(" ", "").Replace(",", ".")
                    Dim value As Decimal
                    If Decimal.TryParse(raw, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, value) Then
                        totalAmount += value

                        If Not currencySums.ContainsKey(col.ColumnName) Then currencySums(col.ColumnName) = 0
                        currencySums(col.ColumnName) += value

                        If Not summaryPerIDCurrency.ContainsKey(idStr) Then
                            summaryPerIDCurrency(idStr) = New Dictionary(Of String, Decimal)
                        End If
                        If Not summaryPerIDCurrency(idStr).ContainsKey(col.ColumnName) Then
                            summaryPerIDCurrency(idStr)(col.ColumnName) = 0
                        End If
                        summaryPerIDCurrency(idStr)(col.ColumnName) += value
                    End If
                Next
            Next

            Using package As New ExcelPackage()
                Dim ws = package.Workbook.Worksheets.Add("Отчёт")

                ws.Cells(1, 1).Value = "Количество транзакций:"
                ws.Cells(1, 2).Value = totalTransactions

                ws.Cells(2, 1).Value = "Общая сумма по всем валютам:"
                ws.Cells(2, 2).Value = totalAmount

                ws.Cells(4, 1).Value = "Общая сумма по валютам:"
                Dim row = 5
                For Each kvp In currencySums
                    ws.Cells(row, 1).Value = kvp.Key
                    ws.Cells(row, 2).Value = kvp.Value
                    row += 1
                Next

                row += 1
                ws.Cells(row, 1).Value = "Сумма по каждому ID и валюте:"
                row += 1

                ws.Cells(row, 1).Value = "ID"
                Dim colIdx = 2
                For Each currency In amountColumns.Select(Function(c) c.ColumnName)
                    ws.Cells(row, colIdx).Value = currency
                    colIdx += 1
                Next
                row += 1

                For Each id In summaryPerIDCurrency.Keys
                    ws.Cells(row, 1).Value = id
                    colIdx = 2
                    For Each currency In amountColumns.Select(Function(c) c.ColumnName)
                        If summaryPerIDCurrency(id).ContainsKey(currency) Then
                            ws.Cells(row, colIdx).Value = summaryPerIDCurrency(id)(currency)
                        End If
                        colIdx += 1
                    Next
                    row += 1
                Next

                ws.Cells.AutoFitColumns()
                package.SaveAs(New FileInfo(sfd.FileName))
            End Using

            MessageBox.Show("Отчет успешно сохранен.")
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim prevForm As New Form7()
        prevForm.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim dt As DataTable = CType(DataGridView1.DataSource, DataTable)
        If dt Is Nothing Then Return

        Dim totalTransactions As Integer = dt.Rows.Count
        Dim totalAmount As Decimal = 0
        Dim currencySums As New Dictionary(Of String, Decimal)
        Dim summaryPerIDCurrency As New Dictionary(Of String, Dictionary(Of String, Decimal))

        Dim amountColumns = dt.Columns.Cast(Of DataColumn)().
                        Where(Function(c) c.ColumnName.StartsWith("Сумма (")).
                        ToList()

        For Each row As DataRow In dt.Rows
            Dim idStr = row("ID").ToString()
            For Each col In amountColumns
                Dim raw = row(col.ColumnName).ToString().Replace(" ", "").Replace(",", ".")
                Dim value As Decimal
                If Decimal.TryParse(raw, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, value) Then
                    totalAmount += value

                    If Not currencySums.ContainsKey(col.ColumnName) Then currencySums(col.ColumnName) = 0
                    currencySums(col.ColumnName) += value

                    If Not summaryPerIDCurrency.ContainsKey(idStr) Then
                        summaryPerIDCurrency(idStr) = New Dictionary(Of String, Decimal)
                    End If
                    If Not summaryPerIDCurrency(idStr).ContainsKey(col.ColumnName) Then
                        summaryPerIDCurrency(idStr)(col.ColumnName) = 0
                    End If
                    summaryPerIDCurrency(idStr)(col.ColumnName) += value
                End If
            Next
        Next

        ' Формирование текста отчета
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("Количество транзакций:" & vbTab & totalTransactions)
        sb.AppendLine("Общая сумма по всем валютам:" & vbTab & totalAmount.ToString("N2"))
        sb.AppendLine()
        sb.AppendLine("Общая сумма по валютам:")
        For Each kvp In currencySums
            sb.AppendLine(kvp.Key & vbTab & kvp.Value.ToString("N2"))
        Next

        sb.AppendLine()
        sb.AppendLine("Сумма по каждому ID и валюте:")
        Dim headers = "ID"
        For Each col In amountColumns
            headers &= vbTab & col.ColumnName
        Next
        sb.AppendLine(headers)

        For Each id In summaryPerIDCurrency.Keys
            Dim line = id
            For Each col In amountColumns
                Dim val As Decimal = 0
                If summaryPerIDCurrency(id).ContainsKey(col.ColumnName) Then
                    val = summaryPerIDCurrency(id)(col.ColumnName)
                End If
                line &= vbTab & val.ToString("N2")
            Next
            sb.AppendLine(line)
        Next

        Dim previewForm As New Form11()
        previewForm.ReportSummaryText = sb.ToString()
        previewForm.ShowDialog()
    End Sub
End Class



