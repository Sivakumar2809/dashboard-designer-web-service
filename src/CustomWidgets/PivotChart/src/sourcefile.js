/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"7fe5b33e-a101-47b1-8f5e-5e2f073c0f92",

    widgetName:"PivotChart",

    init: function () {
        /* init method will be called when the widget is initialized */
		this.designId = $(this.element).parents(".e-customwidget-item").attr("id").split("_" + this.model.widgetId)[0];
		this.monthNames = this.model.properties.sort == "Ascending" ? ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"] : ["Dec","Nov","Oct","Sep","Aug","Jul","Jun","May","Apr","Mar","Feb","Jan"];
		this.yearscoll = [];
		this.monthYear = false;
		this.isMonthDeducted = true;
		this.tempDS = [];
		this.data = [{
            "In_Stock": 34,
            "aSold": 51,
            "aAmount": 383,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Retail Outlets",
            "aYear": "FY 2015",
            "Quarter": "Q1"
        },
        {
            "In_Stock": 4,
            "aSold": 423,
            "aAmount": 3595.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Sales Person",
            "aYear": "FY 2015",
            "Quarter": "Q1"
        },
        {
            "In_Stock": 38,
            "aSold": 234,
            "aAmount": 1813.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Teleshopping",
            "aYear": "FY 2015",
            "Quarter": "Q1"
        },
        {
            "In_Stock": 42,
            "aSold": 127,
            "aAmount": 952.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "App Store",
            "aYear": "FY 2015",
            "Quarter": "Q1"
        },
        {
            "In_Stock": 36,
            "aSold": 89,
            "aAmount": 668,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Retail Outlets",
            "aYear": "FY 2015",
            "Quarter": "Q2"
        },
        {
            "In_Stock": 17,
            "aSold": 340,
            "aAmount": 2890,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Sales Person",
            "aYear": "FY 2015",
            "Quarter": "Q2"
        },
        {
            "In_Stock": 22,
            "aSold": 379,
            "aAmount": 2937.25,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Teleshopping",
            "aYear": "FY 2015",
            "Quarter": "Q2"
        },
        {
            "In_Stock": 12,
            "aSold": 269,
            "aAmount": 2017.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "App Store",
            "aYear": "FY 2015",
            "Quarter": "Q2"
        },
        {
            "In_Stock": 28,
            "aSold": 15,
            "aAmount": 113,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Retail Outlets",
            "aYear": "FY 2015",
            "Quarter": "Q3"
        },
        {
            "In_Stock": 46,
            "aSold": 369,
            "aAmount": 3136.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Sales Person",
            "aYear": "FY 2015",
            "Quarter": "Q3"
        },
        {
            "In_Stock": 16,
            "aSold": 410,
            "aAmount": 3177.5,
            "aCountry": "France",
            "Product_Categories": "Accessories",
            "aProducts": "Bottles and Cages",
            "Order_Source": "Teleshopping",
            "aYear": "FY 2015",
            "Quarter": "Q3"
        }];
		if((this.model.boundColumns.value.length > 0 || this.model.boundColumns.column.length > 0 || this.model.boundColumns.row.length > 0) && this.model.dataSource.length > 0){
			this.data = this.model.dataSource;
		}
		this.widget = document.createElement("div");
		this.widget.setAttribute('id', this.element.id+'_chart');
		var width = this.element.clientWidth;
		var height = this.element.clientHeight;
		$(this.widget).css({"height": '100%'});
		this.element.appendChild(this.widget);
		var that = this;
		this.pivotObj = new ej2chart.pivotview.PivotView({
			dataSourceSettings: {
				enableSorting: false,
				expandAll: this.model.properties.expandAll,
				rows: this.getRows(),
				columns: this.getColumns(),
				values: this.getValues(),
				dataSource: this.data,
				enableSorting: false,
				filters: []
			},
			showFieldList: false,
			displayOption: { view: 'Chart' },
			chartSettings: {
				enableMultiAxis: true,
				chartSeries: { 
					type: this.model.properties.chartType,
					animation:{enable: this.model.properties.animation},
					fill: ((this.model.properties.chartType == 'StackingColumn' || !this.model.properties.cchartColor) ? "" : this.model.properties.chartColor),
					width: 3
				},
				zoomSettings:{
					toolbarItems: []
				},
				primaryXAxis:{
					majorGridLines:{width:this.model.properties.glxaxis ? 1:0},
					visible: this.model.properties.showxaxis,
					title:this.model.properties.showxaxistitle ? this.model.properties.xaxistitletext : " ",
					labelIntersectAction: this.model.properties.labelintersectaction
				},
				primaryYAxis:{
					majorGridLines:{width:this.model.properties.glyaxis ? 1:0},
					visible: this.model.properties.showyaxis,
					title:this.model.properties.showyaxistitle ? "" : "  "
				},
				legendSettings:{
					visible: this.model.properties.showLegend,
					position:this.model.properties.legendPosition
				},
				axisLabelRender: $.proxy(this.axisLableCustomization, this),
				tooltipRender: $.proxy(this.tooltipCustomization, this),
				seriesRender: $.proxy(this.seriesRender, this)
			},
			dataBound: $.proxy(this.dataBoundEvent, this),
			height:height,
			width: width
		});
		this.pivotObj.appendTo('#'+this.element.id+'_chart');
	},
	seriesRender:function(args){
		args.series.legendShape = this.model.properties.legendShape;
	},
	tooltipCustomization: function(args){
		for(var i = 0; i < this.model.boundColumns.value.length; i++){
			if(args.text.indexOf(this.model.boundColumns.value[i].columnName) > -1){
				var splittext = args.text.split('<br/>');
				for(var j = 0; j < splittext.length; j++){
					if(splittext[j].indexOf(this.model.boundColumns.value[i].columnName) > -1){
						var value = splittext[j].split(':')[1];
						var designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
						var formattedText = BoldBIDashboard.DashboardUtil.formattedText((value.replaceAll(',','')), "auto", this.model.properties.decimal, "Currency", JSON.parse('{"__type":"Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.Separator","AliasValue":".","CurrentValue":"."}'), JSON.parse('{"__type":"Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.Separator","AliasValue":",","CurrentValue":","}'), this.model.properties.prefix,this.model.properties.suffix, this.model.properties.rep, true, designerObj);
						formattedText = formattedText.indexOf('$$') > -1 ? formattedText.replaceAll('$$','$'): formattedText;
						args.text = args.text.replace(value, " "+formattedText);
					}
				}
			}
		}
		if(this.model.boundColumns.column.length > 0){
			if(args.text.indexOf('Column') > -1){
				args.text = args.text.replaceAll('Column',this.model.boundColumns.column[0].columnName);
			}
		}
		if(this.model.boundColumns.row.length > 0){
			if(args.text.indexOf('Row') > -1){
				args.text = args.text.replaceAll('Row',this.model.boundColumns.row[0].columnName);
			}
		}
	},
	axisLableCustomization : function(args){
		if(args.axis.orientation == "Vertical" && this.model.properties.useFormat){
			var designerObj = $("#" + this.designId).data("BoldBIDashboardDesigner");
			args.text = BoldBIDashboard.DashboardUtil.formattedText(args.value+'', "auto", this.model.properties.decimal, "Currency", JSON.parse('{"__type":"Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.Separator","AliasValue":".","CurrentValue":"."}'), JSON.parse('{"__type":"Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.Separator","AliasValue":",","CurrentValue":","}'), this.model.properties.prefix,this.model.properties.suffix, this.model.properties.rep, true, designerObj);
			args.text = args.text.indexOf('$$') > -1 ? args.text.replaceAll('$$','$'): args.text;
		}
	},
	dataBoundEvent:function(args){
		$('#'+this.element.id+'_chart_chart_scrollBar_leftArrow_primaryXAxis').remove();
				$('#'+this.element.id+'_chart_chart_scrollBar_leftCircle_primaryXAxis').remove();
				$('#'+this.element.id+'_chart_chart_scrollBar_rightArrow_primaryXAxis').remove();
				$('#'+this.element.id+'_chart_chart_scrollBar_rightCircle_primaryXAxis').remove();
				$('#'+this.element.id+'_chart_chart_scrollBar_gripCircle_primaryXAxis').remove();
				$('#'+this.element.id+'_chart_chart_scrollBarBackRect_primaryXAxis').css('height','10px');
				$('#'+this.element.id+'_chart_chart_scrollBarThumb_primaryXAxis').css({'height':'10px','cursor':'default','fill': '#969191'});
				$('#'+this.element.id+'_chart_chart_scrollBar_svgprimaryXAxis').css('padding-top','8px');
				if(this.model.boundColumns.value.length > 0){
					var title = {'title0': '', 'title1':''};
					if(this.model.properties.yaxistitletext.indexOf(',') == -1){
						title.title0 = this.model.properties.yaxistitletext;
					} else {
						title.title0 = this.model.properties.yaxistitletext.split(',')[0];
						title.title1 = this.model.properties.yaxistitletext.split(',')[1];
					}
					for(var i=0; i<this.model.boundColumns.value.length; i++){
						(this.model.properties.showyaxistitle && title['title'+i] != "") ? (document.getElementById(this.element.id +'_chart_chart_AxisTitle_'+(i+1)).innerHTML = title['title'+i]): this.model.boundColumns.value[i].columnName;
					}
				}
	},
	getRows: function(){
	var rows = [];
	var dataStr = JSON.stringify(this.data);
	if(this.model.boundColumns.row.length > 0 && this.model.dataSource.length > 0){
		for(var i = 0; i < this.model.boundColumns.row.length; i++){
			rows.push({'name':this.model.boundColumns.row[i].columnName + i});
			if(this.isMonthDeducted){
			if(this.model.dataSource[0][this.model.boundColumns.row[i].uniqueColumnName].length >= 3 && this.monthNames.indexOf(this.model.dataSource[0][this.model.boundColumns.row[i].uniqueColumnName].slice(0, this.model.dataSource[0][this.model.boundColumns.row[i].uniqueColumnName].length-5)) >= 0){
				this.monthYear = true;
				var data = this.sortMonth(this.data, this.model.boundColumns.row[i].uniqueColumnName);
				dataStr=JSON.stringify(this.sortYear(data, this.model.boundColumns.row[i].uniqueColumnName));
				this.isMonthDeducted = false;
			}
			else if(this.monthNames.indexOf(this.model.dataSource[0][this.model.boundColumns.row[i].uniqueColumnName]) >= 0){
				dataStr = JSON.stringify(this.sortMonth(this.data, this.model.boundColumns.row[i].uniqueColumnName));
				this.isMonthDeducted = false;
			}
			}
			dataStr = dataStr.replaceAll(this.model.boundColumns.row[i].uniqueColumnName, this.model.boundColumns.row[i].columnName + i);
			this.data = JSON.parse(dataStr);
		}
	} else {
		rows = [{ name: 'aYear', caption: 'Year' },{ name: 'aOrder_Source', caption: 'Order Source' }];
	}
	//this.data = JSON.parse(dataStr);
	return rows;
  },
  getColumns: function(){
	var columns = [];
	var dataStr = JSON.stringify(this.data);
	if(this.model.boundColumns.column.length > 0 && this.model.dataSource.length > 0){
		for(var i = 0; i < this.model.boundColumns.column.length; i++){
			columns.push({'name':this.model.boundColumns.column[i].columnName});
			if(this.isMonthDeducted){
			if(this.model.dataSource[0][this.model.boundColumns.column[i].uniqueColumnName].length >= 3 && this.monthNames.indexOf(this.model.dataSource[0][this.model.boundColumns.column[i].uniqueColumnName].slice(0, this.model.dataSource[0][this.model.boundColumns.column[i].uniqueColumnName].length-5)) >= 0){
				this.monthYear = true;
				var data = this.sortMonth(this.data, this.model.boundColumns.column[i].uniqueColumnName);
				dataStr=JSON.stringify(this.sortYear(data, this.model.boundColumns.column[i].uniqueColumnName));
				this.isMonthDeducted = false;
			}
			else if(this.monthNames.indexOf(this.model.dataSource[0][this.model.boundColumns.column[i].uniqueColumnName]) >= 0){
				dataStr = JSON.stringify(this.sortMonth(this.data, this.model.boundColumns.column[i].uniqueColumnName));
				this.isMonthDeducted = false;
			}
			}
			dataStr = dataStr.replaceAll(this.model.boundColumns.column[i].uniqueColumnName, this.model.boundColumns.column[i].columnName);
			this.data = JSON.parse(dataStr);
		}
	} else {
		columns = [{ name: 'aCountry' }, { name: 'aProducts' }];
	}
	//this.data = JSON.parse(dataStr);
	return columns;
  },
  getValues: function(){
	var values = [];
	var dataStr = JSON.stringify(this.data);
	if(this.model.boundColumns.value.length > 0 && this.model.dataSource.length > 0){
		for(var i = 0; i < this.model.boundColumns.value.length; i++){
			values.push({'name':this.model.boundColumns.value[i].columnName});
			dataStr = dataStr.replaceAll(this.model.boundColumns.value[i].uniqueColumnName, this.model.boundColumns.value[i].columnName);
		}
	} else {
		values = [{ name: 'aAmount', caption: 'Amount' },{name:'aSold',caption: 'Sold'}];
	}
	this.data = JSON.parse(dataStr);
	return values;
  },
  sortMonth: function(data, colName){
	  var tempDS = [];
	  for(var i = 0; i < this.monthNames.length; i++){
		  for(var j = 0; j < data.length; j++){
			  if(this.monthYear){
				if(this.monthNames[i] == data[j][colName].slice(0,data[j][colName].length-5)){
					tempDS.push(data[j]);
					if(this.yearscoll.indexOf(data[j][colName].slice(4,data[j][colName].length)) < 0){
						this.yearscoll.push(data[j][colName].slice(4,data[j][colName].length));
					}
				}  
			  }
			  else {
				if(this.monthNames[i] == data[j][colName]){
					tempDS.push(data[j]);
				}
			  }
		  }
	  }
	  this.model.properties.sort == "Ascending" ? this.yearscoll.sort() : this.yearscoll.sort((a,b)=>b-a);
	  return tempDS;
  },
  sortYear: function(data, colName){
	  var tempDS = [];
	  for(var i = 0; i < this.yearscoll.length; i++){
		  for(var j = 0; j < data.length; j++){
			if(this.yearscoll[i] == data[j][colName].slice(4,data[j][colName].length)){
				tempDS.push(data[j]);
			}
		  }
	  }
	  this.yearscoll = [];
	  this.monthYear = false;
	  return tempDS;
  },
  
    update: function (option) {
        if (option.type === "resize") {
			this.element.innerHTML = "";
			this.init();
			return;
        }
        else if (option.type === "refresh") {
			this.element.innerHTML = "";
			this.init();
			return;
        }
        else if (option.type === "propertyChange") {
			
            /* update type will be 'propertyChange' when any property value is changed in the designer. */

            switch (option.property.name) {
                case "glxaxis":
                case "glyaxis":
                case "showlabel":
				case "showLegend":
				case "legendPosition":
				case "animation":
				case "showxaxis":
				case "showxaxistitle":
				case "xaxistitletext":
				case "showyaxis":
				case "showyaxistitle":
				case "yaxistitletext":
				case "labelintersectaction":
				case "expandAll":
				case "sort":
				case "prefix":
				case "suffix":
				case "rep":
				case "decimal":
				case "chartType":
				case "chartColor":
				case "cchartColor":
				case "legendShape":
				case "useFormat":
					this.element.innerHTML = "";
					this.init();
					return;
			}
        }
    }
});
