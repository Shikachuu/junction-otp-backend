using System;
using System.Collections.Generic;
namespace junctionx_backend
{
    public class CsvData
    {
        public string YYYYMM {get;set;}
        public bool ATM_DEPOSIT_FL {get;set;}
        public int ZIP_CD {get;set;}
        public string CITY {get;set;}
        public string STREET_ADDRESS {get;set;}
        public double GEO_X {get;set;}
        public double GEO_Y {get;set;}
        public string TRX_DAY {get;set;}
        public Dictionary<string,int> TarnsCount {get;set;}
    }
}
