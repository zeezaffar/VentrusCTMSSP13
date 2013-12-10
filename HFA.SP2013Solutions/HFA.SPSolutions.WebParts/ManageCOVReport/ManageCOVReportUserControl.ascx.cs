using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Configuration;
using System.Web;
using System.Data;
using DanFay.SPHelper;
using System.IO;
using ExpertPdf.HtmlToPdf;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Workflow;
using HFA.SPSolutions.WebParts.Libs;
using System.Text.RegularExpressions;

namespace HFA.SPSolutions.WebParts.ManageCOVReport
{
    public partial class ManageCOVReportUserControl : UserControl
    {
        int newReportId = 0;

        protected string PDFDocumentLibrary
        {
            get
            {
                if (ConfigurationManager.AppSettings["PDFDocumentLibrary"] != null)
                    return ConfigurationManager.AppSettings["PDFDocumentLibrary"].ToString();

                else
                    throw new Exception("Missing PDFDocumentLibrary setting in web.config");
            }
        }

        private string ReportId
        {
            get
            {
                if (HttpUtility.ParseQueryString(Request.Url.Query).Get("ReportId") != null)
                    return (HttpUtility.ParseQueryString(Request.Url.Query).Get("ReportId").ToString());
                else
                    return string.Empty;
            }
        }

        private int SiteNo
        {
            get
            {
                if (HttpUtility.ParseQueryString(Request.Url.Query).Get("Site") != null)
                    return ((int.Parse(HttpUtility.ParseQueryString(Request.Url.Query).Get("Site").ToString())));
                else
                    return 0;
            }
        }

        private int SiteId
        {
            get;
            set;
        }

        private string WorkflowName
        {
            get { return ConfigurationManager.AppSettings["WorkflowName"].ToString(); }
        }

        private string UserToImpersonate
        {
            get { return ConfigurationManager.AppSettings["UserToImpersonate"].ToString(); }
        }

        private SiteList SiteInfo
        {
            get;
            set;
        }

        private int PreviousTabIndex
        {
            get
            {
                if (this.ViewState["PreviousTabIndex"] != null)
                    return int.Parse(this.ViewState["PreviousTabIndex"].ToString());
                else
                    return -1;
            }
            set { this.ViewState["PreviousTabIndex"] = value; }
        }

        private bool ShowMessage
        {
            get
            {
                return Convert.ToBoolean(Application[SPContext.Current.Web.CurrentUser.ID.ToString()]);
            }

            set
            {
                Application[SPContext.Current.Web.CurrentUser.ID.ToString()] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                InitializeForm();

                if (ReportId == "0") { FillSiteData(); return; }

                FillHeaderData();

                FillInvestAtt();
                FillSubjectRecruitment();
                FillCData();
                FillDData();
                FillFData();
                FillGData();
                FillHData();
                FillIData();

                if (ShowMessage)
                {
                    lblMessage.Text = "Report successfully saved";
                    ShowMessage = false;
                }
            }
        }

        protected void InitializeForm()
        {
            //Fill Sites dropdown
            SPListItemCollection items = Queries.GetSites();
            List<SPListItem> sortedItems = (from SPListItem x in items orderby x["Site_x0020_Number"] select x).ToList();

            foreach (SPListItem t in sortedItems)
            {
                ddlSites.Items.Add(t["Site_x0020_Number"].ToString());
            }
            ddlSites.Items.Insert(0, new ListItem("Select", "0"));

            //Fill Version
            ddlVersion.DataSource = Utilities.GetVersions();
            ddlVersion.DataBind();

            SiteInfo = new SiteList(SiteNo);
            btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

            hidSiteId.Value = SiteInfo.ID.ToString();
        }

        protected void FillSiteData()
        {
            SiteInfo = new SiteList(SiteNo);

            if (SiteInfo.Exists)
            {
                txtSponsor.Text = "Ventrus Biosciences";
                txtAddress.Text = SiteInfo.Address;
                txtInvestigatorName.Text = SiteInfo.InvestigatorName;
                txtInvestigatorTitle.Text = SiteInfo.InvestigatorTitle;
                ddlSites.SelectedValue = SiteNo.ToString();
                txtProtocol.Text = SiteInfo.Protocol;
                txtTitle.Text = string.Format("{0}_COV_Report_{1}", SiteNo, DateTime.Now.ToString("ddMMMyyyy").ToUpper());

                btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            }
        }

