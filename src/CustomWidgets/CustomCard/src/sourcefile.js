/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid:"116a32ba-a4bf-49f5-a1d2-9d54c684e871",

    widgetName:"CustomCard",

    init: function () {
		
		this.widgetIns = $(this.element).closest('.e-customwidget-item').data('widgetInstance');
		this.textBackground = this.model.properties.textBackground;
		this.textColor = this.model.properties.textForeground;
		this.themeApplied = this.widgetIns.themeHelper.currentTheme.name;
		if(this.widgetIns.themeHelper.currentTheme.name == "Dark"){
			if(this.textBackground == "#ffffff"){
				this.textBackground = "#202635";
			}
			if(this.textColor  == "#000000"){
				this.textColor = "#E5EBF8";
			}
		}
		this.updateHeaderElement();
		var cContainer = $(this.element).parents(".e-customwidget-item");
		if(this.model.boundColumns.name.length > 0 && this.model.dataSource.length  == 0){
			cContainer.find(".e-customwidgetitem").css({ "display": 'none'});
			cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").css({ "display": 'block'});
			cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").find('.widget-info-icon').css({ "display": 'block'});
			cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").find('.e-nodata').css({ "display": 'block'});
		} else {
			cContainer.find(".e-dbrd-control-nodatainfo.e-nodata-container").css({ "display": 'none'});
			cContainer.find(".e-customwidgetitem").css({ "display": 'block'});
		}
		var data = "";
		var titleText = "";
		if(this.model.boundColumns.name.length > 0 && this.model.dataSource.length > 0){
			if(this.model.properties.first){
				data = BoldBIDashboard.isNullOrUndefined(this.model.dataSource[0][this.model.boundColumns.name[0].uniqueColumnName])? (this.model.dataSource[1][this.model.boundColumns.name[0].uniqueColumnName]):(this.model.dataSource[0][this.model.boundColumns.name[0].uniqueColumnName]);
			} else {
				data = BoldBIDashboard.isNullOrUndefined(this.model.dataSource[this.model.dataSource.length -1][this.model.boundColumns.name[0].uniqueColumnName])? (this.model.dataSource[this.model.dataSource.length -2][this.model.boundColumns.name[0].uniqueColumnName]):(this.model.dataSource[this.model.dataSource.length -1][this.model.boundColumns.name[0].uniqueColumnName]);
			}
			
		}
		titleText = this.model.properties.ttext;
		//$(this.element).css({"top":(this.element.clientHeight/3.5)+'%'});
		this.titleDiv = document.createElement("div");
		$(this.element).css({"display":"table"});
		this.titleDiv.setAttribute("id", this.element.getAttribute("id") + "_titleDiv");
		$(this.titleDiv).css({"text-align": this.model.properties.textAlign.toLowerCase(),"vertical-align": "middle","text-overflow": "ellipsis","white-space": "nowrap","width": "100%","overflow": "hidden","max-width": $(this.element).width(),"display":"table-cell", "color":this.textColor});
	
		
		this.titleDiv.innerHTML = titleText+data;
		this.titleDiv.style.backgroundColor = this.textBackground;
        this.titleDiv.style.fontSize = this.model.properties.textSize + "px";
        this.titleDiv.style.fontStyle = this.model.properties.textStyle;
		this.element.appendChild(this.titleDiv);
		if(this.model.properties.bold){
			$(this.titleDiv).css({"font-weight": "bold"});
			}
		if(this.widgetIns.designerInstance.isViewMode()){
			var self = this; 
			var storeTimeInterval = setInterval(function(){
				self.widgetIns = $(self.element).closest('.e-customwidget-item').data('widgetInstance');
	        	if((self.themeApplied != self.widgetIns.themeHelper.currentTheme.name)){
	        		clearInterval(storeTimeInterval);
					self.themeApplied = self.widgetIns.themeHelper.currentTheme.name;
					self.element.innerHTML = "";
					self.init();
					return;
	        	}
			}, 100);	
		}
 
    },
	updateHeaderElement: function () {
        $(this.element).css({ "background-color": this.textBackground });
        var controlContainer = $(this.element).parents(".e-customwidget-item");
        controlContainer.css({ "background-color": this.textBackground});
        controlContainer.find(".ej-background-waiting-popup").css({ "background-color": this.textBackground });
		controlContainer.find(".e-dbrd-control-container-wrapper").css({ "padding": '0px 8px 0px 8px'});
		controlContainer.find(".e-dbrd-content-container").css({ "padding": '0px'});
        //this.title = controlContainer.find('.e-dbrd-control-header .e-title-text').html();
        controlContainer.find(".e-dbrd-control-header").hide();
        controlContainer.find(".e-dbrd-control-sub-header").hide();
        controlContainer.find(".e-dbrd-content-container").css({ "height": "calc(100%)", "background-color": this.textBackground, "border-radius": "8px" });
		controlContainer.find('.e-dbrd-control-background').css('opacity','0');
    },
    update: function (option) {
		this.element.innerHTML = "";
		this.init();
		return;
    }
});