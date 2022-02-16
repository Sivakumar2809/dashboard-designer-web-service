bbicustom.dashboard.registerWidget({
		
		guid : "b0d5348d-f625-4b78-8db9-c5ed9d38eb45",
		
		pluginName : "SunburstChart",
		
		init : function () {
			this.widget = document.createElement("div");
			this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
			this.element.appendChild(this.widget);
			this.renderSunburstChartWithDefaultData();
		},
		
		update : function (option) {
			if(option.type == "resize") {
				this.resizeWidget(option.size);
			}
			else if (option.type == "refresh") {
				var widgetObj = $(this.widget).data("ejSunburstChart");	
				if(this.model.boundColumns.Value.length > 0){
					for (var i=0; i<this.model.dataSource.length; i++) {
						this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName] =  parseInt( this.model.dataSource[i][this.model.boundColumns.Value[0].uniqueColumnName]);     	
					}
					var valueMember = this.model.boundColumns.Value[0].uniqueColumnName;
					if (this.model.boundColumns.Levels.length > 0) {
						var levels = [];				
						for (var level = 0; level < this.model.boundColumns.Levels.length; level++) {
							levels.push({ groupMemberPath : this.model.boundColumns.Levels[level].uniqueColumnName });
						}
						widgetObj.model.levels= levels;
					}
					widgetObj.model.dataSource = this.model.dataSource;
					widgetObj.model.valueMemberPath = valueMember;
					widgetObj.redraw();
				} else {
					this.renderSunburstChartWithDefaultData();
				}
			} else if(option.type === "propertyChange"){
				var widgetObj = $(this.widget).data("ejSunburstChart");
				switch (option.property.name) {
					case "showLegend":
						widgetObj.model.legend.visible = this.model.properties.showLegend;
					case "legendPosition":
						widgetObj.model.legend.position = this.model.properties.legendPosition.toLowerCase();
					case "p1Color":
						widgetObj.model.palette[0] = this.model.properties.p1Color;
					case "p2Color":
						widgetObj.model.palette[1] = this.model.properties.p2Color;
					case "p3Color":
						widgetObj.model.palette[2] = this.model.properties.p3Color;
					case "p4Color":
						widgetObj.model.palette[3] = this.model.properties.p4Color;
					case "p5Color":
						widgetObj.model.palette[4] = this.model.properties.p5Color;
					case "p6Color":
						widgetObj.model.palette[5] = this.model.properties.p6Color;
					case "p7Color":
						widgetObj.model.palette[6] = this.model.properties.p7Color;
					case "p8Color":
						widgetObj.model.palette[7] = this.model.properties.p8Color;
					case "p9Color":
						widgetObj.model.palette[8] = this.model.properties.p9Color;
					case "p10Color":
						widgetObj.model.palette[9] = this.model.properties.p10Color;
					case "showDataLabel":
						widgetObj.model.dataLabelSettings.visible = this.model.properties.showDataLabel;
					case "labelOverflowMode":
						widgetObj.model.dataLabelSettings.labelOverflowMode = this.model.properties.labelOverflowMode.toLowerCase();
					case "animation":
						widgetObj.model.enableAnimation = this.model.properties.animation;
				}
				widgetObj.redraw();
			}			
		},
		
		renderSunburstChartWithDefaultData : function () {			
			$(this.widget).css({"width":$(this.element).width(), "height":$(this.element).height()});
			var dataSource = [{ Item: "Item1", Value: 50 },{ Item: "Item2", Value: 60 },{ Item: "Item3", Value: 70 },{ Item: "Item4", Value: 80 },{ Item: "Item5", Value: 90 },{ Item: "Item6", Value: 90 },{ Item: "Item7", Value: 90 },{ Item: "Item8", Value: 90 },{ Item: "Item9", Value: 90 },{ Item: "Item10", Value: 90 },{ Item: "Item11", Value: 90 }];
			var levels = [];
			var valueMember = "Value";
			levels = [ { groupMemberPath: "Item" }];
			$(this.widget).ejSunburstChart({ dataSource: dataSource, 
							valueMemberPath: valueMember, levels: levels,
							palette: [this.model.properties.p1Color, this.model.properties.p2Color, this.model.properties.p3Color, this.model.properties.p4Color,this.model.properties.p5Color, this.model.properties.p6Color, this.model.properties.p7Color, this.model.properties.p8Color, this.model.properties.p9Color, this.model.properties.p10Color],
							tooltip: { visible: true},
							margin: (this.marginVisibility())? { left: 10, top: 10, bottom: 10, right: 10} :{ left: 0, top: 0, bottom: 0, right: 0} ,
							border: { width: (this.marginVisibility())? 2:0 },
							load: $.proxy(this.sunburstChartLoad),
							enableAnimation: false,
							animationType: 'Rotation',
							innerRadius: 0.2,							
							dataLabelSettings:{visible:(this.model.properties.showDataLabel?this.dataLabelVisibility():false), labelOverflowMode:this.model.properties.labelOverflowMode.toLowerCase()},
							size: { height: $(this.element).height(), width: $(this.element).width()},	 
							legend: { visible:  (this.model.properties.showLegend ? this.legendVisibility():false) , position: this.model.properties.legendPosition.toLowerCase() },
							highlightSettings: {enable: true},
							selectionSettings: {enable: true, mode : "parent"}
							});	
		},
		sunburstChartLoad : function (e) {
			this.elementSpacing = (this.element.width() > 200 && this.element.height() > 200) ? 10 : 0;
		},		
		
		resizeWidget : function (size) {
			$(this.widget).css({"width": size.width, "height": size.height});
			var sunburstObj = $(this.widget).data("ejSunburstChart");
			sunburstObj.model.size.height = size.height-5;
			sunburstObj.model.size.width = size.width; 
			sunburstObj.model.margin =(this.marginVisibility())? { left: 10, top: 10, bottom: 10, right: 10} :{ left: 0, top: 0, bottom: 0, right: 0} ,
			sunburstObj.model.legend.visible = this.model.properties.showLegend ? this.legendVisibility():false;
			//sunburstObj.model.legend.visible = false;
			sunburstObj.model.legend.position = this.model.properties.legendPosition.toLowerCase();
			sunburstObj.model.dataLabelSettings.visible = this.dataLabelVisibility();	
			sunburstObj.redraw();
		},
		
		marginVisibility : function () {
			return ($(this.widget).width() > 200 && $(this.widget).height() > 200);
		},
		
		legendVisibility : function (e) {
			return ($(this.element).width() > 300 && $(this.element).height() > 300);
		},
		
		dataLabelVisibility : function (e) {
			return !($(this.element).width() < 300 || $(this.element).height() < 300);
		},
		
		/*legendPosition : function (e) {
			if ( $(this.element).width() > $(this.element).height())
				return "right";
			else 
				return "top";
		},*/
		
		getAnimationType : function(animationType) {
			if (animationType === "FadeIn")
				return "fadein";
			else if(animationType === "Rotation")
				return "rotation";
		},
		
		getRotationMode : function(rotationMode) {
			if (rotationMode === "Normal")
				return "normal";
			else if(rotationMode === "Angle")
				return "angle";
		}
});