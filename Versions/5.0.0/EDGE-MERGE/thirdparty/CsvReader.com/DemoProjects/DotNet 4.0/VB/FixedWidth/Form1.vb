Imports System.IO
Imports System.Text
Imports System.Xml

Imports DataStreams.FixedWidth

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
        Me.Label3.Location = New System.Drawing.Point(328, 232)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(136, 24)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Fixed Width to DataGrid"
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
        Me.Label2.Location = New System.Drawing.Point(536, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(104, 24)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Fixed Width to XML"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(168, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 24)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Fixed Width"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBox1
        '
        Me.TextBox1.BackColor = System.Drawing.Color.White
        Me.TextBox1.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Dim reader As New StreamReader("../../../../sample data/products.txt", Encoding.Default)
        TextBox1.Text = reader.ReadToEnd()
        reader.Close()

        Dim readerGrid As New FixedWidthReader("../../../../sample data/products.txt")
        readerGrid.Columns.Add(5, "ProductID")
        readerGrid.Columns.Add(35, "ProductName")
        readerGrid.Columns.Add(5, "SupplierID")
        readerGrid.Columns.Add(5, "CategoryID")
        readerGrid.Columns.Add(20, "QuantityPerUnit")
        readerGrid.Columns.Add(10, "UnitPrice")
        readerGrid.Columns.Add(5, "UnitsInStock")
        readerGrid.Columns.Add(5, "UnitsOnOrder")
        readerGrid.Columns.Add(5, "ReorderLevel")
        readerGrid.Columns.Add(1, "Discontinued")

        readerGrid.Columns.Item("ProductID").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("ProductID").PaddingChar = "0"c
        readerGrid.Columns.Item("SupplierID").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("SupplierID").PaddingChar = "0"c
        readerGrid.Columns.Item("CategoryID").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("CategoryID").PaddingChar = "0"c
        readerGrid.Columns.Item("UnitPrice").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("UnitPrice").PaddingChar = "0"c
        readerGrid.Columns.Item("UnitPrice").MinLength = 1
        readerGrid.Columns.Item("UnitsInStock").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("UnitsInStock").PaddingChar = "0"c
        readerGrid.Columns.Item("UnitsInStock").MinLength = 1
        readerGrid.Columns.Item("UnitsOnOrder").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("UnitsOnOrder").PaddingChar = "0"c
        readerGrid.Columns.Item("UnitsOnOrder").MinLength = 1
        readerGrid.Columns.Item("ReorderLevel").Alignment = TextAlignment.Right
        readerGrid.Columns.Item("ReorderLevel").PaddingChar = "0"c
        readerGrid.Columns.Item("ReorderLevel").MinLength = 1

        DataGrid1.DataSource = readerGrid.ReadToEnd()
        readerGrid.Close()

        Dim result As New MemoryStream()
        Dim readerXML As New FixedWidthReader("../../../../sample data/products.txt")

        readerXML.Columns.Add(5, "ProductID")
        readerXML.Columns.Add(35, "ProductName")
        readerXML.Columns.Add(5, "SupplierID")
        readerXML.Columns.Add(5, "CategoryID")
        readerXML.Columns.Add(20, "QuantityPerUnit")
        readerXML.Columns.Add(10, "UnitPrice")
        readerXML.Columns.Add(5, "UnitsInStock")
        readerXML.Columns.Add(5, "UnitsOnOrder")
        readerXML.Columns.Add(5, "ReorderLevel")
        readerXML.Columns.Add(1, "Discontinued")

        readerXML.Columns.Item("ProductID").Alignment = TextAlignment.Right
        readerXML.Columns.Item("ProductID").PaddingChar = "0"c
        readerXML.Columns.Item("SupplierID").Alignment = TextAlignment.Right
        readerXML.Columns.Item("SupplierID").PaddingChar = "0"c
        readerXML.Columns.Item("CategoryID").Alignment = TextAlignment.Right
        readerXML.Columns.Item("CategoryID").PaddingChar = "0"c
        readerXML.Columns.Item("UnitPrice").Alignment = TextAlignment.Right
        readerXML.Columns.Item("UnitPrice").PaddingChar = "0"c
        readerXML.Columns.Item("UnitPrice").MinLength = 1
        readerXML.Columns.Item("UnitsInStock").Alignment = TextAlignment.Right
        readerXML.Columns.Item("UnitsInStock").PaddingChar = "0"c
        readerXML.Columns.Item("UnitsInStock").MinLength = 1
        readerXML.Columns.Item("UnitsOnOrder").Alignment = TextAlignment.Right
        readerXML.Columns.Item("UnitsOnOrder").PaddingChar = "0"c
        readerXML.Columns.Item("UnitsOnOrder").MinLength = 1
        readerXML.Columns.Item("ReorderLevel").Alignment = TextAlignment.Right
        readerXML.Columns.Item("ReorderLevel").PaddingChar = "0"c
        readerXML.Columns.Item("ReorderLevel").MinLength = 1

        Dim data As DataTable = readerXML.ReadToEnd()
        readerXML.Close()

        Dim writer As New XmlTextWriter(result, Encoding.UTF8)

        writer.Formatting = Formatting.Indented
        writer.IndentChar = CChar(vbTab)
        writer.Indentation = 1

        writer.WriteStartDocument()
        writer.WriteStartElement("products")

        Dim row As DataRow
        For Each row In data.Rows
            writer.WriteStartElement("product")

            Dim i As Integer
            For i = 0 To data.Columns.Count - 1
                writer.WriteElementString(data.Columns.Item(i).ColumnName.ToLower().Chars(0) + data.Columns.Item(i).ColumnName.Substring(1), row.Item(i).ToString())
            Next

            writer.WriteEndElement()
        Next

        writer.WriteEndElement()
        writer.WriteEndDocument()
        writer.Close()

        Dim resultReader As New StreamReader(New MemoryStream(result.GetBuffer()))
        TextBox2.Text = resultReader.ReadToEnd()
        resultReader.Close()

        result.Close()
    End Sub
End Class
