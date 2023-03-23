using Newtonsoft.Json;
using Ports.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PortUpload
{
    public class PortHelper
    {
        PortsEntities _db = new PortsEntities();
        public async Task<string> PortData(HttpPostedFileBase file, string path)
        {
            var serializer = new JavaScriptSerializer();
            string response = string.Empty;
            string filePath = path + "\\" + "Ports.json";
            string str = (new StreamReader(file.InputStream)).ReadToEnd();

            List<PortData> allItemsObj;
            List<PortData> portdata = JsonConvert.DeserializeObject < List<PortData>>(str);
            response = AddPortByFile(portdata);
            if (response == "Data added successfully")
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string jsonstring = sr.ReadToEnd();

                    allItemsObj = serializer.Deserialize<List<PortData>>(jsonstring);
                    foreach (var item in portdata)
                    {
                        allItemsObj.Add(item);
                    }
                }

                using (StreamWriter sr = new StreamWriter(filePath))
                {
                    var jsonData = serializer.Serialize(allItemsObj);

                    sr.Write(jsonData);
                }
            }

            return response;
        }

        public async Task<IEnumerable<PORTDATA>> GetPort()
        {
            IEnumerable<PORTDATA> portdata = _db.PORTDATAs;
            return portdata;
        }

        public PORTDATA GetPortbyId(int id)
        {
            var pData = _db.PORTDATAs.Find(id);
            return pData;
        }

        public async Task<string> AddPort(PORTDATA pData)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path1 = dir + "/Data/Ports.json";

            _db.PORTDATAs.Add(pData);
            await _db.SaveChangesAsync();

            try
            {
                string jsonstring = System.IO.File.ReadAllText(path1);
                var serializer = new JavaScriptSerializer();
                List<PORTDATA> pJSONData = serializer.Deserialize<List<PORTDATA>>(jsonstring);
                pJSONData.Add(pData);

                var jsonData = serializer.Serialize(pJSONData);

                File.WriteAllText(path1, jsonData);
                return "Port Added";
            }

            catch (Exception ex)
            {
                LogError(ex);
                return ex.Message;
            }
        }

        public async Task<string> Delete(int id)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path1 = dir + "/Data/Ports.json";
            var pData = _db.PORTDATAs.Find(id);
            if (pData != null)
            {
                string jsonstring = System.IO.File.ReadAllText(path1);
                var serializer = new JavaScriptSerializer();
                List<PORTDATA> pJSONData = serializer.Deserialize<List<PORTDATA>>(jsonstring);
                foreach (PORTDATA item in pJSONData)
                {
                    if (item.ID == pData.ID)
                    {
                        pJSONData.Remove(item);
                        break;
                    }
                }
                string json = serializer.Serialize(pJSONData);
                System.IO.File.WriteAllText(path1, json);

                _db.PORTDATAs.Remove(pData);
                _db.SaveChanges();
                return "Port Delete successfully";
            }
            else
            {
                return "Port not found";
            }
        }

        public string GetCultureInfo(string country)
        {
            string countryCode = string.Empty;
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo regionInfo = new RegionInfo(getCulture.LCID);
                if (regionInfo.DisplayName.ToUpper() == country)
                {
                    countryCode = regionInfo.TwoLetterISORegionName;
                }
            }

            return countryCode;
        }

        public List<string> getCountryList()
        {
            List<string> CultureList = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo regionInfo = new RegionInfo(getCulture.LCID);
                if (!(CultureList.Contains(regionInfo.EnglishName)))
                {
                    CultureList.Add(regionInfo.EnglishName);
                }
            }
            CultureList.Sort();
            return CultureList;
        }

        public string AddPortByFile(List<PortData> allItemsObj)
        {
            PORTDATA pData = new PORTDATA();
            try
            {
                using (var _db = new PortsEntities())
                {
                    foreach (var item in allItemsObj)
                    {
                        pData.NAME = item.NAME;
                        pData.PortCode = item.PortCode;
                        pData.UnctadPortCode = item.UnctadPortCode;
                        pData.Country = item.Country;
                        string lat;
                        if (item.Latitude.ToString().Contains(","))
                        {
                            lat = item.Latitude.ToString().Replace(",", ".");
                            pData.Latitude = lat;
                        }
                        else
                        {
                            pData.Latitude = item.Latitude;
                        }
                        string lon;
                        if (item.Longitude.ToString().Contains(","))
                        {
                            lon = item.Longitude.ToString().Replace(",", ".");
                            pData.Longitude = lon;
                        }
                        else
                        {
                            pData.Longitude = item.Longitude;
                        }
                        pData.Url = item.Url;
                        pData.MainPortCode = item.MainPortCode;
                        _db.PORTDATAs.Add(pData);
                        _db.SaveChanges();
                    }
                }

                return "Data added successfully";
            }

            catch (Exception ex)
            {
                LogError(ex);
                return ex.Message;
            }
        }

        public void LogError(Exception ex)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path =  dir + "/Error/log.txt";
            File.AppendAllText(path, ex.ToString());
        }
    }

    public class AllItems
    {
        public List<PORTDATA> portData { get; set; }
    }

    public class PortData
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string PortCode { get; set; }
        public string UnctadPortCode { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Url { get; set; }
        public string MainPortCode { get; set; }
    }
}