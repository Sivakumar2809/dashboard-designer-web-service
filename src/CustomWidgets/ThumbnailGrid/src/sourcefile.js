/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"79bd7bce-a33f-42a6-928d-2caa75fab25d",

    widgetName:"ThumbnailGrid",

    init: function () {
		 var AvatarImgTemplate = "<script type='text/x-jsrender' id='AvatarImgTemplate'><div></div></script>";
		 var ImageTemplate = "<script type='text/x-jsrender' id='ImageTemplate'><div></div></script>";
		 $(this.element).append(AvatarImgTemplate);
		 $(this.element).append(ImageTemplate);
        this.defaultData = [{ Column: "Item1", Value: 1 }, { Column: "Item2", Value: 2 }, { Column: "Item3", Value: 3 }, { Column: "Item4", Value: 4 }, { Column: "Item5", Value: 5 }];
        this.gridProperty = {
            BasicSettings: { allowSorting: true, horizontal: true, vertical: false, showBorder: true, enableAltRow: false, allowEdit: false, editModeType: "Normal", allowFilter: false },
            HeaderSettings: { showHeader: true, foreground: "#2D3748", background: "#f3f7fa", rowHeight: 42, autoFontSize: true, fontSize: 16, padding: 11 },
            ContentSettings: { foreground: "#000000", background: "#ffffff", rowHeight: 32, autoFontSize: true, fontSize: 16, padding: 11, alternativeforeground: "#000000", alternativebackground: "#f7f7f7" },
			AvatarImageSettings: {avatarImgHeight: "30px", }
        };
        this.isUpdateInitialSelection = false;
        this.listBoxParamValue = null;
        this.listBoxValueUpdated = false;
        this.isDataUpdated = false;
        this.isGridCreated = false;
        this.gridColumns = [];
        this.isInitialRender = false;
        this.selectedRowIndex = -1;
        this.designId = $(this.element).parents(".e-customwidget-item").attr("id").split("_" + this.model.widgetId)[0];
        this.designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
        var widget = document.createElement("div");
        widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
        $(widget).addClass("e-dbrd-custom-widget-grid-container");
        $(widget).css({ "width": $(this.element).width() + "px", "height": ($(this.element).height()-1) + "px"});
        this.element.appendChild(widget);
        this.renderGridElement();
    },

    renderGridElement: function () {
        this.setPropertyValue();
        this.isInitialRender = true;
        if (!this.isGridCreated) {
            this.isGridCreated = true;
            this.getGridColumns();
            var widget = $(this.element).find("#" + this.element.getAttribute("id") + "_widget");
            $(widget).css("border", (this.gridProperty.BasicSettings.showBorder ? "1px solid rgb(200,200,200)" : "0px solid"));
            var gridDiv = $("<div>").addClass("e-dbrd-custom-widget-grid");
            gridDiv.attr("id", this.element.getAttribute("id") + "_widget_grid");
            gridDiv.css({ "height": "100%", "width": "100%" });
            widget.append(gridDiv);
            var gridObject = this.getGridObject(this.defaultData);
            gridObject.appendTo("#" + this.element.getAttribute("id") + "_widget_grid");
        } else if (this.isWidgetConfigured() && this.model.dataSource.length !== 0) {
            this.updateGridData(this.model.dataSource);
        } else {
            this.updateGridData(this.defaultData);
        }
    },

    getGridObject: function (data) {
        return new ej2CustomGrid.grids.Grid({
            dataSource: data,
            gridLines: this.getGridLineType(),
            columns: this.gridColumns,
            height: ($(this.element).height() - (this.getGridContentHeight() + 6)),
            width: ($(this.element).width() - 2),
            allowSorting: this.gridProperty.BasicSettings.allowSorting,
            enableAltRow: this.gridProperty.BasicSettings.enableAltRow,
            allowFiltering: this.gridProperty.BasicSettings.allowFilter,
			allowTextWrap: this.model.properties.enableTextWrap,
            editSettings: {
                allowEditing: false,
                allowAdding: false,
                allowDeleting: false,
            },
            rowSelected: $.proxy(this.gridRowSelected, this),
            rowDeselected: $.proxy(this.gridRowDeselected, this),
            actionComplete: $.proxy(this.gridActionComplete, this),
            queryCellInfo: $.proxy(this.queryCellInfo, this),
			//dataBound : $.proxy(this.dataBound, this)
        });
    },
	
	/* dataBound: function(args){
		if(this.model.properties.enableFitToContent){
			this.gridObject.autoFitColumns();
		}
	}, */
	
	queryCellInfo: function (args) {
		if(args.column.template=="#AvatarImgTemplate"){
			var ImgURL = args.data[this.AvatarImageColumn];
			if(args.data[this.AvatarImageColumn] != null && args.data[this.AvatarImageColumn] != undefined && args.data[this.AvatarImageColumn] != "(Null)"){
				var image='<img src='+ImgURL+' style="height:'+this.model.properties.avatarImgHeight+'px; width:'+this.model.properties.avatarImgWidth+'px;">';
			}
			else{
				var defaultImage = 'data:image/png;base64,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';
				var image='<img src='+defaultImage+' style="height:'+this.model.properties.avatarImgHeight+'px; width:'+this.model.properties.avatarImgWidth+'px;">';
			}
			var AvatarImgDiv = $(args.cell).children();
			$(AvatarImgDiv).append(image);
			
		}
		if(args.column.template=="#ImageTemplate"){
			var ImgDiv = $(args.cell).children();
			if(args.data[this.SnippetImageColumn] != null && args.data[this.SnippetImageColumn] != undefined && args.data[this.SnippetImageColumn] != "(Null)"){
			    var ImgURL = args.data[this.SnippetImageColumn].split(",");
			    for(var i =0; i<ImgURL.length ;i++){
			        var image='<img src='+ImgURL[i]+' style="height:'+this.model.properties.snippetImgHeight+'px; width:'+this.model.properties.snippetImgWidth+'px; padding-right:2px;">';
					$(ImgDiv).append(image);
			    }
			}
		}
	},

    updateGridData: function (data) {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
            this.getGridColumns();
			gridObj.allowTextWrap = this.model.properties.enableTextWrap;
            gridObj.columns = this.gridColumns;
            gridObj.dataSource = data;
        }
    },

    gridActionComplete: function (args) {
        if (args.requestType === "refresh") {
            this.updateBorderVisibility();
            this.updateHeaderVisibility();
            this.updateHeaderProperty();
            this.updateContentProperty();
            this.updateInitialSelection();
            this.updateFilterInputUI();
        }
/*         if (args.action !== null && args.action !== undefined && args.action === "edit" && args.requestType === "save" && this.model.boundColumns.column.length >= 3) {
            // Event for grid cell value edit, delete and add
            var currentData = args.data[this.model.boundColumns.column[2].uniqueColumnName];
            if (args.rowIndex !== -1) {
                var dynamicParam = this.designerObj.modules.dynamicParameterPanel;
                if (dynamicParam.currentValueCollection !== null && dynamicParam.currentValueCollection !== undefined && dynamicParam.currentValueCollection.length !== 0) {
                    var paramValue = this.listBoxValueUpdated && this.listBoxParamValue !== null ? this.listBoxParamValue : parseFloat(dynamicParam.currentValueCollection[0].LiteralModeInfo.StartValue)
                    this.updateGridColumnValue(paramValue, args.rowIndex, currentData);
                } else {
                    $("#" + this.designId + "dashboardparameters").trigger("click");
                    this.gettingParameterValue(dynamicParam, args.rowIndex, currentData);
                }
            }
        } */
    },

