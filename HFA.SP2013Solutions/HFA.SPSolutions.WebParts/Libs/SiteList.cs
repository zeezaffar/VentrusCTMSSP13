using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DanFay.SPHelper;
using Microsoft.SharePoint;

namespace HFA.SPSolutions.WebParts.Libs
{
    public class SiteList
    {
        public int ID
        { get; set; }

        public string InvestigatorName
        { get; set; }

        public string InvestigatorTitle
        { get; set; }

        public string Address
        { get; set; }

        public int SiteNumber
        { get; set; }

        public bool Exists
        { get; set; }

        public string Protocol
        { get; set; }

        public SiteList(int siteNumber)
        {
            SPListItem item = Queries.GetSiteBySiteNo(siteNumber);

            if (item != null)
            {
                ID = item.ID;
                InvestigatorName = Utilities.GetStringValue(item["Investigator_x0020_Name"]);
                InvestigatorTitle = "Principal Investigator";
                Address = Utilities.GetStringValue(item["Address"]);
                SiteNumber = siteNumber;
                Protocol = Utilities.GetStringValue(item["Protocol"]);
                Exists = true;
            }
            else
                Exists = false;
        }
    }
}
