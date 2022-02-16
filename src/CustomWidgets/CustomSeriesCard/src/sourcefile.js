/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"4af15a7a-772f-4602-8e02-9528120083f8",

    widgetName:"CustomSeriesCard",

    init: function () {
        this.isCardElementCreated = false;
        this.isUpdateUI = false;
        this.designId = $(this.element).parents(".e-customwidget-item").attr("id").split("_" + this.model.widgetId)[0];
        this.cardTitle = ["BIG PEAT", "BOWMORE", "BRECHIN (NORTH PORT)", "BRORA", "BRUICHLADDICH", "BUNNAHABHAIN"];
        this.cardTitleWidth = {};
        this.classNameList = ["e-icon-arrow2_pointer_down", "e-icon-arrow2_round", "e-icon-arrow2_pointer_up"];
        this.colorValue = ["#ff0000", "#ffee01", "#09e051"];
        this.realTimeData = [];
        this.dataFieldInfo = [];
        this.columnName = null;
        this.colorSettings = { backgroundColor: "transparent", titleColor: "#f5f8ff", valueColor: "#ffffff" };
        this.isRealTimeDataRequest = false;
        var widget = document.createElement("div");
        widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
        $(widget).css({ "width": "100%", "height": "100%" });
        $(widget).addClass("e-dbrd-custom-widget-card");
        this.element.appendChild(widget);
        this.updateHeaderElement();
        this.renderCardWidget();
    },

    renderCardWidget: function () {
        var widget = document.getElementById(this.element.getAttribute("id") + "_widget");
        this.getDataSourceInfoFromPropertyPanel();
        if (this.columnName !== null && this.columnName !== undefined && this.columnName !== "") {
            if (this.realTimeData.length > 0) {
                var currentValue = this.realTimeData[this.realTimeData.length - 1][this.columnName];
                if (!this.isCardElementCreated) {
                    $(this.element).parents(".e-customwidget-item").find(".ej-background-waiting-popup").css("display", "none");
                    this.isCardElementCreated = true;

                    var titleElement = $("<div>").addClass("e-dbrd-custom-widget-card-title");
                    titleElement.css({ "line-height": $(widget).height() + "px", "height": $(widget).height() + "px" });
                    titleElement.html(this.getTitleValue(this.columnName));

                    var valueEle = $("<div>").addClass("e-dbrd-custom-widget-card-value-container");
                    valueEle.css({
                        "float": "left",
                        "width": "34%",
                        "height": "100%"
                    });
                    $(widget).append(titleElement, valueEle);

                    var iconElement = $("<div>").addClass("e-color-custom-icon " + this.classNameList[currentValue < 0 ? 0 : currentValue > 0 ? 2 : 1] + " e-dbrd-custom-widget-card-icon");
                    iconElement.css({
                        "height": $(widget).height() + "px",
                        "color": this.colorValue[currentValue < 0 ? 0 : currentValue > 0 ? 2 : 1],
                        "padding-top": ($(widget).height() - 14) / 2 + "px"
                    });

                    var valueElement = $("<div>").addClass("e-dbrd-custom-widget-card-value");
                    valueElement.css({
                        "line-height": $(widget).height() + "px",
                        "height": $(widget).height() + "px",
                        "width": 100 - ((16 / valueElement.width()) * 100) + "%",
                    });
                    valueElement.html(currentValue + "%");
                    valueEle.append(iconElement, valueElement);

                    if (this.isRealTimeDataRequest) {
                        window.isFetchRealTimeData = false;
                        this.updateDataForRealTime();
                    }
                } else {
                    $(widget).find(".e-dbrd-custom-widget-card-value").html(currentValue + "%");
                    var iconDiv = $(widget).find(".e-dbrd-custom-widget-card-icon");
                    iconDiv.removeClass();
                    iconDiv.addClass("e-color-custom-icon " + this.classNameList[currentValue < 0 ? 0 : currentValue > 0 ? 2 : 1] + " e-dbrd-custom-widget-card-icon");
                    iconDiv.css("color", this.colorValue[currentValue < 0 ? 0 : currentValue > 0 ? 2 : 1]);
                    if (this.isRealTimeDataRequest) {
                        window.isFetchRealTimeData = false;
                        this.updateDataForRealTime();
                    }
                }
            } else {
                this.updateDataForRealTime();
            }
            this.updateColorFormatting(widget);
        }
    },

    updateColorFormatting: function (widget) {
        $(widget).parents(".e-customwidget-item").css({ "background": this.colorSettings.backgroundColor });
        $(this.element).css({ "background": this.colorSettings.backgroundColor });
        $(widget).find(".e-dbrd-custom-widget-card-title").css({ "color": this.colorSettings.titleColor });
        $(widget).find(".e-dbrd-custom-widget-card-value").css({ "color": this.colorSettings.valueColor });
    },

    getTitleValue: function (title) {
        switch (title) {
            case "Bigpeat":
                return this.cardTitle[0];
            case "Bowmore":
                return this.cardTitle[1];
            case "Brechin":
                return this.cardTitle[2];
            case "Brora":
                return this.cardTitle[3];
            case "Bruichladdich":
                return this.cardTitle[4];
            case "Bunnahabhain":
                return this.cardTitle[5];
            default:
                return title;
        }
    },

    getCardTitleWidth: function () {
        var designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
        for (var i = 0; i < this.cardTitle.length; i++) {
            var bounds = designerObj.measureText(this.cardTitle[i], $(widget).width(), { size: 12 });
            this.cardTitleWidth[this.cardTitle[i]] = bounds.width;
        }
    },

    updateHeaderElement: function () {
        var controlContainer = $(this.element).parents(".e-customwidget-item");
        $(this.element).css({ "background": "transparent" });
        controlContainer.find(".e-dbrd-control-container-wrapper").css({ "padding": "12px 0px" });
        controlContainer.find(".e-dbrd-control-header").hide();
        controlContainer.find(".e-dbrd-control-sub-header").hide();
        controlContainer.css({ "border-radius": "8px", "box-shadow": "none" });
        controlContainer.find(".e-dbrd-content-container").css({
            "height": "calc(100%)",
            "padding": "0px"
        });
    },

    isWidgetConfigured: function () {
        return this.model.boundColumns.column.length > 0;
    },

    resizeCardWidget: function (widget) {
        if (widget !== null) {
            var valueElem = $(widget).find(".e-dbrd-custom-widget-card-value-container");
            $(widget).find(".e-dbrd-custom-widget-card-title").css({
                "line-height": $(widget).height() + "px", "height": $(widget).height() + "px"
            });
            $(widget).find(".e-dbrd-custom-widget-card-icon").css({
                "height": $(widget).height() + "px",
                "padding-top": ($(widget).height() - 14) / 2 + "px"
            });
            $(widget).find(".e-dbrd-custom-widget-card-value").css({
                "line-height": $(widget).height() + "px",
                "height": $(widget).height() + "px",
                "width": 100 - ((16 / valueElem.width()) * 100) + "%"
            });
        }
    },

    updateUIForAllControls: function () {
        var that = this;
        if (this.timer !== null) {
            clearTimeout(this.timer);
        }
        this.timer = setTimeout(function () {
            that.isUpdateUI = true;
            var reportItem = $("#" + that.designId).find(".e-customwidget-item");
            for (var i = 0; i < reportItem.length; i++) {
                if (reportItem.eq(i).hasClass("e-line-item")) {
                    reportItem.eq(i).css({ "box-shadow": "none", "background": "transparent" });
                }
            }
        }, 1);
    },

    updateDataForRealTime: function () {
        window.isFetchRealTimeData = window.isFetchRealTimeData !== null && window.isFetchRealTimeData !== undefined && window.isFetchRealTimeData;
        if (!window.isFetchRealTimeData) {
            window.isFetchRealTimeData = true;
            var that = this;
            $(this.element).parents(".e-customwidget-item").find(".ej-background-waiting-popup").css("display", this.isRealTimeDataRequest ? "none" : "block");
            setTimeout(function () {
                that.isRealTimeDataRequest = true;
                $.ajax({
                    type: "GET",
                    url: "https://demo-realtime.boldbi.com/realtime/api/data-stream/world-whisky",
                    async: true,
                    crossDomain: true,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: "",
                    success: $.proxy(that.responseData, that)
                });
            }, 1500);
        }
    },

    makeDataRequest: function (fieldInfoList, dataSourceName) {
        var widgetInfo = {
            "Id": "CustomWidget6",
            "Name": "CustomWidget6",
            "ControlType": "CustomWidget",
            "DataGroupInfo": [
                {
                    "__type": "Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.FieldContainer",
                    "FieldInfos": [],
                    "canUseInLinking": true,
                    "IsDrillSection": false,
                    "RestrictLargeData": false,
                    "Required": true,
                    "Name": "Value",
                    "DisplayName": "Value",
                    "ContainerType": 0,
                    "CategoryCount": 1
                },
                {
                    "__type": "Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.FieldContainer",
                    "FieldInfos": [],
                    "canUseInLinking": true,
                    "IsDrillSection": false,
                    "RestrictLargeData": false,
                    "Required": true,
                    "Name": "Column",
                    "DisplayName": "Column",
                    "ContainerType": 1,
                    "CategoryCount": 4
                }
            ]
        };
        $("#" + this.designId).data("BoldBIDashboardDesigner").modules.util.doAjaxPost('POST', ((this.model.baseURL.split("bi/designer")[0]) + "bi/designer/" + BoldBIDashboard.Dashboard.ServiceAPI.processdata), {
            action: BoldBIDashboard.DashboardUtil.DesignerAction.fetchData,
            data: JSON.stringify({
                'className': BoldBIDashboard.DashboardUtil.DesignerAction.WidgetAction,
                'designerAction': BoldBIDashboard.DashboardUtil.DesignerAction.fetchData,
                'fieldInfo': fieldInfoList,
                'dataset': dataSourceName,
                'widgetInfo': widgetInfo,
                'request': "Widget",
                'selectedValuesList': []
            })
        }, {
                fnction: $.proxy(this.responseData, this)
            }
        );
    },

    responseData: function (data) {
        if (data !== null && data !== undefined && data !== "{}") {
            this.realTimeData.push(data);
            var widgetEleCollection = $("#" + this.designId).find(".e-dbrd-custom-widget-card");
            for (var i = 0; i < widgetEleCollection.length; i++) {
                var widgetIns = widgetEleCollection.eq(i).parents(".e-customwidget-item").data('widgetInstance');
                if (widgetIns !== null && widgetIns.customWidgetObj !== null && !widgetIns.customWidgetObj.isRealTimeDataRequest) {
                    widgetIns.customWidgetObj.realTimeData = this.realTimeData;
                    widgetIns.customWidgetObj.renderCardWidget();
                }
            }
            this.renderCardWidget();
        }
    },

    getDataSourceInfoFromPropertyPanel: function () {
        if (this.columnName === null || this.columnName === undefined || this.columnName === "") {
            this.columnName = this.model.properties.columnname;
        }
        if (this.model.properties.backgroundcolor !== null && this.model.properties.backgroundcolor !== undefined &&
            this.model.properties.backgroundcolor !== "" && this.model.properties.backgroundcolor !== this.colorSettings.backgroundColor) {
            this.colorSettings.backgroundColor = this.model.properties.backgroundcolor;
        }
        if (this.model.properties.titlecolor !== null && this.model.properties.titlecolor !== undefined &&
            this.model.properties.titlecolor !== "" && this.model.properties.titlecolor !== this.colorSettings.titleColor) {
            this.colorSettings.backgroundColor = this.model.properties.titlecolor;
        }
        if (this.model.properties.valuecolor !== null && this.model.properties.valuecolor !== undefined &&
            this.model.properties.valuecolor !== "" && this.model.properties.valuecolor !== this.colorSettings.valueColor) {
            this.colorSettings.backgroundColor = this.model.properties.valuecolor;
        }
    },

    update: function (option) {
        var widget = document.getElementById(this.element.getAttribute("id") + "_widget");
        if (option.type === "resize") {
            this.updateHeaderElement();
            this.resizeCardWidget(widget);
        }
        else if (option.type === "refresh") {
            //if (option.data !== null && option.data !== undefined) {
            //    $(this.element).parents(".e-customwidget-item").find(".ej-background-waiting-popup").css("display", "none");
            //    this.realTimeData.push(option.data);
            //    this.renderCardWidget();
            //}
        }
        else if (option.type === "propertyChange") {
            /* update type will be 'propertyChange' when any property value is changed in the designer. */

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
                case "columnname":
                    this.columnName = option.property.value;
                    break;
                case "backgroundcolor":
                    this.colorSettings.backgroundColor = option.property.value;
                    $(widget).parents(".e-customwidget-item").css({ "background": option.property.value });
                    $(this.element).css({ "background": option.property.value });
                    break;
                case "titlecolor":
                    this.colorSettings.titleColor = option.property.value;
                    $(widget).find(".e-dbrd-custom-widget-card-title").css({ "color": option.property.value });
                    break;
                case "valuecolor":
                    this.colorSettings.valueColor = option.property.value;
                    $(widget).find(".e-dbrd-custom-widget-card-value").css({ "color": option.property.value });
                    break;
            }
        }
    }
});