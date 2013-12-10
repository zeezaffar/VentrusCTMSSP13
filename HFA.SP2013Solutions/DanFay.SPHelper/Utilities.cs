using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Collections;
using System.IO;
using Microsoft.SharePoint.Workflow;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text.RegularExpressions;

namespace DanFay.SPHelper
{
   public static class Utilities
    {
       public enum IMV_TABS
       {
           Visit_Attendees = 0,
           Subject_Recruitment = 1,
           Informed_Consent = 2,
           Documentation = 3,
           Adverse_Events = 4,
           Site_File = 5,
           Supplies = 6,
           Laboratory = 7,
           SiteStaffChanges = 8,
           SiteAcceptability = 9,
           Discussion = 10
       }

       public enum COV_TABS
       {
           Visit_Attendees = 0,
           Subject_Recruitment = 1,
           Site_File = 2,
           InformedConsent = 3,
           Adverse_Events = 4,
           InvestigationalProduct = 5,
           TrialMaterial = 6,
           Discussion = 7
       }

       public static string GetDDLStringValue(object field)
       {
           if (field != null)
               return field.ToString();
           else
               return "Y";
       }

       public static string GetStringValue(object field)
       {
           if (field != null)
               return field.ToString();
           else
               return string.Empty;
       }

       public static string GetRichTextValue(object field)
       {
           string returnString = string.Empty;

           if (field != null)
           {
               XmlDocument xmlDoc = new XmlDocument();
               xmlDoc.LoadXml(field.ToString());
               returnString = xmlDoc.InnerText;
           }

           return returnString;
       }

       public static string GetReportStringValue(object field)
       {
           if (field != null)
               return RemoveBadChars(field.ToString());
           else
               return "&nbsp;";
       }

       public static string GetShortDateValue(object field)
       {
           if (field != null)
               return Convert.ToDateTime(field.ToString()).ToShortDateString();
           else
               return string.Empty;
       }

       public static string GetLookupFieldValue2(object field)
       {
           if (field != null)
           {
               SPFieldLookupValue lookupValue = new SPFieldLookupValue(field.ToString());
               if (lookupValue != null)
                   return lookupValue.LookupValue;
               else
                   return string.Empty;

           }
           return string.Empty;
       }

       public static string GetLookupFieldValue(object field)
       {
           if (field != null)
           {
               SPFieldLookupValue lookup = new SPFieldLookupValue(field.ToString());
               string[] valueArray = lookup.LookupValue.Split('.');

               if (valueArray[0].Length > 0)
                   return valueArray[0].ToString();
               else
                   return string.Empty;
           }
           else
               return string.Empty;
       }

       public static string GetMultiLineTextFieldValue(SPListItem item, string fieldName)
       {
           try
           {
               SPFieldMultiLineText field = item.Fields.GetField(fieldName) as SPFieldMultiLineText;
               return field.GetFieldValueAsText(item[fieldName]);
           }
           catch
           {
               return string.Empty;
           }
       }

       public static string GetReportMultiLineTextFieldValue(SPListItem item, string fieldName)
       {
           string fieldValue = string.Empty;

           try
           {
               SPFieldMultiLineText field = item.Fields.GetField(fieldName) as SPFieldMultiLineText;
               fieldValue = field.GetFieldValueAsText(item[fieldName]).Trim();
               return RemoveBadChars(fieldValue.ToString());
               //return System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(fieldValue.ToString()));
           }
           catch
           {
               return "&nbsp;";
           }
       }

       public static string GetMultiLineReportTextFieldValue(SPListItem item, string fieldName)
       {
           string value = string.Empty;

           string emptyReturnValue = "&nbsp;";
 
           try
           {
               SPFieldMultiLineText field = item.Fields.GetField(fieldName) as SPFieldMultiLineText;
               value = field.GetFieldValueAsText(item[fieldName]);

               if (value.Length > 0)
                   return RemoveBadChars(value);
               else
                return  emptyReturnValue;
           }
           catch
           {
               return emptyReturnValue;
           }
       }

       public static bool FileExists(SPFolder folder, string fileName)
       {
           foreach (SPFile file in folder.Files)
           {
               if (file.Name.Trim() == fileName.Trim())
               {
                   return true;
               }
           }
           return false;
       }

