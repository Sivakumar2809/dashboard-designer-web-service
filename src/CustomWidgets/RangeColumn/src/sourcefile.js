/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"c12bc951-a461-4f7e-96e7-7d0a199c49c4",

    widgetName:"RangeColumn",

    init: function () {
        /* init method will be called when the widget is initialized */
		this.widgetInstance = $(this.element).closest(".e-customwidget-item").data("widgetInstance");
        this.widget = document.createElement("div");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
        this.element.appendChild(this.widget);
        //$(this.widget).css({ "width": $(this.element).width(), "height": $(this.element).height() });
        window.rangecolumn = this;
        $(this.widget).ejChart(
            {	              
                primaryXAxis:
                {   
                    valueType: 'category',
                    majorGridLines: { visible: this.model.properties.showGridLineForXaxis },
                    labelIntersectAction: this.model.properties.labelOverflowMode,
                    title: {
                        visible: this.model.properties.showCategoryAxisTitle,
                        text: this.model.properties.showCategoryAxisTitletext
                    },
					majorTickLines: { visible: this.model.properties.xmajorticks },
					minorTickLines: { visible: this.model.properties.xminorticks },
                    visible: this.model.properties.showCategoryAxis,
                    //labelRotation: this.model.properties.labelRotation,
                    font: {
                        size: this.model.properties.xlabelFontSize
                    }
                },	
                primaryYAxis:
                {   
                    range: { min: 0, max: 24, interval: 1 },
                    majorGridLines: {
                        visible: this.model.properties.showGridLineForYaxis
                    },
                    labelIntersectAction: 'Hide',
					majorTickLines: { visible: this.model.properties.ymajorticks },
					minorTickLines: { visible: this.model.properties.yminorticks },
                    labelRotation:-90,
                    title: {
                        visible: this.model.properties.showPrimaryAxisTitle,
                        text: this.model.properties.showPrimaryAxisTitletext
                    },
                    visible: this.model.properties.showPrimaryAxis,
                    font: {
                        size: this.model.properties.ylabelFontSize
                    },
                    opposedPosition: true
                },
                commonSeriesOptions:
                {
                    marker: {
                        dataLabel: { visible: this.model.properties.showDatalabels,offset:{y:-3}, textPosition: "middle", font: { color: 'white', size: this.model.properties.datalabelFontSize }}
                    },
                    tooltip: { visible: true },
                    type: 'rangecolumn',
                    enableAnimation: this.model.properties.enableAnimation,
                    isTransposed: true
                },
                series: this.getSeries(),
                size: { height: (this.element.clientHeight - 25).toString(), width: this.element.clientWidth.toString() },
                load: "loadTheme",
                isResponsive: true,
                legend: { visible: this.model.properties.showLegend },
                pointRegionClick: $.proxy(this.selectionChange, this),
                toolTipInitialize: function (args) {
                    if (rangecolumn.model.boundColumns.date.length > 0 && rangecolumn.model.boundColumns.sTime.length > 0 && rangecolumn.model.boundColumns.eTime.length > 0 && rangecolumn.model.boundColumns.person.length > 0) {
                        for (var i = 0; i < rangecolumn.model.dataSource.length; i++) {
                            if (rangecolumn.getDateFormatted(rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.date[0].uniqueColumnName]) == args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].x && rangecolumn.getHighValue(rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.eTime[0].uniqueColumnName],rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.eTime[1].uniqueColumnName]) == args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].high && rangecolumn.getLowValue(rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.sTime[0].uniqueColumnName],rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.sTime[1].uniqueColumnName]) == args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].low) {
                                args.data.currentText = 'Name :  ' + rangecolumn.model.dataSource[i][rangecolumn.model.boundColumns.person[0].uniqueColumnName] + '<br>' + 'Time:  ' + rangecolumn.getTooltip((args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].high > args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].low) ? (args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].high - args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].low) : (args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].low) - args.model.series[args.data.seriesIndex].dataPoint[args.data.pointIndex].high);
                            }
                        }
                    }
                },
                displayTextRendering:function(args){
			           for(var i=0; i< args.model.chartRegions.length; i++) {
			        	   var placement = rangecolumn.model.properties.labelPosition,
			        	   width = args.model.chartRegions[i].Region.Bounds.Width,
			        	   x= args.data.location.x;
			        	 if((args.model.chartRegions[i].SeriesIndex === args.data.seriesIndex) && (args.model.chartRegions[i].Region.PointIndex === args.data.pointIndex)) {
			             switch(placement){
			        		 case "Middle":
			        			args.data.location.x = x + width/2;
			        			break;
			        		 case "Left":
			        			args.data.location.x = x + 15;
			        			break;
			        		 case "Right":
			        			args.data.location.x = x + width-15;
			        			break; 
			        	 }
			           }
			          }
                    var argText = (args.model.series[args.data.seriesIndex].points[args.data.pointIndex].high > args.model.series[args.data.seriesIndex].points[args.data.pointIndex].low) ? (args.model.series[args.data.seriesIndex].points[args.data.pointIndex].high - args.model.series[args.data.seriesIndex].points[args.data.pointIndex].low) : (args.model.series[args.data.seriesIndex].points[args.data.pointIndex].low - args.model.series[args.data.seriesIndex].points[args.data.pointIndex].high);
                    var text = argText.toFixed(1).split('.');
                    var h = parseFloat(text[0]).toString().length > 1 ? parseFloat(text[0]).toString() : '0' + parseFloat(text[0]).toString();
                    if (text.length == 1) {
                        text[1] = 0;
                        var m = ((parseFloat(text[1]) / 10) * 60).toString().length > 1 ? (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString() : '0' + (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString();
                    }
                    var m = ((parseFloat(text[1]) / 10) * 60).toString().length > 1 ? (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString() : '0' + (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString();
                        if (args.model.series[args.data.seriesIndex].points[args.data.pointIndex].high === parseFloat(args.data.text)) {
                            args.data.text = " ";
                        } else {
                            args.data.text = h + ":" + m;
                        }
                },
                axesLabelRendering: function (args) {
                    
                    if (args.data.axis.orientation === "horizontal") {
                        args.data.label.Text = args.data.label.Text == 0 ? "" : args.data.label.Text.toString().length == 1 ? ('0' + args.data.label.Text) + ':' + '00' : (args.data.label.Text) + ':' + '00'; 
                    }
                    else {
						if(rangecolumn.model.boundColumns.date.length > 0){
							args.data.label.Text = (new Date(args.data.label.Text) != "Invalid Date" && rangecolumn.model.boundColumns.date[0].dateTimeFormat != 'dd/MM/yyyy') ? args.data.label.Text + "-" + rangecolumn.getday(args.data.label.Text) : args.data.label.Text + ' - ' + rangecolumn.getday(args.data.label.Text);
						}else{
							args.data.label.Text = args.data.label.Text + "-" + rangecolumn.getday(args.data.label.Text);
						}
					} 
                }
            });
    },
	
    getTooltip: function (value) {
        var text = value.toFixed(1).split('.');
        var h = parseFloat(text[0]).toString().length > 1 ? parseFloat(text[0]).toString() : '0' + parseFloat(text[0]).toString();
        if (text.length == 1) {
            text[1] = 0;
            var m = ((parseFloat(text[1]) / 10) * 60).toString().length > 1 ? (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString() : '0' + (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString();
        }
        var m = ((parseFloat(text[1]) / 10) * 60).toString().length > 1 ? (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString() : '0' + (((parseFloat(text[1]) / 10) * 60).toFixed(0)).toString();

        return h + ":" + m;
    },
    getday: function (date) {
        var days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
		if(date != "") {
		if(rangecolumn.model.boundColumns.date.length > 0){
			if(new Date(date) != "Invalid Date" && rangecolumn.model.boundColumns.date[0].dateTimeFormat != 'dd/MM/yyyy'){
				return days[new Date(date).getDay()];
			} else {
				var values = date.split('/');
				var ndate = new Date((values[1].length == 1 ? '0' + values[1]: values[1]) + '/' + (values[0].length == 1 ? '0' + values[0]: values[0]) + '/' + values[2]);
				return days[new Date(ndate).getDay()];
			}
		}else {
			return days[new Date(date).getDay()];
		}
		}
    },
    selectionChange: function (e) {
        if (this.model.boundColumns.date.length != 0 && this.model.boundColumns.eTime.length != 0 && this.model.boundColumns.sTime.length != 0 && this.model.boundColumns.person.length && this.model.boundColumns.filter.length > 0) {
            var fdata = [];
            var data = e.model._visibleSeries[e.data.region.SeriesIndex].dataPoint[e.data.region.Region.PointIndex];
            for (var i = 0; i < this.model.dataSource.length; i++) {
                if (data.x == this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.date[0].uniqueColumnName]) && fdata.length < 1) {
                    fdata.push(this.model.dataSource[i][this.model.boundColumns.person[0].uniqueColumnName]);
                }
            }
            var selectedFilterInfos = [];
            var filterinfo = new bbicustom.dashboard.selectedColumnInfo();
            filterinfo.condition = "include";
            filterinfo.uniqueColumnName = this.model.boundColumns.person[0].uniqueColumnName;
            filterinfo.values.push(fdata[0]);
            selectedFilterInfos.push(filterinfo);
            bbicustom.dashboard.filterData(this, selectedFilterInfos);
        }
    },
    getSeries: function () {
		window.color = [this.model.properties.color1, this.model.properties.color2, this.model.properties.color3, this.model.properties.color4, this.model.properties.color5];
        if (this.model.boundColumns.date.length > 0 && this.model.boundColumns.filter.length > 0 && this.model.boundColumns.eTime.length > 0 && this.model.boundColumns.sTime.length > 0 && this.model.boundColumns.person.length > 0) {
            var series = [];
            var per = [];
			this.uniqueueXValues = [];
			this.uniqueueFilterValues = [];
            for (var i = 0; i < this.model.dataSource.length; i++) {
                if (per.indexOf(this.model.dataSource[i][this.model.boundColumns.person[0].uniqueColumnName]) == -1) {
                    per.push(this.model.dataSource[i][this.model.boundColumns.person[0].uniqueColumnName]);
                } 
				if (this.uniqueueXValues.indexOf(this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.date[0].uniqueColumnName])) == -1) {
                    this.uniqueueXValues.push(this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.date[0].uniqueColumnName]));
                }
				if (this.uniqueueFilterValues.indexOf(this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.filter[0].uniqueColumnName])) == -1) {
                    this.uniqueueFilterValues.push(this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.filter[0].uniqueColumnName]));
                }
            }
			
            for (var j = 0; j < per.length; j++) {
                series.push({
                    points: this.getPoints(per[j]),
                    name: per[j],
					fill:color[j]
                });
            }
			
			for(var i = 0; i < series.length; i++){
				for(var k = 0; k < this.uniqueueXValues.length; k++){
					var valueExist = false;
					for(var l = 0; l < series[i].points.length; l++){
						if(this.uniqueueXValues[k] == series[i].points[l].x){
							valueExist = true;
						}
					}
					if(!valueExist){
						series[i].points.push({x:this.uniqueueXValues[k], low:0, high:0,test:this.uniqueueFilterValues[k]});
					}
					var sortDirection = "";
					if(this.model.boundColumns.filter.length > 0 && this.widgetInstance.dataGroupInfo.FieldContainers[4].FieldInfos[0].CustomSorting != null && this.widgetInstance.dataGroupInfo.FieldContainers[4].FieldInfos[0].CustomSorting != undefined){
						sortDirection = this.widgetInstance.dataGroupInfo.FieldContainers[4].FieldInfos[0].CustomSorting.CustomSortOrder;
					}
					if(sortDirection == "Descending"){
						series[i].points = (series[i].points).sort(function(a, b) {
							return new Date(b['test']) - new Date(a['test']);
						});
					} else {
						series[i].points = (series[i].points).sort(function(a, b) {
							return new Date(a['test']) - new Date(b['test']);
						});
					}
					
				}
			}
            return series;
        } else {
            return [
                {
                    points: this.getPoints(),
                    name: 'Series 1',
					fill:color[0]
                },

            ];
        }

    },
    getPoints: function (name) {
        if (this.model.boundColumns.date.length > 0 && this.model.boundColumns.filter.length > 0 && this.model.boundColumns.eTime.length > 0 && this.model.boundColumns.sTime.length > 0 && this.model.boundColumns.person.length > 0) {
            var point = [];
            for (var i = 0; i < this.model.dataSource.length; i++) {
				var lowValue = this.getLowValue(this.model.dataSource[i][this.model.boundColumns.sTime[0].uniqueColumnName],this.model.dataSource[i][this.model.boundColumns.sTime[1].uniqueColumnName]);
				var highValue = this.getHighValue(this.model.dataSource[i][this.model.boundColumns.eTime[0].uniqueColumnName],this.model.dataSource[i][this.model.boundColumns.eTime[1].uniqueColumnName]);
                if (name == this.model.dataSource[i][this.model.boundColumns.person[0].uniqueColumnName] && (lowValue > 0 || highValue > 0)) {
                    point.push({
                        x: this.getDateFormatted(this.model.dataSource[i][this.model.boundColumns.date[0].uniqueColumnName]),
                        low: lowValue,
                        high: highValue,
						test: this.model.dataSource[i][this.model.boundColumns.filter[0].uniqueColumnName]
                    });
                }
            }
            return point;
        } else {
            return [
                { x: '01/01/2019', low: 2.5, high: 6.5 },
                { x: '02/10/2019', low: 3, high: 6.5 },
                { x: '03/20/2019', low: 4.5, high: 8.5 },
                { x: '04/30/2019', low: 3.5, high: 10.5 },
                { x: '05/05/2019', low: 5.5, high: 11.5 },
                { x: '06/15/2019', low: 1.5, high: 11.5 },
                { x: '07/25/2019', low: 4.5, high: 11.5 },
                { x: '03/08/2019', low: 2.5, high: 11.5 },
                { x: '06/09/2019', low: 8.5, high: 11.5 },
                { x: '09/10/2019', low: 6.0, high: 11.5 },
                { x: '11/18/2019', low: 1.5, high: 6.5 },
                { x: '12/27/2019', low: 5.0, high: 11.5 }
            ];
        }
    },
    getLowValue: function (value,seconds) {
		if(value === "null" || value === "(Null)") {
			value = null;
		}
        if (new Date(value) != "Invalid Date") {
            return (((new Date(value).getHours() == 24 && new Date(value).getMinutes() > 0)? 0 : new Date(value).getHours()) + (new Date(value).getMinutes() / 60) + (new Date(value).getSeconds() / 3600));
        } else {
            var values = value.split(':');
			var hours = values[0];
			if(value.split(" ")[1] === "PM" && Number(values[0]) !=12) {
				hours = Number(values[0]) + 12;
			}
			if (value.split(" ")[1] === "AM" && Number(values[0]) === 12) {
				hours = 0;
			}
            var h, m, s;
		   h = (Number(hours) == 24 && Number(hours) > 0) ? 0 : Number(hours);
		   m = Number((Number(values[1].split(" ")[0]) / 60).toFixed(1));
		    s = seconds != "(NULL)" ? Number((Number(seconds)/3600).toFixed(1)) : 0;
           return (h + m + s);
        }
    },
    getHighValue: function (value,seconds) {
		if(value === "null" || value === "(Null)") {
			value = null;
		}
        if (new Date(value) != "Invalid Date") {
            return (((new Date(value).getHours() == 24 && new Date(value).getMinutes() > 0) ? 0 : new Date(value).getHours()) + (new Date(value).getMinutes() / 60) + (new Date(value).getSeconds() / 3600));
        } else {
            var values = value.split(':');
			var hours = values[0];
			if(value.split(" ")[1] === "PM" && Number(values[0]) !=12) {
				hours = Number(values[0]) + 12;
			}
			if (value.split(" ")[1] === "AM" && Number(values[0]) === 12) {
				hours = 0;
			}
            var h, m, s;
		   h = (Number(hours) == 24 && Number(hours) > 0) ? 0 : Number(hours);
		   m = Number((Number(values[1].split(" ")[0]) / 60).toFixed(1));
		    s = seconds != "(NULL)" ? Number((Number(seconds)/3600).toFixed(1)) : 0;
           return (h + m + s);
        }
    },
	getDateFormatted : function(value) {
	var date, day;
	if (value.toString().includes("/")){
		date = value.toString().split(' ')[0];
	} else if(isNaN(Number(value.toString().split(" ")[0]))) {
		var month = new Date(value).getMonth() + 1;
		month = month < 10 ? "0" + month.toString() : month.toString();
		date = month + "/" + value.toString().split(' ')[1] + "/" + value.split(' ')[2];
	}  else if(!isNaN(Number(value.toString().split(" ")[0]))) {
		var month = new Date(value).getMonth() + 1;
		month = month < 10 ? "0" + month.toString() : month.toString();
		date = value.toString().split(' ')[0] + "/" + month + "/" + value.toString().split(' ')[2];
	}
	return date;
	},
    //getAngle: function (args) {
    //    switch (args) {
    //        case "0�":
    //            return 0;
    //            break;
    //        case "-45�":
    //            return -45;
    //            break;
    //        case "-90�":
    //            return -90;
    //            break;
    //        case "45�":
    //            return 45;
    //            break;
    //        case "90�":
    //            return 90;
    //            break;
    //    }
    //},
    update: function (option) {
        var widgetObj = $(this.widget).data("ejChart");
        /* update method will be called when any update needs to be performed in the widget. */

        if (option.type == "resize") {
            /* update type will be 'resize' if the widget is being resized. */
            widgetObj.model.size.width = option.size.width.toString();
            widgetObj.model.size.height = option.size.height.toString();
            
        }
        else if (option.type == "refresh") {
            /* update type will be 'refresh' when the data is refreshed. */
            this.model.dataSource = option.data;
            this.element.innerHTML = '';
            this.init();
            return;
        }
        else if (option.type == "propertyChange") {
            /* update type will be 'propertyChange' when any property value is changed in the designer. */
            //var widgetObj = $(this.widget).data("ejChart");
            switch (option.property.name) {
                case "showGridLineForXaxis":
                    widgetObj.model.primaryXAxis.majorGridLines.visible = option.property.value;
                    break;
                case "showGridLineForYaxis":
                    widgetObj.model.primaryYAxis.majorGridLines.visible = option.property.value;
                    break;
                case "enableAnimation":
                    widgetObj.model.commonSeriesOptions.enableAnimation = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "showLegend":
                    widgetObj.model.legend.visible = option.property.value;
                    break;
                case "showDatalabels":
                    widgetObj.model.commonSeriesOptions.marker.dataLabel.visible = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "showCategoryAxis":
                    widgetObj.model.primaryXAxis.visible = option.property.value;
                    break;
                case "showCategoryAxisTitle":
                    widgetObj.model.primaryXAxis.title.visible = option.property.value;
                    break;
                case "showCategoryAxisTitletext":
                    widgetObj.model.primaryXAxis.title.text = option.property.value;
                    break;
                case "labelOverflowMode":
                    widgetObj.model.primaryXAxis.labelIntersectAction = option.property.value;
                    break;
                case "labelRotation":
                    widgetObj.model.primaryXAxis.labelRotation = option.property.value;
                    break;
                case "xlabelFontSize":
                    widgetObj.model.primaryXAxis.font.size = option.property.value;
                    break;
                case "showPrimaryAxis":
                    widgetObj.model.primaryYAxis.visible = option.property.value;
                    break;
                case "showPrimaryAxisTitle":
                    widgetObj.model.primaryYAxis.title.visible = option.property.value;
                    break;
                case "showPrimaryAxisTitletext":
                    widgetObj.model.primaryYAxis.title.text = option.property.value;
                    break;
                case "ylabelFontSize":
                    widgetObj.model.primaryYAxis.font.size = option.property.value;
                    break;
				case 'datalabelFontSize':
                    this.model.properties.datalabelFontSize = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
				case "color1":
                    this.model.properties.color1 = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "color2":
                    this.model.properties.color2 = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "color3":
                    this.model.properties.color3 = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "color4":
                    this.model.properties.color4 = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "color5":
                    this.model.properties.color5 = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
				case "xmajorticks":
                    this.model.properties.xmajorticks = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "xminorticks":
                    this.model.properties.xminorticks = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "ymajorticks":
                    this.model.properties.ymajorticks = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
                case "yminorticks":
                    this.model.properties.yminorticks = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;
				case "labelPosition":
                    this.model.properties.labelPosition = option.property.value;
                    this.element.innerHTML = '';
                    this.init();
                    return;
                    break;	
					
            }
        }
        widgetObj.redraw();
    }
});