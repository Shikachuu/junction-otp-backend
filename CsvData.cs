using System;
using System.Collections.Generic;
namespace junctionx_backend
{
    public class CsvData
    {
        public string YYYYMM;
        public bool ATM_DEPOSIT_FL;
        public int ZIP_CD;
        public string CITY;
        public string STREET_ADDRESS;
        public double GEO_X;
        public double GEO_Y;
        public string TRX_DAY;
        public Dictionary<string,int> TarnsCount;
        public CsvData()
        {
            
        }
    }
}
