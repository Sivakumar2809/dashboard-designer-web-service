/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"52ffacbc-bbeb-4e85-b6a4-3562656dba1e",

    widgetName:"CustomTreeGrid",

    init: function () {
        /* init method will be called when the widget is initialized */
        this.isGridCreated = false;
        this.childIndexes = [];
        this.childUcn = [];
        var widget = document.createElement("div");
        widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
        $(widget).css({ "height": $(this.element).height(), "width": $(this.element).width(), "border": "1px solid rgb(200,200,200)" });
        this.element.appendChild(widget);
        this.renderTreeGridElement();
		$('#'+this.element.id).addClass("customTreeGrid");
    },

    renderTreeGridElement: function () {
        this.getChildIndex();
        var data = this.getDataCollection();
        if (!this.isGridCreated) {
            this.isGridCreated = true;
            var widget = $(this.element).find("#" + this.element.getAttribute("id") + "_widget");
            var treeGridDiv = $("<div>").addClass(".e-dbrd-custom-treegrid-widget").attr("id", (this.element.getAttribute("id") + "_widget_grid"));
            treeGridDiv.css({ "height": "100%", "width": "100%" });
            widget.append(treeGridDiv);
            var gridObject = this.getTreeGridObject(data);
            gridObject.appendTo("#" + this.element.getAttribute("id") + "_widget_grid");
        } else {
            this.updateGridData(data);
        }
    },

    getChildIndex: function () {
        this.childIndexes = [];
        this.childUcn = [];
        if (this.model.boundColumns.column.length > 0) {
            var tempIndexes = [];
            if (!isNaN(Number(this.model.properties.childs))) {
                tempIndexes.push(this.model.properties.childs.toString());
                if (tempIndexes[0] !== this.model.properties.parent.toString() && parseInt(tempIndexes[0]) < this.model.boundColumns.column.length) {
                    this.childIndexes.push(tempIndexes[0]);
                    this.childUcn.push(this.model.boundColumns.column[parseInt(tempIndexes[0])].uniqueColumnName);
                }
            }
            else {
                tempIndexes.push(this.model.properties.childs);
                tempIndexes = tempIndexes[0].split(',');
                for (var i = 0; i < tempIndexes.length; i++) {
                    if (tempIndexes[i] !== this.model.properties.parent.toString() && parseInt(tempIndexes[i]) < this.model.boundColumns.column.length) {
                        this.childIndexes.push(tempIndexes[i]);
                        this.childUcn.push(this.model.boundColumns.column[parseInt(tempIndexes[i])].uniqueColumnName);
                    }
                }
            }
        }
    },

    getDataCollection: function () {
        if (this.model.dataSource.length > 0) {
            var modifiedData = [];
            if (this.childUcn.length === 0) {
                modifiedData = this.model.dataSource;
            } else {
                var parentUcn = this.model.boundColumns.column[parseInt(this.model.properties.parent)].uniqueColumnName;
                this.tempNames = [];
                this.tempObject = {};
                for (var i = 0; i < this.model.dataSource.length; i++) {
                    if (this.model.dataSource[i][parentUcn] !== "(Null)" && this.model.dataSource[i][parentUcn] !== "(Blanks)") {
                        if (modifiedData.length === 0) {
                            modifiedData.push(this.getData(this.model.dataSource[i]));
                        } else {
                            var index = modifiedData.findIndex(x => x["TaskName"] === this.model.dataSource[i][parentUcn]);
                            if (index === -1) {
                                modifiedData.push(this.getData(this.model.dataSource[i]));
                            }
                        }
                    }
                }
                for (var j = 0; j < this.childUcn.length; j++) {
                    this.updateSubLevelData(j, modifiedData, this.childUcn[j], (j === 0 ? parentUcn : this.childUcn[j - 1]));
                }
            }
            return modifiedData;
        } else {
            return [
                {
                    taskID: 1,
                    taskName: 'Item 1',
                    value1: 100,
                    value2: 5,
                    subtasks: [{ taskID: 2, taskName: 'Sub Item 11', value1: 5, value2: 100 }]
                },
                {
                    taskID: 3,
                    taskName: 'Item 2',
                    value1: 3,
                    value2: 86,
                    subtasks: [{ taskID: 4, taskName: 'Sub Item 21', value1: 3, value2: 60 }]
                },
                {
                    taskID: 5,
                    taskName: 'Item 3',
                    value1: 11,
                    value2: 66,
                    subtasks: [{ taskID: 6, taskName: 'Sub Item 31', value1: 50, value2: 11 }]
                }
            ];
        }
    },

    getData: function (currentData) {
        var tempData = {};
        for (var j = 0; j < this.model.boundColumns.column.length; j++) {
            if (this.childUcn.indexOf(this.model.boundColumns.column[j].uniqueColumnName) === -1) {
                var uniqueColumnName = this.model.boundColumns.column[parseInt(this.model.properties.parent)].uniqueColumnName === this.model.boundColumns.column[j].uniqueColumnName ? "TaskName" : this.model.boundColumns.column[j].uniqueColumnName;
                tempData[uniqueColumnName] = currentData[this.model.boundColumns.column[j].uniqueColumnName] === "(Null)" ? "" : currentData[this.model.boundColumns.column[j].uniqueColumnName];
            }
        }
        tempData["subTasks"] = [];
        return tempData;
    },

    updateSubLevelData: function (level, modifiedData, childUnique, parentUnique) {
        var dataSourceList = JSON.parse(JSON.stringify(this.model.dataSource));
        for (var i = 0; i < dataSourceList.length; i++) {
            var index = -1;
            var subLevelData = {};
            var subIndex = -1;
            var isUpdate = false;
            if (level === 0) {
                if (this.model.dataSource[i][childUnique] !== "(Null)" && this.model.dataSource[i][childUnique] !== "(Blanks)") {
                    index = modifiedData.findIndex(x => x["TaskName"] === this.model.dataSource[i][parentUnique]);
                    if (index !== -1) {
                        subIndex = modifiedData[index].subTasks.length > 0 ? modifiedData[index].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][childUnique]) : -1;
                        if (subIndex === -1) {
                            subLevelData = this.getData(this.model.dataSource[i]);
                            subLevelData["TaskName"] = this.model.dataSource[i][childUnique];
                            modifiedData[index].subTasks.push(subLevelData);
                        }
                    }
                }
            } else if (level === 1) {
                if (this.model.dataSource[i][childUnique] !== "(Null)" && this.model.dataSource[i][childUnique] !== "(Blanks)") {
                    for (var j = 0; j < modifiedData.length; j++) {
                        if (modifiedData[j].subTasks.length > 0) {
                            index = modifiedData[j].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][parentUnique]);
                            if (index !== -1) {
                                subIndex = modifiedData[j].subTasks[index].subTasks.length > 0 ? modifiedData[j].subTasks[index].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][childUnique]) : -1;
                                if (subIndex === -1) {
                                    subLevelData = this.getData(this.model.dataSource[i]);
                                    subLevelData["TaskName"] = this.model.dataSource[i][childUnique];
                                    modifiedData[j].subTasks[index].subTasks.push(subLevelData);
                                }
                                break;
                            }
                        }
                    }
                }
            } else if (level === 2) {
                if (this.model.dataSource[i][childUnique] !== "(Null)" && this.model.dataSource[i][childUnique] !== "(Blanks)") {
                    for (var j = 0; j < modifiedData.length; j++) {
                        if (modifiedData[j].subTasks.length > 0) {
                            for (var m = 0; m < modifiedData[j].subTasks.length; m++) {
                                index = modifiedData[j].subTasks[m].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][parentUnique]);
                                if (index !== -1) {
                                    subIndex = modifiedData[j].subTasks[m].subTasks[index].subTasks.length > 0 ? modifiedData[j].subTasks[m].subTasks[index].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][childUnique]) : -1;
                                    if (subIndex === -1) {
                                        subLevelData = this.getData(this.model.dataSource[i]);
                                        subLevelData["TaskName"] = this.model.dataSource[i][childUnique];
                                        modifiedData[j].subTasks[m].subTasks[index].subTasks.push(subLevelData);
                                    }
                                    isUpdate = true;
                                }
                            }
                        }
                        if (isUpdate) {
                            break;
                        }
                    }
                }
            } else if (level === 3) {
                if (this.model.dataSource[i][childUnique] !== "(Null)" && this.model.dataSource[i][childUnique] !== "(Blanks)") {
                    for (var j = 0; j < modifiedData.length; j++) {
                        if (modifiedData[j].subTasks.length > 0) {
                            for (var m = 0; m < modifiedData[j].subTasks.length; m++) {
                                for (var n = 0; n < modifiedData[j].subTasks[m].subTasks.length; n++) {
                                    index = modifiedData[j].subTasks[m].subTasks[n].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][parentUnique]);
                                    if (index !== -1) {
                                        subIndex = modifiedData[j].subTasks[m].subTasks[n].subTasks[index].subTasks.length > 0 ? modifiedData[j].subTasks[m].subTasks[n].subTasks[index].subTasks.findIndex(x => x["TaskName"] === this.model.dataSource[i][childUnique]) : -1;
                                        if (subIndex === -1) {
                                            subLevelData = this.getData(this.model.dataSource[i]);
                                            subLevelData["TaskName"] = this.model.dataSource[i][childUnique];
                                            modifiedData[j].subTasks[m].subTasks[n].subTasks[index].subTasks.push(subLevelData);
                                        }
                                        isUpdate = true;
                                    }
                                }
                            }
                        }
                        if (isUpdate) {
                            break;
                        }
                    }
                }
            }
        }
    },

    updateGridData: function (data) {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2.base.getComponent(gridElement[0], 'treegrid');
            gridObj.columns = this.getTreeGridColumns();
            gridObj.treeColumnIndex = this.getTreeColumnIndex(this.model.properties.parent, this.childIndexes);
            gridObj.dataSource = data;
        }
    },

    getTreeGridObject: function (data) {
        return new ej2.treegrid.TreeGrid({
            dataSource: data,
            childMapping: 'subTasks',
            treeColumnIndex: this.getTreeColumnIndex(this.model.properties.parent, this.childIndexes),
            allowPaging: this.model.properties.paging,
            allowFiltering: this.model.properties.filtering,
            allowTextWrapping: this.model.properties.wrapping,
            gridLines: this.model.properties.gridline,
            height: ($(this.element).height() - (this.model.properties.filtering ? 80 : 45)),
            width: $(this.element).width(),
            columns: this.getTreeGridColumns(),
            rowSelected: $.proxy(this.gridRowSelected, this)
        });
    },

    getTreeColumnIndex: function (parentIndex, childIndexCollection) {
        var index = Number(parentIndex);
        for (var i = 0; i < childIndexCollection.length; i++) {
            if (Number(childIndexCollection[i]) < Number(parentIndex)) {
                index = index - 1;
            }
        }
        return index;
    },

    getTreeGridColumns: function () {
        if (this.model.dataSource.length > 0 && this.model.boundColumns.column.length > 0) {
            var columns = [];
            if (this.childIndexes.length <= 0) {
                for (var i = 0; i < this.model.boundColumns.column.length; i++) {
                    var ucn = this.childUcn.length > 0 && this.model.boundColumns.column[i].uniqueColumnName === this.model.boundColumns.column[parseInt(this.model.properties.parent)].uniqueColumnName ? "TaskName" : this.model.boundColumns.column[i].uniqueColumnName;
                    columns.push({
                        field: ucn,
                        headerText: this.model.boundColumns.column[i].columnName,
                        textAlign: ((this.model.boundColumns.column[i].dataType === 'double') ? 'Right' : 'Left'),
                    });
                }
            }
            else {
                for (var i = 0; i < this.model.boundColumns.column.length; i++) {
                    if (this.childIndexes.indexOf(i.toString()) === -1) {
                        var uniqueColumnName = this.childUcn.length > 0 && this.model.boundColumns.column[i].uniqueColumnName === this.model.boundColumns.column[parseInt(this.model.properties.parent)].uniqueColumnName ? "TaskName" : this.model.boundColumns.column[i].uniqueColumnName;
                        columns.push({
                            field: uniqueColumnName,
                            headerText: this.model.boundColumns.column[i].columnName,
                            textAlign: ((this.model.boundColumns.column[i].dataType === 'double') ? 'Right' : 'Left'),
                        });
                    }
                }
            }
            return columns;
        } else {
            return [
                { field: 'taskID', headerText: 'Task ID', width: 70, textAlign: 'Right' },
                { field: 'taskName', headerText: 'Task Name', width: 200, textAlign: 'Left' },
                { field: 'value1', headerText: 'Value 1', width: 80, textAlign: 'Right' },
                { field: 'value2', headerText: 'Value 2', width: 80, textAlign: 'Right' }
            ];
        }
    },

    gridRowSelected: function (args) {
        var that = this;
        if ($(args.target).hasClass('e-treegridcollapse') !== true && $(args.target).hasClass('e-treegridexpand') !== true) {
            var UCN = "TaskName";
            var level = args.data.level;
            var selectedFilterInfos = [];
            var filterinfo = new bbicustom.dashboard.selectedColumnInfo();
            for (var l = 0; l <= level; l++) {
                if (l === 4) {
                    var data = args.data.parentItem.parentItem.parentItem.parentItem[UCN];
                    filterinfo.condition = "include";
                    filterinfo.uniqueColumnName = that.model.boundColumns.column[that.model.properties.parent].uniqueColumnName;
                    filterinfo.values.push(data);
                    selectedFilterInfos.push(filterinfo);
                    filterinfo = new bbicustom.dashboard.selectedColumnInfo();
                }
                if (l === 3) {
                    var data = args.data.parentItem.parentItem.parentItem[UCN];
                    var cn;
                    for (var i = 0; i < that.model.dataSource.length; i++) {
                        for (var j = 0; j < that.model.boundColumns.column.length; j++) {
                            if (data === that.model.dataSource[i][that.model.boundColumns.column[j].uniqueColumnName]) {
                                cn = that.model.boundColumns.column[j].uniqueColumnName;
                            }
                        }
                    }
                    filterinfo.condition = "include";
                    filterinfo.uniqueColumnName = cn;
                    filterinfo.values.push(data);
                    selectedFilterInfos.push(filterinfo);
                    filterinfo = new bbicustom.dashboard.selectedColumnInfo();
                }

                if (l === 2) {
                    var data = args.data.parentItem.parentItem[UCN];
                    var cn;
                    for (var i = 0; i < that.model.dataSource.length; i++) {
                        for (var j = 0; j < that.model.boundColumns.column.length; j++) {
                            if (data === that.model.dataSource[i][that.model.boundColumns.column[j].uniqueColumnName]) {
                                cn = that.model.boundColumns.column[j].uniqueColumnName;
                            }
                        }
                    }
                    filterinfo.condition = "include";
                    filterinfo.uniqueColumnName = cn;
                    filterinfo.values.push(data);
                    selectedFilterInfos.push(filterinfo);
                    filterinfo = new bbicustom.dashboard.selectedColumnInfo();
                }
                if (l === 1) {
                    var data = args.data.parentItem[UCN];
                    var cn;
                    for (var i = 0; i < that.model.dataSource.length; i++) {
                        for (var j = 0; j < that.model.boundColumns.column.length; j++) {
                            if (data === that.model.dataSource[i][that.model.boundColumns.column[j].uniqueColumnName]) {
                                cn = that.model.boundColumns.column[j].uniqueColumnName;
                            }
                        }
                    }
                    filterinfo.condition = "include";
                    filterinfo.uniqueColumnName = cn;
                    filterinfo.values.push(data);
                    selectedFilterInfos.push(filterinfo);
                    filterinfo = new bbicustom.dashboard.selectedColumnInfo();
                }
                if (l === 0) {
                    var data = args.data[UCN];
                    var cn;
                    for (var i = 0; i < that.model.dataSource.length; i++) {
                        for (var j = 0; j < that.model.boundColumns.column.length; j++) {
                            if (data === that.model.dataSource[i][that.model.boundColumns.column[j].uniqueColumnName]) {
                                cn = that.model.boundColumns.column[j].uniqueColumnName;
                            }
                        }
                    }
                    filterinfo.condition = "include";
                    filterinfo.uniqueColumnName = cn;
                    filterinfo.values.push(data);
                    selectedFilterInfos.push(filterinfo);
                    filterinfo = new bbicustom.dashboard.selectedColumnInfo();
                }
            }
            bbicustom.dashboard.filterData(that, selectedFilterInfos);
        }
    },

    getFormatData: function (colInfo, data) {
        if (colInfo.dataType === 'double') {
            return Number(data);
        }
        else if (colInfo.dataType === 'datetime') {
            return data;
        }
        return data;
    },

    update: function (option) {
        var widget = document.getElementById(this.element.getAttribute("id") + "_widget");
        if (option.type === "resize") {
            $(widget).css({ "height": $(this.element).height(), "width": $(this.element).width(), "border": "1px solid rgb(200,200,200)" });
            this.resizeGrid();
        }
        else if (option.type === "refresh") {
            this.renderTreeGridElement();
        }
        else if (option.type === "propertyChange") {
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
                case "paging":
                    this.model.properties.paging = option.property.value;
                    this.updateGridProperties();
                    return;
                case "filtering":
                    this.model.properties.filtering = option.property.value;
                    this.updateAllowFilter();
                    return;
                case "wrapping":
                    this.model.properties.wrapping = option.property.value;
                    this.updateGridProperties();
                    return;
                case "gridline":
                    this.model.properties.gridline = option.property.value;
                    this.updateGridProperties();
                    return;
                case "parent":
                    this.model.properties.parent = option.property.value;
                    var data = this.getDataCollection();
                    this.updateGridData(data);
                    return;
                case "childs":
                    this.model.properties.childs = option.property.value;
                    var data = this.getDataCollection();
                    this.updateGridData(data);
                    return;
            }
        }
    },

    resizeGrid: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2.base.getComponent(gridElement[0], 'treegrid');
            gridObj.height = $(this.element).height() - (this.model.properties.filtering ? 80 : 45);
            gridObj.width = $(this.element).width();
            gridObj.refresh();
        }
    },

    updateAllowFilter: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2.base.getComponent(gridElement[0], 'treegrid');
            gridObj.allowFiltering = this.model.properties.filtering;
            gridObj.height = $(this.element).height() - 80;
            gridObj.width = $(this.element).width();
            gridObj.refresh();
        }
    },

    updateGridProperties: function () {
        var gridElement = $(this.element).find("#" + this.element.getAttribute("id") + "_widget_grid");
        if (gridElement !== null && gridElement !== undefined && gridElement.length !== 0) {
            var gridObj = ej2.base.getComponent(gridElement[0], 'treegrid');
            gridObj.allowPaging = this.model.properties.paging;
            gridObj.allowTextWrapping = this.model.properties.wrapping;
            gridObj.gridLines = this.model.properties.gridline;
            gridObj.refresh();
        }
    },

});