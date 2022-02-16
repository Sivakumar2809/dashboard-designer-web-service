/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"7c7d05a9-9823-4453-ab82-481e7c63729d",

    widgetName:"CustomScatterCharWithStripLine",

    init: function () {
        this.widget = document.createElement("div");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		this.element.appendChild(this.widget);
		this.uniqueRowValues = [];
		this.stripLines = [];
		this.total = 0;
		this.dataLength = 0;
		if(this.model.boundColumns.Value.length > 0 && this.model.boundColumns.Column.length > 0 && this.model.boundColumns.row.length > 0 && this.model.dataSource.length > 0){
			for(var i = 0; i < this.model.dataSource.length; i++){
				if(this.uniqueRowValues.indexOf(this.model.dataSource[i][this.model.boundColumns.row[0].uniqueColumnName]) == -1 && this.model.dataSource[i][this.model.boundColumns.row[0].uniqueColumnName] != '(Blanks)' && this.model.dataSource[i][this.model.boundColumns.row[0].uniqueColumnName] != '(Null)'){
					this.total += Number((this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0));
					this.dataLength ++;
					this.uniqueRowValues.push(this.model.dataSource[i][this.model.boundColumns.row[0].uniqueColumnName]);
				} else {
					this.total += Number((this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0));
					this.dataLength ++;
				}
			}
			var totalPercentage = Number((this.total/this.dataLength).toFixed(0));
			this.stripLines.push({start: (totalPercentage-this.model.properties.striplinewidth), end: totalPercentage, text: (totalPercentage+this.model.properties.totalavgtext), color: 'orange',textStyle: { size: this.model.properties.totalfontsize+'px', color: 'grey', fontWeight: '600' },border: { width: 0 }, rotation: 0, visible: true
			});
		} else if(this.model.boundColumns.Value.length > 0 && this.model.boundColumns.Column.length > 0 && this.model.dataSource.length > 0){
			for(var i = 0; i < this.model.dataSource.length; i++){
				if(this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName] != '(Blanks)' && this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName] != '(Null)'){
					this.total += Number((this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0));
					this.dataLength ++;
				}
			}
			var totalPercentage = Number((this.total/this.dataLength).toFixed(0));
			this.stripLines.push({start: (totalPercentage-this.model.properties.striplinewidth), end: totalPercentage, text: (totalPercentage+this.model.properties.totalavgtext), color: 'orange',textStyle: { size: this.model.properties.totalfontsize+'px', color: 'grey', fontWeight: '600' },border: { width: 0 }, rotation: 0, visible: true
			});
		}
		if(this.model.boundColumns.Value.length > 0 && this.model.boundColumns.Column.length > 0 && this.model.boundColumns.row.length > 0 && this.model.dataSource.length > 0){
		this.chart = new ej2dashboard.charts.Chart({
			width: this.element.clientWidth.toString(),
			height:this.element.clientHeight.toString(),
			primaryXAxis: {
				valueType: "Double",
				stripLines: this.stripLines,
				majorGridLines: { width: this.model.properties.showGridLineForXaxis ? 1 : 0 },
				labelIntersectAction: this.model.properties.labelOverflowMode,
				title:this.model.properties.showCategoryAxisTitle ? this.model.properties.showCategoryAxisTitletext : "",
				visible: this.model.properties.showCategoryAxis,
				labelRotation: this.model.properties.labelRotation,
				labelStyle:{size:this.model.properties.xlabelFontSize}
			},
			primaryYAxis: {
				minimum: Number(this.model.properties.min),
				maximum: Number(this.model.properties.max),
				majorGridLines: { width: this.model.properties.showGridLineForYaxis ? 1 : 0 }, 
				visible: this.model.properties.showPrimaryAxis, 
				labelStyle: { size:this.model.properties.ylabelFontSize },
				title:this.uniqueRowValues[0]+'_'+this.model.boundColumns.Value[0].columnName,
				labelIntersectAction: "Hide",
				titlePadding: 30
			},
			rows:this.getRows(),
			axes:this.getAxes(),
			legendSettings: {visible:this.model.properties.showLegend, position: this.model.properties.legendPosition},
			tooltip: {
				enable: true,
				format: this.getFormat()
			},
			series: this.getSeries(),
			pointClick:$.proxy(this.pointClick, this),
			textRender:$.proxy(this.textRender, this),
			legendRender:$.proxy(this.legendRender, this),
			axisLabelRender:$.proxy(this.axisLabelRender, this),
			loaded:$.proxy(this.loaded, this)

		});
		} else {
			this.chart = new ej2dashboard.charts.Chart({
			width: this.element.clientWidth.toString(),
			height:this.element.clientHeight.toString(),
			primaryXAxis: {
				valueType: "Double",
				stripLines: this.stripLines,
				majorGridLines: { width: this.model.properties.showGridLineForXaxis ? 1 : 0 },
				labelIntersectAction: this.model.properties.labelOverflowMode,
				title:this.model.properties.showCategoryAxisTitle ? this.model.properties.showCategoryAxisTitletext : "",
				visible: this.model.properties.showCategoryAxis,
				labelRotation: this.model.properties.labelRotation,
				labelStyle:{size:this.model.properties.xlabelFontSize}
			},
			primaryYAxis: {
				minimum: Number(this.model.properties.min),
				maximum: Number(this.model.properties.max),
				majorGridLines: { width: this.model.properties.showGridLineForYaxis ? 1 : 0 }, 
				visible: this.model.properties.showPrimaryAxis, 
				labelStyle: { size:this.model.properties.ylabelFontSize },
				title:this.model.properties.showPrimaryAxisTitle ? this.model.properties.showPrimaryAxisTitletext : "",
				labelIntersectAction: "Hide"
			},
			legendSettings: {visible:this.model.properties.showLegend, position: this.model.properties.legendPosition},
			tooltip: {
				enable: true,
				format: this.getFormat()
			},
			series: this.getSeries(),
			pointClick:$.proxy(this.pointClick, this),
			textRender:$.proxy(this.textRender, this),
			legendRender:$.proxy(this.legendRender, this),
			axisLabelRender:$.proxy(this.axisLabelRender, this),
			loaded:$.proxy(this.loaded, this)
		});
		}
		this.chart.appendTo('#'+this.element.getAttribute("id") + "_widget");
    },
	getRows: function(args){
		var rows = [];
		if(this.uniqueRowValues.length > 0){
			var axesPercent = Number((100/this.uniqueRowValues.length).toFixed(0));
			for(var i = 0; i < this.uniqueRowValues.length; i++){
				rows.push({'height':axesPercent+'%'});
			}
		}
		return rows;
	},
	getAxes: function(args){
		var axes = [];
		if(this.uniqueRowValues.length > 0){
			for(var i = 0; i < this.uniqueRowValues.length; i++){
				if(i > 0){
				axes.push({
					majorGridLines: { width: this.model.properties.showGridLineForYaxis ? 1 : 0 },
					rowIndex: i, opposedPosition: false,
					lineStyle: { width: 0 },
					name: 'yAxis'+i, title: this.uniqueRowValues[i]+'_'+this.model.boundColumns.Value[0].columnName,
					labelFormat: '{value}',
					titlePadding: 30
				});
				}
			}
		}
		return axes;
	},
	loaded: function(args){
		for(var i = 0; i < this.uniqueRowValues.length; i++){
			$("#"+this.widget.getAttribute("id")+"_AxisTitle_"+(i+1)).removeAttr('transform');
		}
	},
	axisLabelRender: function(args){
		if(args.axis.orientation != "Vertical"){
			args.text = args.text + this.model.properties.casuffixtext;
		}
	},
	pointClick: function(args){
		var widgetIns = $(this.element).closest('.e-customwidget-item').data('widgetInstance');
		if(this.model.boundColumns.Value.length > 0 && this.model.boundColumns.Column.length > 0 && widgetIns.designerInstance.model.mode != 'design'){
			var selectedColumnsFilter = [];
			var filterColumn = new bbicustom.dashboard.selectedColumnInfo();
			filterColumn.condition = "include";
			filterColumn.uniqueColumnName = this.model.boundColumns.Column[0].uniqueColumnName;
			filterColumn.values =[args.point.x] ;
			selectedColumnsFilter.push(filterColumn);
			bbicustom.dashboard.filterData(this, selectedColumnsFilter);
		}
	},
	getFormat: function(){
		if(this.model.boundColumns.Column.length > 0 && this.model.boundColumns.row.length > 0){
			return this.model.boundColumns.Column[0].columnName+' : <b>${point.x}</b><br/>'+this.model.boundColumns.Value[0].columnName+' : <b>${point.y}</b>';
		} else if(this.model.boundColumns.Column.length > 0){
			return this.model.boundColumns.Column[0].columnName+' : <b>${point.x}</b><br/>'+this.model.boundColumns.Value[0].columnName+' : <b>${point.y}</b>';
		} else {
			return "";
		}
	},
	legendRender: function(args){
	},
	textRender: function(args){
		args.text = Number(args.text).toFixed(0);
	},
	getSeries: function(){
		var series = [];
		if(this.model.boundColumns.Value.length > 0 && this.model.dataSource.length > 0 && this.model.boundColumns.Column.length > 0 && this.model.boundColumns.row.length > 0){
			for(var i = 0; i < this.uniqueRowValues.length; i++){
				if(i == 0){
				series.push({
					type: "Scatter",
					dataSource: this.getData(this.uniqueRowValues[i],i),
					xName: "x",
					yName:'y',
					animation: {enable: this.model.properties.enableAnimation},
					fill: this.model.properties.scolor,
					name:this.uniqueRowValues[i],
					border: { width: Number(this.model.properties.bubbleSize), color: this.model.properties.scolor ,height: Number(this.model.properties.bubbleSize)},
					marker: { visible: false, dataLabel:{visible:this.model.properties.showDatalabels,name: 'text',font:{size:this.model.properties.datalabelFontSize}}}
				});
				} else {
					series.push({
					type: "Scatter",
					dataSource: this.getData(this.uniqueRowValues[i],i),
					xName: "x",
					yName:'y',
					yAxisName: 'yAxis'+i,
					name: this.uniqueRowValues[i],
					animation: {enable: this.model.properties.enableAnimation},
					fill: this.model.properties.scolor,
					border: { width: Number(this.model.properties.bubbleSize),height: Number(this.model.properties.bubbleSize), color: this.model.properties.scolor },
					marker: { visible: false, dataLabel:{visible:this.model.properties.showDatalabels,name: 'text',font:{size:this.model.properties.datalabelFontSize}}}
				});
				}
			}
		} else if(this.model.boundColumns.Value.length > 0 && this.model.dataSource.length > 0 && this.model.boundColumns.Column.length > 0){
				series.push({
					type: "Scatter",
					dataSource: this.getData(),
					xName: "x",
					yName:'y',
					animation: {enable: this.model.properties.enableAnimation},
					name: this.model.boundColumns.Value[0].columnName,
					fill: this.model.properties.scolor,
					border: { width: Number(this.model.properties.bubbleSize),height: Number(this.model.properties.bubbleSize), color: this.model.properties.scolor },
					marker: { visible: false, dataLabel:{visible:this.model.properties.showDatalabels,name: 'text',font:{size:this.model.properties.datalabelFontSize}}}
				});
		} else {
			return [
				{
					type: "Scatter",
					dataSource: [
						{ x: 10, y: 20 },{ x: 20, y: 15},{ x: 30, y: 10 },
						{ x: 40, y: 18 },{ x: 50, y: 26 }
					],
					xName: "x",
					yName:'y',
					fill: this.model.properties.scolor,
					marker: { visible: false,fill: "#1E90FF",height:10, width:10,name: 'text'}
				}
			];
		}
		return series;
	},
	getData: function(rowData, rowIndex){
		var data = [];
		if(rowData != undefined){
			var tempTotal = 0;
			var tempMaxValue = 0;
			for(var j = 0; j < this.model.dataSource.length; j++){
				if(rowData == this.model.dataSource[j][this.model.boundColumns.row[0].uniqueColumnName] && this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName] != "(Blanks)" && this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName] != "(Blanks)" && this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName] != "(Null)" && this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName] != "(Null)"&& this.model.dataSource[j][this.model.boundColumns.row[0].uniqueColumnName] != "(Null)" && this.model.dataSource[j][this.model.boundColumns.row[0].uniqueColumnName] != "(Null)"){
					tempTotal += Number((this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0));
					if(tempMaxValue < Number((this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0))){
						tempMaxValue = Number((this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0));
					}
					data.push({
						x:Number((this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName]).toFixed(0)), 
						y:Number((this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0))
					});
				}
			}
			var seriesPercentage = Number((tempTotal/data.length).toFixed(0));
			if(rowData != this.uniqueRowValues[0]){
				this.stripLines.push({start: (seriesPercentage-this.model.properties.striplinewidth), end: seriesPercentage, text: (seriesPercentage+this.model.properties.subavgtext), color: 'green',textStyle: { size: this.model.properties.subfontsize+'px', color: 'grey', fontWeight: '600' },border: { width: 0 }, rotation: 0, visible: true
			,isSegmented:true,segmentStart:this.model.properties.min,segmentEnd:(tempMaxValue+1000000), segmentAxisName:('yAxis'+rowIndex)});
			} else {
				this.stripLines.push({start: (seriesPercentage-this.model.properties.striplinewidth), end: seriesPercentage, text: (seriesPercentage+this.model.properties.subavgtext), color: 'green',textStyle: { size: this.model.properties.subfontsize+'px', color: 'grey', fontWeight: '600' },border: { width: 0 }, rotation: 0, visible: true
			,isSegmented:true,segmentStart:this.model.properties.min,segmentEnd:(tempMaxValue+1000000)});
			}
		} else {
			for(var j = 0; j < this.model.dataSource.length; j++){
				if(this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName] != "(Blanks)" && this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName] != "(Blanks)" && this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName] != "(Null)" && this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName] != "(Null)"){
					data.push({
						x:Number((this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName]).toFixed(0)), 
						y:Number((this.model.dataSource[j][this.model.boundColumns.Value[0].uniqueColumnName]).toFixed(0))
					});
				}
			}
		}
		return data;
	},
    update: function (option) {
        if (option.type == "resize") {
			this.element.innerHTML = "";
			this.init();
			return;
		} else if (option.type == "refresh") {
			this.element.innerHTML = "";
			this.init();
			return;
		}else if (option.type === "propertyChange") {
			switch (option.property.name) {
				case "showGridLineForXaxis":
                case "showGridLineForYaxis":
                case "enableAnimation":
                case "showLegend":
                case "showDatalabels":
				case 'datalabelFontSize':
                case "showCategoryAxis":
                case "showCategoryAxisTitle":
                case "showCategoryAxisTitletext":
                case "labelOverflowMode":
                case "labelRotation":
                case "xlabelFontSize":
                case "showPrimaryAxis":
                case "showPrimaryAxisTitle":
                case "showPrimaryAxisTitletext":
                case "ylabelFontSize":
				case "legendPosition":
				case "bubbleSize":
				case "TV":
				case "min":
				case "max":
				case "totalavgtext":
				case "subavgtext":
				case "striplinewidth":
				case "scolor":
				case "subfontsize":
				case "totalfontsize":
				case "casuffixtext":
					this.element.innerHTML = "";
					this.init();
					return;
			}
		}
    }
});