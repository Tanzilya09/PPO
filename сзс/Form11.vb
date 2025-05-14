Public Class Form11
    Public Property ReportSummaryText As String
    Private Sub Form11_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RichTextBox1.Text = ReportSummaryText
    End Sub

End Class