/*     gettingParameterValue: function (dynamicParam, rowIndex, currentData) {
        var that = this;
        if (this.parameterTimer !== null && this.parameterTimer !== undefined) {
            clearTimeout(this.parameterTimer);
        }
        this.parameterTimer = setTimeout(function () {
            if (dynamicParam.currentValueCollection !== null && dynamicParam.currentValueCollection !== undefined && dynamicParam.currentValueCollection.length !== 0) {
                $("#" + that.designId + "_dashboardparameter_dialog_cancel").trigger("click");
                var paramValue = that.listBoxValueUpdated && that.listBoxParamValue !== null ? that.listBoxParamValue : parseFloat(dynamicParam.currentValueCollection[0].LiteralModeInfo.StartValue)
                that.updateGridColumnValue(paramValue, rowIndex, currentData);
            } else {
                that.gettingParameterValue(dynamicParam, rowIndex, currentData);
            }
        }, 0);
    },

    updateGridColumnValue: function (paramValue, rowIndex, currentData) {
        if (paramValue !== null) {
            this.model.dataSource[rowIndex][this.model.boundColumns.column[2].uniqueColumnName] = parseFloat(currentData);
            if (this.model.boundColumns.column.length >= 4) {
                this.model.dataSource[rowIndex][this.model.boundColumns.column[3].uniqueColumnName] = parseFloat(currentData) * paramValue;
            }
            this.updateGridData(this.model.dataSource);
        }
    }, */

