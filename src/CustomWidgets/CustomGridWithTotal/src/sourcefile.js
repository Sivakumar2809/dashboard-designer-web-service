/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid: "9253c650-8875-4942-beea-8eb8f3c796f2",

    widgetName: "CustomGridWithTotal",

    init: function () {

        var cContainer = $(this.element).parents(".e-customwidget-item");
        if (this.model.boundColumns.column.length > 0 && this.model.dataSource.length == 0) {
            cContainer.find(".e-customwidgetitem").css({ "display": 'none' });
            cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").css({ "display": 'block' });
            cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").find('.widget-info-icon').css({ "display": 'block' });
            cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").find('.e-nodata').css({ "display": 'block' });
        } else {
            cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").css({ "display": 'none' });
            cContainer.find(".e-customwidgetitem").css({ "display": 'block' });
        }

        $(this.element).closest(".e-customwidget-item").addClass('e-custom-summaryGrid');
        $(this.element).parent().css({ 'overflow': 'visible' });
        this.defaultData = [{ Column: "Item1", Value: 1 }, { Column: "Item2", Value: 2 }, { Column: "Item3", Value: 3 }, { Column: "Item4", Value: 4 }, { Column: "Item5", Value: 5 }];
        this.gridProperty = {
            BasicSettings: { allowSorting: true, horizontal: true, vertical: false, showBorder: true, enableAltRow: false, allowEdit: false, editModeType: "Normal", allowFilter: false },
            HeaderSettings: { showHeader: true, foreground: "#2D3748", background: "#f3f7fa", rowHeight: 42, autoFontSize: true, fontSize: 16, padding: 11 },
            ContentSettings: { foreground: "#000000", background: "#ffffff", rowHeight: 32, autoFontSize: true, fontSize: 16, padding: 11, alternativeforeground: "#000000", alternativebackground: "#f7f7f7" }
        };

        this.designId = $(this.element).parents(".e-customwidget-item").attr("id").split("_" + this.model.widgetId)[0];
        this.designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
        this.widgetInstance = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
        this.themeApplied = this.widgetInstance.themeHelper.currentTheme.name;
        this.measureColumnNames = [];
        this.measureColumnFormatInfo = [];
        if (this.model.dataSource.length > 0) {
            this.columnNames = [];
            for (var i = 0; i < this.widgetInstance.dataGroupInfo.FieldContainers.length; i++) {
                if (this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos.length > 0) {
                    for (var j = 0; j < this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos.length; j++) {
                        if (this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].FieldActualType == 'e-reportdesigner-dataset-number') {
                            this.measureColumnNames.push(this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].IsDisplayNameEdited ? this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].DisplayName : this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].Name);
                            this.measureColumnFormatInfo[this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].UniqueColumnName] = this.widgetInstance.dataGroupInfo.FieldContainers[i].FieldInfos[j].MeasureFormatting;
                        }
                    }
                }
            }
        }


        this.isUpdateInitialSelection = false;
        this.isDataUpdated = false;
        this.isGridCreated = false;
        this.gridColumns = [];
        this.isInitialRender = false;
        this.selectedRowIndex = -1;
        this.designId = $(this.element).parents(".e-customwidget-item").attr("id").split("_" + this.model.widgetId)[0];
        this.designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
        var widget = document.createElement("div");
        widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
        this.element.appendChild(widget);
        this.renderGridElement();
    },
    renderGridElement: function () {
        this.setPropertyValue();
        this.getGridColumns();
        var widget = $(this.element).find("#" + this.element.getAttribute("id") + "_widget");
        $(widget).css("border", (this.gridProperty.BasicSettings.showBorder ? "1px solid rgb(200,200,200)" : "0px solid"));
        $(widget).addClass('summaryGridcustomwidgetContainer');
        var gridDiv = $("<div>").addClass("e-dbrd-custom-widget-grid");
        gridDiv.attr("id", this.element.getAttribute("id") + "_widget_grid");
        gridDiv.css({ "height": (this.element.clientHeight - 4) + 'px', "width": (this.element.clientWidth) + "px" });
        widget.append(gridDiv);
        this.gridObject = this.getGridObject(this.defaultData);
        this.gridObject.appendTo("#" + this.element.getAttribute("id") + "_widget_grid");

        this.getGridContentHeight();
        this.updateGridLines();
        this.updateAllowSorting();
        this.updateEnableAltRow();
        this.updateAllowFilter();
        this.updateBorderVisibility();
        this.updateHeaderVisibility();
        this.updateHeaderProperty();
        this.updateContentProperty();
        this.updateFilterInputUI();

        if (this.themeApplied.toLowerCase() === 'dark') {
            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_darktheme_changes1'>.e-custom-summaryGrid .e-dashboardviewer .e-grid .e-rowcell, .e-custom-summaryGrid .e-dashboardviewer .summaryGridcustomwidgetContainer, .e-custom-summaryGrid .e-dashboardviewer .e-grid .e-headercell, .e-custom-summaryGrid .e-dashboardviewer .e-lib.e-grid .e-gridheader {border-color:#92949c !important;}</style>");
            //#505a71 - Border as per pivotgrid
            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_darktheme_changes2'>.e-custom-summaryGrid .e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-row, .e-custom-summaryGrid .e-dbrd-control-title, .e-custom-summaryGrid .e-lib.e-grid .e-content{background:#202635 !important; color: #e5ebf8 !important;}</style>");

            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_darktheme_changes3'>.e-custom-summaryGrid .e-customwidget-element{height:98% !important;}</style>");

        }

        if (this.widgetInstance.designerInstance.isViewMode()) {
            var self = this;
            var storeTimeInterval = setInterval(function () {
                if ((self.themeApplied != self.widgetInstance.themeHelper.currentTheme.name) && self.model.dataSource.length > 0) {
                    clearInterval(storeTimeInterval);
                    self.themeApplied = self.widgetInstance.themeHelper.currentTheme.name;
                    self.element.innerHTML = "";
                    self.init();
                    return;
                }
            }, 1000);
        }
    },

    getGridObject: function (data) {
        return new ej2GridSummaryRow.grids.Grid({
            dataSource: this.getData(),
            gridLines: this.getGridLineType(),
            columns: this.gridColumns,
            height: ($(this.element).height() - (this.getGridContentHeight() + 5)) + 'px',
            width: ($(this.element).width()) + 'px',
            allowSorting: this.gridProperty.BasicSettings.allowSorting,
            enableAltRow: this.gridProperty.BasicSettings.enableAltRow,
            allowFiltering: this.gridProperty.BasicSettings.allowFilter,
            enableHover: false,
            selectionSettings: {
                allowColumnSelection: true,
                mode: 'Cell'
            },
            actionComplete: $.proxy(this.gridActionComplete, this),
            queryCellInfo: $.proxy(this.queryCellInfo, this),
            cellSelecting: $.proxy(this.cellSelecting, this),
			dataBound: $.proxy(this.dataBound, this)
        });
    },
	dataBound: function (args) {
		if (this.model.properties.enableFitToContent) {
            this.gridObject.autoFitColumns();
        }
    },
    cellSelecting: function (args) {
        var colIndexes = 0;
        var url = "";
        var paramUCN = '';
        for (var j = 0; j < 5; j++) {
            for (var i = 0; i < this.model.boundColumns.column.length; i++) {
                if (this.model.properties['linkColumnName' + (j + 1)] == this.model.boundColumns.column[i].columnName && args.cellIndex.cellIndex === i) {
                    colIndexes = i;
                    url = this.model.properties['linkURL' + (j + 1)];
                    break;
                }
            }
            if (url !== "") {
                break;
            }
        }
        if (this.model.dataSource.length > 0 && this.designerObj.model.mode != 'design' && args.cellIndex.cellIndex === colIndexes && url != "") {

            var paramColName = url.slice(url.indexOf('{'), url.lastIndexOf('}') + 1);
            if (paramColName != "") {
                var colName = paramColName.replaceAll('{', "").replaceAll('}', "");
                var ucn = '';
                for (var i = 0; i < this.model.boundColumns.column.length; i++) {
                    if (colName == this.model.boundColumns.column[i].columnName) {
                        ucn = this.model.boundColumns.column[i].uniqueColumnName
                        break;
                    }
                }
                url = args.data[ucn] != undefined ? url.replace(paramColName, args.data[ucn]) : url;
            }
            if (window.clientBeforeNavigateUrlLinking != undefined && window.clientBeforeNavigateUrlLinking != null) {
                window.clientBeforeNavigateUrlLinking(url);
            } else {
                var overlayDiv = $("<div>").attr({ "id": this.element.getAttribute("id") + "_widget_overlay" }).css({ "height": window.innerHeight, "width": window.innerWidth, "position": "fixed", "background-color": "black", 'min-width': '400px', 'min-height': '235px', 'z-index': 10000, 'opacity': 0.5, "filter": "blur(3px)" });
                $(overlayDiv).addClass('customoverlay');

                var popUpDiv = $("<div>").attr({ "id": this.element.getAttribute("id") + "_widget_popup" }).css({ "height": window.innerHeight - 100, "width": window.innerWidth - 200, "position": "fixed", "left": 100, "top": 50, "border": "3px solid transparent", "background-color": "#f9f9f9", 'z-index': 10000, 'min-width': '400px', 'min-height': '235px' });


                var buttonDiv = document.createElement("button");
                buttonDiv.setAttribute("id", "closeButton");
                buttonDiv.innerHTML = "<b>X</b>";
                $(buttonDiv).css({ "height": "20px", "border": "0px", "margin": "10px", "float": "right" });
                $(buttonDiv).hover(function () {
                    $(this).css("background-color", "lightgrey");
                }, function () {
                    $(this).css("background-color", "#f9f9f9");
                });

                $(buttonDiv).click(function () {
                    this.parentElement.remove();
                    $('#' + this.parentElement.id.replace('popup', 'overlay')).remove();
                });
                popUpDiv[0].appendChild(buttonDiv);


                var iframeId = this.element.getAttribute("id") + "_widget";

                var iframeContainer = ($("<iframe>").attr({ "id": iframeId, "frameBorder": "0", "margin": "2px", "marginwidth": "0", "marginheight": "0", "scrolling": "no", "src": url }).css({ "width": "100%", "height": "93.4%", 'border-bottom': '2px solid lightgray', 'border-top': '2px solid lightgray' }));
                overlayDiv.appendTo($('#' + this.designId));
                popUpDiv.appendTo($('#' + this.designId));
                iframeContainer.appendTo(popUpDiv[0]);
            }
        }
        args.cancel = true;
    },
    queryCellInfo: function (args) {
        if (this.model.dataSource.length !== 0) {
            if (this.measureColumnNames.indexOf(args.column.headerText) > -1) {
                var formatInfo = this.measureColumnFormatInfo[args.column.field];
                if (Number(args.cell.innerText) !== NaN) {
                    args.cell.innerText = BoldBIDashboard.DashboardUtil.formattedText(Number(args.cell.innerText), formatInfo.Culture, formatInfo.DecimalPoints, formatInfo.FormatType, formatInfo.DecimalSeparator, formatInfo.GroupSeparator, formatInfo.Prefix, formatInfo.Suffix, formatInfo.Unit, true, this.designerObj);
                }
            }
            switch (args.column.headerText.toLowerCase()) {
                case this.model.properties.columnText1.toLowerCase():
                    args.cell.bgColor = this.model.properties.columnBColor1;
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        /* if(this.themeApplied.toLowerCase() === 'dark'){
                            $(args.cell).attr('style',('color:'+this.model.properties.columnFColor1+' !important;text-align:center;font-weight:800;background:#202635;'));
                        } else { */
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor1 + ' !important;text-align:center;font-weight:800;'));
                        //}
                    }/*  else if(this.themeApplied.toLowerCase() === 'dark'){
						$(args.cell).attr('style',('color:'+this.model.properties.columnFColor1+' !important;text-align:center;background:#202635;'));
					} */ else {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor1 + ' !important;text-align:center;'));
                    }
                    break;
                case this.model.properties.columnText2.toLowerCase():
                    args.cell.bgColor = this.model.properties.columnBColor2;
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        if (this.themeApplied.toLowerCase() === 'dark') {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor2 + ' !important;text-align:center;font-weight:800;background:#202635;'));
                        } else {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor2 + ' !important;text-align:center;font-weight:800;'));
                        }
                    } else if (this.themeApplied.toLowerCase() === 'dark') {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor2 + ' !important;text-align:center;background:#202635;'));
                    } else {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor2 + ' !important;text-align:center;'));
                    }
                    break;
                case this.model.properties.columnText3.toLowerCase():
                    args.cell.bgColor = this.model.properties.columnBColor3;
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        if (this.themeApplied.toLowerCase() === 'dark') {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor3 + ' !important;text-align:center;font-weight:800;background:#202635;'));
                        } else {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor3 + ' !important;text-align:center;font-weight:800;'));
                        }
                    } else if (this.themeApplied.toLowerCase() === 'dark') {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor3 + ' !important;text-align:center;background:#202635;'));
                    } else {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor3 + ' !important;text-align:center;'));
                    }
                    break;
                case this.model.properties.columnText4.toLowerCase():
                    args.cell.bgColor = this.model.properties.columnBColor4;
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        if (this.themeApplied.toLowerCase() === 'dark') {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor4 + ' !important;text-align:center;font-weight:800;background:#202635;'));
                        } else {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor4 + ' !important;text-align:center;font-weight:800;'));
                        }
                    } else if (this.themeApplied.toLowerCase() === 'dark') {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor4 + ' !important;text-align:center;background:#202635;'));
                    } else {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor4 + ' !important;text-align:center;'));
                    }
                    break;
                case this.model.properties.columnText5.toLowerCase():
                    args.cell.bgColor = this.model.properties.columnBColor5;
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        if (this.themeApplied.toLowerCase() === 'dark') {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor5 + ' !important;text-align:center;font-weight:800;background:#202635;'));
                        } else {
                            $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor5 + ' !important;text-align:center;font-weight:800;'));
                        }
                    } else if (this.themeApplied.toLowerCase() === 'dark') {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor5 + ' !important;text-align:center;background:#202635;'));
                    } else {
                        $(args.cell).attr('style', ('color:' + this.model.properties.columnFColor5 + ' !important;text-align:center;'));
                    }
                    break;
                default:
                    if (JSON.stringify(args.data).indexOf('Total') > -1) {
                        $(args.cell).attr('style', ('text-align:center;font-weight:800;'));
                    } else if (this.themeApplied.toLowerCase() === 'dark') {
                        $(args.cell).attr('style', ('text-align:center;background:#202635;'));
                    }
                    break;
            }
            /* switch(args.column.headerText.toLowerCase()){
                case this.model.properties.linkColumnName1.toLowerCase():
                    if(this.model.properties.linkURL1 != ""){
                        $(args.cell).attr('style',('color:blue !important;text-align:center;text-decoration: underline;cursor: pointer;'));
                    }
                    break;
                case this.model.properties.linkColumnName2.toLowerCase():
                    if(this.model.properties.linkURL2 != ""){
                        $(args.cell).attr('style',('color:blue !important;text-align:center;text-decoration: underline;cursor: pointer;'));
                    }
                    break;
                case this.model.properties.linkColumnName3.toLowerCase():
                    if(this.model.properties.linkURL3 != ""){
                        $(args.cell).attr('style',('color:blue !important;text-align:center;text-decoration: underline;cursor: pointer;'));
                    }
                    break;
                case this.model.properties.linkColumnName4.toLowerCase():
                    if(this.model.properties.linkURL4 != ""){
                        $(args.cell).attr('style',('color:blue !important;text-align:center;text-decoration: underline;cursor: pointer;'));
                    }
                    break;
                case this.model.properties.linkColumnName5.toLowerCase():
                    if(this.model.properties.linkURL5 != ""){
                        $(args.cell).attr('style',('color:blue !important;text-align:center;text-decoration: underline;cursor: pointer;'));
                    }
                    break;				
            } */
        }
    },
    getGridColumns: function () {
        this.gridColumns = [];
        if (this.model.dataSource.length !== 0) {
            for (var i = 0; i < this.model.boundColumns.column.length; i++) {
                this.gridColumns.push({
                    field: this.model.boundColumns.column[i].uniqueColumnName,
                    headerText: this.model.boundColumns.column[i].columnName,
                    textAlign: 'Center',
                    width: "10",
                    allowSorting: false
                });
            }
        } else {
            this.gridColumns.push({
                field: "Column",
                headerText: "Name",
                textAlign: 'Left',
                width: "100"
            },
                {
                    field: "Value",
                    headerText: "Value",
                    format: 'N2',
                    textAlign: 'Left',
                    width: "100"
                });
        }
    },
    getData: function () {
        var data = [];
        this.obj = {};
        if (this.model.dataSource.length > 0) {
            if (this.measureColumnNames.length > 0) {
                for (var i = 0; i < this.model.boundColumns.column.length > 0; i++) {
                    if (this.measureColumnNames.indexOf(this.model.boundColumns.column[i].columnName) > -1) {
                        this.obj[this.model.boundColumns.column[i].uniqueColumnName] = 0;
                    } else {
                        this.obj[this.model.boundColumns.column[i].uniqueColumnName] = "Total";
                    }
                }
                for (var i = 0; i < this.model.dataSource.length > 0; i++) {
                    var keys = Object.keys(this.obj);
                    for (var j = 0; j < keys.length > 0; j++) {
                        this.obj[keys[j]] = (this.obj[keys[j]] === "Total") ? "Total" : (((this.model.dataSource[i][keys[j]] != null && this.model.dataSource[i][keys[j]] != undefined && this.model.dataSource[i][keys[j]] != "(Blanks)") ? this.model.dataSource[i][keys[j]] : 0) + this.obj[keys[j]]);
                    }
                }
                data = this.model.dataSource.slice(0, this.model.dataSource.length);
                data.push(this.obj);
            } else {
                data = this.model.dataSource;
            }

        } else {
            data = this.defaultData;
        }
        return data;
    },

    gridActionComplete: function (args) {
        if (args.requestType === "refresh") {
            this.updateBorderVisibility();
            this.updateHeaderVisibility();
            this.updateHeaderProperty();
            this.updateContentProperty();
            //this.updateInitialSelection();
            this.updateFilterInputUI();
            this.rowIndex = 0;
        }
        $(".e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-rowcell").css("line-height", this.gridProperty.ContentSettings.rowHeight + "px");
    },
    isWidgetConfigured: function () {
        return this.model.boundColumns.column.length !== 0;
    },

    setPropertyValue: function () {
        if (this.model.properties.horizontal !== null && this.model.properties.horizontal !== undefined) {
            this.gridProperty.BasicSettings.horizontal = this.model.properties.horizontal;
        }
        if (this.model.properties.vertical !== null && this.model.properties.vertical !== undefined) {
            this.gridProperty.BasicSettings.vertical = this.model.properties.vertical;
        }
        if (this.model.properties.showBorder !== null && this.model.properties.showBorder !== undefined) {
            this.gridProperty.BasicSettings.showBorder = this.model.properties.showBorder;
        }
        if (this.model.properties.allowSorting !== null && this.model.properties.allowSorting !== undefined) {
            this.gridProperty.BasicSettings.allowSorting = this.model.properties.allowSorting;
        }
        if (this.model.properties.showHeader !== null && this.model.properties.showHeader !== undefined) {
            this.gridProperty.HeaderSettings.showHeader = this.model.properties.showHeader;
        }
        if (this.model.properties.headerforeground !== null && this.model.properties.headerforeground !== undefined && this.model.properties.headerforeground !== "") {
            this.gridProperty.HeaderSettings.foreground = this.model.properties.headerforeground;
        }
        if (this.model.properties.headerbackground !== null && this.model.properties.headerbackground !== undefined && this.model.properties.headerbackground !== "") {
            this.gridProperty.HeaderSettings.background = this.model.properties.headerbackground;
        }
        if (this.model.properties.headerrowheight !== null && this.model.properties.headerrowheight !== undefined) {
            this.gridProperty.HeaderSettings.rowHeight = this.model.properties.headerrowheight;
        }
        if (this.model.properties.headerautofontsize !== null && this.model.properties.headerautofontsize !== undefined) {
            this.gridProperty.HeaderSettings.autoFontSize = this.model.properties.headerautofontsize;
        }
        if (this.model.properties.headerfontsize !== null && this.model.properties.headerfontsize !== undefined) {
            this.gridProperty.HeaderSettings.fontSize = this.model.properties.headerfontsize;
        }
        if (this.model.properties.headerpadding !== null && this.model.properties.headerpadding !== undefined) {
            this.gridProperty.HeaderSettings.padding = this.model.properties.headerpadding;
        }
        if (this.model.properties.contentforeground !== null && this.model.properties.contentforeground !== undefined && this.model.properties.contentforeground !== "") {
            this.gridProperty.ContentSettings.foreground = this.model.properties.contentforeground;
        }
        if (this.model.properties.contentbackground !== null && this.model.properties.contentbackground !== undefined && this.model.properties.contentbackground !== "") {
            this.gridProperty.ContentSettings.background = this.model.properties.contentbackground;
        }
        if (this.model.properties.contentrowheight !== null && this.model.properties.contentrowheight !== undefined) {
            this.gridProperty.ContentSettings.rowHeight = this.model.properties.contentrowheight;
        }
        if (this.model.properties.contentautofontsize !== null && this.model.properties.contentautofontsize !== undefined) {
            this.gridProperty.ContentSettings.autoFontSize = this.model.properties.contentautofontsize;
        }
        if (this.model.properties.contentfontsize !== null && this.model.properties.contentfontsize !== undefined) {
            this.gridProperty.ContentSettings.fontSize = this.model.properties.contentfontsize;
        }
        if (this.model.properties.contentpadding !== null && this.model.properties.contentpadding !== undefined) {
            this.gridProperty.ContentSettings.padding = this.model.properties.contentpadding;
        }
        if (this.model.properties.enablealtrow !== null && this.model.properties.enablealtrow !== undefined) {
            this.gridProperty.BasicSettings.enableAltRow = this.model.properties.enablealtrow;
        }
        if (this.model.properties.alternativebackground !== null && this.model.properties.alternativebackground !== undefined && this.model.properties.alternativebackground !== "") {
            this.gridProperty.ContentSettings.alternativebackground = this.model.properties.alternativebackground;
        }
        if (this.model.properties.alternativeforeground !== null && this.model.properties.alternativeforeground !== undefined && this.model.properties.alternativeforeground !== "") {
            this.gridProperty.ContentSettings.alternativeforeground = this.model.properties.alternativeforeground;
        }
        if (this.model.properties.alllowedit !== null && this.model.properties.alllowedit !== undefined) {
            this.gridProperty.BasicSettings.allowEdit = this.model.properties.alllowedit;
        }
        if (this.model.properties.editmodetype !== null && this.model.properties.editmodetype !== undefined && this.model.properties.editmodetype !== "") {
            this.gridProperty.BasicSettings.editModeType = this.model.properties.editmodetype;
        }
        if (this.model.properties.allowfilter !== null && this.model.properties.allowfilter !== undefined) {
            this.gridProperty.BasicSettings.allowFilter = this.model.properties.allowfilter;
        }
    },

    update: function (option) {
        /* if (option.type === "resize") {
            this.resizeGrid();
        }
        else if (option.type === "refresh") {
            this.element.innerHTML = "";
            this.init();
            return;
        }
        else if (option.type === "propertyChange") { */
        this.element.innerHTML = "";
        this.init();
        return;
        //}
    },

    resizeGrid: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.height = ($(this.element).height() - (this.getGridContentHeight() + 5)) + 'px';
            gridObj.width = ($(this.element).width()) + 'px';
            gridObj.refresh();
        }
    },

    getGridContentHeight: function () {
        var headerHeight = this.gridProperty.HeaderSettings.showHeader ? (this.gridProperty.HeaderSettings.rowHeight + (this.gridProperty.HeaderSettings.padding * 2)) : 0;
        return this.gridProperty.BasicSettings.allowFilter ? (headerHeight + 65) : headerHeight;
    },

    /*Basic settings*/
    updateGridLines: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.gridLines = this.getGridLineType();
            gridObj.dataBind();
        }
    },
    getGridLineType: function () {
        return this.gridProperty.BasicSettings.horizontal && this.gridProperty.BasicSettings.vertical ? "Both" : this.gridProperty.BasicSettings.horizontal ? "Horizontal" : this.gridProperty.BasicSettings.vertical ? "Vertical" : "None";
    },
    updateAllowSorting: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.allowSorting = this.gridProperty.BasicSettings.allowSorting;
            gridObj.refresh();
        }
    },
    updateEnableAltRow: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.enableAltRow = this.gridProperty.BasicSettings.enableAltRow;
            gridObj.refresh();
        }
    },
    updateEditSettings: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.editSettings.allowEditing = this.gridProperty.BasicSettings.allowEdit;
            gridObj.editSettings.allowAdding = this.gridProperty.BasicSettings.allowEdit;
            gridObj.editSettings.allowDeleting = this.gridProperty.BasicSettings.allowEdit;
            //gridObj.editSettings.mode = this.gridProperty.BasicSettings.editModeType;
            gridObj.dataBind();
        }
    },
    updateAllowFilter: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2GridSummaryRow.base.getComponent(gridElement[0], 'grid');
            gridObj.allowFiltering = this.gridProperty.BasicSettings.allowFilter;
            gridObj.height = $(this.element).height() - (this.getGridContentHeight() + 5);
            gridObj.width = $(this.element).width()-7;
            gridObj.refresh();
        }
    },

    /* Header Settings */
    updateBorderVisibility: function () {
        var border = (this.gridProperty.BasicSettings.showBorder ? "1px solid rgb(200,200,200)" : "0px solid");
        $(this.element).find("#" + this.element.getAttribute("id") + "_widget").css("border", border);
    },
    updateHeaderVisibility: function () {
        if (this.gridProperty.HeaderSettings.showHeader) {
            $(this.element).find(".e-gridheader").removeClass("e-dbrd-custom-gridheader-visibility");
        } else {
            $(this.element).find(".e-gridheader").addClass("e-dbrd-custom-gridheader-visibility");
        }
    },
    updateHeaderProperty: function () {
        var fontSize = this.gridProperty.HeaderSettings.fontSize + "px"
        if (this.gridProperty.HeaderSettings.autoFontSize) {
            var widgetIns = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
            fontSize = widgetIns.getFontSizeBasedOnResolution();
        }
        var backgroundColor = this.gridProperty.HeaderSettings.background !== null && this.gridProperty.HeaderSettings.background !== undefined && this.gridProperty.HeaderSettings.background !== "" ? this.gridProperty.HeaderSettings.background : "#f3f7fa";
        var foreground = this.gridProperty.HeaderSettings.foreground !== null && this.gridProperty.HeaderSettings.foreground !== undefined && this.gridProperty.HeaderSettings.foreground !== "" ? this.gridProperty.HeaderSettings.foreground : "#2D3748";
        var styleText = '.e-dbrd-custom-widget-grid.e-grid .e-columnheader {line-height:' + this.gridProperty.HeaderSettings.rowHeight + 'px !important; background: ' + backgroundColor + ' } ' +
            '.e-dbrd-custom-widget-grid.e-grid .e-headercell {height: ' + this.gridProperty.HeaderSettings.rowHeight + 'px !important; padding:' + this.gridProperty.HeaderSettings.padding + 'px 11px !important;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-headercelldiv {line-height:' + this.gridProperty.HeaderSettings.rowHeight + 'px !important; height:' + this.gridProperty.HeaderSettings.rowHeight + 'px !important;color:' + foreground + '}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-headertext {font-size:' + fontSize + ' !important;}';
        if ($(this.element).find("#" + this.element.getAttribute("id") + "_header_property_value").length > 0) {
            $(this.element).find("#" + this.element.getAttribute("id") + "_header_property_value").html(styleText);
        } else {
            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_header_property_value'>" + styleText + "</style>");
        }
    },
    updateContentProperty: function () {
        var fontSize = this.gridProperty.ContentSettings.fontSize + "px"
        if (this.gridProperty.ContentSettings.autoFontSize) {
            var widgetIns = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
            fontSize = widgetIns.getFontSizeBasedOnResolution();
        }
        var alternativeBackground = this.designerObj.model.mode === BoldBIDashboard.Designer.mode.view && this.gridProperty.ContentSettings.alternativebackground === "#f7f7f7" && this.designerObj.isDarkTypeTheme() ? '#384055' : this.gridProperty.ContentSettings.alternativebackground;
        var alternativeforeground = this.gridProperty.ContentSettings.alternativeforeground !== "#000000" ? this.gridProperty.ContentSettings.alternativeforeground : this.designerObj.modules.themeHelper.getGridDataTextColor();
        var background = this.gridProperty.ContentSettings.background !== "#ffffff" ? this.gridProperty.ContentSettings.background : this.designerObj.modules.themeHelper.getWidgetTileBackground();
        var foreground = this.gridProperty.ContentSettings.foreground !== "#000000" ? this.gridProperty.ContentSettings.foreground : this.designerObj.modules.themeHelper.getGridDataTextColor();
        if (!this.gridProperty.BasicSettings.enableAltRow) {
            alternativeforeground = foreground;
            alternativeBackground = background
        }
        var styleText = '.e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-altrow{background:' + alternativeBackground + ' !important;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-altrow>.e-rowcell {color:' + alternativeforeground + ' !important;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-rowcell {color:' + foreground + ' !important; font-size: ' + fontSize + '; padding: ' + this.gridProperty.ContentSettings.padding + 'px 11px; line-height:' + this.gridProperty.ContentSettings.rowHeight + 'px;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-gridcontent .e-row {background:' + background + '}';
        if ($(this.element).find("#" + this.element.getAttribute("id") + "_content_property_value").length > 0) {
            $(this.element).find("#" + this.element.getAttribute("id") + "_content_property_value").html(styleText);
        } else {
            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_content_property_value'>" + styleText + "</style>");
        }
        if ($(this.element).find(".e-dbrd-custom-grid-button").length > 0) {
            $(this.element).find(".e-dbrd-custom-grid-button").css("height", (this.gridProperty.ContentSettings.rowHeight - 2) + "px");
        }
    },
    updateFilterInputUI: function () {
        var styleText = '.e-dbrd-custom-widget-grid.e-grid .e-filterbar .e-filterbarcell{padding:11px 11px !important;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-filterbar .e-filterbarcell .e-filterdiv{height: 30px;text-align:center;font-size: 16px;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-filterbar .e-filterbarcell .e-filterdiv .e-filtertext.e-input{font-size: 16px;background-color:#ffffff;height: 30px !important;}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-filterbar .e-filterbarcell .e-filterdiv .e-clear-icon{top:0% !important;right:0% !important;padding:7px 4px}' +
            '.e-dbrd-custom-widget-grid.e-grid .e-filterbar .e-filterbarcell .e-filterdiv .e-input-group{top:0% !important;right:0% !important;px 0px;height:30px;}';
        if ($(this.element).find("#" + this.element.getAttribute("id") + "_allow_filter_input").length > 0) {
            $(this.element).find("#" + this.element.getAttribute("id") + "_allow_filter_input").html(styleText);
        } else {
            $(this.element).append("<style id='" + this.element.getAttribute("id") + "_allow_filter_input'>" + styleText + "</style>");
        }
    }
});