/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid: "0f175fca-3c61-1549-9e81-04f4a626a3e2",

    widgetName: "WaterfallChart",
    init: function () {
        this.widget = document.createElement("div");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		this.element.appendChild(this.widget);
		this.chart = new ej2WaterfallChart.charts.Chart({
			width: this.element.clientWidth.toString(),
			height:this.element.clientHeight.toString(),
			primaryXAxis: {
				valueType: "Category",
				edgeLabelPlacement: "Shift",
				majorGridLines: { width: this.model.properties.categorygridline ? 1:0 },
				title:this.model.properties.customcategoryaxis?this.model.properties.categoryaxistitle:"",
				visible: this.model.properties.categoryaxis,
				labelIntersectAction:this.model.properties.labeloverflowmode == "Trim"?"":this.model.properties.labeloverflowmode,
				labelRotation:this.model.properties.labelrotation,
				labelStyle:{size:12}
			},
			primaryYAxis: {
				labelFormat: "{value}%",
				majorGridLines: { width: this.model.properties.primarygridline ? 1:0 }, 
				visible: this.model.properties.primaryvalueaxis, 
				labelStyle: { size:12 },
				title:this.model.properties.customprimaryvalueaxis?this.model.properties.primaryvalueaxistitle:"",
				minimum:0
			},
			series:this.getSeries(),
			legendSettings: {visible:false, position: 'Bottom'},
			isTransposed:(this.model.properties.orientations == 'Vertical' ? false:true),
			tooltip: {
				enable: true
			},
			textRender: $.proxy(this.textRender, this),
			tooltipRender: $.proxy(this.tooltipRender, this)
		});
		this.chart.appendTo('#'+this.element.getAttribute("id") + "_widget");
    },
	tooltipRender:function(args){
		args.text = args.text.replaceAll('.000','');
	},
	textRender:function(args){
		args.text = Number(args.text.replaceAll('%','')).toFixed(0).toString()+'%';
	},
	getSeries: function(){
		var series = [];
		if(this.model.boundColumns.Column.length > 0 && this.model.boundColumns.Value.length > 0 && this.model.dataSource.length > 0){
			for(var i = 0; i<this.model.boundColumns.Value.length; i++){
				series.push({
					type: 'Waterfall',fill:this.model.properties.increasecolor,negativeFillColor: this.model.properties.decreasecolor,xName: 'x', yName: 'y', intermediateSumIndexes: [], sumIndexes: [this.model.dataSource.length],dataSource:this.getData(i),marker: { dataLabel: { position:'Outer',visible: this.model.properties.showvaluelabel, font: {color: '#000' } } }, connector: { color: '#5F6A6A', width: 0.5 },summaryFillColor:this.model.properties.totalcolor, animation: { enable: this.model.properties.animation }
				});
			}
		} else {
			return [
				{
					type: 'Waterfall',fill:this.model.properties.increasecolor,negativeFillColor: this.model.properties.decreasecolor,xName: 'x', yName: 'y', intermediateSumIndexes: [], sumIndexes: [6],dataSource:[{ x: 'Income', y: 4711 },{ x: 'Sales', y: -1015 },{ x: 'Development', y: -688 },{ x: 'Revenue', y: 1030 },{ x: 'Expense', y: -361 },{ x: 'Tax', y: -695 },{ x: 'Total'}],marker: { dataLabel: { visible: this.model.properties.showvaluelabel, font: {color: '#ffffff' } } }, connector: { color: '#5F6A6A', width: 0.5 },summaryFillColor:this.model.properties.totalcolor, animation: { enable: this.model.properties.animation }
				}
			];
		}
		return series;
	},
	getData: function(valColIndex){
		var data = [];
		//var maxValue = this.GetMaxValue();
		for(var j = 0; j<this.model.dataSource.length; j++){
			data.push({x:this.model.dataSource[j][this.model.boundColumns.Column[0].uniqueColumnName], y:(this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName] == 0 ?0.00001:this.model.dataSource[j][this.model.boundColumns.Value[valColIndex].uniqueColumnName])});
		}
		if(this.model.properties.showtotal){
			data.push({x:this.model.properties.totaltitle});
		}
		return data;
	},
	GetMaxValue: function(){
		var maxValue = 0;
		for(var i = 0; i<this.model.dataSource.length; i++){
			if(this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName] > maxValue){
				maxValue = this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName];
			}
		}
		return maxValue;
	},
    update: function (option) {
        if (option.type == "resize") {
			//this.element.innerHTML = "";
			//this.init();
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
				case "animation":
				case "showvaluelabel":
				case "orientations":
				case "increasecolor":
				case "decreasecolor":
				case "categoryaxis":
				case "customcategoryaxis":
				case "categoryaxistitle":
				case "primaryvalueaxis":
				case "customprimaryvalueaxis":
				case "labelrotation":
				case "labeloverflowmode":
				case "primaryvalueaxistitle":
				case "primarygridline":
				case "categorygridline":
				case "showtotal":
				case "totaltitle":
				case "totalcolor":
					this.element.innerHTML = "";
					this.init();
					return;
			}
		}
    }
});