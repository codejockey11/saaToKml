using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Collections.Generic;
using System.Web;
using aviationLib;
using System.Web.UI.WebControls;

namespace saaToKml
{
    class Program
    {
        static XmlTextReader reader;

        static readonly StreamWriter saaMoa = new StreamWriter("saaMoa.kml");
        static readonly StreamWriter saaControlled = new StreamWriter("saaControlled.kml");
        static readonly StreamWriter saaProhibited = new StreamWriter("saaProhibited.kml");
        static readonly StreamWriter saaRestricted = new StreamWriter("saaRestricted.kml");
        static readonly StreamWriter saaAlert = new StreamWriter("saaAlert.kml");
        static readonly StreamWriter saaWarning = new StreamWriter("saaWarning.kml");
        static readonly StreamWriter saaNational = new StreamWriter("saaNational.kml");

        static readonly StreamWriter saaLocation = new StreamWriter("saaLocation.txt");
        static readonly StreamWriter saaGeometry = new StreamWriter("saaGeometry.txt");
        static readonly StreamWriter saaTimes = new StreamWriter("saaTimes.txt");
        static readonly StreamWriter saaNote = new StreamWriter("saaNote.txt");

        static String name;
        static String designator;

        static String upperLimituom;
        static String upperLimit;
        static String upperLimitReference;

        static String lowerLimituom;
        static String lowerLimit;
        static String lowerLimitReference;

        static readonly List<String> pos = new List<String>();

        static String radiusuom;
        static String radius;

        static String startAngle;
        static String endAngle;

        static Int16 sequence;

        static void Main(string[] args)
        {
            ZipArchive archive = ZipFile.OpenRead("Saa_Sub_File.zip");

            if (args.Length == 0)
            {
                WriteDocumentStart(saaMoa);
                WriteDocumentStart(saaControlled);
                WriteDocumentStart(saaProhibited);
                WriteDocumentStart(saaRestricted);
                WriteDocumentStart(saaAlert);
                WriteDocumentStart(saaWarning);
                WriteDocumentStart(saaNational);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile("temp.xml", true);

                    StreamWriter sw = saaControlled;

                    if (entry.Name.Contains("A-"))
                    {
                        sw = saaAlert;
                    }

                    if (entry.Name.Contains("MOA"))
                    {
                        sw = saaMoa;
                    }

                    if (entry.Name.Contains(" NSA"))
                    {
                        sw = saaNational;
                    }

                    if (entry.Name.Contains("P-"))
                    {
                        sw = saaProhibited;
                    }

                    if (entry.Name.Contains("R-"))
                    {
                        sw = saaRestricted;
                    }

                    if (entry.Name.Contains("W-105"))
                    {
                        sw = saaWarning;
                    }

                    ProcessSpecialUseAirspace(sw);
                }

                WriteDocumentEnd(saaMoa);
                WriteDocumentEnd(saaControlled);
                WriteDocumentEnd(saaProhibited);
                WriteDocumentEnd(saaRestricted);
                WriteDocumentEnd(saaAlert);
                WriteDocumentEnd(saaWarning);
                WriteDocumentEnd(saaNational);

                saaMoa.Close();
                saaControlled.Close();
                saaProhibited.Close();
                saaRestricted.Close();
                saaAlert.Close();
                saaWarning.Close();
                saaNational.Close();

                saaLocation.Close();
                saaGeometry.Close();
                saaTimes.Close();
                saaNote.Close();
            }
            else
            {
                ZipArchiveEntry entry = archive.GetEntry(args[0]);

                entry.ExtractToFile("temp.xml", true);

                StreamWriter sw = saaControlled;

                if (entry.Name.Contains("A-"))
                {
                    sw = saaAlert;
                }

                if (entry.Name.Contains("MOA"))
                {
                    sw = saaMoa;
                }

                if (entry.Name.Contains("NSA"))
                {
                    sw = saaNational;
                }

                if (entry.Name.Contains("P-"))
                {
                    sw = saaProhibited;
                }

                if (entry.Name.Contains("R-"))
                {
                    sw = saaRestricted;
                }

                if (entry.Name.Contains("W-"))
                {
                    sw = saaWarning;
                }

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                sw.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
                sw.WriteLine("<Document>");
                sw.WriteLine("\t<Style id=\"arbHigh\">");
                sw.WriteLine("\t\t<LineStyle>");
                sw.WriteLine("\t\t\t<color>FF006400</color>");
                sw.WriteLine("\t\t\t<width>2</width>");
                sw.WriteLine("\t\t</LineStyle>");
                sw.WriteLine("\t\t<PolyStyle>");
                sw.WriteLine("\t\t\t<color>1196FF5A</color>");
                sw.WriteLine("\t\t</PolyStyle>");
                sw.WriteLine("\t</Style>");
                sw.WriteLine("\t<Style id=\"arbLow\">");
                sw.WriteLine("\t\t<LineStyle>");
                sw.WriteLine("\t\t\t<color>FF5C5CCD</color>");
                sw.WriteLine("\t\t\t<width>2</width>");
                sw.WriteLine("\t\t</LineStyle>");
                sw.WriteLine("\t\t<PolyStyle>");
                sw.WriteLine("\t\t\t<color>118080F0</color>");
                sw.WriteLine("\t\t</PolyStyle>");
                sw.WriteLine("\t</Style>");
                sw.WriteLine("\t<Style id=\"arbOther\">");
                sw.WriteLine("\t\t<LineStyle>");
                sw.WriteLine("\t\t\t<color>FFFF6400</color>");
                sw.WriteLine("\t\t\t<width>2</width>");
                sw.WriteLine("\t\t</LineStyle>");
                sw.WriteLine("\t\t<PolyStyle>");
                sw.WriteLine("\t\t\t<color>1169FFA5</color>");
                sw.WriteLine("\t\t</PolyStyle>");
                sw.WriteLine("\t</Style>");

                ProcessSpecialUseAirspace(sw);

                sw.WriteLine("</Document>");
                sw.WriteLine("</kml>");

                sw.Close();

                saaLocation.Close();
                saaGeometry.Close();
                saaTimes.Close();
                saaNote.Close();
            }

        }