        protected void FillHeaderData()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            if (item != null)
            {
                txtTitle.Text = Utilities.GetReportStringValue(item.Title);
                txtAddress.Text = Utilities.GetMultiLineTextFieldValue(item, "Address");
                txtInvestigatorName.Text = Utilities.GetStringValue(item["InvestigatorName"]);
                txtSponsor.Text = Utilities.GetStringValue(item["Sponsor"]);
                txtProtocol.Text = Utilities.GetStringValue(item["Protocol_x0020__x0023_"]);

                if (item["Visit_x0020_Date"] != null)
                    calVisitDate.SelectedDate = Convert.ToDateTime(item["Visit_x0020_Date"].ToString());

                //if (item["Next_x0020_Visit_x0020_Date"] != null)
                //    calNextVisitDate.SelectedDate = Convert.ToDateTime(item["Next_x0020_Visit_x0020_Date"].ToString());

                if (item["Status"] != null)
                    ddlStatus.SelectedValue = ddlStatus.Items.FindByText(item["Status"].ToString()).Value;

                if (item["Monitor"] != null)
                {
                    try
                    {
                        SPFieldLookupValue monitorLookupValue = new SPFieldLookupValue(item["Monitor"].ToString());
                        if (monitorLookupValue != null)
                        {
                            SPUser user = SPContext.Current.Web.SiteUsers.GetByID(monitorLookupValue.LookupId);
                            Microsoft.SharePoint.WebControls.PickerEntity entity = new Microsoft.SharePoint.WebControls.PickerEntity();
                            entity.Key = user.LoginName;
                            System.Collections.ArrayList entityArrayList = new System.Collections.ArrayList();
                            entityArrayList.Add(entity);
                            speMonitor.UpdateEntities(entityArrayList);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = ex.Message;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }

                ddlVersion.SelectedValue = Utilities.GetStringValue(item["VersionNumber"]);
                if (item["VersionDate"] != null)
                    calVersionDate.SelectedDate = Convert.ToDateTime(Utilities.GetShortDateValue(item["VersionDate"]));

                txtMiscComments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e__x0020_Miscellaneous_x00");
                txtGenRevComments.Text = Utilities.GetMultiLineTextFieldValue(item, "General_x0020_Reviewer_x0020_Com");

                //Set dropdown to current site
                if (item["Site_x0020_Number"] != null)
                {
                    SPFieldLookupValue siteLookup = new SPFieldLookupValue(item["Site_x0020_Number"].ToString());
                    if (siteLookup.LookupValue != null && siteLookup.LookupValue.Contains("."))
                    {
                        string[] fieldArray = siteLookup.LookupValue.Split('.');
                        ddlSites.Items.FindByText(fieldArray[0]).Selected = true;
                    }
                }
            }
        }

        protected void PDFGenerator()
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
              using (SPSite site = new SPSite(siteID))
              {
                  using (SPWeb web = site.AllWebs[webID])
                  {
                      string pdfFileName = string.Empty;
                      string mainTemplateFileName = "C:\\HFA\\COVReportTemplate-Main.html";
                      string signatureTemplateFileName = "C:\\HFA\\COVReportTemplate-Signature.html";

                      string convertedFileNameMain = "C:\\HFA\\temp\\COVReportConverted-Main.html";
                      string convertedFileNameSignature = "C:\\HFA\\temp\\COVReportConverted-Signature.html";

                      SPListItem report = GetCOVReportData(int.Parse(ReportId));
                      pdfFileName = string.Format("{0}.pdf", report.Title);

                      if (report == null)
                          return;

                      // Read in the contents of the Receipt.htm HTML template file
                      string mainHtml = File.ReadAllText(mainTemplateFileName).Replace("\r\n", string.Empty);

                      #region Tabs
                      //Header
                      mainHtml = mainHtml.Replace("varSponsor", Utilities.GetReportStringValue(report["Sponsor"]));
                      mainHtml = mainHtml.Replace("varInc", Utilities.GetReportStringValue(report["Inc #"]));
                      mainHtml = mainHtml.Replace("varSiteNo", ReportId.ToString());
                      mainHtml = mainHtml.Replace("varStudySiteNumber", Utilities.GetLookupFieldValue(report["Site_x0020_Number"]));
                      mainHtml = mainHtml.Replace("varVisitDate", Utilities.GetShortDateValue(report["Visit Date"]));
                      mainHtml = mainHtml.Replace("varInvestigatorName", Utilities.GetReportStringValue(report["InvestigatorName"]));
                      mainHtml = mainHtml.Replace("varAddress", Utilities.GetReportStringValue(report["Address"]));

                      //Visit Attendees
                      mainHtml = mainHtml.Replace("varInvestigatorTitle", Utilities.GetReportStringValue(report["InvestigatorTitle"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle1", Utilities.GetReportStringValue(report["SitePersonnelTitle"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName1", Utilities.GetReportStringValue(report["SitePersonnelName"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle2", Utilities.GetReportStringValue(report["SitePersonnelTitle2"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName2", Utilities.GetReportStringValue(report["SitePersonnelName2"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle3", Utilities.GetReportStringValue(report["SitePersonnelTitle3"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName3", Utilities.GetReportStringValue(report["SitePersonnelName3"]));

                      mainHtml = mainHtml.Replace("varOtherSitePersonnelComments", Utilities.GetReportStringValue(report["Intro_x002e_Other_x0020_Personne0"]));
                      mainHtml = mainHtml.Replace("varPersonnelComments", Utilities.GetReportStringValue(report["Intro_x002e_Clinical_x0020_Site_0"]));

                      mainHtml = mainHtml.Replace("varMonitorName", Utilities.GetReportStringValue(report["MonitorName"]));
                      mainHtml = mainHtml.Replace("varMonitorTitle", Utilities.GetReportStringValue(report["MonitorTitle"]));

                      mainHtml = mainHtml.Replace("varOtherSitePersonnel", Utilities.GetReportStringValue(report["Intro.Other Personnel"]));
                      mainHtml = mainHtml.Replace("varOtherPersonnel", Utilities.GetReportStringValue(report["Intro.Clinical Site Personnel"]));

                      //Section B - Subject Recruitment
                      mainHtml = mainHtml.Replace("varNumberScreenedFailed", Utilities.GetReportStringValue(report["NumberScreenedFailed"]));
                      mainHtml = mainHtml.Replace("varNumberScreened", Utilities.GetReportStringValue(report["NumberScreened"]));
                      mainHtml = mainHtml.Replace("varNumberRandomized", Utilities.GetReportStringValue(report["NumberRandomized"]));
                      mainHtml = mainHtml.Replace("varNumberActiveTreatment", Utilities.GetReportStringValue(report["NumberActiveTreatment"]));
                      mainHtml = mainHtml.Replace("varNumberCompletedTreatment", Utilities.GetReportStringValue(report["NumberCompletedTreatment"]));
                      mainHtml = mainHtml.Replace("varNumberDiscontinuation", Utilities.GetReportStringValue(report["NumberDiscontinuation"]));

                      //C Section
                      mainHtml = mainHtml.Replace("varC1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varC2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varC3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varC4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varC5Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_5_x0020_Comments"));

                      mainHtml = mainHtml.Replace("varC1", Utilities.GetDDLStringValue(report["C_x002e_1"]));
                      mainHtml = mainHtml.Replace("varC2", Utilities.GetDDLStringValue(report["C_x002e_2"]));
                      mainHtml = mainHtml.Replace("varC3", Utilities.GetDDLStringValue(report["C_x002e_3"]));
                      mainHtml = mainHtml.Replace("varC4", Utilities.GetDDLStringValue(report["C_x002e_4"]));
                      mainHtml = mainHtml.Replace("varC5", Utilities.GetDDLStringValue(report["C_x002e_5"]));

                      //Document section
                      mainHtml = mainHtml.Replace("varCDocument", Utilities.GetDDLStringValue(report["C_x002e_Document"]));
                      mainHtml = mainHtml.Replace("varCDateLastChecked", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_DateLastChecked"));
                      mainHtml = mainHtml.Replace("varCQueriesOutstanding", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_QueriesOutstanding"));

                      //C Date fields
                      mainHtml = mainHtml.Replace("varSiteVisitLogDate", Utilities.GetShortDateValue(report["C_x002e_SiteVisitLogDate"]));
                      mainHtml = mainHtml.Replace("varProtocolDate", Utilities.GetShortDateValue(report["C_x002e_ProtocolDate"]));
                      mainHtml = mainHtml.Replace("varBrochureDate", Utilities.GetShortDateValue(report["C_x002e_BrochureDate"]));
                      mainHtml = mainHtml.Replace("varDisclosureDate", Utilities.GetShortDateValue(report["C_x002e_DisclosureDate"]));
                      mainHtml = mainHtml.Replace("varLicenseDate", Utilities.GetShortDateValue(report["C_x002e_LicenseDate"]));
                      mainHtml = mainHtml.Replace("varRegulatoryDate", Utilities.GetShortDateValue(report["C_x002e_RegulatoryDate"]));
                      mainHtml = mainHtml.Replace("varVersionsDate", Utilities.GetShortDateValue(report["C_x002e_VersionsDate"]));
                      mainHtml = mainHtml.Replace("varCertificationDate", Utilities.GetShortDateValue(report["C_x002e_CertificationDate"]));
                      mainHtml = mainHtml.Replace("varCorrespondenceDate", Utilities.GetShortDateValue(report["C_x002e_CorrespondenceDate"]));
                      mainHtml = mainHtml.Replace("varMiscellaneousDate", Utilities.GetShortDateValue(report["C_x002e_MiscDate"]));

                      //C Comments fields
                      mainHtml = mainHtml.Replace("varSiteVisitLogComments", Utilities.GetReportStringValue(report["C_x002e_SiteVisitLogComments"]));
                      mainHtml = mainHtml.Replace("varProtocolComments", Utilities.GetReportStringValue(report["C_x002e_ProtocolComments"]));
                      mainHtml = mainHtml.Replace("varBrochureComments", Utilities.GetReportStringValue(report["C_x002e_BrochureComments"]));
                      mainHtml = mainHtml.Replace("varDisclosureComments", Utilities.GetReportStringValue(report["C_x002e_DisclosureComments"]));
                      mainHtml = mainHtml.Replace("varLicenseComments", Utilities.GetReportStringValue(report["C_x002e_LicenseComments"]));
                      mainHtml = mainHtml.Replace("varRegulatoryComments", Utilities.GetReportStringValue(report["C_x002e_RegulatoryComments"]));
                      mainHtml = mainHtml.Replace("varVersionsComments", Utilities.GetReportStringValue(report["C_x002e_VersionsComments"]));
                      mainHtml = mainHtml.Replace("varCertificationComments", Utilities.GetReportStringValue(report["C_x002e_CertificationComments"]));
                      mainHtml = mainHtml.Replace("varCorrespondenceComments", Utilities.GetReportStringValue(report["C_x002e_CorrespondenceComments"]));
                      mainHtml = mainHtml.Replace("varMiscellaneousComments", Utilities.GetReportStringValue(report["C_x002e_MiscComments"]));

                      mainHtml = mainHtml.Replace("varProtocol", Utilities.GetReportStringValue(report["Protocol #"]));

                      //D Section
                      mainHtml = mainHtml.Replace("varD1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "D_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varD1", Utilities.GetDDLStringValue(report["D_x002e_1"]));

                      //F Section
                      mainHtml = mainHtml.Replace("varF1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "F_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "F_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF1", Utilities.GetDDLStringValue(report["F_x002e_1"]));
                      mainHtml = mainHtml.Replace("varF2", Utilities.GetDDLStringValue(report["F_x002e_2"]));

                      //G Section
                      mainHtml = mainHtml.Replace("varG1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG5Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG6Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_6_x0020_Comments"));

                      mainHtml = mainHtml.Replace("varG1", Utilities.GetDDLStringValue(report["G_x002e_1"]));
                      mainHtml = mainHtml.Replace("varG2", Utilities.GetDDLStringValue(report["G_x002e_2"]));
                      mainHtml = mainHtml.Replace("varG3", Utilities.GetDDLStringValue(report["G_x002e_3"]));
                      mainHtml = mainHtml.Replace("varG4", Utilities.GetDDLStringValue(report["G_x002e_4"]));
                      mainHtml = mainHtml.Replace("varG5", Utilities.GetDDLStringValue(report["G_x002e_5"]));
                      mainHtml = mainHtml.Replace("varG6", Utilities.GetDDLStringValue(report["G_x002e_6"]));


                      //H Section
                      mainHtml = mainHtml.Replace("varH1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "H_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varH2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "H_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varH3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "H_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varH4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "H_x002e_4_x0020_Comments"));

                      mainHtml = mainHtml.Replace("varH1", Utilities.GetDDLStringValue(report["H_x002e_1"]));
                      mainHtml = mainHtml.Replace("varH2", Utilities.GetDDLStringValue(report["H_x002e_2"]));
                      mainHtml = mainHtml.Replace("varH3", Utilities.GetDDLStringValue(report["H_x002e_3"]));
                      mainHtml = mainHtml.Replace("varH4", Utilities.GetDDLStringValue(report["H_x002e_4"]));

                      //I Section
                      mainHtml = mainHtml.Replace("varI1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_4_x0020_Comments"));

                      mainHtml = mainHtml.Replace("varI1", Utilities.GetDDLStringValue(report["I_x002e_1"]));
                      mainHtml = mainHtml.Replace("varI2", Utilities.GetDDLStringValue(report["I_x002e_2"]));
                      mainHtml = mainHtml.Replace("varI3", Utilities.GetDDLStringValue(report["I_x002e_3"]));
                      mainHtml = mainHtml.Replace("varI4", Utilities.GetDDLStringValue(report["I_x002e_4"]));

                      //Last section
                      mainHtml = mainHtml.Replace("varMiscComments", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e__x0020_Miscellaneous_x00"));
                      //mainHtml = mainHtml.Replace("varNextVisit", Utilities.GetShortDateValue(report["Next_x0020_Visit_x0020_Date"]));

                      #endregion

                      #region Issues

                      //string issueId = string.Empty;
                      //string creationDate = string.Empty;
                      //string closeDate = string.Empty;
                      //string description = string.Empty;
                      //string category = string.Empty;
                      //string action = string.Empty;
                      //string subjectId = string.Empty;

                      //string issuesList = string.Empty;

                      //Write active issues
                      SPListItemCollection activeIssues = Queries.GetActiveIssues(SiteNo);
                      //foreach (SPListItem issue in activeIssues)
                      //{
                      //    issueId = issue.ID.ToString();
                      //    subjectId = Utilities.GetStringValue(issue["SubjectId"]);
                      //    creationDate = (issue["Issue_x0020_Creation_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Creation_x0020_Date"]).ToShortDateString() : string.Empty;
                      //    closeDate = (issue["Issue_x0020_Close_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Close_x0020_Date"]).ToShortDateString() : string.Empty;
                      //    description = Utilities.GetStringValue(issue["Description"]);
                      //    category = Utilities.GetStringValue(issue["Category"]);
                      //    action = Utilities.GetStringValue(issue["Action_x0020_Required"]);

                      //    issuesList += string.Format(Utilities.GetIssueItemHTML(), issueId, GetCellValue(creationDate), GetCellValue(closeDate), GetCellValue(category), GetCellValue(subjectId), GetCellValue(description), GetCellValue(action));
                      //}

                      //mainHtml = mainHtml.Replace("varOpenIssues", issuesList);

                      //issuesList = string.Empty;

                      ////Write closed issues
                      //SPListItemCollection closedIssues = Queries.GetClosedIssues(SiteNo);
                      //foreach (SPListItem issue in closedIssues)
                      //{
                      //    issueId = issue.ID.ToString();
                      //    subjectId = Utilities.GetStringValue(issue["SubjectId"]);
                      //    creationDate = (issue["Issue_x0020_Creation_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Creation_x0020_Date"]).ToShortDateString() : string.Empty;
                      //    closeDate = (issue["Issue_x0020_Close_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Close_x0020_Date"]).ToShortDateString() : string.Empty;
                      //    description = Utilities.GetStringValue(issue["Description"]);
                      //    category = Utilities.GetStringValue(issue["Category"]);
                      //    action = Utilities.GetStringValue(issue["Action_x0020_Required"]);

                      //    issuesList += string.Format(Utilities.GetIssueItemHTML(), issueId, GetCellValue(creationDate), GetCellValue(closeDate), GetCellValue(category), GetCellValue(subjectId), GetCellValue(description), GetCellValue(action));
                      //}

                      //mainHtml = mainHtml.Replace("varClosedIssues", issuesList);

                      #endregion

                      //Write main html file to disk
                      File.WriteAllText(convertedFileNameMain, mainHtml);

                      //signature page section
                      string signatureHtml = File.ReadAllText(signatureTemplateFileName).Replace("\r\n", string.Empty);
                      signatureHtml = signatureHtml.Replace("varVersionNumber", Utilities.GetReportStringValue(report["VersionNumber"]));
                      signatureHtml = signatureHtml.Replace("varVersionDate", Utilities.GetShortDateValue(report["VersionDate"]));

                      //Write signature html file to disk
                      File.WriteAllText(convertedFileNameSignature, signatureHtml);

                      string pdfFile = string.Format("C:\\HFA\\temp\\{0}", pdfFileName);

                      PDFConversion.CreateFinalPDF(convertedFileNameMain, convertedFileNameSignature, pdfFile);

                      SPFolder pdfLibrary = web.Folders[PDFDocumentLibrary];

                      //Upload PDF file to document library
                      if (!Utilities.FileExists(pdfLibrary, pdfFileName))
                      {
                          if (Utilities.UplodFileToDocLibrary(web, PDFDocumentLibrary, pdfFile, SiteNo, "COV", WorkflowName))
                              lblMessage.Text = "PDF Report successfully created";
                      }
                      else
                          lblMessage.Text = "Report with the same name already exists";
                  }
              }
          });
        }

        protected string GetCellValue(string field)
        {
            return field.Length > 0 ? field : "&nbsp;";
        }

        protected SPListItem GetCOVReportData(int reportId)
        {
            if (reportId == 0) return null;

            Guid webID = SPContext.Current.Web.ID; 
            Guid siteID = SPContext.Current.Site.ID;

             SPListItemCollection listItems = null;

             SPSecurity.RunWithElevatedPrivileges(delegate()
              {
                  using (SPSite site = new SPSite(siteID))
                  {
                      using (SPWeb web = site.AllWebs[webID])
                      {
                          SPList siteList = web.Lists["COV Report"];
                          SPQuery oQuery = new SPQuery();

                          string query = "<Where>";
                          query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
                          query += "</Where>";
                          oQuery.Query = query;

                          //If is match is foud then delete list item
                          listItems = siteList.GetItems(oQuery);
                      }
                  }
              });

             if (listItems.Count > 0)
                 return listItems[0];
            else
                return null;
        }

        protected SPListItem GetCOVReportData(SPWeb web, int reportId)
        {
            if (reportId == 0) return null;

            //Guid webID = SPContext.Current.Web.ID;
            //Guid siteID = SPContext.Current.Site.ID;

            SPListItemCollection listItems = null;

            //SPSecurity.RunWithElevatedPrivileges(delegate()
            //{
            //    using (SPSite site = new SPSite(siteID))
            //    {
            //        using (SPWeb web = site.AllWebs[webID])
            //        {
            SPList siteList = web.Lists["COV Report"];
            SPQuery oQuery = new SPQuery();

            string query = "<Where>";
            query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //If is match is foud then delete list item
            listItems = siteList.GetItems(oQuery);
            //        }
            //    }
            //});

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
        }

        protected void btnPDFGenerator_Click(object sender, EventArgs e)
        {
            if (ReportId.Equals("0"))
            {
                lblMessage.Text = "Report does not exists. Please save a report first";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            PDFGenerator();
        }

        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            if (speMonitor.ResolvedEntities.Count <= 0)
                return;

            SPListItem item = null;
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPList siteList = null;

            SPWeb oWeb = SPContext.Current.Web;
            SPUserToken userToken = oWeb.AllUsers[@UserToImpersonate].UserToken;

            using (SPSite site = new SPSite(siteID, userToken))
            {
                using (SPWeb web = site.AllWebs[webID])
                {
                    if (ReportId.Equals("0"))
                    {
                        if (Queries.ReportExists("COV Report", txtTitle.Text))
                        {
                            lblMessage.Text = "Report already exists with the same name. Please enter a different name and try again.";
                            return;
                        }

                        siteList = web.Lists["COV Report"];
                        web.AllowUnsafeUpdates = true;
                        item = siteList.Items.Add();
                        item["Title"] = txtTitle.Text;
                        item.Update();
                        web.AllowUnsafeUpdates = false;

                        //Get new ID
                        newReportId = item.ID;
                        item = GetCOVReportData(web, newReportId);
                    }
                    else
                        item = GetCOVReportData(web, int.Parse(ReportId));

                    item.Web.AllowUnsafeUpdates = true;
                    web.AllowUnsafeUpdates = true;

                    //Get Site info. by siteNo and create site lookup value field
                    SPListItem siteItem = Queries.GetSiteBySiteNo(int.Parse(ddlSites.SelectedItem.Text));
                    SiteInfo = new SiteList(int.Parse(ddlSites.SelectedItem.Text));

                    if (SiteInfo.Exists)
                        btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

                    SPFieldLookupValue siteLookupValue = new SPFieldLookupValue(siteItem.ID, siteItem.Title);
                    item["Site_x0020_Number"] = siteLookupValue.ToString();

                    //Header
                    item["Title"] = txtTitle.Text;
                    item["Sponsor"] = txtSponsor.Text;
                    item["Protocol_x0020__x0023_"] = txtProtocol.Text;

                    if (!calVisitDate.IsDateEmpty)
                        item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    else
                        item["Visit_x0020_Date"] = null;

                    item["Status"] = item.Fields["Status"].GetFieldValue(ddlStatus.SelectedItem.Text);
                    item["K_x002e__x0020_Miscellaneous_x00"] = txtMiscComments.Text;
                    item["General_x0020_Reviewer_x0020_Com"] = txtGenRevComments.Text;
                    item["Address"] = txtAddress.Text;
                    item["VersionNumber"] = ddlVersion.SelectedValue;
                    item["VersionDate"] = calVersionDate.SelectedDate;

                    //People picker monitor field
                    SPUser user = SPContext.Current.Web.SiteUsers[speMonitor.CommaSeparatedAccounts];
                    if (user != null)
                        item["Monitor"] = user;

                    //Set hyperlink values
                    SPFieldUrlValue sitePageUrl = new SPFieldUrlValue();
                    SPFieldUrlValue reportTitleUrl = new SPFieldUrlValue();

                    //Site Page hyperlink field
                    sitePageUrl.Url = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}", SPContext.Current.Web.Url, SiteNo);
                    sitePageUrl.Description = SiteNo.ToString();
                    item["Site_x0020_Page"] = sitePageUrl;

                    //Report Title Page hyperlink field
                    reportTitleUrl.Url = string.Format("{0}/SitePages/COVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteNo, item.ID);
                    reportTitleUrl.Description = txtTitle.Text;
                    item["Report_x0020_Title"] = reportTitleUrl;

                    //Save the first Visit Attendees section as well
                    item["InvestigatorName"] = txtInvestigatorName.Text;
                    item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    item["MonitorName"] = txtMonitorName.Text;
                    item["MonitorTitle"] = txtMonitorTitle.Text;

                    //Site
                    item["Intro_x002e_Clinical_x0020_Site_"] = ddlOtherSitePersonnel.SelectedValue;
                    item["Intro_x002e_Clinical_x0020_Site_0"] = txtOtherSitePersonnelComments.Text;

                    //Personnel
                    item["Intro_x002e_Other_x0020_Personne"] = ddlOtherPersonnel.SelectedValue;
                    item["Intro_x002e_Other_x0020_Personne0"] = txtPersonnelComments.Text;

                    item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                    //Save changes
                    item.Update();

                    web.AllowUnsafeUpdates = false;
                    item.Web.AllowUnsafeUpdates = false;

                    //If new report, redirect to current page to get updated report id in the querystring
                    if (ReportId.Equals("0"))
                    {
                        //lblMessage.Text = "Report successfully saved";
                        ShowMessage = true;
                        Response.Redirect(reportTitleUrl.Url);
                    }

                    //Save current tab
                    switch (int.Parse(formMenu.SelectedValue))
                    {
                        case (int)Utilities.COV_TABS.Visit_Attendees:
                            SaveVisitAttendees();
                            break;

                        case (int)Utilities.COV_TABS.Subject_Recruitment:
                            SaveSubjectRecruitment();
                            break;

                        case (int)Utilities.COV_TABS.Site_File:
                            SaveInvestigatorSiteFile();
                            break;

                        case (int)Utilities.COV_TABS.InformedConsent:
                            SaveInformedConsent();
                            break;

                        case (int)Utilities.COV_TABS.Adverse_Events:
                            SaveAdverseEvents();
                            break;

                        case (int)Utilities.COV_TABS.InvestigationalProduct:
                            SaveInvestigationalProduct();
                            break;

                        case (int)Utilities.COV_TABS.TrialMaterial:
                            SaveTrialMaterial();
                            break;

                        case (int)Utilities.COV_TABS.Discussion:
                            SaveInvestigatorDiscussion();
                            break;
                    }

                    //Visit Attendees
                    //item["InvestigatorName"] = txtInvestigatorName.Text;
                    //item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    //item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    //item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    //item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    //item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    //item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    //item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    //item["MonitorName"] = txtMonitorName.Text;
                    //item["MonitorTitle"] = txtMonitorTitle.Text;

                    //Site
                    //item["Intro_x002e_Clinical_x0020_Site_"] = ddlOtherSitePersonnel.SelectedValue;
                    //item["Intro_x002e_Clinical_x0020_Site_0"] = txtOtherSitePersonnelComments.Text;

                    //Personnel
                    //item["Intro_x002e_Other_x0020_Personne"] = ddlOtherPersonnel.SelectedValue;
                    //item["Intro_x002e_Other_x0020_Personne0"] = txtPersonnelComments.Text;

                    //item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                   
                    //Subject Recruitment
                    //item["NumberScreened"] = txtNoSceened.Text;
                    //item["NumberScreenedFailed"] = txtNoScreenedFailed.Text;
                    //item["NumberRandomized"] = txtNoRandomized.Text;
                    //item["NumberActiveTreatment"] = txtNoActiveTreatment.Text;
                    //item["NumberCompletedTreatment"] = txtNoCompletedTreatment.Text;
                    //item["NumberDiscontinuation"] = txtNoDiscontinuationo.Text;

                    //C section
                    //item["C_x002e_1"] = ddlC1.SelectedValue;
                    //item["C_x002e_2"] = ddlC2.SelectedValue;
                    //item["C_x002e_3"] = ddlC3.SelectedValue;
                    //item["C_x002e_4"] = ddlC4.SelectedValue;
                    //item["C_x002e_5"] = ddlC5.SelectedValue;

                    //item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
                    //item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
                    //item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;
                    //item["C_x002e_4_x0020_Comments"] = txtC4Comments.Text;
                    //item["C_x002e_5_x0020_Comments"] = txtC5Comments.Text;

                    //item["C.Reviewer"] = txtCReviewerComments.Text;

                    ////Site Visit Log Date
                    //if (!calSiteVisitLogDate.IsDateEmpty)
                    //    item["C_x002e_SiteVisitLogDate"] = calSiteVisitLogDate.SelectedDate; 
                    //else
                    //    item["C_x002e_SiteVisitLogDate"] = null;

                    ////Protocol Date
                    //if (!calProtocolDate.IsDateEmpty)
                    //    item["C_x002e_ProtocolDate"] = calProtocolDate.SelectedDate; 
                    //else
                    //    item["C_x002e_ProtocolDate"] = null;

                    ////Brochure Date
                    //if (!calBrochureDate.IsDateEmpty)
                    //    item["C_x002e_BrochureDate"] = calBrochureDate.SelectedDate;
                    //else
                    //    item["C_x002e_BrochureDate"] = null;

                    ////Disclosure Date
                    //if (!calDisclosureDate.IsDateEmpty)
                    //    item["C_x002e_DisclosureDate"] = calDisclosureDate.SelectedDate;
                    //else
                    //    item["C_x002e_DisclosureDate"] = null;

                    ////License Date
                    //if (!calLicenseDate.IsDateEmpty)
                    //    item["C_x002e_LicenseDate"] = calLicenseDate.SelectedDate;
                    //else
                    //    item["C_x002e_LicenseDate"] = null;

                    ////Regulatory Date
                    //if (!calRegulatoryDate.IsDateEmpty)
                    //    item["C_x002e_RegulatoryDate"] = calRegulatoryDate.SelectedDate;
                    //else
                    //    item["C_x002e_RegulatoryDate"] = null;

                    ////Versions Date
                    //if (!calVersionsDate.IsDateEmpty)
                    //    item["C_x002e_VersionsDate"] = calVersionsDate.SelectedDate;
                    //else
                    //    item["C_x002e_VersionsDate"] = null;

                    ////Certification Date
                    //if (!calCertificationDate.IsDateEmpty)
                    //    item["C_x002e_CertificationDate"] = calCertificationDate.SelectedDate;
                    //else
                    //    item["C_x002e_CertificationDate"] = null;

                    ////Correspondence date
                    //if (!calCorrespondenceDate.IsDateEmpty)
                    //    item["C_x002e_CorrespondenceDate"] = calCorrespondenceDate.SelectedDate;
                    //else
                    //    item["C_x002e_CorrespondenceDate"] = null;

                    ////Misc Date                  
                    //if (!calMiscellaneousDate.IsDateEmpty)
                    //    item["C_x002e_MiscDate"] = calMiscellaneousDate.SelectedDate;
                    //else
                    //    item["C_x002e_MiscDate"] = null;


                    //item["C_x002e_SiteVisitLogComments"] = txtSiteVisitLogComments.Text;
                    //item["C_x002e_ProtocolComments"] = txtProtocolComments.Text;
                    //item["C_x002e_DisclosureComments"] = txtDisclosureComments.Text;
                    //item["C_x002e_BrochureComments"] = txtBrochureComments.Text;
                    //item["C_x002e_LicenseComments"] = txtLicenseComments.Text;
                    //item["C_x002e_RegulatoryComments"] = txtRegulatoryComments.Text;
                    //item["C_x002e_VersionsComments"] = txtVersionsComments.Text;
                    //item["C_x002e_CertificationComments"] = txtCertificationComments.Text;
                    //item["C_x002e_CorrespondenceComments"] = txtCorrespondenceComments.Text;
                    //item["C_x002e_MiscComments"] = txtMiscellaneousComments.Text;

                    //D section
                    //item["D_x002e_1"] = ddlD1.SelectedValue;
                    //item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
                    //item["D.Reviewer"] = txtDReviewerComments.Text;

                    //F section
                    //item["F_x002e_1"] = ddlF1.SelectedValue;
                    //item["F_x002e_2"] = ddlF2.SelectedValue;

                    //item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
                    //item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;

                    //item["F.Reviewer"] = txtFReviewerComments.Text;

                    //G section
                    //item["G_x002e_1"] = ddlG1.SelectedValue;
                    //item["G_x002e_2"] = ddlG2.SelectedValue;
                    //item["G_x002e_3"] = ddlG3.SelectedValue;
                    //item["G_x002e_4"] = ddlG4.SelectedValue;
                    //item["G_x002e_5"] = ddlG5.SelectedValue;
                    //item["G_x002e_6"] = ddlG6.SelectedValue;

                    //item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
                    //item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
                    //item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
                    //item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;
                    //item["G_x002e_5_x0020_Comments"] = txtG5Comments.Text;
                    //item["G_x002e_6_x0020_Comments"] = txtG6Comments.Text;

                    //item["G.Reviewer"] = txtGReviewerComments.Text;

                    //H section
                    //item["H_x002e_1"] = ddlH1.SelectedValue;
                    //item["H_x002e_2"] = ddlH2.SelectedValue;
                    //item["H_x002e_3"] = ddlH3.SelectedValue;
                    //item["H_x002e_4"] = ddlH4.SelectedValue;

                    //item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
                    //item["H_x002e_2_x0020_Comments"] = txtH2Comments.Text;
                    //item["H_x002e_3_x0020_Comments"] = txtH3Comments.Text;
                    //item["H_x002e_4_x0020_Comments"] = txtH4Comments.Text;

                    //item["H.Reviewer"] = txtHReviewerComments.Text;

                    //I section
                    //item["I_x002e_1"] = ddlI1.SelectedValue;
                    //item["I_x002e_2"] = ddlI2.SelectedValue;
                    //item["I_x002e_3"] = ddlI3.SelectedValue;
                    //item["I_x002e_4"] = ddlI4.SelectedValue;

                    //item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
                    //item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
                    //item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;
                    //item["I_x002e_4_x0020_Comments"] = txtI4Comments.Text;

                    //item["I.Reviewer"] = txtIReviewerComments.Text;

                   
                }
            }

            lblMessage.Text = "Report successfully saved";
        }

        protected void formMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (ReportId.Equals("0"))
            {
                lblMessage.Text = "Please click on the 'Save' button  below to save the report first before adding any other information";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (ddlSites.SelectedValue == "0")
            {
                lblMessage.Text = "Please select a site number";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            //Get index
            int idx = int.Parse(e.Item.Value);
            formMultiView.ActiveViewIndex = idx;

            if (idx == (int)Utilities.COV_TABS.Site_File)
                FillCData();

            switch (PreviousTabIndex)
            {
                case (int)Utilities.COV_TABS.Visit_Attendees:
                    SaveVisitAttendees();
                    break;

                case (int)Utilities.COV_TABS.Subject_Recruitment:
                    SaveSubjectRecruitment();
                    break;

                case (int)Utilities.COV_TABS.Site_File:
                    SaveInvestigatorSiteFile();
                    break;

                case (int)Utilities.COV_TABS.InformedConsent:
                    SaveInformedConsent();
                    break;

                case (int)Utilities.COV_TABS.Adverse_Events:
                    SaveAdverseEvents();
                    break;

                case (int)Utilities.COV_TABS.InvestigationalProduct:
                    SaveInvestigationalProduct();
                    break;

                case (int)Utilities.COV_TABS.TrialMaterial:
                    SaveTrialMaterial();
                    break;

                case (int)Utilities.COV_TABS.Discussion:
                    SaveInvestigatorDiscussion();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void SaveVisitAttendees()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            if (item != null)
            {
                item.Web.AllowUnsafeUpdates = true;

                //Visit Attendees
                item["InvestigatorName"] = txtInvestigatorName.Text;
                item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                item["SitePersonnelName"] = txtSitePersonnelName.Text;
                item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                item["MonitorName"] = txtMonitorName.Text;
                item["MonitorTitle"] = txtMonitorTitle.Text;

                //Site
                item["Intro_x002e_Clinical_x0020_Site_"] = ddlOtherSitePersonnel.SelectedValue;
                item["Intro_x002e_Clinical_x0020_Site_0"] = txtOtherSitePersonnelComments.Text;

                //Personnel
                item["Intro_x002e_Other_x0020_Personne"] = ddlOtherPersonnel.SelectedValue;
                item["Intro_x002e_Other_x0020_Personne0"] = txtPersonnelComments.Text;

                item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                item.Update();
                item.Web.AllowUnsafeUpdates = false;
            }
        }

        protected void SaveSubjectRecruitment()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["NumberScreened"] = txtNoSceened.Text;
            item["NumberScreenedFailed"] = txtNoScreenedFailed.Text;
            item["NumberRandomized"] = txtNoRandomized.Text;
            item["NumberActiveTreatment"] = txtNoActiveTreatment.Text;
            item["NumberCompletedTreatment"] = txtNoCompletedTreatment.Text;
            item["NumberDiscontinuation"] = txtNoDiscontinuationo.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigatorSiteFile()
        {
            string errorMsg = string.Empty;

            if (!calSiteVisitLogDate.IsDateEmpty && !calSiteVisitLogDate.IsValid)
            {
                errorMsg += "Please enter a valid Site Visit Log Date" + "<br/>";
            }

            if (!calProtocolDate.IsDateEmpty && !calProtocolDate.IsValid)
            {
                errorMsg += "Please enter a valid Protocol date" + "<br/>";
            }

            if (!calBrochureDate.IsDateEmpty && !calBrochureDate.IsValid)
            {
                errorMsg += "Please enter a valid Brochure date" + "<br/>";
            }

            if (!calDisclosureDate.IsDateEmpty && !calDisclosureDate.IsValid)
            {
                errorMsg += "Please enter a valid Disclosure date" + "<br/>";
            }

            if (!calLicenseDate.IsDateEmpty && !calLicenseDate.IsValid)
            {
                errorMsg += "Please enter a valid License date" + "<br/>";
            }

            if (!calRegulatoryDate.IsDateEmpty && !calRegulatoryDate.IsValid)
            {
                errorMsg += "Please enter a valid Regulatory date" + "<br/>";
            }

            if (!calVersionDate.IsDateEmpty && !calVersionDate.IsValid)
            {
                errorMsg += "Please enter a valid Version date" + "<br/>";
            }

            if (!calCertificationDate.IsDateEmpty && !calCertificationDate.IsValid)
            {
                errorMsg = "Please enter a valid Certification date" + "<br/>";
            }

            if (!calCorrespondenceDate.IsDateEmpty && !calCorrespondenceDate.IsValid)
            {
                errorMsg += "Please enter a valid Correspondence date" + "<br/>";
            }

            if (!calMiscellaneousDate.IsDateEmpty && !calMiscellaneousDate.IsValid)
            {
                errorMsg += "Please enter a valid Miscellaneous date" + "<br/>";
            }

            if (errorMsg.Length > 0)
            {
                lblMessage.Text = errorMsg;
                return;
            }

            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //C section
            item["C_x002e_1"] = ddlC1.SelectedValue;
            item["C_x002e_2"] = ddlC2.SelectedValue;
            item["C_x002e_3"] = ddlC3.SelectedValue;
            item["C_x002e_4"] = ddlC4.SelectedValue;
            item["C_x002e_5"] = ddlC5.SelectedValue;

            item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
            item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
            item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;
            item["C_x002e_4_x0020_Comments"] = txtC4Comments.Text;
            item["C_x002e_5_x0020_Comments"] = txtC5Comments.Text;

            item["C.Reviewer"] = txtCReviewerComments.Text;


            //Site Visit Log Date
            if (!calSiteVisitLogDate.IsDateEmpty)
                item["C_x002e_SiteVisitLogDate"] = calSiteVisitLogDate.SelectedDate;
            else
                item["C_x002e_SiteVisitLogDate"] = null;

            //Protocol Date
            if (!calProtocolDate.IsDateEmpty)
                item["C_x002e_ProtocolDate"] = calProtocolDate.SelectedDate;
            else
                item["C_x002e_ProtocolDate"] = null;

            //Brochure Date
            if (!calBrochureDate.IsDateEmpty)
                item["C_x002e_BrochureDate"] = calBrochureDate.SelectedDate;
            else
                item["C_x002e_BrochureDate"] = null;

            //Disclosure Date
            if (!calDisclosureDate.IsDateEmpty)
                item["C_x002e_DisclosureDate"] = calDisclosureDate.SelectedDate;
            else
                item["C_x002e_DisclosureDate"] = null;

            //License Date
            if (!calLicenseDate.IsDateEmpty)
                item["C_x002e_LicenseDate"] = calLicenseDate.SelectedDate;
            else
                item["C_x002e_LicenseDate"] = null;

            //Regulatory Date
            if (!calRegulatoryDate.IsDateEmpty)
                item["C_x002e_RegulatoryDate"] = calRegulatoryDate.SelectedDate;
            else
                item["C_x002e_RegulatoryDate"] = null;

            //Versions Date
            if (!calVersionsDate.IsDateEmpty)
                item["C_x002e_VersionsDate"] = calVersionsDate.SelectedDate;
            else
                item["C_x002e_VersionsDate"] = null;

            //Certification Date
            if (!calCertificationDate.IsDateEmpty)
                item["C_x002e_CertificationDate"] = calCertificationDate.SelectedDate;
            else
                item["C_x002e_CertificationDate"] = null;

            //Correspondence date
            if (!calCorrespondenceDate.IsDateEmpty)
                item["C_x002e_CorrespondenceDate"] = calCorrespondenceDate.SelectedDate;
            else
                item["C_x002e_CorrespondenceDate"] = null;

            //Misc Date                  
            if (!calMiscellaneousDate.IsDateEmpty)
                item["C_x002e_MiscDate"] = calMiscellaneousDate.SelectedDate;
            else
                item["C_x002e_MiscDate"] = null;


            item["C_x002e_SiteVisitLogComments"] = txtSiteVisitLogComments.Text;
            item["C_x002e_ProtocolComments"] = txtProtocolComments.Text;
            item["C_x002e_DisclosureComments"] = txtDisclosureComments.Text;
            item["C_x002e_BrochureComments"] = txtBrochureComments.Text;
            item["C_x002e_LicenseComments"] = txtLicenseComments.Text;
            item["C_x002e_RegulatoryComments"] = txtRegulatoryComments.Text;
            item["C_x002e_VersionsComments"] = txtVersionsComments.Text;
            item["C_x002e_CertificationComments"] = txtCertificationComments.Text;
            item["C_x002e_CorrespondenceComments"] = txtCorrespondenceComments.Text;
            item["C_x002e_MiscComments"] = txtMiscellaneousComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInformedConsent()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["D_x002e_1"] = ddlD1.SelectedValue;
            item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
            item["D.Reviewer"] = txtDReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveAdverseEvents()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //F section
            item["F_x002e_1"] = ddlF1.SelectedValue;
            item["F_x002e_2"] = ddlF2.SelectedValue;

            item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
            item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;

            item["F.Reviewer"] = txtFReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveGeneralInfo()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item.Web.AllowUnsafeUpdates = true;
            item["C_x002e_2"] = ddlC2.SelectedValue;
            item["C_x002e_1"] = ddlC1.SelectedValue;

            item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
            item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;

            item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigationalProduct()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //G section
            item["G_x002e_1"] = ddlG1.SelectedValue;
            item["G_x002e_2"] = ddlG2.SelectedValue;
            item["G_x002e_3"] = ddlG3.SelectedValue;
            item["G_x002e_4"] = ddlG4.SelectedValue;
            item["G_x002e_5"] = ddlG5.SelectedValue;
            item["G_x002e_6"] = ddlG6.SelectedValue;

            item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
            item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
            item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
            item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;
            item["G_x002e_5_x0020_Comments"] = txtG5Comments.Text;
            item["G_x002e_6_x0020_Comments"] = txtG6Comments.Text;

            item["G.Reviewer"] = txtGReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveTrialMaterial()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //H section
            item["H_x002e_1"] = ddlH1.SelectedValue;
            item["H_x002e_2"] = ddlH2.SelectedValue;
            item["H_x002e_3"] = ddlH3.SelectedValue;
            item["H_x002e_4"] = ddlH4.SelectedValue;

            item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
            item["H_x002e_2_x0020_Comments"] = txtH2Comments.Text;
            item["H_x002e_3_x0020_Comments"] = txtH3Comments.Text;
            item["H_x002e_4_x0020_Comments"] = txtH4Comments.Text;

            item["H.Reviewer"] = txtHReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigatorDiscussion()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //I section
            item["I_x002e_1"] = ddlI1.SelectedValue;
            item["I_x002e_2"] = ddlI2.SelectedValue;
            item["I_x002e_3"] = ddlI3.SelectedValue;
            item["I_x002e_4"] = ddlI4.SelectedValue;

            item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
            item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
            item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;
            item["I_x002e_4_x0020_Comments"] = txtI4Comments.Text;

            item["I.Reviewer"] = txtIReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void FillSubjectRecruitment()
        {
            //C Section
            if (ReportId.Equals("0")) return;

            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            if (item != null)
            {
                txtCReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C.Reviewer");

                //Number fields
                txtNoSceened.Text = Utilities.GetStringValue(item["NumberScreened"]);
                txtNoScreenedFailed.Text = Utilities.GetStringValue(item["NumberScreenedFailed"]);
                txtNoRandomized.Text = Utilities.GetStringValue(item["NumberRandomized"]);
                txtNoActiveTreatment.Text = Utilities.GetStringValue(item["NumberActiveTreatment"]);
                txtNoCompletedTreatment.Text = Utilities.GetStringValue(item["NumberCompletedTreatment"]);
                txtNoDiscontinuationo.Text = Utilities.GetStringValue(item["NumberDiscontinuation"]);
            }
        }

        protected void FillCData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlC1.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_1"]);
            ddlC2.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_2"]);
            ddlC3.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_3"]);
            ddlC4.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_4"]);
            ddlC5.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_5"]);

            txtC1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_1_x0020_Comments");
            txtC2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_2_x0020_Comments");
            txtC3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_3_x0020_Comments");
            txtC4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_4_x0020_Comments");
            txtC5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_5_x0020_Comments");


            txtCReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C.Reviewer");

            if (item["C_x002e_SiteVisitLogDate"] != null)
                calSiteVisitLogDate.SelectedDate = Convert.ToDateTime(item["C_x002e_SiteVisitLogDate"].ToString());

            txtSiteVisitLogComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_SiteVisitLogComments");

            if (item["C_x002e_ProtocolDate"] != null)
                calProtocolDate.SelectedDate = Convert.ToDateTime(item["C_x002e_ProtocolDate"].ToString());

            txtProtocolComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_ProtocolComments");

            if (item["C_x002e_BrochureDate"] != null)
                calBrochureDate.SelectedDate = Convert.ToDateTime(item["C_x002e_BrochureDate"].ToString());

            txtBrochureComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_BrochureComments");

            if (item["C_x002e_DisclosureDate"] != null)
                calDisclosureDate.SelectedDate = Convert.ToDateTime(item["C_x002e_DisclosureDate"].ToString());

            txtDisclosureComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_DisclosureComments");

            if (item["C_x002e_LicenseDate"] != null)
                calLicenseDate.SelectedDate = Convert.ToDateTime(item["C_x002e_LicenseDate"].ToString());

            txtLicenseComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_LicenseComments");

            if (item["C_x002e_RegulatoryDate"] != null)
                calRegulatoryDate.SelectedDate = Convert.ToDateTime(item["C_x002e_RegulatoryDate"].ToString());

            txtRegulatoryComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_RegulatoryComments");

            if (item["C_x002e_VersionsDate"] != null)
                calVersionsDate.SelectedDate = Convert.ToDateTime(item["C_x002e_VersionsDate"].ToString());

            txtVersionsComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_VersionsComments");

            if (item["C_x002e_CertificationDate"] != null)
                calCertificationDate.SelectedDate = Convert.ToDateTime(item["C_x002e_CertificationDate"].ToString());

            txtCertificationComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_CertificationComments");

            if (item["C_x002e_CorrespondenceDate"] != null)
                calCorrespondenceDate.SelectedDate = Convert.ToDateTime(item["C_x002e_CorrespondenceDate"].ToString());

            txtCorrespondenceComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_CorrespondenceComments");

            if (item["C_x002e_MiscDate"] != null)
                calMiscellaneousDate.SelectedDate = Convert.ToDateTime(item["C_x002e_MiscDate"].ToString());

            txtMiscellaneousComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_MiscComments");
        }

        protected void FillDData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlD1.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_1"]);

            txtD1Comments.Text = Utilities.GetStringValue(item["D_x002e_1_x0020_Comments"]);

            txtDReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "D.Reviewer");
        }

        protected void FillInvestAtt()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetCOVReportData(int.Parse(ReportId));

             //Utilities.GetStringValue(item["Investigator"]);
            txtInvestigatorTitle.Text = Utilities.GetStringValue(item["InvestigatorTitle"]);

            txtSitePersonnelName.Text = Utilities.GetStringValue(item["SitePersonnelName"]);
            txtSitePersonnelTitle.Text = Utilities.GetStringValue(item["SitePersonnelTitle"]);
            txtSitePersonnelName2.Text = Utilities.GetStringValue(item["SitePersonnelName2"]);
            txtSitePersonnelTitle2.Text = Utilities.GetStringValue(item["SitePersonnelTitle2"]);
            txtSitePersonnelName3.Text = Utilities.GetStringValue(item["SitePersonnelName3"]);
            txtSitePersonnelTitle3.Text = Utilities.GetStringValue(item["SitePersonnelTitle3"]);

            txtMonitorName.Text = Utilities.GetStringValue(item["MonitorName"]);
            txtMonitorTitle.Text = Utilities.GetStringValue(item["MonitorTitle"]);

            ddlOtherSitePersonnel.SelectedValue = Utilities.GetStringValue(item["Intro_x002e_Clinical_x0020_Site_"]);
            txtOtherSitePersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Clinical_x0020_Site_0"]);

            ddlOtherPersonnel.SelectedValue = Utilities.GetStringValue(item["Intro_x002e_Other_x0020_Personne"]);
            txtPersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Other_x0020_Personne0"]);

            txtVAReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Visit_x002e_Reviewer");
        }

        protected void FillFData()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlF1.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_1"]);
            ddlF2.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_2"]);

            txtF1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_1_x0020_Comments");
            txtF2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_2_x0020_Comments");

            txtFReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "F.Reviewer");
        }

        protected void FillGData()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlG1.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_1"]);
            ddlG2.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_2"]);
            ddlG3.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_3"]);
            ddlG4.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_4"]);
            ddlG5.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_5"]);
            ddlG6.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_6"]);

            txtG1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_1_x0020_Comments");
            txtG2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_2_x0020_Comments");
            txtG3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_3_x0020_Comments");
            txtG4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_4_x0020_Comments");
            txtG5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_5_x0020_Comments");
            txtG6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_6_x0020_Comments");

            txtGReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "G.Reviewer");
        }

        protected void FillHData()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlH1.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_1"]);
            ddlH2.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_2"]);
            ddlH3.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_3"]);
            ddlH4.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_4"]);

            txtH1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_1_x0020_Comments");
            txtH2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_2_x0020_Comments");
            txtH3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_3_x0020_Comments");
            txtH4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_4_x0020_Comments");

            txtHReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "H.Reviewer");
        }

        protected void FillIData()
        {
            SPListItem item = GetCOVReportData(int.Parse(ReportId));

            ddlI1.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_1"]);
            ddlI2.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_2"]);
            ddlI3.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_3"]);
            ddlI4.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_4"]);

            txtI1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_1_x0020_Comments");
            txtI2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_2_x0020_Comments");
            txtI3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_3_x0020_Comments");
            txtI4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_4_x0020_Comments");

            txtIReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "I.Reviewer");
        }

        protected void btnSiteOverview_Click(object sender, EventArgs e)
        {
            //Save all changes
            btnSaveAll_Click(null, EventArgs.Empty);

            //Redirect page
            Response.Redirect(btnSiteOverview.CommandArgument);
            //hplSiteOverView.NavigateUrl = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

        }
    }
}
