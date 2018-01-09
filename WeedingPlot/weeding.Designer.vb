<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class weeding
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Group_Layerlist = New System.Windows.Forms.GroupBox()
        Me.BtnAssign = New System.Windows.Forms.Button()
        Me.BtnClear = New System.Windows.Forms.Button()
        Me.BtnPlot = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker()
        Me.group1 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Btn_Deselect = New System.Windows.Forms.Button()
        Me.BtnSel = New System.Windows.Forms.Button()
        Me.box2 = New System.Windows.Forms.ComboBox()
        Me.box1 = New System.Windows.Forms.ComboBox()
        Me.Group_Layerlist.SuspendLayout()
        Me.group1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Group_Layerlist
        '
        Me.Group_Layerlist.Controls.Add(Me.BtnAssign)
        Me.Group_Layerlist.Controls.Add(Me.BtnClear)
        Me.Group_Layerlist.Controls.Add(Me.BtnPlot)
        Me.Group_Layerlist.Controls.Add(Me.ComboBox1)
        Me.Group_Layerlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Group_Layerlist.ForeColor = System.Drawing.Color.Black
        Me.Group_Layerlist.Location = New System.Drawing.Point(3, 3)
        Me.Group_Layerlist.Name = "Group_Layerlist"
        Me.Group_Layerlist.Size = New System.Drawing.Size(227, 113)
        Me.Group_Layerlist.TabIndex = 0
        Me.Group_Layerlist.TabStop = False
        '
        'BtnAssign
        '
        Me.BtnAssign.BackColor = System.Drawing.Color.Silver
        Me.BtnAssign.FlatAppearance.BorderColor = System.Drawing.Color.Gray
        Me.BtnAssign.FlatAppearance.BorderSize = 0
        Me.BtnAssign.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnAssign.Font = New System.Drawing.Font("Open Sans", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnAssign.ForeColor = System.Drawing.Color.Black
        Me.BtnAssign.Location = New System.Drawing.Point(6, 19)
        Me.BtnAssign.Name = "BtnAssign"
        Me.BtnAssign.Size = New System.Drawing.Size(131, 24)
        Me.BtnAssign.TabIndex = 4
        Me.BtnAssign.Text = "Assign Coord. Sys"
        Me.BtnAssign.UseVisualStyleBackColor = False
        '
        'BtnClear
        '
        Me.BtnClear.BackColor = System.Drawing.Color.Silver
        Me.BtnClear.FlatAppearance.BorderColor = System.Drawing.Color.Gray
        Me.BtnClear.FlatAppearance.BorderSize = 0
        Me.BtnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnClear.Font = New System.Drawing.Font("Open Sans", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnClear.Location = New System.Drawing.Point(143, 85)
        Me.BtnClear.Name = "BtnClear"
        Me.BtnClear.Size = New System.Drawing.Size(74, 22)
        Me.BtnClear.TabIndex = 3
        Me.BtnClear.Text = "Clear Layers"
        Me.BtnClear.UseVisualStyleBackColor = False
        '
        'BtnPlot
        '
        Me.BtnPlot.BackColor = System.Drawing.Color.Silver
        Me.BtnPlot.FlatAppearance.BorderColor = System.Drawing.Color.Gray
        Me.BtnPlot.FlatAppearance.BorderSize = 0
        Me.BtnPlot.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPlot.Font = New System.Drawing.Font("Open Sans", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnPlot.Location = New System.Drawing.Point(143, 49)
        Me.BtnPlot.Name = "BtnPlot"
        Me.BtnPlot.Size = New System.Drawing.Size(74, 22)
        Me.BtnPlot.TabIndex = 2
        Me.BtnPlot.Text = "QC Plot"
        Me.BtnPlot.UseVisualStyleBackColor = False
        '
        'ComboBox1
        '
        Me.ComboBox1.BackColor = System.Drawing.Color.White
        Me.ComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(6, 49)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(131, 21)
        Me.ComboBox1.TabIndex = 0
        '
        'group1
        '
        Me.group1.Controls.Add(Me.Label2)
        Me.group1.Controls.Add(Me.Btn_Deselect)
        Me.group1.Controls.Add(Me.BtnSel)
        Me.group1.Controls.Add(Me.box2)
        Me.group1.Controls.Add(Me.box1)
        Me.group1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.group1.Font = New System.Drawing.Font("Open Sans", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.group1.ForeColor = System.Drawing.Color.Black
        Me.group1.Location = New System.Drawing.Point(3, 122)
        Me.group1.Name = "group1"
        Me.group1.Size = New System.Drawing.Size(227, 123)
        Me.group1.TabIndex = 4
        Me.group1.TabStop = False
        Me.group1.Text = "QC Weeding Plot Selection"
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.White
        Me.Label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(6, 92)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(213, 22)
        Me.Label2.TabIndex = 7
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Btn_Deselect
        '
        Me.Btn_Deselect.BackColor = System.Drawing.Color.Silver
        Me.Btn_Deselect.FlatAppearance.BorderColor = System.Drawing.Color.Gray
        Me.Btn_Deselect.FlatAppearance.BorderSize = 0
        Me.Btn_Deselect.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Btn_Deselect.Font = New System.Drawing.Font("Open Sans", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Btn_Deselect.Location = New System.Drawing.Point(144, 57)
        Me.Btn_Deselect.Name = "Btn_Deselect"
        Me.Btn_Deselect.Size = New System.Drawing.Size(75, 23)
        Me.Btn_Deselect.TabIndex = 6
        Me.Btn_Deselect.Text = "Deselect"
        Me.Btn_Deselect.UseVisualStyleBackColor = False
        '
        'BtnSel
        '
        Me.BtnSel.BackColor = System.Drawing.Color.Silver
        Me.BtnSel.FlatAppearance.BorderColor = System.Drawing.Color.Gray
        Me.BtnSel.FlatAppearance.BorderSize = 0
        Me.BtnSel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnSel.Font = New System.Drawing.Font("Open Sans", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnSel.Location = New System.Drawing.Point(144, 28)
        Me.BtnSel.Name = "BtnSel"
        Me.BtnSel.Size = New System.Drawing.Size(75, 23)
        Me.BtnSel.TabIndex = 3
        Me.BtnSel.Text = "Select"
        Me.BtnSel.UseVisualStyleBackColor = False
        '
        'box2
        '
        Me.box2.BackColor = System.Drawing.Color.White
        Me.box2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.box2.FormattingEnabled = True
        Me.box2.Location = New System.Drawing.Point(6, 55)
        Me.box2.Name = "box2"
        Me.box2.Size = New System.Drawing.Size(127, 23)
        Me.box2.TabIndex = 2
        '
        'box1
        '
        Me.box1.BackColor = System.Drawing.Color.White
        Me.box1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.box1.FormattingEnabled = True
        Me.box1.Location = New System.Drawing.Point(6, 28)
        Me.box1.Name = "box1"
        Me.box1.Size = New System.Drawing.Size(127, 23)
        Me.box1.TabIndex = 0
        '
        'weeding
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Controls.Add(Me.group1)
        Me.Controls.Add(Me.Group_Layerlist)
        Me.Name = "weeding"
        Me.Size = New System.Drawing.Size(235, 253)
        Me.Group_Layerlist.ResumeLayout(False)
        Me.group1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Group_Layerlist As Windows.Forms.GroupBox
    Friend WithEvents BackgroundWorker1 As ComponentModel.BackgroundWorker
    Friend WithEvents ComboBox1 As Windows.Forms.ComboBox
    Friend WithEvents BtnPlot As Windows.Forms.Button
    Friend WithEvents BackgroundWorker2 As ComponentModel.BackgroundWorker
    Friend WithEvents BtnClear As Windows.Forms.Button
    Friend WithEvents group1 As Windows.Forms.GroupBox
    Friend WithEvents BtnSel As Windows.Forms.Button
    Friend WithEvents box2 As Windows.Forms.ComboBox
    Friend WithEvents box1 As Windows.Forms.ComboBox
    Friend WithEvents Btn_Deselect As Windows.Forms.Button
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents BtnAssign As Windows.Forms.Button
End Class