        static void WriteDocumentStart(StreamWriter kml)
        {
            kml.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            kml.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            kml.WriteLine("<Document>");
            kml.WriteLine("\t<Style id=\"arbHigh\">");
            kml.WriteLine("\t\t<LineStyle>");
            kml.WriteLine("\t\t\t<color>FF006400</color>");
            kml.WriteLine("\t\t\t<width>2</width>");
            kml.WriteLine("\t\t</LineStyle>");
            kml.WriteLine("\t\t<PolyStyle>");
            kml.WriteLine("\t\t\t<color>1196FF5A</color>");
            kml.WriteLine("\t\t</PolyStyle>");
            kml.WriteLine("\t</Style>");
            kml.WriteLine("\t<Style id=\"arbLow\">");
            kml.WriteLine("\t\t<LineStyle>");
            kml.WriteLine("\t\t\t<color>FF5C5CCD</color>");
            kml.WriteLine("\t\t\t<width>2</width>");
            kml.WriteLine("\t\t</LineStyle>");
            kml.WriteLine("\t\t<PolyStyle>");
            kml.WriteLine("\t\t\t<color>118080F0</color>");
            kml.WriteLine("\t\t</PolyStyle>");
            kml.WriteLine("\t</Style>");
            kml.WriteLine("\t<Style id=\"arbOther\">");
            kml.WriteLine("\t\t<LineStyle>");
            kml.WriteLine("\t\t\t<color>FFFF6400</color>");
            kml.WriteLine("\t\t\t<width>2</width>");
            kml.WriteLine("\t\t</LineStyle>");
            kml.WriteLine("\t\t<PolyStyle>");
            kml.WriteLine("\t\t\t<color>1169FFA5</color>");
            kml.WriteLine("\t\t</PolyStyle>");
            kml.WriteLine("\t</Style>");
        }

        static void WriteDocumentEnd(StreamWriter kml)
        {
            kml.WriteLine("</Document>");
            kml.WriteLine("</kml>");
        }

