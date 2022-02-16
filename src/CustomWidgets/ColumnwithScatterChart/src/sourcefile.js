/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid: "6f275fca-3c61-4915-1e23-05f5a626a3e2",

    widgetName: "ColumnwithScatterChart",
    init: function () {
        this.widget = document.createElement("div");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		this.element.appendChild(this.widget);
		this.chart = new ej2Chart.charts.Chart({
			width: this.element.clientWidth.toString(),
			height:this.element.clientHeight.toString(),
			primaryXAxis: {
				valueType: "Category",
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
			axes:
			[{
				visible: this.model.properties.showSecondaryaxis,
                majorGridLines: { width: this.model.properties.showGridLineForSecondary ? 1 : 0 }, 
				opposedPosition: this.model.boundColumns.Value.length > 0 ? true : false,
                lineStyle: { width: 0 },
				labelStyle: { size:this.model.properties.secondaryAxisLabelFontSize },
				title:this.model.properties.showSecondaryaxisTitle ? this.model.properties.showSecondaryaxisTitletext : "",
                name: 'yAxis'
            }],
			legendSettings: {visible:this.model.properties.showLegend, position: this.model.properties.legendPosition,shapeHeight: 15,
          shapeWidth: 15},
			tooltip: {
				enable: true
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
	legendRender: function(args){
		if(this.model.boundColumns.SValue.length > 0){
			for(var i = 0; i<this.model.boundColumns.SValue.length; i++){
				if((args.text == this.model.boundColumns.SValue[i].columnName || args.text == this.model.properties["scatterCustomLegend"+(i+1)])||((args.text).indexOf(this.model.boundColumns.SValue[i].columnName)> -1)){
					args.shape = "Circle";
				}
			}
		}
	},
	textRender: function(args){
		args.text = Number(args.text).toFixed(0);
	},
	getSeries: function(){
		var rowValues = [];
		if(this.model.boundColumns.Row.length > 0){
			for(var i = 0; i < this.model.dataSource.length; i++){
				if(rowValues.indexOf(this.model.dataSource[i][this.model.boundColumns.Row[0].uniqueColumnName]) == -1){
					rowValues.push(this.model.dataSource[i][this.model.boundColumns.Row[0].uniqueColumnName]);
				}
			}
		}
		var series = [];
		if((this.model.boundColumns.Value.length > 0 || this.model.boundColumns.SValue.length > 0) && this.model.dataSource.length > 0){
			var temp = 0;
			if(this.model.boundColumns.Row.length > 0 && rowValues.length > 0){
				for(var k=0; k<rowValues.length; k++){
				for(var i = 0; i<this.model.boundColumns.Value.length; i++){
				series.push({
					type: "Column",
					dataSource: this.getData(i,0,rowValues[k]),
					xName: "x",
					yName:'y',
					animation: {enable: this.model.properties.enableAnimation},
					name: (this.model.boundColumns.Row.length > 0 ? (rowValues[k]+'('+this.model.boundColumns.Value[i].columnName+')'):this.model.properties["columnCustomLegend"+(i+1)] != "" ? this.model.properties["columnCustomLegend"+(i+1)] : this.model.boundColumns.Value[i].columnName),
                    fill: this.model.properties['rc' + (temp > 14 ? ((temp % 14) + 1) : (temp + 1))],
					marker: { visible: false,height:this.model.properties.bubbleSize, width:this.model.properties.bubbleSize,dataLabel:{visible:this.model.properties.showDatalabels,font:{size:this.model.properties.datalabelFontSize}}}
				});
				temp++;
				}
				for(var i = 0; i<this.model.boundColumns.SValue.length; i++){
				series.push({
					type: "Line",
					dataSource: this.getData(i,1,rowValues[k]),
					xName: "x",
					yName:'y',
					width: 0,
					yAxisName: 'yAxis',
					animation: {enable: this.model.properties.enableAnimation},
					name: (this.model.boundColumns.Row.length > 0 ? (rowValues[k]+'('+this.model.boundColumns.SValue[i].columnName+')'):this.model.properties["scatterCustomLegend"+(i+1)] != "" ? this.model.properties["scatterCustomLegend"+(i+1)] : this.model.boundColumns.SValue[i].columnName),
					fill: this.model.properties['rc'+(temp>14?((temp%14)+1):(temp+1))],
                    marker: { visible: true, fill: this.model.properties['rc' + (temp > 14 ? ((temp % 14) + 1) : (temp + 1))],height:this.model.properties.bubbleSize, width:this.model.properties.bubbleSize,dataLabel:{visible:this.model.properties.showDatalabels,font:{size:this.model.properties.datalabelFontSize}}}
				});
				temp++;
				}
				}
			} else {
				for(var i = 0; i<this.model.boundColumns.Value.length; i++){
				series.push({
					type: "Column",
					dataSource: this.getData(i,0),
					xName: "x",
					yName:'y',
					animation: {enable: this.model.properties.enableAnimation},
					name: this.model.properties["columnCustomLegend"+(i+1)] != "" ? this.model.properties["columnCustomLegend"+(i+1)] : this.model.boundColumns.Value[i].columnName,
					fill: this.model.properties['cc'+(i+1)],
					marker: { visible: false,fill: this.model.properties['cc'+(i+1)],height:this.model.properties.bubbleSize, width:this.model.properties.bubbleSize,dataLabel:{visible:this.model.properties.showDatalabels,font:{size:this.model.properties.datalabelFontSize}}}
				});
				}
				for(var i = 0; i<this.model.boundColumns.SValue.length; i++){
				series.push({
					type: "Line",
					dataSource: this.getData(i,1),
					xName: "x",
					yName:'y',
					width: 0,
					yAxisName: 'yAxis',
					animation: {enable: this.model.properties.enableAnimation},
					name: this.model.properties["scatterCustomLegend"+(i+1)] != "" ? this.model.properties["scatterCustomLegend"+(i+1)] : this.model.boundColumns.SValue[i].columnName,
					fill: this.model.properties['sc'+(i+1)],
					marker: { visible: true,fill: this.model.properties['sc'+(i+1)],height:this.model.properties.bubbleSize, width:this.model.properties.bubbleSize,dataLabel:{visible:this.model.properties.showDatalabels,font:{size:this.model.properties.datalabelFontSize}}}
				});
				}
			}
			
		} else {
			return [
				{
					type: "Column",
					dataSource: [
						{ x: 'USA', y: 2 },{ x: "France", y: 1.5 },{ x: "Italy", y: 1 },
						{ x: 'India', y: 1.8 },{ x: "Germany", y: 2.6 },{ x: "China", y: 2.9 }
					],
					xName: "x",
					yName:'y',
					fill: "#1E90FF",
					marker: { visible: false,fill: "#1E90FF",height:10, width:10}
				},
				{
					type: "Line",
					dataSource: [
						{ x: 'USA', y: 4 },{ x: "France", y: 3.0 },{ x: "Italy", y: 3.8 },
						{ x: 'India', y: 3.4 },{ x: "Germany", y: 3.2 },{ x: "China", y: 3.9 }
					],
					xName: "x",
					yName:'y',
					width: 0,
					fill: "#1E90FF",
					marker: { visible: true,fill: "#1E90FF",height:10, width:10}
				}
			];
		}
		return series;
	},
	getData: function(valColIndex, seriesType, rowValue){
		var data = [];
		if(this.model.boundColumns.Column.length > 0 && this.model.boundColumns.Row.length > 0){
			for(var j = 0; j < this.model.dataSource.length; j++){
				if(seriesType == 0 && rowValue == this.model.dataSource[j][this.model.boundColumns.Row[0].uniqueColumnName]){
					data.push({x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], y:this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName]});
				} else if(rowValue == this.model.dataSource[j][this.model.boundColumns.Row[0].uniqueColumnName]) {
					data.push({x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], y:this.model.dataSource[j][this.model.boundColumns.SValue[valColIndex].uniqueColumnName]});
				}
			}
		}
		else if(this.model.boundColumns.Column.length > 0){
			for(var j = 0; j < this.model.dataSource.length; j++){
				if(seriesType == 0){
					data.push({x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], y:this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName]});
				} else {
					data.push({x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], y:this.model.dataSource[j][this.model.boundColumns.SValue[valColIndex].uniqueColumnName]});
				}
			}
		}else {
			if(seriesType == 0){
				data.push({x:this.model.boundColumns.Value[valColIndex].columnName, y:this.model.dataSource[0][this.model.boundColumns.Value[valColIndex].uniqueColumnName]});
			} else {
				data.push({x:this.model.boundColumns.SValue[valColIndex].columnName, y:this.model.dataSource[0][this.model.boundColumns.SValue[valColIndex].uniqueColumnName]});
			}
		}
		return data;
	},
    update: function (option) {
        if (option.type == "resize") {
			$(this.widget).css({'width':option.size.width, 'height': option.size.height});
			this.chart.width = this.element.clientWidth.toString();
			this.chart.height = this.element.clientHeight.toString();
			this.chart.refresh();
			return;
		} else if (option.type == "refresh") {
			this.element.innerHTML = "";
			this.init();
			return;
		}else if (option.type === "propertyChange") {
			switch (option.property.name) {
				case "cc1":
				case "cc2":
				case "cc3":
				case "cc4":
				case "cc5":
				case "sc1":
				case "sc2":
				case "sc3":
				case "sc4":
				case "sc5":
				case "columnCustomLegend1":
				case "columnCustomLegend2":
				case "columnCustomLegend3":
				case "columnCustomLegend4":
				case "columnCustomLegend5":
				case "scatterCustomLegend1":
				case "scatterCustomLegend2":
				case "scatterCustomLegend3":
				case "scatterCustomLegend4":
				case "scatterCustomLegend5":
					if(this.model.boundColumns.Row.length > 0){
						return;
					} else {
						this.element.innerHTML = "";
						this.init();
						return;
					}
				case "rc1":
				case "rc2":
				case "rc3":
				case "rc4":
				case "rc5":
				case "rc6":
				case "rc7":
				case "rc8":
				case "rc9":
				case "rc10":
				case "rc11":
				case "rc12":
				case "rc13":
				case "rc14":
				case "rc15":
					if(this.model.boundColumns.Row.length == 0){
						return;
					} else {
						this.element.innerHTML = "";
						this.init();
						return;
					}
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
				case "bubbleSize":
				case "legendPosition":
				case "showSecondaryaxis":
				case "showSecondaryaxisTitle":
				case "showSecondaryaxisTitletext":
				case "secondaryAxisLabelFontSize":
				case "showGridLineForSecondary":
					this.element.innerHTML = "";
					this.init();
					return;
			}
		}
    }
});