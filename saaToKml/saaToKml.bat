DEL saa*.kml
DEL saa*.txt

DEL temp.xml

REM saaToKml.exe "A-381 GULF COAST, GULF OF MEXICO.xml"
REM program also used in LoadFlightPlanTables
saaToKml.exe

REM SET GDAL_DATA=C:\\Program Files\\QGIS 3.22.1\\apps\\gdal-dev\\data
REM SET GDAL_DRIVER_PATH=C:\\Program Files\\QGIS 3.22.1\\bin\\gdalplugins
REM SET OSGEO4W_ROOT=C:\\Program Files\\QGIS 3.22.1
REM SET PATH=%OSGEO4W_ROOT%\\bin;%PATH%
REM SET PYTHONHOME=%OSGEO4W_ROOT%\\apps\\Python37
REM SET PYTHONPATH=%OSGEO4W_ROOT%\\apps\\Python37

REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\moa.shp" "moa.kml" moa
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\prohibited.shp" "prohibited.kml" prohibited
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\restricted.shp" "restricted.kml" restricted
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\alert.shp" "alert.kml" alert
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\warning.shp" "warning.kml" warning
REM ogr2ogr.exe -f "ESRI Shapefile" -skipfailures "shape\\national.shp" "national.kml" national
