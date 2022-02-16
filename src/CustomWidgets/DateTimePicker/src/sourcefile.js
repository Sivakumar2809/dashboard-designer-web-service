/* Register the widget in dashboard.*/
bbicustom.dashboard.registerWidget({

    guid: "6f275fca-3c61-4444-9e81-05f5a626a3e2",

    widgetName: "DateTimePicker",
    init: function () {
		var minDate  = "";
		var maxDate = "";
		this.tempArray = [];
		if(this.model.boundColumns.Column.length > 0 && this.model.dataSource.length > 0){
			minDate = new Date(this.model.dataSource[0][this.model.boundColumns.Column[0].uniqueColumnName]);
			maxDate = new Date(this.model.dataSource[this.model.dataSource.length-1][this.model.boundColumns.Column[0].uniqueColumnName]);
			for(var i = 0; i<this.model.dataSource.length; i++){
				this.tempArray.push(new Date(this.model.dataSource[i][this.model.boundColumns.Column[0].uniqueColumnName]).toDateString());
			}
		} else {
			minDate = new Date(1);
			maxDate = new Date();
		}
        this.widget = document.createElement("input");
        this.widget.setAttribute("id", this.element.getAttribute("id") + "_widget");
		this.element.appendChild(this.widget);
		this.DTPicker = new ej2DateTimePicker.calendars.DateTimePicker({
			min: minDate,
			max: maxDate,
			allowEdit: true,
			renderDayCell: $.proxy(this.disableDate, this),
			change: $.proxy(this.onChange, this),
			format: 'M/d/yyyy hh:mm a'
		});
		this.DTPicker.appendTo('#'+this.element.getAttribute("id") + "_widget");
    },
	disableDate: function(args){
		if(this.model.boundColumns.Column.length > 0 && this.model.dataSource.length > 0){
			if(this.tempArray.indexOf(args.date.toDateString()) == -1){
				args.isDisabled = true;
			}
		}
	},
	onChange: function(args){
		//var dateStr = (args.value.getMonth()+1)+'/'+args.value.getDate()+'/'+args.value.getFullYear()+" "+args.value.getHours()+':'+args.value.getMinutes()+' ';
		if(this.model.boundColumns.Column.length > 0){
			var selectedFilterInfos = [];
			var filterinfo = new bbicustom.dashboard.selectedColumnInfo();
			filterinfo.condition = "Include";
			filterinfo.values =[this.DTPicker.inputElement.value];
			filterinfo.uniqueColumnName = this.model.boundColumns.Column[0].uniqueColumnName;
			selectedFilterInfos.push(filterinfo);
			bbicustom.dashboard.filterData(this, selectedFilterInfos); 
		}
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
					this.element.innerHTML = "";
					this.init();
					return;
			}
		}
    }
});