/*     updateListBoxValueToGrid: function (paramValue) {
        if (this.model.boundColumns.column.length >= 4) {
            this.listBoxValueUpdated = true;
            this.listBoxParamValue = paramValue;
            for (var i = 0; i < this.model.dataSource.length; i++) {
                this.model.dataSource[i][this.model.boundColumns.column[3].uniqueColumnName] = this.model.dataSource[i][this.model.boundColumns.column[2].uniqueColumnName] * paramValue;
            }
            this.updateGridData(this.model.dataSource);
        }
    }, */

    updateInitialSelection: function () {
        this.getSelectedRowIndex();
        if (this.selectedRowIndex !== -1) {
            var widgetIns = $(this.element).closest('.e-customwidget-item').data('widgetInstance');
            if (widgetIns.widgetJson.FilterSettings.ActAsMasterWidget) {
                var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
                if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
                    var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
                    this.isUpdateInitialSelection = true;
                    gridObj.selectRow(this.selectedRowIndex);
                }
            }
        }
    },

    getSelectedRowIndex: function () {
        var widgetIns = $(this.element).closest('.e-customwidget-item').data('widgetInstance');
        if (widgetIns.widgetJson.SelectedFilterValues !== null && widgetIns.widgetJson.SelectedFilterValues.length !== 0) {
            var selectedValue = "";
            var filterValues = widgetIns.widgetJson.SelectedFilterValues[0];
            if (filterValues.InitialDateFilter.DateFilterList.length !== 0) {
                selectedValue = filterValues.InitialDateFilter.DateFilterList[0].toString();
            } else if (filterValues.InitialDimensionFilter.Text.length !== 0) {
                selectedValue = filterValues.InitialDimensionFilter.Text[0].toString();
            }
            this.selectedRowIndex = this.model.dataSource.findIndex(data => data[filterValues.UniqueColumnName].toString() === selectedValue);
        } else {
            this.selectedRowIndex = -1;
        }
    },

    gridRowSelected: function (selectedArgs) { // Event for grid row is selected
        if (this.designerObj.model.mode !== BoldBIDashboard.Designer.mode.design && !this.isUpdateInitialSelection) {
            var widgetInstance = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
            var fieldInfo = widgetInstance.dataGroupInfo.FieldContainers[0].FieldInfos;
            var ucnList = [];
            for (var j = 0; j < fieldInfo.length; j++) {
                if (fieldInfo[j].FieldActualType !== "e-reportdesigner-dataset-number") {
                    ucnList.push(fieldInfo[j].UniqueColumnName);
                }
            }
            if (this.selectedRowIndex !== selectedArgs.rowIndex && ucnList.length !== 0) {
                var selectedColumnsFilter = [];
                this.selectedRowIndex = selectedArgs.rowIndex;
                for (var i = 0; i < ucnList.length; i++) {
                    var filterColumn = new bbicustom.dashboard.selectedColumnInfo();
                    filterColumn.condition = "include";
                    filterColumn.uniqueColumnName = ucnList[i];
                    filterColumn.values = selectedArgs.data[ucnList[i]];
                    selectedColumnsFilter.push(filterColumn);
                }
                bbicustom.dashboard.filterData(this, selectedColumnsFilter);
            }
        } else {
            this.isUpdateInitialSelection = false;
        }
    },
    gridRowDeselected: function (args) { // Event for grid row is deselected
        if (!this.isDataUpdated) {
            var widgetInstance = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
            widgetInstance.clearFilter();
        } else {
            this.isDataUpdated = false;
        }
    },

    getGridColumns: function () {
        if (this.model.dataSource.length > 0) {
            this.gridColumns = [];
            //var columnKeys = Object.keys(this.model.dataSource[0]);
			if(this.model.boundColumns.column1.length > 0){
				 this.gridColumns.push({
                        field: this.model.boundColumns.column1[0].uniqueColumnName,
                        headerText: this.model.boundColumns.column1[0].columnName,
                        format: 'N2',
                        textAlign: 'Center',
                        width: "100",
						template: '#AvatarImgTemplate',
                    });
					this.AvatarImageColumn = this.model.boundColumns.column1[0].uniqueColumnName;
			}
			if(this.model.boundColumns.column.length > 0){
				for (var i = 0; i < this.model.boundColumns.column.length; i++) {
					this.gridColumns.push({
						field: this.model.boundColumns.column[i].uniqueColumnName,
						headerText: this.model.boundColumns.column[i].columnName,
						format: 'N2',
						textAlign: 'Left',
						width: "100",
					});
				}
			}
			if(this.model.boundColumns.column2.length > 0){
				 this.gridColumns.push({
                        field: this.model.boundColumns.column2[0].uniqueColumnName,
                        headerText: this.model.boundColumns.column2[0].columnName,
                        format: 'N2',
                        textAlign: 'Center',
                        width: this.model.properties.snippetImgWidth*4.5,
						template: '#ImageTemplate',
                    });
					this.SnippetImageColumn = this.model.boundColumns.column2[0].uniqueColumnName;
			}
        } 
		else {
            this.gridColumns = [{
                field: "Column",
                headerText: "Column",
                textAlign: 'Left',
                width: "100",
            }, {
                field: "Value",
                headerText: "Value",
                format: 'N2',
                textAlign: 'Left',
                width: "100",
            }];
        }
    },

    isWidgetConfigured: function () {
        return (this.model.boundColumns.column.length > 0 || this.model.boundColumns.column1.length > 0 || this.model.boundColumns.column2.length > 0);
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
        var widget = document.getElementById(this.element.getAttribute("id") + "_widget");
        $(widget).css({ "width": $(this.element).width() + "px", "height": ($(this.element).height() - 1) + "px" });
        if (option.type === "resize") {
            this.resizeGrid();
        }
        else if (option.type === "refresh") {
            this.isDataUpdated = true;
            this.renderGridElement();
        }
        else if (option.type === "propertyChange") {
            var that = this;
            switch (option.property.name) {
                case "showText":
                    widget.style.display = (option.property.value) ? "block" : "none";
                    break;
                case "text":
                    widget.innerHTML = option.property.value;
                    break;
                case "textBackground":
                    widget.style.backgroundColor = option.property.value;
                    break;
                case "textSize":
                    widget.style.fontSize = option.property.value + "px";
                    break;
                case "textStyle":
                    widget.style.fontStyle = option.property.value;
                    break;
                case "horizontal":
                    this.gridProperty.BasicSettings.horizontal = option.property.value;
                    this.updateGridLines();
                    break;
                case "vertical":
                    this.gridProperty.BasicSettings.vertical = option.property.value;
                    this.updateGridLines();
                    break;
                case "showBorder":
                    this.gridProperty.BasicSettings.showBorder = option.property.value;
                    this.updateBorderVisibility();
                    break;
                case "allowSorting":
                    this.gridProperty.BasicSettings.allowSorting = option.property.value;
                    this.updateAllowSorting();
                    break;
                case "enablealtrow":
                    this.gridProperty.BasicSettings.enableAltRow = option.property.value;
                    this.updateEnableAltRow();
                    this.updateContentProperty();
                    break;
                case "showHeader":
                    this.gridProperty.HeaderSettings.showHeader = option.property.value;
                    this.updateHeaderVisibility();
                    this.resizeGrid();
                    break;
                case "headerforeground":
                    this.gridProperty.HeaderSettings.foreground = option.property.value;
                    this.updateHeaderProperty();
                    break;
                case "headerbackground":
                    this.gridProperty.HeaderSettings.background = option.property.value;
                    this.updateHeaderProperty();
                    break;
                case "headerrowheight":
                    this.gridProperty.HeaderSettings.rowHeight = option.property.value;
                    if (this.headerHeightTimer !== null) {
                        clearTimeout(this.headerHeightTimer);
                    }
                    this.headerHeightTimer = setTimeout(function () {
                        that.updateHeaderProperty();
                        that.resizeGrid();
                    }, 500);
                    break;
                case "headerautofontsize":
                    this.gridProperty.HeaderSettings.autoFontSize = option.property.value;
                    this.updateHeaderProperty();
                    break;
                case "headerfontsize":
                    this.gridProperty.HeaderSettings.fontSize = option.property.value;
                    if (this.headerFontTimer !== null) {
                        clearTimeout(this.headerFontTimer);
                    }
                    this.headerFontTimer = setTimeout(function () {
                        that.updateHeaderProperty();
                    }, 500);
                    break;
                case "headerpadding":
                    this.gridProperty.HeaderSettings.padding = option.property.value;
                    if (this.headerPaddingTimer !== null) {
                        clearTimeout(this.headerPaddingTimer);
                    }
                    this.headerPaddingTimer = setTimeout(function () {
                        that.updateHeaderProperty();
                        that.resizeGrid();
                    }, 500);
                    break;
                case "contentforeground":
                    this.gridProperty.ContentSettings.foreground = option.property.value;
                    this.updateContentProperty();
                    break;
                case "contentbackground":
                    this.gridProperty.ContentSettings.background = option.property.value;
                    this.updateContentProperty();
                    break;
                case "contentrowheight":
                    this.gridProperty.ContentSettings.rowHeight = option.property.value;
                    if (this.contentHeightTimer !== null) {
                        clearTimeout(this.contentHeightTimer);
                    }
                    this.contentHeightTimer = setTimeout(function () {
                        that.updateContentProperty();
                    }, 500);
                    break;
                case "contentautofontsize":
                    this.gridProperty.ContentSettings.autoFontSize = option.property.value;
                    this.updateContentProperty();
                    break;
                case "contentfontsize":
                    this.gridProperty.ContentSettings.fontSize = option.property.value;
                    if (this.contentFontTimer !== null) {
                        clearTimeout(this.contentFontTimer);
                    }
                    this.contentFontTimer = setTimeout(function () {
                        that.updateContentProperty();
                    }, 500);
                    break;
                case "contentpadding":
                    this.gridProperty.ContentSettings.padding = option.property.value;
                    if (this.contentPaddingTimer !== null) {
                        clearTimeout(this.contentPaddingTimer);
                    }
                    this.contentPaddingTimer = setTimeout(function () {
                        that.updateContentProperty();
                    }, 500);
                    break;
                case "alternativeforeground":
                    this.gridProperty.ContentSettings.alternativeforeground = option.property.value;
                    this.updateContentProperty();
                    break;
                case "alternativebackground":
                    this.gridProperty.ContentSettings.alternativebackground = option.property.value;
                    this.updateContentProperty();
                    break;
                case "alllowedit":
                    this.gridProperty.BasicSettings.allowEdit = option.property.value;
                    this.updateEditSettings();
                    break;
                case "editmodetype":
                    this.gridProperty.BasicSettings.editModeType = option.property.value;
                    if (this.gridProperty.BasicSettings.allowEdit) {
                        this.updateEditSettings();
                    }
                    break;
                case "allowfilter":
                    this.gridProperty.BasicSettings.allowFilter = option.property.value;
                    this.updateAllowFilter();
                    this.updateFilterInputUI();
                    break;
				case "avatarImgHeight":
				case "avatarImgWidth":
				case "snippetImgHeight":
				case "snippetImgWidth":
				case "enableTextWrap":
				    this.renderGridElement();
				    return;
            }
        }
    },

    resizeGrid: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
            gridObj.height = $(this.element).height() - (this.getGridContentHeight() + 6);
            gridObj.width = $(this.element).width() - 2;
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
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
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
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
            gridObj.allowSorting = this.gridProperty.BasicSettings.allowSorting;
            gridObj.refresh();
        }
    },
    updateEnableAltRow: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
            gridObj.enableAltRow = this.gridProperty.BasicSettings.enableAltRow;
            gridObj.refresh();
        }
    },
    updateEditSettings: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
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
            var gridObj = ej2CustomGrid.base.getComponent(gridElement[0], 'grid');
            gridObj.allowFiltering = this.gridProperty.BasicSettings.allowFilter;
            gridObj.height = $(this.element).height() - (this.getGridContentHeight() + 5);
            gridObj.width = $(this.element).width() - 2;
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