        static void WriteLineStringSegment(StreamWriter kml)
        {
            reader.Read();

            Boolean notDone = true;

            while (notDone)
            {
                if (reader.Name == "pos")
                {
                    pos.Add(reader.ReadElementContentAsString());
                }
                else
                {
                    notDone = false;
                }
            }

            kml.WriteLine("\t<Placemark>");
            kml.WriteLine("\t\t<name>" + name + "</name>");
            kml.WriteLine("\t\t<styleUrl>#arbHigh</styleUrl>");
            kml.WriteLine("\t\t<LineString>");
            kml.WriteLine("\t\t\t<coordinates>");

            Int16 count = 0;

            foreach (String p in pos)
            {
                String[] pa = p.Split(' ');

                LatLon all = new LatLon(pa[1], pa[0]);

                kml.WriteLine("\t\t\t\t" + all.decimalLon.ToString("F6") + "," + all.decimalLat.ToString("F6") + ",100");

                saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + all.decimalLon.ToString("F6") + "~" + all.decimalLat.ToString("F6"));

                count++;
            }

            kml.WriteLine("\t\t\t</coordinates>");
            kml.WriteLine("\t\t</LineString>");
            kml.WriteLine("\t</Placemark>");

            pos.Clear();

        }

        static void WriteLinearRing(StreamWriter kml)
        {
            reader.Read();

            Boolean notDone = true;

            while (notDone)
            {
                if (reader.Name == "pos")
                {
                    pos.Add(reader.ReadElementContentAsString());
                }
                else
                {
                    notDone = false;
                }
            }

            kml.WriteLine("\t<Placemark>");
            kml.WriteLine("\t\t<name>" + name + "</name>");
            kml.WriteLine("\t\t<styleUrl>#arbHigh</styleUrl>");
            kml.WriteLine("\t\t<LineString>");
            kml.WriteLine("\t\t\t<coordinates>");

            Int16 count = 0;

            foreach (String p in pos)
            {
                String[] pa = p.Split(' ');

                LatLon all = new LatLon(pa[1], pa[0]);

                kml.WriteLine("\t\t\t\t" + all.decimalLon.ToString("F6") + "," + all.decimalLat.ToString("F6") + ",100");

                saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + all.decimalLon.ToString("F6") + "~" + all.decimalLat.ToString("F6"));

                count++;
            }

            String[] posa = pos[0].Split(' ');

            LatLon fll = new LatLon(posa[1], posa[0]);

            kml.WriteLine("\t\t\t\t" + fll.decimalLon.ToString("F6") + "," + fll.decimalLat.ToString("F6") + ",100");

            kml.WriteLine("\t\t\t</coordinates>");
            kml.WriteLine("\t\t</LineString>");
            kml.WriteLine("\t</Placemark>");

            pos.Clear();

        }

        /*
        gml:ArcByCenterPoint 

        A gml:ArcByCenterPoint element contains
            a gml:pos element to represent the point where the arc is centered around,
            a gml:radius element to represent the distance the arc is from the center point,
            and gml:startAngle and gml:endAngle elements. 

        <gml:ArcByCenterPoint numArc=”1”> 
            <gml:pos>-91 39</gml:pos> 
            <gml:radius uom=”NM”>5</gml:radius> 
            <gml:startAngle uom=”deg”>0.0</gml:startAngle> 
            <gml:endAngle uom=”deg”>45.0</gml:endAngle> 
        </gml:ArcByCenterPoint>  

        The startAngle and the endAngle will capture how the arc is drawn from the beginning and ending points.
        This information is determined through the following two rules using non-normalized start and end angles:
            If the start angle is smaller than the end angle, then the arc will be drawn counterclockwise from the start angle to the end angle. 
            If the start angle is larger than the end angle, then the arc will be drawn clockwise from the start angle to the end angle. 
        */

        static void WriteArcByCenterPoint(StreamWriter kml)
        {
            while (reader.Read())
            {
                if (reader.Name == "pos")
                {
                    pos.Add(reader.ReadElementContentAsString());
                }

                if (reader.Name == "radius")
                {
                    radiusuom = reader.GetAttribute("uom");

                    radius = reader.ReadElementContentAsString();

                    if (radiusuom.Contains("FT"))
                    {
                        radius = Convert.ToString(Convert.ToDouble(radius) / 5280.0);
                    }
                }

                if (reader.Name == "startAngle")
                {
                    startAngle = reader.ReadElementContentAsString();
                }

                if (reader.Name == "endAngle")
                {
                    endAngle = reader.ReadElementContentAsString();

                    break;
                }
            }

            String[] p = pos[0].Split(' ');

            LatLon center = new LatLon(p[1], p[0]);

            Double dist = Convert.ToDouble(radius);

            Double res = 10;

            Double start = Convert.ToDouble(startAngle);
            Double end = Convert.ToDouble(endAngle);

            String direction = "L";
            if (start > end)
            {
                direction = "R";
            }

            start = DegreeToTrueCourse(start);
            end = DegreeToTrueCourse(end);

            kml.WriteLine("\t<Placemark>");
            kml.WriteLine("\t\t<name>" + name + "</name>");
            //kml.WriteLine("    <name>" + direction + "/" + startAngle + "/" + start + "/" + endAngle + "/" + end + "/" + center.decimalLat.ToString("F10") + "/" + center.decimalLon.ToString("F10") + "</name>");

            kml.WriteLine("\t\t<styleUrl>#arbHigh</styleUrl>");
            kml.WriteLine("\t\t<LineString>");
            kml.WriteLine("\t\t\t<coordinates>");

            LatLon sll = center.PointFromHeadingDistance(dist * 1.15, start);
            LatLon ell = center.PointFromHeadingDistance(dist * 1.15, end);

            // dtc = distance true course for each leg of arc
            Double dtc;

            if (direction == "L")
            {
                if (end > start)
                {
                    dtc = Math.Abs((start + 360) - end) / res;
                }
                else
                {
                    dtc = Math.Abs(start - end) / res;
                }
            }
            else
            {
                if (start > end)
                {
                    dtc = Math.Abs(start - (end + 360)) / res;
                }
                else
                {
                    dtc = Math.Abs(start - end) / res;
                }
            }
            
            Int16 count = 0;

            kml.WriteLine("\t\t\t\t" + sll.decimalLon.ToString("F6") + "," + sll.decimalLat.ToString("F6") + ",100");

            saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + sll.decimalLon.ToString("F6") + "~" + sll.decimalLat.ToString("F6"));

            count++;

            // ai arc index
            for (Int32 ai = 1; ai < res; ai++)
            {
                if (direction == "L")
                {
                    Double tc = start - (dtc * ai);

                    if (tc < 0)
                    {
                        tc += 360;
                    }

                    LatLon all = center.PointFromHeadingDistance(dist * 1.15, tc);

                    kml.WriteLine("\t\t\t\t" + all.decimalLon.ToString("F6") + "," + all.decimalLat.ToString("F6") + ",100");


                    saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + all.decimalLon.ToString("F6") + "~" + all.decimalLat.ToString("F6"));

                    count++;
                }
                else
                {
                    Double tc = start + (dtc * ai);

                    if (tc > 360)
                    {
                        tc -= 360;
                    }

                    LatLon all = center.PointFromHeadingDistance(dist * 1.15, tc);

                    kml.WriteLine("\t\t\t\t" + all.decimalLon.ToString("F6") + "," + all.decimalLat.ToString("F6") + ",100");


                    saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + all.decimalLon.ToString("F6") + "~" + all.decimalLat.ToString("F6"));

                    count++;
                }
            }

            kml.WriteLine("\t\t\t\t" + ell.decimalLon.ToString("F6") + "," + ell.decimalLat.ToString("F6") + ",100");

            saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + ell.decimalLon.ToString("F6") + "~" + ell.decimalLat.ToString("F6"));

            kml.WriteLine("\t\t\t</coordinates>");
            kml.WriteLine("\t\t</LineString>");
            kml.WriteLine("\t</Placemark>");

            pos.Clear();
        }

        /*
           page 76-77

                  -270/90
                     |
                     |
        -180/180 ----+---- -360/0/360
                     |
                     |
                 -90/270

        */
        static Double DegreeToTrueCourse(Double deg)
        {
            // positive degree
            if (deg >= 270.0)
            {
                deg = deg - 270.0;
                deg = 180.0 - deg;

                return deg;
            }
            else if (deg >= 180.0)
            {
                deg = deg - 180.0;
                deg = 270.0 - deg;

                return deg;
            }
            else if (deg >= 90.0)
            {
                deg = deg - 90.0;
                deg = 360.0 - deg;

                return deg;
            }
            else if (deg >= 0.0)
            {
                deg = deg - 90.0;
                deg = 360.0 - deg;

                return deg;
            }

            // negative degree
            if (deg <= -270.0)
            {
                deg = deg + 270.0;
                deg = 0.0 + Math.Abs(deg);
            }
            else if (deg <= -180.0)
            {
                deg = deg + 180.0;
                deg = 270.0 + Math.Abs(deg);
            }
            else if (deg <= -90.0)
            {
                deg = deg + 90.0;
                deg = 180.0 + Math.Abs(deg);
            }
            else if (deg <= 0.0)
            {
                deg = deg + 0.0;
                deg = 100.0 + Math.Abs(deg);
            }

            return deg;
        }

        static void WriteCircleByCenterPoint(StreamWriter kml)
        {
            while (reader.Read())
            {
                if (reader.Name == "pos")
                {
                    pos.Add(reader.ReadElementContentAsString());
                }

                if (reader.Name == "radius")
                {
                    radiusuom = reader.GetAttribute("uom");

                    radius = reader.ReadElementContentAsString();

                    if (radiusuom.Contains("FT"))
                    {
                        radius = Convert.ToString(Convert.ToDouble(radius) / 5280.0);
                    }

                    break;
                }
            }

            kml.WriteLine("\t<Placemark>");
            kml.WriteLine("\t\t<name>" + name + "</name>");
            kml.WriteLine("\t\t<styleUrl>#arbHigh</styleUrl>");
            kml.WriteLine("\t\t<LineString>");
            kml.WriteLine("\t\t\t<coordinates>");

            String[] p = pos[0].Split(' ');

            LatLon center = new LatLon(p[1], p[0]);

            Double dist = Convert.ToDouble(radius);

            Double stc = 0;

            Double res = 36;

            Double dtc = 360 / res;

            Int16 count = 0;

            for (Int32 ai = 0; ai < res + 1; ai++)
            {
                Double tc = stc + (dtc * ai);

                LatLon all = center.PointFromHeadingDistance(dist * 1.15, tc);

                kml.WriteLine("\t\t\t\t" + all.decimalLon.ToString("F6") + "," + all.decimalLat.ToString("F6") + ",100");


                saaGeometry.WriteLine(designator + "~" + upperLimituom + "~" + upperLimit + "~" + upperLimitReference + "~" + lowerLimituom + "~" + lowerLimit + "~" + lowerLimitReference + "~LS~" + sequence.ToString("D3") + "~" + count.ToString("D3") + "~" + all.decimalLon.ToString("F6") + "~" + all.decimalLat.ToString("F6"));

                count++;
            }

            kml.WriteLine("\t\t\t</coordinates>");
            kml.WriteLine("\t\t</LineString>");
            kml.WriteLine("\t</Placemark>");

            pos.Clear();
        }

        static void ProcessSpecialUseAirspace(StreamWriter kml)
        {
            reader = new XmlTextReader("temp.xml");

            Boolean airspaceFound = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "hasMember" && airspaceFound)
                    {
                        airspaceFound = false;
                    }

                    if (reader.Name == "Airspace")
                    {
                        airspaceFound = true;
                    }

                    if (airspaceFound)
                    {
                        if (reader.Name == "designator")
                        {
                            designator = reader.ReadElementContentAsString();

                            sequence = 0;
                        }

                        if (reader.Name == "name")
                        {
                            name = reader.ReadElementContentAsString();

                            saaLocation.WriteLine(designator + "~" + name);
                        }

                        if (reader.Name == "AirspaceVolume")
                        {
                            while (reader.Read())
                            {
                                if (reader.Name == "upperLimit")
                                {
                                    upperLimituom = reader.GetAttribute("uom");

                                    upperLimit  = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "upperLimitReference")
                                {
                                    upperLimitReference = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "lowerLimit")
                                {
                                    lowerLimituom = reader.GetAttribute("uom");

                                    lowerLimit = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "lowerLimitReference")
                                {
                                    lowerLimitReference = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "horizontalProjection")
                                {
                                    break;
                                }

                            }
                        }

                        if (reader.Name == "ArcByCenterPoint")
                        {
                            WriteArcByCenterPoint(kml);

                            sequence++;
                        }

                        if (reader.Name == "CircleByCenterPoint")
                        {
                            WriteCircleByCenterPoint(kml);

                            sequence++;
                        }

                        if (reader.Name == "LineStringSegment")
                        {
                            WriteLineStringSegment(kml);

                            sequence++;
                        }

                        if (reader.Name == "LineString")
                        {
                            WriteLinearRing(kml);

                            sequence++;
                        }

                        if (reader.Name == "LinearRing")
                        {
                            WriteLinearRing(kml);

                            sequence++;
                        }

                    }

                    if (reader.Name == "propertyName")
                    {
                        if (reader.ReadElementContentAsString() == "legalDefinitionType")
                        {
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.Name == "note")
                                    {
                                        saaNote.WriteLine(designator + "~" + HttpUtility.HtmlEncode(reader.ReadElementContentAsString().Replace("”", "\"").Replace("’", "'")).Replace("\r\n", "</br>"));
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (reader.Name == "schedule")
                    {
                        String[] startDate = { "", "" };
                        String[] endDate = { "", "" };
                        String startTime = "";
                        String endTime = "";
                        String day = "";
                        String startEvent = "";
                        String endEvent = "";

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (reader.Name == "startDate")
                                {
                                    startDate = reader.ReadElementContentAsString().Split('-');
                                }

                                if (reader.Name == "endDate")
                                {
                                    endDate = reader.ReadElementContentAsString().Split('-');
                                }

                                if (reader.Name == "day")
                                {
                                    day = reader.ReadElementContentAsString();

                                    switch (day)
                                    {
                                        case "ANY":
                                        {
                                            day = "0";

                                            break;
                                        }

                                        case "SUN":
                                        {
                                            day = "1";

                                            break;
                                        }

                                        case "MON":
                                        {
                                            day = "2";

                                            break;
                                        }

                                        case "TUE":
                                        {
                                            day = "3";

                                            break;
                                        }

                                        case "WED":
                                        {
                                            day = "4";

                                            break;
                                        }

                                        case "THU":
                                        {
                                            day = "5";

                                            break;
                                        }

                                        case "FRI":
                                        {
                                            day = "6";

                                            break;
                                        }

                                        case "SAT":
                                        {
                                            day = "7";

                                            break;
                                        }

                                        case "WORK_DAY":
                                        {
                                            day = "9";

                                            break;
                                        }
                                    }

                                }

                                if (reader.Name == "startTime")
                                {
                                    startTime = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "endTime")
                                {
                                    endTime = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "startEvent")
                                {
                                    startEvent = reader.ReadElementContentAsString();
                                }

                                if (reader.Name == "endEvent")
                                {
                                    endEvent = reader.ReadElementContentAsString();
                                }

                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (reader.Name == "Timesheet")
                                {
                                    saaTimes.Write(designator + "~");

                                    saaTimes.Write(startDate[1] + "-" + startDate[0] + "~");
                                    saaTimes.Write(endDate[1] + "-" + endDate[0] + "~");
                                    saaTimes.Write(startTime + "~");
                                    saaTimes.Write(endTime + "~");
                                    saaTimes.Write(day + "~");
                                    saaTimes.Write(startEvent + "~");
                                    saaTimes.Write(endEvent + "~");

                                    saaTimes.Write(saaTimes.NewLine);
                                }
                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (reader.Name == "schedule")
                                {
                                    break;
                                }
                            }
                        }

                    }
                }
            }

            reader.Close();

        }

    }
}
