Option Explicit On
Imports Mfd = Manifold.Interop
Imports System.Drawing
Imports System.Xml


Public Class weeding
    Implements Mfd.Scripts.IEventsConnection
    Private App As Mfd.Application
    Private Doc As Mfd.Document
    Private Comps As Mfd.ComponentSet
    Private CmpObjs As Mfd.ObjectSet
    Dim win As Mfd.WindowSet

    Public Shared m_Document As String = "T:\geodata\Drone\XML Templates\Layout\mfd_temp_layout.xml"

    Public Sub ConnectEvents(ByVal ev As Manifold.Interop.Scripts.Events) _
        Implements Manifold.Interop.Scripts.IEventsConnection.ConnectEvents

        ' ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        '    Possible Manifold.Interop.Script.Events
        ' ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ' 
        ' AddinLoaded                     WindowActivated
        ' 
        ' ComponentsAdded                 DocumentClosed
        ' ComponentsRemoved	              DocumentCreated
        ' ComponentDataChanged            DocumentOpened
        ' ComponentNameChanged            DocumentSaved
        ' ComponentProjectionChanged 
        ' ComponentSelectionChanged  
        ' ComponentStateChanged

        ' ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        AddHandler ev.AddinLoaded, AddressOf Document_Changed

        AddHandler ev.DocumentClosed, AddressOf Document_Changed
        AddHandler ev.DocumentCreated, AddressOf Document_Changed
        AddHandler ev.DocumentOpened, AddressOf Document_Changed
        AddHandler ev.DocumentSaved, AddressOf Document_Changed
        AddHandler ev.ComponentsAdded, AddressOf Document_Changed
        AddHandler ev.ComponentsRemoved, AddressOf Document_Changed

        AddHandler ev.ComponentNameChanged, AddressOf Component_Changed
        AddHandler ev.ComponentProjectionChanged, AddressOf Component_Changed

    End Sub

    Private Sub Document_Changed(
      ByVal Sender As System.Object,
      ByVal Args As Manifold.Interop.Scripts.DocumentEventArgs)
        Try
            Initialze_Form()
            ' Option Strict requires casting below
            App = CType(Args.Document.Application, Manifold.Interop.Application)
            Doc = CType(App.ActiveDocument, Manifold.Interop.Document)
            Comps = CType(Doc.ComponentSet, Manifold.Interop.ComponentSet)
            If App Is Nothing Then Exit Sub
            If Doc Is Nothing Then Exit Sub
            Load_ListBoxes()
        Catch ex As Exception
            If Err.Number = 91 Or Err.Number = -2147352567 Then
                ' Ignore Error 91:
                '     Object reference not set to an instance of an object.
                ' Ignore Error -2147352567: 
                '     Object is Deleted - generally occurs if the Add-In
                '     is active and you close the project. I think I have
                '     this fixed and it probably won't show up anyway.
            Else
                Error_DisplayAndClear(ex, "Document_Changed")
            End If
        End Try
    End Sub

    Private Sub Component_Changed(
   ByVal sender As System.Object,
   ByVal args As Manifold.Interop.Scripts.ComponentEventArgs)
        Try
            Initialze_Form()
            If App Is Nothing Then Exit Sub
            If Doc Is Nothing Then Exit Sub
            Load_ListBoxes()
        Catch ex As Exception
            If Err.Number = 91 Or Err.Number = -2147352567 Then
                ' Ignore Error 91:
                '     Object reference not set to an instance of an object.
                ' Ignore Error -2147352567: 
                '     Object is Deleted - generally occurs if the Add-In
                '     is active and you close the project. I think I have
                '     this fixed and it probably won't show up anyway.
            Else
                Error_DisplayAndClear(ex, "Component_Changed")
            End If
        End Try
    End Sub

    Private Sub Initialze_Form()
        Me.ComboBox1.Items.Clear()
        Me.ComboBox1.Text = "Choose GPS Data"
        Group_Layerlist.Enabled = False
        BtnPlot.Enabled = False

        Me.box1.Items.Clear()
        Me.box1.Text = "Select Plot No"
        BtnSel.Enabled = False

        Me.box2.Items.Clear()
        Me.box2.Text = "Compartment No"
        group1.Enabled = False
    End Sub

    Private Sub Load_ListBoxes()

        If Doc Is Nothing Then Exit Sub

        Dim Comp As Mfd.Component

        ComboBox1.Items.Clear()

        For Each Comp In Comps
            If Comp.Type = Mfd.ComponentType.ComponentDrawing Then
                ComboBox1.Items.Add(Comp.Name)
            End If
        Next
        If ComboBox1.Items.Count = 0 Then Exit Sub

        Group_Layerlist.Enabled = True
        BtnPlot.Enabled = True

        box1.Items.Clear()

        Dim draw = Comps(Comps.ItemByName("Survey Plot"))
        If draw Is Nothing Then Exit Sub
        Dim objs = draw.ObjectSet
        Dim table = draw.OwnedTable
        Dim recs = table.RecordSet
        Dim cols = table.ColumnSet
        Dim colname = "Station"
        For Each rec In recs
            box1.Items.Add(rec.Data(colname))
        Next

        group1.Enabled = True
        If box1.Items.Count = 0 Then Exit Sub
        BtnSel.Enabled = True

    End Sub

    Private Sub BtnPlot_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPlot.Click
        Dim input = ComboBox1.SelectedItem
        If input Is Nothing Then Exit Sub
        'check object type
        Dim objs = Doc.ComponentSet(input).ObjectSet
        Dim objtype = objs.Item(0)
        If objtype.Type = Mfd.ObjectType.ObjectPoint Then
        Else
            MsgBox("'" & input & "' drawing object is not a point.Retry!", MsgBoxStyle.RetryCancel, "WARNING")
            Exit Sub
        End If
        checkGPS()
        plotlines()
        surveydistance()
        surveyplot()
        surveyplotselect()
        deletelines()
        createmap()
        createXML()
        createlayout()
    End Sub

    Private Sub BtnAssign_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnAssign.Click
        Assign_CoordSys("Universal Transverse Mercator - Zone 49 (N)", "World Geodetic 1984 (WGS84)", Mfd.ComponentType.ComponentDrawing)
    End Sub

    Private Sub BtnSel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnSel.Click

        Dim staNo = box1.SelectedItem
        If staNo < 0 Then Exit Sub
        If staNo = 0 Then Exit Sub

        Label2.Text = "Plot no. " & staNo & ""

        Dim q = Doc.NewQuery("Temp")
        q.Text = "UPDATE [Survey Plot Table] SET [Selection (I)] = FALSE;"
        q.Run()
        Comps.Remove(q)
        Dim q2 = Doc.NewQuery("Temp")
        q2.Text = "UPDATE [Survey Plot Table] SET [Selection (I)] = TRUE WHERE [Station] =" & staNo & ""
        q2.Run()
        Comps.Remove(q2)

        Dim dataidx = Comps.ItemByName("Survey Plot Selection")
        Dim data = Comps.Item(dataidx)
        Dim dataobjects = data.objectset
        Dim datatable = data.OwnedTable
        Dim recs2 = datatable.RecordSet
        For x = 0 To dataobjects.Count - 1
            dataobjects.Remove(0)
        Next
        Dim s = Comps(Comps.ItemByName("Survey Plot"))
        Dim objs = s.Selection
        Dim recs1 = s.OwnedTable.Recordset
        Dim Objgeoms = objs.geomset
        Dim geom = Objgeoms.Item(0)
        dataobjects.Add(geom)

        If existsComponent("Selection Buffer") Then
            Dim buffidx = Comps.ItemByName("Selection Buffer")
            Dim buffdata = Comps.Item(buffidx)
            Dim buffObjs = buffdata.ObjectSet
            Dim bufftable = buffdata.OwnedTable
            Dim buffrecs = bufftable.RecordSet
            For x = 0 To buffObjs.Count - 1
                buffObjs.Remove(0)
            Next

            Dim q3 = Doc.NewQuery("temp")
            q3.Text = "INSERT INTO [Selection Buffer] ([Geom (I)])" &
                       "SELECT Buffer([Geom (I)], 10)" &
                       "FROM [Survey Plot Selection] WHERE IsArea([ID]);"
            q3.Run()
            Comps.Remove(q3)
        End If


        Dim drwidx = Comps.ItemByName("Survey Plot")
        Dim drw = Comps.Item(drwidx)

        Dim lyt = Comps("Survey Plot Map Layout")
        Dim LEntrySet = lyt.EntrySet
        For i = 0 To (LEntrySet.Count - 1)
            Dim LEntry = LEntrySet.Item(i)
            If LEntry.Type = 4 Then
                If (StrComp("Surv", Strings.Left(LEntry.Text, 4)) = 0) Then
                    LEntry.Text = "Survey Plot No. " & staNo
                End If
            End If
        Next
        lyt.Open()
    End Sub

    Private Sub Btn_Deselect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btn_Deselect.Click
        Dim q = Doc.NewQuery("Temp")
        q.Text = "UPDATE [Survey Plot] SET [Selection (I)] = FALSE;"
        q.Run()
        Comps.Remove(q)
        Dim dataidx = Comps.ItemByName("Survey Plot Selection")
        Dim data = Comps.Item(dataidx)
        Dim dataobjects = data.objectset
        Dim datatable = data.OwnedTable
        Dim recs2 = datatable.RecordSet
        For x = 0 To dataobjects.Count - 1
            dataobjects.Remove(0)
        Next
    End Sub

    Private Sub checkGPS()
        Dim input = ComboBox1.SelectedItem
        Dim objsSource = Comps(input).objectset

        If existsComponent("GPS") Then

        Else

            If existsComponent(input) Then
                Dim draw = Doc.NewDrawing("GPS", Comps(input).coordinateSystem)
                Dim objsTarget = Comps("GPS").objectset
                Dim colsSource = Comps(input).OwnedTable.ColumnSet
                Dim colsTarget = Comps("GPS").OwnedTable.ColumnSet
                Dim newcol = colsTarget.NewColumn
                newcol.Name = "NAME"
                newcol.Type = Mfd.ColumnType.ColumnTypeWText
                colsTarget.Add(newcol)
                Dim recs1 = Comps(input).ownedtable.recordset
                Dim recs2 = Comps("GPS").ownedtable.recordset
                Doc.BatchUpdates = True
                For Each obj In objsSource
                    Dim ge = obj.Geom
                    objsTarget.Add(ge)
                    Dim rec1 = recs1(recs1.itembyID(obj.ID))
                    Dim rec2 = recs2(recs2.itembyID(objsTarget.LastAdded.ID))
                    rec2.Data("NAME") = rec1.Data("NAME")
                Next
                Doc.BatchUpdates = False
                Dim cmp = Doc.ComponentSet("GPS")
                With cmp
                    .PointSize.defaultvalue.Formatting = 6
                    .PointStyle.defaultvalue.Formatting = 6
                    .PointBackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
                    .Pointforeground.LoadFrom("<theme><type>color</type><colorDef>#6495ED</colorDef></theme>") 'blue
                End With

                'change NAME to Station
                Dim colidx = colsTarget.Itembyname("NAME")
                If colidx = 1 Then
                    colsTarget(colidx).Type = Mfd.ColumnType.ColumnTypeInt32
                    colsTarget(colidx).Name = "Station"
                Else
                End If
                Dim q = Doc.NewQuery("checkGPS")
                q.Text = "DELETE FROM [GPS] WHERE [Station] = 0"
                q.Run()
                Comps.Remove(q)
                Dim lblgps = Doc.NewLabels("GPS Lables", cmp)
                lblgps.Text = "[Station]"
                Dim lbl = Comps("GPS Lables")
                With lbl
                    .LabelSize.defaultvalue.Formatting = 15
                    .LabelStyle.defaultvalue.Formatting = 6
                    .Synchronized = False
                    .Synchronized = True
                    .perlabelformat = True
                    .labelalignx = 0
                    .labelaligny = 0
                    .fontface = "Georgia"
                    .bold = True
                    .zoomrender = 400
                    .labelbackground.LoadFrom("<theme><type>color</type><colorDef>#FFFFFF</colorDef></theme>") 'white
                End With
            End If
        End If
    End Sub

    Private Sub plotlines()
        Dim compindex = Comps.ItemByName("GPS")
        Dim drawcomp = Comps.Item(compindex)
        Dim drawtable = drawcomp.OwnedTable
        If drawcomp.Type <> Mfd.ComponentType.ComponentDrawing Then
            Exit Sub
        End If
        Dim objects = drawcomp.ObjectSet
        Dim x = 0
        Do While x < objects.count
            Dim obj = objects.Item(x)
            If obj.Type = Mfd.ObjectType.ObjectLine Then
                objects.Remove(x)
                x = x - 1
            End If
            x = x + 1
        Loop
        Dim columns = drawtable.columnset
        Dim columnidx = columns.ItemByName("Bearing")
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("Distance")
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("Width") 'experimental
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("Height") 'experimental
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("X")
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("Y")
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If
        columnidx = columns.ItemByName("Remarks")
        If columnidx > -1 Then
            columns.Remove(columnidx)
        End If

        Dim column = columns.NewColumn()
        column.Name = "Bearing"
        column.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column)
        Dim column2 = columns.NewColumn()
        column2.Name = "Distance"
        column2.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column2)
        Dim column3 = columns.NewColumn()
        column3.Name = "Width"
        column3.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column3)
        Dim column4 = columns.NewColumn()
        column4.Name = "Height"
        column4.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column4)
        Dim column5 = columns.NewColumn()
        column5.Name = "X"
        column5.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column5)
        Dim column6 = columns.NewColumn()
        column6.Name = "Y"
        column6.Type = Mfd.ColumnType.ColumnTypeFloat64
        columns.Add(column6)
        Dim column7 = columns.NewColumn()
        column7.Name = "Remarks"
        column7.Type = Mfd.ColumnType.ColumnTypeAText
        columns.Add(column7)


        'min val
        Dim q1 = Doc.NewQuery("QMinStation")
        q1.Text = "SELECT Min(Station) AS MINSTATION FROM [" + drawcomp.Name + "]"
        Dim tab1 = q1.Table
        Dim q1records = tab1.RecordSet
        Dim q1record = q1records(0)
        Dim minvalue = q1record.Data("MINSTATION")
        Comps.Remove(Comps.ItemByID(q1.ID))

        'max val
        Dim q2 = Doc.NewQuery("QMaxStation")
        q2.Text = "SELECT Max(Station) AS MAXSTATION FROM [" + drawcomp.Name + "]"
        Dim tab2 = q2.Table
        Dim q2records = tab2.RecordSet
        Dim q2record = q2records(0)
        Dim maxvalue = q2record.Data("MAXSTATION")
        Comps.Remove(Comps.ItemByID(q2.ID))

        Dim table = drawcomp.OwnedTable
        Doc.BatchUpdates = True
        'create station pair and line between them
        For x = minvalue To (maxvalue - 1)
            Dim records = table.RecordSet
            Dim pointset = App.NewPointSet
            'first station point
            Dim recordIndex1 = records.Itembyvalue("Station", x)
            Dim record = records(recordIndex1)
            Dim point = App.NewPoint
            point.X = CDbl(record.Data("X (I)"))
            point.Y = CDbl(record.Data("Y (I)"))
            pointset.Add(point)
            Dim startstation = record.Data("Station")
            Dim startid = record.Data("ID")

            'second station point
            Dim recordIndex2 = records.Itembyvalue("Station", x + 1)
            Dim record2 = records(recordIndex2)
            Dim point2 = App.NewPoint
            point2.X = CDbl(record2.Data("X (I)"))
            point2.Y = CDbl(record2.Data("Y (I)"))
            pointset.Add(point2)

            'Create a new line in between 1st and 2nd point
            Dim itemindex = 0
            Dim p1 = pointset.Item(itemindex)
            Dim p2 = pointset.Item(itemindex + 1)
            Dim geom = App.NewGeom(Mfd.GeomType.GeomLine, pointset)
            objects.Add(geom)

            'station no for each line
            Dim objAdd = objects.Lastadded
            Dim recAdd = objAdd.Record
            recAdd.Data("Station") = startstation
            Dim tobearing = recAdd.Data("Bearing (I)")
            Dim todistance = recAdd.Data("Length (I)")
            recAdd.Data("Remarks") = ("line")

            'allocate bearing and distance from the traverse to the start station
            Dim recId = records.ItemByID(startid)
            Dim traverserec = records.Item(recId)
            record.Data("Bearing") = tobearing
            record.Data("Distance") = todistance
            record.Data("Y") = Math.Round(traverserec.Data("X (I)"), 2)
            record.Data("X") = Math.Round(traverserec.Data("Y (I)"), 2)
            record.Data("Remarks") = ("point")

            'for maxvalue
            Dim recIndex = records.ItemByValue("Station", maxvalue)
            Dim record5 = records(recIndex)
            Dim p = App.NewPoint
            p.X = CDbl(record5.Data("X (I)"))
            p.Y = CDbl(record5.Data("Y (I)"))
            pointset.Add(p)
            Dim id = record5.Data("ID")
            Dim recidx = records.ItemByID(id)
            Dim rec = records.Item(recidx)
            rec.Data("Y") = Math.Round(record5.Data("X (I)"), 2)
            rec.Data("X") = Math.Round(record5.Data("Y (I)"), 2)
            rec.Data("Remarks") = ("point")
        Next
        Doc.BatchUpdates = False
        Dim q3 = Doc.NewQuery("temp")
        q3.Text = "UPDATE [GPS] SET [Width]=5,[Height]=62.5,[Distance]=62.5;" '5,62.5,62.5
        q3.Run()
        Comps.Remove(q3)
    End Sub

    Private Sub surveydistance()
        Dim drawidx = Comps.ItemByName("Survey Distance")
        If drawidx < 0 Then
            Dim dist = Doc.NewDrawing("Survey Distance", Comps("GPS").coordinateSystem)
            Dim columns = dist.OwnedTable.ColumnSet
            Dim objects = dist.ObjectSet
            Dim column1 = columns.NewColumn()
            column1.Name = "Station"
            column1.Type = Mfd.ColumnType.ColumnTypeInt32
            columns.Add(column1)
            Dim column2 = columns.NewColumn()
            column2.Name = "Bearing"
            column2.Type = Mfd.ColumnType.ColumnTypeFloat64
            columns.Add(column2)
            Dim column3 = columns.NewColumn()
            column3.Name = "Distance"
            column3.Type = Mfd.ColumnType.ColumnTypeFloat64
            columns.Add(column3)
            Dim column4 = columns.NewColumn()
            column4.Name = "Width"
            column4.Type = Mfd.ColumnType.ColumnTypeFloat64
            columns.Add(column4)
            Dim column5 = columns.NewColumn()
            column5.Name = "Height"
            column5.Type = Mfd.ColumnType.ColumnTypeFloat64
            columns.Add(column5)
            Dim q = Doc.NewQuery("temp")
            q.Text = "Insert into [Survey Distance]([Geom (I)],[Station],[Bearing],[Distance],[Width],[Height])" &
                "Select AssignCoordSys(NewLine(NewPoint([X], [Y]), NewPoint([X1], [Y1]))," &
                "Coordsys(""Universal Transverse Mercator - Zone 49 (N)""))as geometry," &
                "[Station],[Bearing],[Distance],[Width],[Height] From" &
                "(Select [X (I)] AS [X], [Y (I)] AS [Y], [Station],[Bearing],[Distance],[Width],[Height]," &
                "(Sin(CDbl([Bearing])*3.141592653589793/180)*[Distance])+[X (I)] AS [X1]," &
                "(Cos(CDbl([Bearing])*3.141592653589793/180)*[Distance])+[Y (I)] AS [Y1]" &
                "From [GPS] WHERE [Remarks] <> ""line"")"
            q.Run
            Comps.Remove(q)
            Dim dval = Comps("Survey Distance").LineSize.defaultvalue
            dval.Formatting = 2
            Dim dval2 = Comps("Survey Distance").LineStyle.defaultvalue
            dval2.Formatting = 60
        Else
            Dim data = Comps.Item(drawidx)
            Dim dataobjs = data.objectset
            For x = 0 To dataobjs.Count - 1
                dataobjs.Remove(0)
            Next
            Dim q2 = Doc.NewQuery("temp")
            q2.Text = "Insert into [Survey Distance]([Geom (I)],[Station],[Bearing],[Distance],[Width],[Height])" &
                "Select AssignCoordSys(NewLine(NewPoint([X], [Y]), NewPoint([X1], [Y1]))," &
                "Coordsys(""Universal Transverse Mercator - Zone 49 (N)""))as geometry," &
                "[Station],[Bearing],[Distance],[Width],[Height] From" &
                "(Select [X (I)] AS [X], [Y (I)] AS [Y], [Station],[Bearing],[Distance],[Width],[Height]," &
                "(Sin(CDbl([Bearing])*3.141592653589793/180)*[Distance])+[X (I)] AS [X1]," &
                "(Cos(CDbl([Bearing])*3.141592653589793/180)*[Distance])+[Y (I)] AS [Y1]" &
                "From [GPS] WHERE [Remarks] <> ""line"")"
            q2.Run()
            Comps.Remove(q2)
        End If
    End Sub

    Private Sub surveyplot()
        Dim drawidx = Comps.ItemByName("Survey Plot")
        If drawidx < 0 Then
            Dim plot = Doc.NewDrawing("Survey Plot", Comps("GPS").coordinateSystem)
            Dim columns = plot.OwnedTable.ColumnSet
            Dim objects = plot.ObjectSet
            Dim column = columns.NewColumn()
            column.Name = "Station"
            column.Type = Mfd.ColumnType.ColumnTypeInt32
            columns.Add(column)
            Dim q = Doc.NewQuery("temp")
            q.Text = "INSERT INTO [Survey Plot]([Geom (I)],[Station])" &
                "SELECT AssignCoordSys(Rotate(BoundingBox(AllBranches(NewPoint" &
                "([X (I)] + [Width] * [Direction] / 2 , [Y (I)] + [Height] * [Direction] / 2 ))),First([Bearing]))," &
                "COORDSYS (""Universal Transverse Mercator - Zone 49 (N)"")) as geometry,[Station]" &
                "FROM [Survey Distance] CROSS JOIN(VALUES (-1), (1) NAMES ([Direction])) GROUP BY [Station]"
            q.Run
            Comps.Remove(q)
            Dim dval1 = Comps("Survey Plot").AreaSize.defaultvalue
            dval1.Formatting = 1
            Dim dval2 = Comps("Survey Plot").AreaStyle.defaultvalue
            dval2.Formatting = 0
            Dim dval3 = Comps("Survey Plot").Areaborderstyle.defaultvalue
            dval3.Formatting = 75
            Dim dval4 = Comps("Survey Plot").Areabordersize.defaultvalue
            dval4.Formatting = 2

            'color
            Dim draw = Doc.ComponentSet("Survey Plot")
            draw.Areabackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
            draw.Areaborderbackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
            draw.Areaborderforeground.LoadFrom("<theme><type>color</type><colorDef>#FF0000</colorDef></theme>") 'red

        Else
            Dim data = Comps.Item(drawidx)
            Dim dataobjs = data.objectset
            For x = 0 To dataobjs.Count - 1
                dataobjs.Remove(0)
            Next
            Dim q = Doc.NewQuery("temp")
            q.Text = "INSERT INTO [Survey Plot]([Geom (I)],[Station])" &
                "SELECT AssignCoordSys(Rotate(BoundingBox(AllBranches(NewPoint" &
                "([X (I)] + [Width] * [Direction] / 2 , [Y (I)] + [Height] * [Direction] / 2 ))),First([Bearing]))," &
                "COORDSYS (""Universal Transverse Mercator - Zone 49 (N)"")) as geometry,[Station]" &
                "FROM [Survey Distance] CROSS JOIN(VALUES (-1), (1) NAMES ([Direction])) GROUP BY [Station]"
            q.Run
            Comps.Remove(q)
        End If
    End Sub

    Private Sub surveyplotselect()
        Dim drawidx = Comps.ItemByName("Survey Plot Selection")
        If drawidx < 0 Then
            Dim s = Doc.NewDrawing("Survey Plot Selection", Comps("GPS").coordinateSystem)
            Dim columns = s.OwnedTable.ColumnSet
            Dim objects = s.ObjectSet
            Dim column = columns.NewColumn()
            column.Name = "Station"
            column.Type = Mfd.ColumnType.ColumnTypeInt32
            columns.Add(column)

            Dim dval1 = Comps("Survey Plot Selection").AreaSize.defaultvalue
            dval1.Formatting = 1
            Dim dval2 = Comps("Survey Plot Selection").AreaStyle.defaultvalue
            dval2.Formatting = 0
            Dim dval4 = Comps("Survey Plot Selection").Areaborderstyle.defaultvalue
            dval4.Formatting = 75
            Dim dval5 = Comps("Survey Plot Selection").Areabordersize.defaultvalue
            dval5.Formatting = 2

            'color
            Dim draw = Doc.ComponentSet("Survey Plot Selection")
            draw.Areabackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
            draw.Areaborderbackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
            draw.Areaborderforeground.LoadFrom("<theme><type>color</type><colorDef>#FF0000</colorDef></theme>") 'red

        Else
            Dim data = Comps.Item(drawidx)
            Dim dataobjs = data.objectset
            For x = 0 To dataobjs.Count - 1
                dataobjs.Remove(0)
            Next
        End If

        Dim buffidx = Comps.ItemByName("Selection Buffer")
        If buffidx < 0 Then
            Dim b = Doc.NewDrawing("Selection Buffer", Comps("GPS").coordinateSystem)
            Dim bufobjs = b.ObjectSet
            With b
                .AreaBackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
                .AreaBorderBackground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'transparent
                .AreaBorderForeground.LoadFrom("<theme><type>color</type><colorDef>#000001</colorDef></theme>") 'red
            End With
        Else
            Dim data = Comps.Item(buffidx)
            Dim dataobjs = data.objectset
            For x = 0 To dataobjs.Count - 1
                dataobjs.Remove(0)
            Next
        End If
    End Sub

    Private Sub deletelines()
        Dim compindex = Comps.ItemByName("GPS")
        Dim drawcomp = Comps.Item(compindex)
        Dim drawtable = drawcomp.OwnedTable
        If drawcomp.Type <> Mfd.ComponentType.ComponentDrawing Then
            Exit Sub
        End If
        Dim objects = drawcomp.ObjectSet
        Dim x = 0
        Do While x < objects.count
            Dim obj = objects.Item(x)
            If obj.Type = Mfd.ObjectType.ObjectLine Then
                objects.Remove(x)
                x = x - 1
            End If
            x = x + 1
        Loop
    End Sub

    Private Sub createmap()
        Dim dataidx = Comps.ItemByName("Survey Plot Map")
        If dataidx < 0 Then
            Dim mapdata = Doc.NewComponentSet()
            For Each comp In Comps
                If comp.typename = "Drawing" Then
                    mapdata.Add(comp)
                End If
            Next
            If mapdata.Count = 0 Then
                App.MessageBox("No Existing drawing component(s)")
                Exit Sub
            End If
            Dim newmap = Doc.NewMap("Survey Plot Map", Comps("Survey Plot Selection"), Comps("GPS").coordinateSystem)

            Dim map = Comps.Item("Survey Plot Map")
            Dim layers = map.LayerSet
            For x = layers.Count - 1 To 0 Step -1
                Dim layer = layers.Item(x)
                If layer.component.name <> "Survey Plot Selection" Then
                    layers.remove(x)
                End If
            Next
            Dim layerset As Array = {"Selection Buffer", "Survey Distance", "GPS", "GPS Lables"}
            For x = 0 To UBound(layerset)
                Dim Index = Comps.ItemByName(layerset(x))
                If Index >= 0 Then
                    Dim comp = Comps.Item(Index)
                    Dim newlayer = Doc.NewLayer(comp)
                    layers.Add(newlayer)
                End If
            Next
            Dim displaymap = Comps(Comps.ItemByName("Survey Plot Map"))
            displaymap.Open()
        Else
            Dim map = Comps.Item("Survey Plot Map")
            Dim layers = map.LayerSet
            For x = layers.Count - 1 To 0 Step -1
                Dim layer = layers.Item(x)
                If layer.component.name <> "Survey Plot Selection" Then
                    layers.remove(x)
                End If
            Next
            Dim layerset As Array = {"Selection Buffer", "Survey Distance", "GPS", "GPS Lables"}
            For x = 0 To UBound(layerset)
                Dim Index = Comps.ItemByName(layerset(x))
                If Index >= 0 Then
                    Dim comp = Comps.Item(Index)
                    Dim newlayer = Doc.NewLayer(comp)
                    layers.Add(newlayer)
                End If
            Next
            'Dim displaymap = Comps(Comps.ItemByName("Survey Plot Map"))
            'displaymap.Open()
        End If
    End Sub

    Private Sub createXML()
        Dim writer As XmlWriter = Nothing

        Try

            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.Indent = True
            writer = XmlWriter.Create(m_Document, settings)

            writer.WriteComment("Manifold Layout Theme XML")

            ' Write an layout element 
            writer.WriteStartElement("layout")

            'Write the name element
            writer.WriteElementString("name", "Survey Plot Map Layout")
            writer.WriteElementString("pagesByX", "1")
            writer.WriteElementString("pagesByY", "1")
            writer.WriteStartElement("elements")

            ' Write the elements layout components.
            '*1
            writer.WriteStartElement("component")
            writer.WriteAttributeString("background", "auto")
            writer.WriteAttributeString("border", "coordinates")
            writer.WriteAttributeString("borderDegMinSec", "false")
            writer.WriteAttributeString("borderEachPage", "true")
            writer.WriteAttributeString("borderFont", "Bookman Old Style, 8")
            writer.WriteAttributeString("borderFore", "#000000")
            writer.WriteAttributeString("borderMargin", "12")
            writer.WriteAttributeString("borderNoOverlaps", "true")
            writer.WriteAttributeString("borderRounding", "0")
            writer.WriteAttributeString("borderStep", "20.000000")
            writer.WriteAttributeString("borderUnit", "Meter")
            writer.WriteAttributeString("controlPoints", "auto")
            writer.WriteAttributeString("graticule", "show")
            writer.WriteAttributeString("grid", "auto")
            writer.WriteAttributeString("legend", "auto")
            writer.WriteAttributeString("name", "Survey Plot Map")
            writer.WriteAttributeString("northArrow", "auto")
            writer.WriteAttributeString("paging", "continuous")
            writer.WriteAttributeString("ref", "r1")
            writer.WriteAttributeString("scaleBar", "auto")
            writer.WriteAttributeString("scope", "layer")
            writer.WriteAttributeString("scopeDetail", "Selection Buffer")
            writer.WriteAttributeString("xmax", "0.99342264688697401")
            writer.WriteAttributeString("xmin", "0.0068159358276979349")
            writer.WriteAttributeString("ymax", "0.99030303030303035")
            writer.WriteAttributeString("ymin", "0.009696969696969697")
            writer.WriteEndElement()
            '*2
            writer.WriteStartElement("rect")
            writer.WriteAttributeString("fore", "#000000")
            writer.WriteAttributeString("xmax", "0.99597862282236072")
            writer.WriteAttributeString("xmin", "0.0042599598923112091")
            writer.WriteAttributeString("ymax", "0.99515151515151512")
            writer.WriteAttributeString("ymin", "0.0060606060606060606")
            writer.WriteEndElement()
            '*3
            writer.WriteStartElement("rect")
            writer.WriteAttributeString("back", "#ffffff")
            writer.WriteAttributeString("xmax", "0.40043622987725369")
            writer.WriteAttributeString("xmin", "0.017039839569244836")
            writer.WriteAttributeString("ymax", "0.97575757575757571")
            writer.WriteAttributeString("ymin", "0.87561997391189617")
            writer.WriteEndElement()
            '*4
            writer.WriteStartElement("text")
            writer.WriteAttributeString("font", "Bookman Old Style, 24, bold")
            writer.WriteAttributeString("text", "AERIAL MAPPING OF ")
            writer.WriteAttributeString("textAlignX", "center")
            writer.WriteAttributeString("textAlignY", "center")
            writer.WriteAttributeString("xmax", "0.40043622987725369")
            writer.WriteAttributeString("xmin", "0.017039839569244836")
            writer.WriteAttributeString("ymax", "0.97454545454545449")
            writer.WriteAttributeString("ymin", "0.94424242424242422")
            writer.WriteEndElement()
            '*5
            writer.WriteStartElement("text")
            writer.WriteAttributeString("font", "Bookman Old Style, 15, bold")
            writer.WriteAttributeString("text", "Survey Plot No. ")
            writer.WriteAttributeString("textAlignX", "center")
            writer.WriteAttributeString("textAlignY", "center")
            writer.WriteAttributeString("xmax", "0.40043622987725369")
            writer.WriteAttributeString("xmin", "0.017039839569244836")
            writer.WriteAttributeString("ymax", "0.94424242424242422")
            writer.WriteAttributeString("ymin", "0.92484848484848481")
            writer.WriteEndElement()
            '*6
            writer.WriteStartElement("text")
            writer.WriteAttributeString("font", "Bookman Old Style, 12")
            writer.WriteAttributeString("text", "Reference : W1/")
            writer.WriteAttributeString("textAlignX", "center")
            writer.WriteAttributeString("textAlignY", "center")
            writer.WriteAttributeString("xmax", "0.40076618374728534")
            writer.WriteAttributeString("xmin", "0.017369793439276516")
            writer.WriteAttributeString("ymax", "0.92381966689100958")
            writer.WriteAttributeString("ymin", "0.90806209113343384")
            writer.WriteEndElement()
            '7
            writer.WriteStartElement("text")
            writer.WriteAttributeString("font", "Bookman Old Style, 12")
            writer.WriteAttributeString("text", "Prepared by : GPP/RM/[Computer]")
            writer.WriteAttributeString("textAlignX", "center")
            writer.WriteAttributeString("textAlignY", "center")
            writer.WriteAttributeString("xmax", "0.40096150151602711")
            writer.WriteAttributeString("xmin", "0.017565111208018201")
            writer.WriteAttributeString("ymax", "0.9077028120828794")
            writer.WriteAttributeString("ymin", "0.89194523632530365")
            writer.WriteEndElement()
            '*8
            writer.WriteStartElement("text")
            writer.WriteAttributeString("font", "Bookman Old Style, 12")
            writer.WriteAttributeString("text", "Revision : [Date]")
            writer.WriteAttributeString("textAlignX", "center")
            writer.WriteAttributeString("textAlignY", "center")
            writer.WriteAttributeString("xmax", "0.40096150151602711")
            writer.WriteAttributeString("xmin", "0.017565111208018201")
            writer.WriteAttributeString("ymax", "0.89158595727474921")
            writer.WriteAttributeString("ymin", "0.87582838151717346")
            writer.WriteEndElement()
            '*9
            writer.WriteStartElement("northArrow")
            writer.WriteAttributeString("font", "Bookman Old Style, 12")
            writer.WriteAttributeString("refParent", "r1")
            writer.WriteAttributeString("xmax", "0.98411462160316054")
            writer.WriteAttributeString("xmin", "0.91948719145019475")
            writer.WriteAttributeString("ymax", "0.9776647348318187")
            writer.WriteAttributeString("ymin", "0.83638361607874556")
            writer.WriteEndElement()

            writer.WriteEndElement()

            writer.WriteEndElement()

            ' Write the XML to file and close the writer.
            writer.Flush()
            writer.Close()

        Finally
            If Not (writer Is Nothing) Then
                writer.Close()
            End If
        End Try
    End Sub

    Private Sub createlayout()
        Dim layoutidx = Comps.ItemByName("Survey Plot Map Layout")
        If layoutidx < 0 Then
            Dim maplayout = Doc.NewLayout("Survey Plot Map Layout")
            'Dim ui = App.UserInterface
            'Doc.ComponentSet("Survey Plot Map Layout").Open()
            'pagesetup("ISO A3")
            layoutidx = Comps.ItemByName("Survey Plot Map Layout")
            Dim layout = Comps.Item(layoutidx)
            Dim entryset = layout.entryset
            For x = entryset.count - 1 To 0 Step -1
                entryset.Remove(x)
            Next
            layout.LoadFromFile("T:\geodata\Drone\XML Templates\Layout\mfd_temp_layout.xml")
            layout.Open()
        Else
            Exit Sub
        End If
    End Sub

    Function existsComponent(ByRef theComponent)
        Dim doc = App.ActiveDocument
        Dim foundComponent = False
        For Each cmp In doc.ComponentSet
            If cmp.Name = theComponent Then
                foundComponent = True
                Exit For
            End If
        Next
        existsComponent = foundComponent
    End Function

    Private Sub Assign_CoordSys(ByRef NewProjection, ByRef NewDatum, ByRef Comp_Type)
        For Each Comp In Comps
            If Comp.Type = Comp_Type Then
                Dim Comp_CoodSys = Comp.CoordinateSystem
                Dim Comp_CoodSys_Params = Comp_CoodSys.Parameters
                Dim LocalScaleX = Comp_CoodSys_Params.Item("LocalScaleX").Value
                Dim LocalScaleY = Comp_CoodSys_Params.Item("LocalScaleY").Value
                Dim LocalOffsetX = Comp_CoodSys_Params.Item("LocalOffsetX").Value
                Dim LocalOffsetY = Comp_CoodSys_Params.Item("LocalOffsetY").Value
                Comp_CoodSys.Load(NewProjection)
                Comp_CoodSys.Datum.Load(NewDatum)
                Comp_CoodSys_Params.Item("LocalScaleX").Value = LocalScaleX
                Comp_CoodSys_Params.Item("LocalScaleY").Value = LocalScaleY
                Comp_CoodSys_Params.Item("LocalOffsetX").Value = LocalOffsetX
                Comp_CoodSys_Params.Item("LocalOffsetY").Value = LocalOffsetY
            End If
        Next
    End Sub

    'Function pagesetup(ByRef papersize)
    'Dim ui = App.UserInterface
    'ui.InvokeCommand("FilePageSetup")
    'Dim dlg = ui.ModalDialog
    'With dlg.ControlSet
    '.Item("ComboBox0").Text = papersize
    '.Item("ButtonLandscape").Checked = True
    'dlg.Accept()
    'End With
    'End Function

    'private sub addcolumn(
    '        byref columns as mfd.columnset,
    '       byval colname as string,
    '      byval coltype as mfd.columntype
    '     )
    '    dim column as mfd.column
    '    column = columns.newcolumn
    '    with column
    '        .name = colname
    '        .type = coltype
    '    end with
    '    columns.add(column)
    'end sub

    Private Sub Error_DisplayAndClear(
    ByVal ex As Exception,
    ByVal Routine_Name As String)

        System.Windows.Forms.MessageBox.Show(
            text:="Function or Subroutine:" & vbNewLine &
                  "~~~~~~~~~~~~~~~" & vbNewLine &
                   Routine_Name & vbNewLine & vbNewLine &
                  "Error #:" & vbNewLine &
                  "~~~~~~~~~~~~~~~" & vbNewLine &
                   Err.Number.ToString & vbNewLine & vbNewLine &
                  "Error Message:" & vbNewLine &
                  "~~~~~~~~~~~~~~~" & vbNewLine &
                   ex.Message,
            caption:="Run-Time Error",
            buttons:=System.Windows.Forms.MessageBoxButtons.OK,
            icon:=System.Windows.Forms.MessageBoxIcon.Error)
        Err.Clear()
    End Sub

    Private Sub BtnClear_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnClear.Click
        Dim drawings As Array = {"GPS", "Survey Distance", "Survey Plot", "Survey Plot Selection", "Selection Buffer", "Survey Plot Map", "Survey Plot Map Layout"}
        For x = 0 To UBound(drawings)
            Dim drawidx = Comps.ItemByName(drawings(x))
            If drawidx > 0 Then
                Comps.Remove(drawidx)
            End If
        Next
    End Sub
End Class