       public static bool UplodFileToDocLibrary(byte[] fileBytes, string docLibraryName, string fileName, int siteNo, string reportType)
       {
           try
           {


               SPFolder library = SPContext.Current.Web.Folders[docLibraryName];

               Hashtable ht = new Hashtable();
               SPListItem site = Queries.GetSiteBySiteNo(siteNo);

               SPFieldLookupValue siteNumberLookupValue = new SPFieldLookupValue(site.ID, site.Title);

               ht["Report_x0020_Type"] = reportType;

               SPFile spfile = library.Files.Add(fileName, fileBytes, false);
             
               SPListItem item = spfile.Item;
               item.Update();

               SPField siteNumberLookupField = item.Fields.GetField("Site Number");

               item[siteNumberLookupField.Id] = siteNumberLookupValue.ToString();
               item["Report_x0020_Type"] = reportType;

               item.Update();

               return true;
           }
           catch (Exception ex)
           {
               throw new Exception("UplodFileToDocLibrary:" + ex.Message);
           }

       }
     
       public static bool UplodFileToDocLibrary(string docLibraryName, string fileToUpload, int siteNo, string reportType)
       {
           try
           {
               //Throw error if file doesn't exist
               if (!System.IO.File.Exists(fileToUpload))
                   throw new FileNotFoundException("File not found", fileToUpload);

               //Continue if file exists
               SPFolder library = SPContext.Current.Web.Folders[docLibraryName];

               SPListItem site = Queries.GetSiteBySiteNo(siteNo);

               FileStream stream = File.OpenRead(fileToUpload);
               string fileName = System.IO.Path.GetFileName(fileToUpload);
               SPFile spfile = library.Files.Add(fileName, stream, false);

               SPListItem item = spfile.Item;
               item.Update();

               //Fill metadata fields
               SPFieldLookupValue siteNumberLookupValue = new SPFieldLookupValue(site.ID, site.Title);
               SPField siteNumberLookupField = item.Fields.GetField("Site Number");
               item[siteNumberLookupField.Id] = siteNumberLookupValue.ToString();
               item["Report_x0020_Type"] = reportType;

               //Save changes
               item.Update();

               return true;
           }
           catch (Exception ex)
           {
               throw new Exception("UplodFileToDocLibrary:" + ex.Message);
           }
       }

       public static bool UplodFileToDocLibrary(SPWeb web, string docLibraryName, string fileToUpload, int siteNo, string reportType, string wfName)
       {
           try
           {
               //Throw error if file doesn't exist
               if (!System.IO.File.Exists(fileToUpload))
                   throw new FileNotFoundException("File not found", fileToUpload);

               //Continue if file exists
               SPFolder library = web.Folders[docLibraryName];

               SPListItem site = Queries.GetSiteBySiteNo(siteNo);

               FileStream stream = File.OpenRead(fileToUpload);
               string fileName = System.IO.Path.GetFileName(fileToUpload);
               web.AllowUnsafeUpdates = true;

               SPFile spfile = library.Files.Add(fileName, stream, false);
               library.Update(); //Commit document upload add

               SPListItem item = spfile.Item;

               //Fill metadata fields
               SPFieldLookupValue siteNumberLookupValue = new SPFieldLookupValue(site.ID, site.Title);
               SPField siteNumberLookupField = item.Fields.GetField("Site Number");
               item[siteNumberLookupField.Id] = siteNumberLookupValue.ToString();
               item["Report_x0020_Type"] = reportType;

               SPFieldUrlValue sitePageUrl = new SPFieldUrlValue();

               //Site Page hyperlink field
               sitePageUrl.Url = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, siteNo,site.ID);
               sitePageUrl.Description = siteNo.ToString();
               item["Site_x0020_Page"] = sitePageUrl;

               item.Update();

               //SPListItem wfItem = spfile.Item;
               //SPWorkflowAssociation wfAssoc = item.ParentList.WorkflowAssociations.GetAssociationByName(wfName, System.Globalization.CultureInfo.CurrentCulture);
               //web.Site.WorkflowManager.StartWorkflow(wfItem, wfAssoc, wfAssoc.AssociationData, true);
               ////Save changes
               //wfItem.Update();

               web.AllowUnsafeUpdates = false;

               return true;
           }
           catch (SPException ex)
           {
               if (ex.ErrorCode == -2130575305)
                   return true;
               else
                   return false;
           }
       }

