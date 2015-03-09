using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using DataStreams.Csv;
using DataStreams.Xml;

namespace DemoXML
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.White;
			this.textBox1.Location = new System.Drawing.Point(8, 26);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(384, 200);
			this.textBox1.TabIndex = 10;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox2.BackColor = System.Drawing.Color.White;
			this.textBox2.Location = new System.Drawing.Point(400, 26);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox2.Size = new System.Drawing.Size(384, 200);
			this.textBox2.TabIndex = 7;
			this.textBox2.Text = "";
			this.textBox2.WordWrap = false;
			// 
			// dataGrid1
			// 
			this.dataGrid1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(8, 258);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.Size = new System.Drawing.Size(776, 312);
			this.dataGrid1.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(184, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 24);
			this.label1.TabIndex = 11;
			this.label1.Text = "XML";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(552, 2);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 24);
			this.label2.TabIndex = 9;
			this.label2.Text = "XML to CSV";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label3.Location = new System.Drawing.Point(352, 234);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 24);
			this.label3.TabIndex = 8;
			this.label3.Text = "XML to DataGrid";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(792, 573);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBox1,
																		  this.textBox2,
																		  this.dataGrid1,
																		  this.label1,
																		  this.label2,
																		  this.label3});
			this.Name = "Form1";
			this.Text = "Form1";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			using (StreamReader reader = new StreamReader("../../../../../sample data/products.xml", Encoding.UTF8))
			{
				textBox1.Text = reader.ReadToEnd();
			}

			using (XmlRecordReader reader = new XmlRecordReader("../../../../../sample data/products.xml", "products/product"))
			{
				reader.Columns.Add("productID", "ProductID");
				reader.Columns.Add("productName", "ProductName");
				reader.Columns.Add("supplierID", "SupplierID");
				reader.Columns.Add("categoryID", "CategoryID");
				reader.Columns.Add("quantityPerUnit", "QuantityPerUnit");
				reader.Columns.Add("unitPrice", "UnitPrice");
				reader.Columns.Add("unitsInStock", "UnitsInStock");
				reader.Columns.Add("unitsOnOrder", "UnitsOnOrder");
				reader.Columns.Add("reorderLevel", "ReorderLevel");
				reader.Columns.Add("discontinued", "Discontinued");

				dataGrid1.DataSource = reader.ReadToEnd();
			}

			using (MemoryStream result = new MemoryStream())
			{
				using (XmlRecordReader reader = new XmlRecordReader("../../../../../sample data/products.xml", "products/product"))
				using (CsvWriter writer = new CsvWriter(result, ',', Encoding.Default))
				{
					reader.Columns.Add("productID", "ProductID");
					reader.Columns.Add("productName", "ProductName");
					reader.Columns.Add("supplierID", "SupplierID");
					reader.Columns.Add("categoryID", "CategoryID");
					reader.Columns.Add("quantityPerUnit", "QuantityPerUnit");
					reader.Columns.Add("unitPrice", "UnitPrice");
					reader.Columns.Add("unitsInStock", "UnitsInStock");
					reader.Columns.Add("unitsOnOrder", "UnitsOnOrder");
					reader.Columns.Add("reorderLevel", "ReorderLevel");
					reader.Columns.Add("discontinued", "Discontinued");

					writer.WriteAll(reader.ReadToEnd());
				}

				using (StreamReader resultReader = new StreamReader(new MemoryStream(result.GetBuffer())))
				{
					textBox2.Text = resultReader.ReadToEnd();
				}
			}
		}
	}
}
