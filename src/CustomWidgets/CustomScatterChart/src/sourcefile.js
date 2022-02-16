/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"6cb36272-f03e-4a75-9987-9b339e1b4096",

    widgetName:"CustomScatterChart",

    init: function () {
        this.widget = document.createElement("div");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		this.element.appendChild(this.widget);
		this.chart = new ej2Chart.charts.Chart({
			width: this.element.clientWidth.toString(),
			height:this.element.clientHeight.toString(),
			primaryXAxis: {
				valueType: this.getCategory(),//"Category",
				edgeLabelPlacement: "Shift",
				majorGridLines: { width: this.model.properties.showGridLineForXaxis ? 1 : 0 },
				labelIntersectAction: this.model.properties.labelOverflowMode,
				title:this.model.properties.showCategoryAxisTitle ? this.model.properties.showCategoryAxisTitletext : "",
				visible: this.model.properties.showCategoryAxis,
				labelRotation: this.model.properties.labelRotation,
				labelStyle:{size:this.model.properties.xlabelFontSize}
			},
			primaryYAxis: {
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
			legendRender:$.proxy(this.legendRender, this)
		});
		this.chart.appendTo('#'+this.element.getAttribute("id") + "_widget");
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
		if(this.model.boundColumns.Column.length > 0 && this.model.boundColumns.Tooltip.length > 0){
			return '${point.text}<br/>X Value : <b>${point.x}</b><br/>Y Value : <b>${point.y}</b>';
		} else if(this.model.boundColumns.Column.length > 0){
			return 'X Value : <b>${point.x}</b><br/>Y Value : <b>${point.y}</b>';
		} else {
			return "";
		}
	},
	legendRender: function(args){
		/*if(this.model.boundColumns.SValue.length > 0){
			for(var i = 0; i<this.model.boundColumns.SValue.length; i++){
				if(args.text == this.model.boundColumns.SValue[i].columnName){
					args.shape = "Circle";
				}
			}
		}*/
	},
	textRender: function(args){
		args.text = Number(args.text).toFixed(0);
	},
	
	getCategory : function(){
		if(this.model.boundColumns.Value.length > 0 && this.model.boundColumns.Column.length > 0 && this.model.dataSource.length > 0){
			for(var i = 0; i < this.model.dataSource.length; i++){
				if(this.model.dataSource[i][this.model.boundColumns.Column[0].uniqueColumnName] != null && this.model.dataSource[i][this.model.boundColumns.Column[0].uniqueColumnName] != undefined){
					if(Number(this.model.dataSource[i][this.model.boundColumns.Column[0].uniqueColumnName]).toString() == "NaN"){
						return "Category";
					} else {
						return "Double";
					}
				}
			}
		} else {
			return "Category";
		}
	},
	getSeries: function(){
		var series = [];
		if(this.model.boundColumns.Value.length > 0 && this.model.dataSource.length > 0){
			for(var i = 0; i<this.model.boundColumns.Value.length; i++){
				series.push({
					type: "Scatter",
					dataSource: this.getData(i),
					xName: "x",
					yName:'y',
					animation: {enable: this.model.properties.enableAnimation},
					name: this.model.boundColumns.Value[i].columnName,
					fill: this.model.properties['cc'+(i+1)],
					marker: { visible: false,fill: this.model.properties['cc'+(i+1)],height:this.model.properties.bubbleSize, width:this.model.properties.bubbleSize,dataLabel:{visible:this.model.properties.showDatalabels,name: 'text'}}
				});
			}
		} else {
			return [
				{
					type: "Bubble",
					dataSource: [
						{ x: 'Item1', y: 2,size:1 },{ x: "Item2", y: 1.5,size:2 },{ x: "Item3", y: 1,size:3 },
						{ x: 'Item4', y: 1.8,size:4 },{ x: "Item5", y: 2.6,size:5 }
					],
					xName: "x",
					yName:'y',
					fill: "#1E90FF",
					marker: { visible: false,fill: "#1E90FF",height:10, width:10,name: 'text'}
				}
			];
		}
		return series;
	},
	getData: function(valColIndex){
		var data = [];
		if(this.model.boundColumns.Column.length > 0&& this.model.boundColumns.Tooltip.length > 0){
			for(var j = 0; j < this.model.dataSource.length; j++){
				data.push({
					x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], 
					y:this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName],
					text:this.model.dataSource[j][this.model.boundColumns.Tooltip[0].uniqueColumnName]
				});
			}
		} else if(this.model.boundColumns.Column.length > 0){
			for(var j = 0; j < this.model.dataSource.length; j++){
				data.push({
					x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], 
					y:this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName]
				});
			}
		} else {
			data.push({x:this.model.boundColumns.Value[valColIndex].columnName, y:this.model.dataSource[0][this.model.boundColumns.Value[valColIndex].uniqueColumnName]});
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
				case "cc1":
				case "cc2":
				case "cc3":
				case "cc4":
				case "cc5":
				case "legendPosition":
				case "bubbleSize":
					this.element.innerHTML = "";
					this.init();
					return;
			}
		}
    }
});