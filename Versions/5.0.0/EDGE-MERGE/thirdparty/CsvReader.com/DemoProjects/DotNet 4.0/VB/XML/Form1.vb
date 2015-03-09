Imports System.IO
Imports System.Text
Imports System.Xml

Imports DataStreams.Csv
Imports DataStreams.Xml

Public Class Form1
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents DataGrid1 As System.Windows.Forms.DataGrid
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.DataGrid1 = New System.Windows.Forms.DataGrid()
        CType(Me.DataGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label3.Location = New System.Drawing.Point(347, 234)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 24)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "XML to DataGrid"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = ((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right)
        Me.TextBox2.BackColor = System.Drawing.Color.White
        Me.TextBox2.Location = New System.Drawing.Point(399, 26)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        Me.TextBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox2.Size = New System.Drawing.Size(384, 200)
        Me.TextBox2.TabIndex = 10
        Me.TextBox2.Text = ""
        Me.TextBox2.WordWrap = False
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(551, 2)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(72, 24)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "XML to CSV"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(183, 2)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 24)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "XML"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.White
        Me.TextBox1.Location = New System.Drawing.Point(7, 26)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox1.Size = New System.Drawing.Size(384, 200)
        Me.TextBox1.TabIndex = 7
        Me.TextBox1.Text = ""
        Me.TextBox1.WordWrap = False
        '
        'DataGrid1
        '
        Me.DataGrid1.Anchor = (((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right)
        Me.DataGrid1.DataMember = ""
        Me.DataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.DataGrid1.Location = New System.Drawing.Point(7, 258)
        Me.DataGrid1.Name = "DataGrid1"
        Me.DataGrid1.ReadOnly = True
        Me.DataGrid1.Size = New System.Drawing.Size(779, 312)
        Me.DataGrid1.TabIndex = 6
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(792, 573)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.Label3, Me.TextBox2, Me.Label2, Me.Label1, Me.TextBox1, Me.DataGrid1})
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.DataGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim readerXML As New StreamReader("../../../../sample data/products.xml", Encoding.UTF8)
        TextBox1.Text = readerXML.ReadToEnd()
        readerXML.Close()

        Dim readerGrid As New XmlRecordReader("../../../../sample data/products.xml", "products/product")

        readerGrid.Columns.Add("productID", "ProductID")
        readerGrid.Columns.Add("productName", "ProductName")
        readerGrid.Columns.Add("supplierID", "SupplierID")
        readerGrid.Columns.Add("categoryID", "CategoryID")
        readerGrid.Columns.Add("quantityPerUnit", "QuantityPerUnit")
        readerGrid.Columns.Add("unitPrice", "UnitPrice")
        readerGrid.Columns.Add("unitsInStock", "UnitsInStock")
        readerGrid.Columns.Add("unitsOnOrder", "UnitsOnOrder")
        readerGrid.Columns.Add("reorderLevel", "ReorderLevel")
        readerGrid.Columns.Add("discontinued", "Discontinued")

        DataGrid1.DataSource = readerGrid.ReadToEnd()

        readerGrid.Close()

        Dim result As New MemoryStream()
        Dim readerCSV As New XmlRecordReader("../../../../sample data/products.xml", "products/product")
        Dim writer As New CsvWriter(result, ","c, Encoding.Default)

        readerCSV.Columns.Add("productID", "ProductID")
        readerCSV.Columns.Add("productName", "ProductName")
        readerCSV.Columns.Add("supplierID", "SupplierID")
        readerCSV.Columns.Add("categoryID", "CategoryID")
        readerCSV.Columns.Add("quantityPerUnit", "QuantityPerUnit")
        readerCSV.Columns.Add("unitPrice", "UnitPrice")
        readerCSV.Columns.Add("unitsInStock", "UnitsInStock")
        readerCSV.Columns.Add("unitsOnOrder", "UnitsOnOrder")
        readerCSV.Columns.Add("reorderLevel", "ReorderLevel")
        readerCSV.Columns.Add("discontinued", "Discontinued")

        writer.WriteAll(readerCSV.ReadToEnd())
        writer.Close()
        readerCSV.Close()

        Dim resultReader As New StreamReader(New MemoryStream(result.GetBuffer()))
        TextBox2.Text = resultReader.ReadToEnd()
        resultReader.Close()

        result.Close()
    End Sub
End Class
