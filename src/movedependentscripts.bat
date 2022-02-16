@cd /d "%~dp0"

xcopy "../../designer-repos/dashboard-designer-web-designer/designer-source/webdesignerservice/themes/fonts/*.*"  "fonts/" /f /s /y /r


xcopy "../../designer-repos/dashboard-designer-web-designer/designer-source/webdesignerservice/themes/*.*"  "themes/" /f /s /y /r


xcopy "../../designer-repos/dashboard-designer-web-designer/designer-source/webdesignerservice/Scripts/*.*"  "scripts/" /f /s /y /r


xcopy "../../designer-repos/dashboard-designer-web-designer/designer-source/webdesignerservice/Locale/*.*"  "locale/" /f /s /y /r

xcopy "../../designer-repos/dashboard-designer-web-designer/designer-source/webdesignerservice/Scripts/ShapeFiles/*.*"  "shapefiles/" /f /s /y /r


