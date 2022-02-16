/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"13423676-238e-464e-9a98-99af98475f2f",

    widgetName:"QuadrantChart",

    init: function () {
        /* init method will be called when the widget is initialized */
        var widget = document.createElement("div");
        widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		widget.style.width = '100%';
		widget.style.height = '100%';
        this.element.appendChild(widget);
		this.renderChart(widget);
    },
	renderChart: function(widget){
		var xUniqueColumnName = 'x';
		var xColumnName = 'x axis';
		var yUniqueColumnName = 'y';
		var yColumnName = 'y axis';
	    var dataSource = [{ 'x': 115, 'y': 57 }, { 'x': 138, 'y': 57 }, { 'x': 166, 'y': 57 }, { 'x': 122, 'y': 57 },
    { 'x': 126, 'y': 57 }, { 'x': 130, 'y': 57 }, { 'x': 125, 'y': 57 }, { 'x': 144, 'y': 57 },
    { 'x': 150, 'y': 57 }, { 'x': 120, 'y': 57 }, { 'x': 125, 'y': 57 }, { 'x': 130, 'y': 57 },
    { 'x': 103, 'y': 58 }, { 'x': 116, 'y': 58 }, { 'x': 130, 'y': 58 }, { 'x': 126, 'y': 58 },
    { 'x': 136, 'y': 58 }, { 'x': 148, 'y': 58 }, { 'x': 119, 'y': 58 }, { 'x': 141, 'y': 58 },
    { 'x': 159, 'y': 58 }, { 'x': 120, 'y': 58 }, { 'x': 135, 'y': 58 }, { 'x': 163, 'y': 58 },
    { 'x': 119, 'y': 59 }, { 'x': 131, 'y': 59 }, { 'x': 148, 'y': 59 }, { 'x': 123, 'y': 59 },
    { 'x': 137, 'y': 59 }, { 'x': 149, 'y': 59 }, { 'x': 121, 'y': 59 }, { 'x': 142, 'y': 59 },
    { 'x': 160, 'y': 59 }, { 'x': 118, 'y': 59 }, { 'x': 130, 'y': 59 }, { 'x': 146, 'y': 59 },
    { 'x': 119, 'y': 60 }, { 'x': 133, 'y': 60 }, { 'x': 150, 'y': 60 }, { 'x': 133, 'y': 60 },
    { 'x': 149, 'y': 60 }, { 'x': 165, 'y': 60 }, { 'x': 130, 'y': 60 }, { 'x': 139, 'y': 60 },
    { 'x': 154, 'y': 60 }, { 'x': 118, 'y': 60 }, { 'x': 152, 'y': 60 }, { 'x': 154, 'y': 60 },
    { 'x': 130, 'y': 61 }, { 'x': 145, 'y': 61 }, { 'x': 166, 'y': 61 }, { 'x': 131, 'y': 61 },
    { 'x': 143, 'y': 61 }, { 'x': 162, 'y': 61 }, { 'x': 131, 'y': 61 }, { 'x': 145, 'y': 61 },
    { 'x': 162, 'y': 61 }, { 'x': 115, 'y': 61 }, { 'x': 149, 'y': 61 }, { 'x': 183, 'y': 61 },
    { 'x': 121, 'y': 62 }, { 'x': 139, 'y': 62 }, { 'x': 159, 'y': 62 }, { 'x': 135, 'y': 62 },
    { 'x': 152, 'y': 62 }, { 'x': 178, 'y': 62 }, { 'x': 130, 'y': 62 }, { 'x': 153, 'y': 62 },
    { 'x': 172, 'y': 62 }, { 'x': 114, 'y': 62 }, { 'x': 135, 'y': 62 }, { 'x': 154, 'y': 62 },
    { 'x': 126, 'y': 63 }, { 'x': 141, 'y': 63 }, { 'x': 160, 'y': 63 }, { 'x': 135, 'y': 63 },
    { 'x': 149, 'y': 63 }, { 'x': 180, 'y': 63 }, { 'x': 132, 'y': 63 }, { 'x': 144, 'y': 63 },
    { 'x': 163, 'y': 63 }, { 'x': 122, 'y': 63 }, { 'x': 146, 'y': 63 }, { 'x': 156, 'y': 63 },
    { 'x': 133, 'y': 64 }, { 'x': 150, 'y': 64 }, { 'x': 176, 'y': 64 }, { 'x': 133, 'y': 64 },
    { 'x': 149, 'y': 64 }, { 'x': 176, 'y': 64 }, { 'x': 136, 'y': 64 }, { 'x': 157, 'y': 64 },
    { 'x': 174, 'y': 64 }, { 'x': 131, 'y': 64 }, { 'x': 155, 'y': 64 }, { 'x': 191, 'y': 64 },
    { 'x': 136, 'y': 65 }, { 'x': 149, 'y': 65 }, { 'x': 177, 'y': 65 }, { 'x': 143, 'y': 65 },
    { 'x': 149, 'y': 65 }, { 'x': 184, 'y': 65 }, { 'x': 128, 'y': 65 }, { 'x': 146, 'y': 65 },
    { 'x': 157, 'y': 65 }, { 'x': 133, 'y': 65 }, { 'x': 153, 'y': 65 }, { 'x': 173, 'y': 65 },
    { 'x': 141, 'y': 66 }, { 'x': 156, 'y': 66 }, { 'x': 175, 'y': 66 }, { 'x': 125, 'y': 66 },
    { 'x': 138, 'y': 66 }, { 'x': 165, 'y': 66 }, { 'x': 122, 'y': 66 }, { 'x': 164, 'y': 66 },
    { 'x': 182, 'y': 66 }, { 'x': 137, 'y': 66 }, { 'x': 157, 'y': 66 }, { 'x': 176, 'y': 66 },
    { 'x': 149, 'y': 67 }, { 'x': 159, 'y': 67 }, { 'x': 179, 'y': 67 }, { 'x': 156, 'y': 67 },
    { 'x': 179, 'y': 67 }, { 'x': 186, 'y': 67 }, { 'x': 147, 'y': 67 }, { 'x': 166, 'y': 67 },
    { 'x': 185, 'y': 67 }, { 'x': 140, 'y': 67 }, { 'x': 160, 'y': 67 }, { 'x': 180, 'y': 67 },
    { 'x': 145, 'y': 68 }, { 'x': 155, 'y': 68 }, { 'x': 170, 'y': 68 }, { 'x': 129, 'y': 68 },
    { 'x': 164, 'y': 68 }, { 'x': 189, 'y': 68 }, { 'x': 150, 'y': 68 }, { 'x': 157, 'y': 68 },
    { 'x': 183, 'y': 68 }, { 'x': 144, 'y': 68 }, { 'x': 170, 'y': 68 }, { 'x': 180, 'y': 68 }];
	         this.xmin = 100;
			this.ymin = 50;
			this.xmiddle = 150;
			this.ymiddle = 70;
		var chart = new ejdashboard.charts.Chart({
			//Initializing Primary X Axis
			primaryXAxis: {
				//majorGridLines: { width: 0 },
				visible : false,
				minimum: 100,
				maximum: 220,
				title: xColumnName,
				 stripLines: [
               {
              startFromAxis: true, size: 50, isSegmented: true, segmentStart: 50,
              segmentEnd: 65, visible: true, color: this.model.properties.colorbottomleft,
            },
			{
              startFromAxis: true, size: 50, isSegmented: true, segmentStart: 65,
              segmentEnd: 80, visible: true, color: this.model.properties.colortopleft,
            },
			{
              start:150,size: 70, isSegmented: true, segmentStart: 50,
              segmentEnd: 65, visible: true, color: this.model.properties.colorbottomright,
            },
			{
              start:150,size: 70, isSegmented: true, segmentStart: 65,
              segmentEnd: 80, visible: true, color: this.model.properties.colortopright,
            },
            ]
			},
			chartArea: {
				border: {
					width: 0
				}
			},
			//Initializing Primary Y Axis
			primaryYAxis:
			{
				//majorTickLines: {width: 0},
				visible : false,
				minimum: 50,
				maximum: 80,
				//lineStyle: {width: 0},
				title: yColumnName,
				//rangePadding: 'None',
			},		
			//Initializing Chart Series
			series: [
				{
					type: 'Scatter',
					dataSource: dataSource,
					xName: xUniqueColumnName, width: 2, 
					marker: {
						visible: true,
						width: 12,
						height: 12,
						shape: 'Circle',
						
					},
					yName: yUniqueColumnName, name: yColumnName, opacity: 1,
					border:{width: 2, color: 'black'},
						fill : 'white'
				}
			],
		   // pointClick: $.proxy(this.selectionChange, this),
			//Initializing Chart title
			tooltip: {
				enable: true,
				template: "<div>Text</div>"
				// format: 'Weight: <b>${point.x} lbs</b> <br/> Height: <b>${point.y}"</b>'
			},
			//Initializing User Interaction Tooltip
			tooltipRender: $.proxy(this.tooltipRender, this),
			//pointRender: $.proxy(this.pointRender, this),
			//loaded : $.proxy(this.loaded, this),
			height: '100%',
			width: '100%',
			//selectionMode: 'Point',
			 
		});
		chart.appendTo(widget);
	},
	loaded : function() {
		$("#" + this.element.getAttribute("id") + "_widget_stripline_Behind_rect_" + "primaryXAxis_0").attr("style", "stroke: black");
        $("#" + this.element.getAttribute("id") + "_widget_stripline_Behind_rect_" + "primaryYAxis_0").attr("style", "stroke: black");
	},
	pointRender : function (args) {
		if(args.point.y <= this.ymiddle && args.point.x <= this.xmiddle) {
			args.fill = this.model.properties.colorbottomleft;
		}
		if(args.point.y > this.ymiddle && args.point.x <= this.xmiddle) {
		args.fill = this.model.properties.colortopleft;
		}
		if(args.point.y > this.ymiddle && args.point.x > this.xmiddle) {
		args.fill = this.model.properties.colortopright;
		}
		if(args.point.y < this.ymiddle && args.point.x > this.xmiddle) {
		args.fill = this.model.properties.colorbottomright;
		}
	},
	isWidgetConfigured: function(){
		return this.model.boundColumns.xvalue.length > 0 && this.model.boundColumns.yvalue.length > 0;
	},
	selectionChange: function(selectedItem, that) {
		var selectedFilterInfos = [];
		if (selectedItem != null && this.model.boundColumns.series.length > 0) {        
			var filterinfo = new bbicustom.dashboard.selectedColumnInfo();
            filterinfo.condition = "include";
            filterinfo.uniqueColumnName = this.model.boundColumns.series[0].uniqueColumnName;
            filterinfo.values = [this.model.dataSource[selectedItem.pointIndex][this.model.boundColumns.series[0].uniqueColumnName]];
            selectedFilterInfos.push(filterinfo);            
		}
		bbicustom.dashboard.filterData(this,selectedFilterInfos);    
    },
	tooltipRender: function(args){
		var tooltipTemplate;
		if(this.isWidgetConfigured()) {
			var seriesTemplate = this.model.boundColumns.series.length > 0 ? '<tr><td class="column-name">'+ this.model.boundColumns.series[0].columnName + ':</td><td><b>' + this.model.dataSource[args.point.index][this.model.boundColumns.series[0].uniqueColumnName] + '</b></td></tr>' : '';
			tooltipTemplate = '<div style="background:white;border: 1px solid #d4d4d4;box-shadow: 0 2px 4px 0 rgba(0,0,0,.12);border-radius: 4px;padding: 5px;" ><table> ' + seriesTemplate + '<tr><td class="column-name">' + this.model.boundColumns.xvalue[0].columnName + ': </td><td><b>' + args.point.xValue
			+ '</b></td></tr>' + '<tr><td class="column-name">' + this.model.boundColumns.yvalue[0].columnName + ': </td><td><b>' + args.point.yValue
			+ '</b></td></tr></table>'; 
		} else{
			tooltipTemplate = '<div style="background:white;border: 1px solid #d4d4d4";><table><tr><td>X : </td><td>' + args.point.xValue + '</td></tr>' + '<tr><td>Y : </td><td>' + args.point.yValue + '</td></tr></table>';
		}
		args.template = tooltipTemplate;
	},
	getmax: function(val) {
		var max = val[0];
		for(var i=0; i< val.length;i++) {
			if(val[i] > max) {
				max = val[i];
			}
		}
		return max;
	},
	getmin: function(val) {
		var min = val[0];
		for(var i=0; i< val.length;i++) {
			if(val[i] < min) {
				min = val[i];
			}
		}
		return min;
	},
    update: function (option) {
		debugger;
        var widget = document.getElementById(this.element.getAttribute("id") + "_widget");
        var chartObj = widget.ej2_instances[0];

        /* update method will be called when any update needs to be performed in the widget. */

        if (option.type == "resize") {
            /* update type will be 'resize' if the widget is being resized. */
        }
        else if (option.type == "refresh") {
			var xValue, yValue, xName, yName, dataSource, tooltipFormat;
			if(!this.isWidgetConfigured()) {
				xUniqueColumnName = 'x';
				xColumnName = 'x axis';
				yUniqueColumnName = 'y';
				yColumnName = 'y axis';
				//tooltipFormat = 'x: <b>${point.x} lbs</b> <br/> y: <b>${point.y}"</b>';
				dataSource = [{ 'x': 115, 'y': 57 }, { 'x': 138, 'y': 57 }, { 'x': 166, 'y': 57 }, { 'x': 122, 'y': 57 },
    { 'x': 126, 'y': 57 }, { 'x': 130, 'y': 57 }, { 'x': 125, 'y': 57 }, { 'x': 144, 'y': 57 },
    { 'x': 150, 'y': 57 }, { 'x': 120, 'y': 57 }, { 'x': 125, 'y': 57 }, { 'x': 130, 'y': 57 },
    { 'x': 103, 'y': 58 }, { 'x': 116, 'y': 58 }, { 'x': 130, 'y': 58 }, { 'x': 126, 'y': 58 },
    { 'x': 136, 'y': 58 }, { 'x': 148, 'y': 58 }, { 'x': 119, 'y': 58 }, { 'x': 141, 'y': 58 },
    { 'x': 159, 'y': 58 }, { 'x': 120, 'y': 58 }, { 'x': 135, 'y': 58 }, { 'x': 163, 'y': 58 },
    { 'x': 119, 'y': 59 }, { 'x': 131, 'y': 59 }, { 'x': 148, 'y': 59 }, { 'x': 123, 'y': 59 },
    { 'x': 137, 'y': 59 }, { 'x': 149, 'y': 59 }, { 'x': 121, 'y': 59 }, { 'x': 142, 'y': 59 },
    { 'x': 160, 'y': 59 }, { 'x': 118, 'y': 59 }, { 'x': 130, 'y': 59 }, { 'x': 146, 'y': 59 },
    { 'x': 119, 'y': 60 }, { 'x': 133, 'y': 60 }, { 'x': 150, 'y': 60 }, { 'x': 133, 'y': 60 },
    { 'x': 149, 'y': 60 }, { 'x': 165, 'y': 60 }, { 'x': 130, 'y': 60 }, { 'x': 139, 'y': 60 },
    { 'x': 154, 'y': 60 }, { 'x': 118, 'y': 60 }, { 'x': 152, 'y': 60 }, { 'x': 154, 'y': 60 },
    { 'x': 130, 'y': 61 }, { 'x': 145, 'y': 61 }, { 'x': 166, 'y': 61 }, { 'x': 131, 'y': 61 },
    { 'x': 143, 'y': 61 }, { 'x': 162, 'y': 61 }, { 'x': 131, 'y': 61 }, { 'x': 145, 'y': 61 },
    { 'x': 162, 'y': 61 }, { 'x': 115, 'y': 61 }, { 'x': 149, 'y': 61 }, { 'x': 183, 'y': 61 },
    { 'x': 121, 'y': 62 }, { 'x': 139, 'y': 62 }, { 'x': 159, 'y': 62 }, { 'x': 135, 'y': 62 },
    { 'x': 152, 'y': 62 }, { 'x': 178, 'y': 62 }, { 'x': 130, 'y': 62 }, { 'x': 153, 'y': 62 },
    { 'x': 172, 'y': 62 }, { 'x': 114, 'y': 62 }, { 'x': 135, 'y': 62 }, { 'x': 154, 'y': 62 },
    { 'x': 126, 'y': 63 }, { 'x': 141, 'y': 63 }, { 'x': 160, 'y': 63 }, { 'x': 135, 'y': 63 },
    { 'x': 149, 'y': 63 }, { 'x': 180, 'y': 63 }, { 'x': 132, 'y': 63 }, { 'x': 144, 'y': 63 },
    { 'x': 163, 'y': 63 }, { 'x': 122, 'y': 63 }, { 'x': 146, 'y': 63 }, { 'x': 156, 'y': 63 },
    { 'x': 133, 'y': 64 }, { 'x': 150, 'y': 64 }, { 'x': 176, 'y': 64 }, { 'x': 133, 'y': 64 },
    { 'x': 149, 'y': 64 }, { 'x': 176, 'y': 64 }, { 'x': 136, 'y': 64 }, { 'x': 157, 'y': 64 },
    { 'x': 174, 'y': 64 }, { 'x': 131, 'y': 64 }, { 'x': 155, 'y': 64 }, { 'x': 191, 'y': 64 },
    { 'x': 136, 'y': 65 }, { 'x': 149, 'y': 65 }, { 'x': 177, 'y': 65 }, { 'x': 143, 'y': 65 },
    { 'x': 149, 'y': 65 }, { 'x': 184, 'y': 65 }, { 'x': 128, 'y': 65 }, { 'x': 146, 'y': 65 },
    { 'x': 157, 'y': 65 }, { 'x': 133, 'y': 65 }, { 'x': 153, 'y': 65 }, { 'x': 173, 'y': 65 },
    { 'x': 141, 'y': 66 }, { 'x': 156, 'y': 66 }, { 'x': 175, 'y': 66 }, { 'x': 125, 'y': 66 },
    { 'x': 138, 'y': 66 }, { 'x': 165, 'y': 66 }, { 'x': 122, 'y': 66 }, { 'x': 164, 'y': 66 },
    { 'x': 182, 'y': 66 }, { 'x': 137, 'y': 66 }, { 'x': 157, 'y': 66 }, { 'x': 176, 'y': 66 },
    { 'x': 149, 'y': 67 }, { 'x': 159, 'y': 67 }, { 'x': 179, 'y': 67 }, { 'x': 156, 'y': 67 },
    { 'x': 179, 'y': 67 }, { 'x': 186, 'y': 67 }, { 'x': 147, 'y': 67 }, { 'x': 166, 'y': 67 },
    { 'x': 185, 'y': 67 }, { 'x': 140, 'y': 67 }, { 'x': 160, 'y': 67 }, { 'x': 180, 'y': 67 },
    { 'x': 145, 'y': 68 }, { 'x': 155, 'y': 68 }, { 'x': 170, 'y': 68 }, { 'x': 129, 'y': 68 },
    { 'x': 164, 'y': 68 }, { 'x': 189, 'y': 68 }, { 'x': 150, 'y': 68 }, { 'x': 157, 'y': 68 },
    { 'x': 183, 'y': 68 }, { 'x': 144, 'y': 68 }, { 'x': 170, 'y': 68 }, { 'x': 180, 'y': 68 }];
			} else {
				this.checkforNaN();
				var xvalue = [];
				var yvalue = [];
				//tooltipFormat = this.model.boundColumns.xvalue[0].columnName + ': <b>${point.x} </b> <br/> ' + this.model.boundColumns.yvalue[0].columnName + ': <b>${point.y}</b> <br/> ' + this.model.boundColumns.series[0].columnName + '<b>${point.}</b> <br/>' ;
				xUniqueColumnName = this.model.boundColumns.xvalue[0].uniqueColumnName;
				xColumnName =  this.model.boundColumns.xvalue[0].columnName;
				yUniqueColumnName = this.model.boundColumns.yvalue[0].uniqueColumnName;
				yColumnName =   this.model.boundColumns.yvalue[0].columnName;
				dataSource = this.model.dataSource;
				for(var i= 0; i < dataSource.length; i++){
					xvalue.push(dataSource[i][xUniqueColumnName]);
					yvalue.push(dataSource[i][yUniqueColumnName]);
				}
				var xmax = this.getmax(xvalue);
				var ymax = this.getmax(yvalue);
				var xmin = this.getmin(xvalue);	
				var ymin = this.getmin(yvalue);				
			}
			chartObj.series[0].yName = yUniqueColumnName;
			chartObj.series[0].name =  '';
			chartObj.series[0].xName = xUniqueColumnName;
			chartObj.primaryYAxis.title = yColumnName;	
			chartObj.primaryXAxis.title = xColumnName;
			chartObj.primaryYAxis.minimum = ymin;
			chartObj.primaryYAxis.maximum = ymax;
		    chartObj.primaryXAxis.minimum = xmin;
			chartObj.primaryXAxis.maximum = xmax;
			
			chartObj.primaryXAxis.stripLines[0].visible = this.model.boundColumns.series.length > 0 ? true : false;
			chartObj.primaryXAxis.stripLines[0].segmentStart = ymin;
			chartObj.primaryXAxis.stripLines[0].size = ((xmin+xmax)/2) - xmin;
			chartObj.primaryXAxis.stripLines[0].segmentEnd = (ymin+ymax)/2;
			
			chartObj.primaryXAxis.stripLines[1].visible = this.model.boundColumns.series.length > 0 ? true : false;
			chartObj.primaryXAxis.stripLines[1].segmentStart = (ymin+ymax)/2;
			chartObj.primaryXAxis.stripLines[1].size = ((xmin+xmax)/2) - xmin;
			chartObj.primaryXAxis.stripLines[1].segmentEnd = ymax;
			
			chartObj.primaryXAxis.stripLines[2].visible = this.model.boundColumns.series.length > 0 ? true : false;
			chartObj.primaryXAxis.stripLines[2].segmentStart = ymin;
			chartObj.primaryXAxis.stripLines[2].start = (xmin+xmax)/2;
			chartObj.primaryXAxis.stripLines[2].segmentEnd = (ymin+ymax)/2;
			chartObj.primaryXAxis.stripLines[2].size = xmax - chartObj.primaryXAxis.stripLines[2].start;
			
			chartObj.primaryXAxis.stripLines[3].visible = this.model.boundColumns.series.length > 0 ? true : false;
			chartObj.primaryXAxis.stripLines[3].segmentStart = (ymin+ymax)/2;
			chartObj.primaryXAxis.stripLines[3].start = (xmin+xmax)/2;
			chartObj.primaryXAxis.stripLines[3].segmentEnd = ymax;
			chartObj.primaryXAxis.stripLines[3].size = xmax - chartObj.primaryXAxis.stripLines[2].start;
			
			
			this.xmin = xmin;
			this.ymin = ymin;
			this.xmiddle = (xmin + xmax)/2;
			this.ymiddle = (ymin + ymax)/2;
			//chartObj.title = yColumnName + ' vs ' + xColumnName;
			chartObj.series[0].dataSource = dataSource;
        }
        else if (option.type == "propertyChange") {
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
				case "colorbottomleft" :
				chartObj.primaryXAxis.stripLines[0].color = option.property.value;
				chartObj.refresh();	
				break;
				case "colortopleft":
				chartObj.primaryXAxis.stripLines[1].color = option.property.value;
				chartObj.refresh();	
				break;
				case "colortopright":
				chartObj.primaryXAxis.stripLines[3].color = option.property.value;
				chartObj.refresh();	
				break;
				case "colorbottomright":
				chartObj.primaryXAxis.stripLines[2].color = option.property.value;
				chartObj.refresh();	
				break;
				
            }
        }
		chartObj.refresh();

    },
	 checkforNaN : function () {
		 for(var i=0;i<this.model.dataSource.length;i++) {
			 if(isNaN(this.model.dataSource[i][this.model.boundColumns.xvalue[0].uniqueColumnName])){
				 this.model.dataSource[i][this.model.boundColumns.xvalue[0].uniqueColumnName] = 0;
			 }
			 if(isNaN(this.model.dataSource[i][this.model.boundColumns.yvalue[0].uniqueColumnName])){
				 this.model.dataSource[i][this.model.boundColumns.yvalue[0].uniqueColumnName] = 0;
			 }
		 }
	 }
});