       public static bool UplodFileToDocLibrary2(string docLibraryName, string fileToUpload, int siteNo, string reportType)
       {
           try
           {
               //Throw error if file doesn't exist
               if (!System.IO.File.Exists(fileToUpload))
                   throw new FileNotFoundException("File not found", fileToUpload);

               
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;


            //SPSecurity.RunWithElevatedPrivileges(delegate()
            //{
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        web.AllowUnsafeUpdates = true;
                        //Continue if file exists
                        SPFolder library = web.Folders[docLibraryName];

                        SPListItem siteItem = Queries.GetSiteBySiteNo(siteNo);
                        siteItem.Web.AllowUnsafeUpdates = true;

                        FileStream stream = File.OpenRead(fileToUpload);
                        string fileName = System.IO.Path.GetFileName(fileToUpload);
                        SPFile spfile = library.Files.Add(fileName, stream, false);
                        library.Update();

                        SPListItem fileItem = spfile.Item;
                        //fileItem.Update();

                        //Fill metadata fields
                        SPFieldLookupValue siteNumberLookupValue = new SPFieldLookupValue(siteItem.ID, siteItem.Title);
                        SPField siteNumberLookupField = fileItem.Fields.GetField("Site Number");
                        fileItem[siteNumberLookupField.Id] = siteNumberLookupValue.ToString();
                        fileItem["Report_x0020_Type"] = reportType;

                        //Save changes
                        fileItem.Update();

                        fileItem.Web.AllowUnsafeUpdates = false;
                        web.AllowUnsafeUpdates = false;
                    }
                }
            //});

               return true;
           }
           catch (Exception ex)
           {
               throw new Exception("UplodFileToDocLibrary:" + ex.Message);
           }
       }

       public static string GetIssueItemHTML()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("<tr><td></td><td style='text-align:center;'>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>");
           sb.Append("<tr><td class='columnHeading' style='width:140px;'>Description</td><td colspan='5'>{5}</td></tr>");
           sb.Append("<tr><td class='columnHeading' style='width:140px;'>Action</td><td colspan='5'>{6}</td></tr>");

           return sb.ToString();
       }

       public static string GetSubjectItemHTML()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>");
           return sb.ToString();
       }

       public static string GetSAEHTML()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>");
           return sb.ToString();
       }

       public static string GetSATHTML()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("<tr><td>{0}</td><td>{1}</td></tr>");
           return sb.ToString();
       }

       public static List<string> GetVersions()
       {
           List<string> versions = new List<string>();

           versions.Add("Draft");
           versions.Add("Final");
           versions.Add("Revision 1");
           versions.Add("Revision 2");

           return versions;
       }

       public static string RemoveBadChars(string text)
       {
           string returnString = text;
           char[] BAD_CHARS = new char[] { '“', '‘', '’', '–', '”' };

           char goodChar1 = '"';
           string goodChar2 = "'";
           string goodChar3 = "-";

           foreach (char bad in BAD_CHARS)
           {
               if (returnString.Contains(bad) && bad == '“')
                   returnString = returnString.Replace(bad.ToString(), goodChar1.ToString());

               if (returnString.Contains(bad) && bad == '‘')
                   returnString = returnString.Replace(bad.ToString(), goodChar2.ToString());

               if (returnString.Contains(bad) && bad == '’')
                   returnString = returnString.Replace(bad.ToString(), goodChar2.ToString());

               if (returnString.Contains(bad) && bad == '–')
                   returnString = returnString.Replace(bad.ToString(), goodChar3.ToString());

               if (returnString.Contains(bad) && bad == '”')
                   returnString = returnString.Replace(bad.ToString(), goodChar1.ToString());
           }

           if (returnString.Contains("\r\n"))
               returnString = returnString.Replace("\r\n", "<br>");

           returnString = Regex.Replace(returnString, @"[^\u0000-\u007F]", "");

           return returnString;
       }
   }
}
