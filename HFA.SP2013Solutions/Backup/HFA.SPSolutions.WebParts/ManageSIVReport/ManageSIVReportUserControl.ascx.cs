using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using Microsoft.SharePoint;
using System.IO;
using ExpertPdf.HtmlToPdf;
using System.Drawing;
using System.Data;
using System.Configuration;
using DanFay.SPHelper;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using HFA.SPSolutions.WebParts.Libs;

namespace HFA.SPSolutions.WebParts.ManageSIVReport
{
    public partial class ManageSIVReportUserControl : UserControl
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

                if (ReportId.Equals("0")) { FillSiteData(); return; }

                FillHeaderData();

                FillInvestAtt();
                FillCData();
                FillDData();
                FillEData();
                FillFData();
                FillGData();
                FillHData();
                FillIData();
                FillJData();
                FillKData();
                FillLData();
                FillMData();
                FillNData();
                FillOData();
                FillPData();
                FillQData();
                FillRData();
                FillSData();
                FillTData();

                if (ShowMessage)
                {
                    lblMessage.Text = "Report successfully saved";
                    ShowMessage = false;
                }
            }
        }

        protected void InitializeForm()
        {
            SiteInfo = new SiteList(SiteNo);
            //hplSiteOverView.NavigateUrl = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID); 
            
            //Bind Sites
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
                txtTitle.Text = string.Format("{0}_SIV_Report_{1}", SiteNo, DateTime.Now.ToString("ddMMMyyyy").ToUpper());
                btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            }
        }

        protected void FillHeaderData()
        {
            SPListItem sivReport = GetSIVReportData(int.Parse(ReportId));

            if (sivReport != null)
            {
                txtTitle.Text = sivReport.Title;
                txtInvestigatorName.Text = Utilities.GetStringValue(sivReport["InvestigatorName"]);
                txtSponsor.Text = Utilities.GetStringValue(sivReport["Sponsor"]);
                txtProtocol.Text = Utilities.GetStringValue(sivReport["Protocol_x0020__x0023_"]);

                if (sivReport["Visit_x0020_Date"] != null)
                    calVisitDate.SelectedDate = Convert.ToDateTime(sivReport["Visit_x0020_Date"].ToString());

                if (sivReport["Next_x0020_Visit_x0020_Date"] != null)
                    calNextVisitDate.SelectedDate = Convert.ToDateTime(sivReport["Next_x0020_Visit_x0020_Date"].ToString());

                txtAddress.Text = Utilities.GetMultiLineTextFieldValue(sivReport, "Address");
                ddlStatus.SelectedValue = ddlStatus.Items.FindByText(sivReport["Status"].ToString()).Value;

                if (sivReport["Monitor"] != null)
                {
                    try
                    {
                        SPFieldLookupValue monitorLookupValue = new SPFieldLookupValue(sivReport["Monitor"].ToString());
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

                ddlVersion.SelectedValue = Utilities.GetStringValue(sivReport["VersionNumber"]);
                if (sivReport["VersionDate"] != null)
                    calVersionDate.SelectedDate = Convert.ToDateTime(Utilities.GetShortDateValue(sivReport["VersionDate"]));

                txtMiscComments.Text = Utilities.GetMultiLineTextFieldValue(sivReport, "U_x002e__x0020_Miscellaneous_x00");
                txtGenRevComments.Text = Utilities.GetMultiLineTextFieldValue(sivReport, "General_x0020_Reviewer_x0020_Com");

                //Set dropdown to current site
                if (sivReport["Site_x0020_Number"] != null)
                {
                    SPFieldLookupValue siteLookup = new SPFieldLookupValue(sivReport["Site_x0020_Number"].ToString());
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
            string pdfFileName = string.Empty;

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
          {
              using (SPSite site = new SPSite(siteID))
              {
                  using (SPWeb web = site.AllWebs[webID])
                  {

                      string mainTemplateFileName = "C:\\HFA\\SIVReportTemplate-Main.html";
                      string signatureTemplateFileName = "C:\\HFA\\SIVReportTemplate-Signature.html";

                      string convertedFileNameMain = "C:\\HFA\\temp\\SIVReportConverted-Main.html";
                      string convertedFileNameSignature = "C:\\HFA\\temp\\SIVReportConverted-Signature.html";

                      SPListItem SIVReport = GetSIVReportData(int.Parse(ReportId));
                      pdfFileName = string.Format("{0}.pdf", SIVReport.Title);

                      if (SIVReport == null)
                          return;

                      // Read in the contents of the Receipt.htm HTML template file
                      string mainHtml = File.ReadAllText(mainTemplateFileName).Replace("\r\n", string.Empty);

                      string field = string.Empty;

                      //Replace variable tags
                      mainHtml = mainHtml.Replace("varSponsor", Utilities.GetStringValue(SIVReport["Sponsor"]));
                      mainHtml = mainHtml.Replace("varProtocol", Utilities.GetStringValue(SIVReport["Protocol #"]));
                      mainHtml = mainHtml.Replace("varSiteNo", ReportId.ToString());
                      mainHtml = mainHtml.Replace("varStudySiteNumber", Utilities.GetLookupFieldValue(SIVReport["Site_x0020_Number"]));
                      mainHtml = mainHtml.Replace("varVisitDate", Utilities.GetShortDateValue(SIVReport["Visit_x0020_Date"]));
                      mainHtml = mainHtml.Replace("varInvestigatorName", Utilities.GetReportStringValue(SIVReport["InvestigatorName"]));
                      mainHtml = mainHtml.Replace("varAddress", Utilities.GetReportStringValue(SIVReport["Address"]));

                      //Visit Attendees
                      mainHtml = mainHtml.Replace("varInvestigatorTitle", Utilities.GetReportStringValue(SIVReport["InvestigatorTitle"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName1", Utilities.GetReportStringValue(SIVReport["SitePersonnelName"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle1", Utilities.GetReportStringValue(SIVReport["SitePersonnelTitle"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle2", Utilities.GetReportStringValue(SIVReport["SitePersonnelTitle2"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName2", Utilities.GetReportStringValue(SIVReport["SitePersonnelName2"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelTitle3", Utilities.GetReportStringValue(SIVReport["SitePersonnelTitle3"]));
                      mainHtml = mainHtml.Replace("varSitePersonnelName3", Utilities.GetReportStringValue(SIVReport["SitePersonnelName3"]));

                      mainHtml = mainHtml.Replace("varOtherSitePersonnelComments", Utilities.GetReportStringValue(SIVReport["Intro_x002e_Clinical_x0020_Site_"]));
                      mainHtml = mainHtml.Replace("varPersonnelComments", Utilities.GetReportStringValue(SIVReport["Intro_x002e_Other_x0020_Personne0"]));

                      mainHtml = mainHtml.Replace("varMonitorName", Utilities.GetReportStringValue(SIVReport["MonitorName"]));
                      mainHtml = mainHtml.Replace("varMonitorTitle", Utilities.GetReportStringValue(SIVReport["MonitorTitle"]));

                      mainHtml = mainHtml.Replace("varOtherSitePersonnel", Utilities.GetReportStringValue(SIVReport["Intro.Clinical Site Personnel"]));
                      mainHtml = mainHtml.Replace("varOtherPersonnel", Utilities.GetReportStringValue(SIVReport["Intro.Other Personnel"]));

                      //C1 Section
                      mainHtml = mainHtml.Replace("varC1Comments", Utilities.GetReportStringValue(SIVReport["C_x002e_1_x0020_Comments"]));
                      mainHtml = mainHtml.Replace("varC2Comments", Utilities.GetReportStringValue(SIVReport["C_x002e_2_x0020_Comments"]));
                      mainHtml = mainHtml.Replace("varC1", Utilities.GetReportStringValue(SIVReport["C_x002e_1"]));
                      mainHtml = mainHtml.Replace("varC2", Utilities.GetReportStringValue(SIVReport["C_x002e_2"]));

                      //D1 Section
                      mainHtml = mainHtml.Replace("varD1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "D_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varD1", Utilities.GetReportStringValue(SIVReport["D_x002e_1"]));
                      mainHtml = mainHtml.Replace("varD2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "D_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varD2", Utilities.GetReportStringValue(SIVReport["D_x002e_2"]));
                      mainHtml = mainHtml.Replace("varD3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "D_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varD3", Utilities.GetReportStringValue(SIVReport["D_x002e_3"]));

                      //E Section
                      mainHtml = mainHtml.Replace("varE1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE1", Utilities.GetReportStringValue(SIVReport["E_x002e_1"]));
                      mainHtml = mainHtml.Replace("varE2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE2", Utilities.GetReportStringValue(SIVReport["E_x002e_2"]));
                      mainHtml = mainHtml.Replace("varE3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE3", Utilities.GetReportStringValue(SIVReport["E_x002e_3"]));
                      mainHtml = mainHtml.Replace("varE4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE4", Utilities.GetReportStringValue(SIVReport["E_x002e_4"]));
                      mainHtml = mainHtml.Replace("varE5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE5", Utilities.GetReportStringValue(SIVReport["E_x002e_5"]));
                      mainHtml = mainHtml.Replace("varE6Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_6_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE6", Utilities.GetReportStringValue(SIVReport["E_x002e_6"]));
                      mainHtml = mainHtml.Replace("varE7Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "E_x002e_7_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varE7", Utilities.GetReportStringValue(SIVReport["E_x002e_7"]));

                      //F Section
                      mainHtml = mainHtml.Replace("varF1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "F_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF1", Utilities.GetReportStringValue(SIVReport["F_x002e_1"]));
                      mainHtml = mainHtml.Replace("varF2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "F_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF2", Utilities.GetReportStringValue(SIVReport["F_x002e_2"]));
                      mainHtml = mainHtml.Replace("varF3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "F_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF3", Utilities.GetReportStringValue(SIVReport["F_x002e_3"]));
                      mainHtml = mainHtml.Replace("varF4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "F_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF4", Utilities.GetReportStringValue(SIVReport["F_x002e_4"]));
                      mainHtml = mainHtml.Replace("varF5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "F_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varF5", Utilities.GetReportStringValue(SIVReport["F_x002e_5"]));

                      //G Section
                      mainHtml = mainHtml.Replace("varG1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "G_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG1", Utilities.GetReportStringValue(SIVReport["G_x002e_1"]));
                      mainHtml = mainHtml.Replace("varG2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "G_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG2", Utilities.GetReportStringValue(SIVReport["G_x002e_2"]));
                      mainHtml = mainHtml.Replace("varG3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "G_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG3", Utilities.GetReportStringValue(SIVReport["G_x002e_3"]));
                      mainHtml = mainHtml.Replace("varG4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "G_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varG4", Utilities.GetReportStringValue(SIVReport["G_x002e_4"]));

                      //H Section
                      mainHtml = mainHtml.Replace("varH1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "H_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varH1", Utilities.GetReportStringValue(SIVReport["H_x002e_1"]));

                      //I Section
                      mainHtml = mainHtml.Replace("varI1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "I_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI1", Utilities.GetReportStringValue(SIVReport["I_x002e_1"]));
                      mainHtml = mainHtml.Replace("varI2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "I_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI2", Utilities.GetReportStringValue(SIVReport["I_x002e_2"]));
                      mainHtml = mainHtml.Replace("varI3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "I_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI3", Utilities.GetReportStringValue(SIVReport["I_x002e_3"]));
                      mainHtml = mainHtml.Replace("varI4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "I_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varI4", Utilities.GetReportStringValue(SIVReport["I_x002e_4"]));

                      //J Section
                      mainHtml = mainHtml.Replace("varJ1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "J_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varJ1", Utilities.GetReportStringValue(SIVReport["J_x002e_1"]));
                      mainHtml = mainHtml.Replace("varJ2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "J_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varJ2", Utilities.GetReportStringValue(SIVReport["J_x002e_2"]));
                      mainHtml = mainHtml.Replace("varJ3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "J_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varJ3", Utilities.GetReportStringValue(SIVReport["J_x002e_3"]));
                      mainHtml = mainHtml.Replace("varJ4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "J_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varJ4", Utilities.GetReportStringValue(SIVReport["J_x002e_4"]));

                      //K Section
                      mainHtml = mainHtml.Replace("varK1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK1", Utilities.GetReportStringValue(SIVReport["K_x002e_1"]));
                      mainHtml = mainHtml.Replace("varK2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK2", Utilities.GetReportStringValue(SIVReport["K_x002e_2"]));
                      mainHtml = mainHtml.Replace("varK3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK3", Utilities.GetReportStringValue(SIVReport["K_x002e_3"]));
                      mainHtml = mainHtml.Replace("varK4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK4", Utilities.GetReportStringValue(SIVReport["K_x002e_4"]));
                      mainHtml = mainHtml.Replace("varK5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK5", Utilities.GetReportStringValue(SIVReport["K_x002e_5"]));
                      mainHtml = mainHtml.Replace("varK6Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_6_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK6", Utilities.GetReportStringValue(SIVReport["K_x002e_6"]));
                      mainHtml = mainHtml.Replace("varK7Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "K_x002e_7_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varK7", Utilities.GetReportStringValue(SIVReport["K_x002e_7"]));

                      //L Section
                      mainHtml = mainHtml.Replace("varL1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "L_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varL1", Utilities.GetReportStringValue(SIVReport["L_x002e_1"]));
                      mainHtml = mainHtml.Replace("varL2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "L_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varL2", Utilities.GetReportStringValue(SIVReport["L_x002e_2"]));
                      mainHtml = mainHtml.Replace("varL3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "L_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varL3", Utilities.GetReportStringValue(SIVReport["L_x002e_3"]));

                      //M Section
                      mainHtml = mainHtml.Replace("varM1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "M_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varM1", Utilities.GetReportStringValue(SIVReport["M_x002e_1"]));

                      //N Section
                      mainHtml = mainHtml.Replace("varN1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN1", Utilities.GetReportStringValue(SIVReport["N_x002e_1"]));
                      mainHtml = mainHtml.Replace("varN2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN2", Utilities.GetReportStringValue(SIVReport["N_x002e_2"]));
                      mainHtml = mainHtml.Replace("varN3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN3", Utilities.GetReportStringValue(SIVReport["N_x002e_3"]));
                      mainHtml = mainHtml.Replace("varN4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN4", Utilities.GetReportStringValue(SIVReport["N_x002e_4"]));
                      mainHtml = mainHtml.Replace("varN5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN5", Utilities.GetReportStringValue(SIVReport["N_x002e_5"]));
                      mainHtml = mainHtml.Replace("varN6Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_6_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN6", Utilities.GetReportStringValue(SIVReport["N_x002e_6"]));
                      mainHtml = mainHtml.Replace("varN7Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_7_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN7", Utilities.GetReportStringValue(SIVReport["N_x002e_7"]));
                      mainHtml = mainHtml.Replace("varN8Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_8_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN8", Utilities.GetReportStringValue(SIVReport["N_x002e_8"]));
                      mainHtml = mainHtml.Replace("varN9Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "N_x002e_9_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varN9", Utilities.GetReportStringValue(SIVReport["N_x002e_9"]));

                      //O Section
                      mainHtml = mainHtml.Replace("varO1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "O_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varO1", Utilities.GetReportStringValue(SIVReport["O_x002e_1"]));
                      mainHtml = mainHtml.Replace("varO2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "O_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varO2", Utilities.GetReportStringValue(SIVReport["O_x002e_2"]));

                      //P Section
                      mainHtml = mainHtml.Replace("varP1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "P_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varP1", Utilities.GetReportStringValue(SIVReport["P_x002e_1"]));
                      mainHtml = mainHtml.Replace("varP2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "P_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varP2", Utilities.GetReportStringValue(SIVReport["P_x002e_2"]));
                      mainHtml = mainHtml.Replace("varP3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "P_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varP3", Utilities.GetReportStringValue(SIVReport["P_x002e_3"]));
                      mainHtml = mainHtml.Replace("varP4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "P_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varP4", Utilities.GetReportStringValue(SIVReport["P_x002e_4"]));
                      mainHtml = mainHtml.Replace("varP5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "P_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varP5", Utilities.GetReportStringValue(SIVReport["P_x002e_5"]));

                      string emptyField = "&nbsp;";
                      //Q Section
                      mainHtml = mainHtml.Replace("varQ1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ1", Utilities.GetReportStringValue(SIVReport["Q_x002e_1"]));
                      mainHtml = mainHtml.Replace("varQ2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ2", Utilities.GetReportStringValue(SIVReport["Q_x002e_2"]));
                      mainHtml = mainHtml.Replace("varQ3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ3", Utilities.GetReportStringValue(SIVReport["Q_x002e_3"]));
                      mainHtml = mainHtml.Replace("varQ4Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_4_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ4", Utilities.GetReportStringValue(SIVReport["Q_x002e_4"]));
                      mainHtml = mainHtml.Replace("varQ5Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_5_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ5", Utilities.GetReportStringValue(SIVReport["Q_x002e_5"]));
                      mainHtml = mainHtml.Replace("varQ6Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_6_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ6", Utilities.GetReportStringValue(SIVReport["Q_x002e_6"]));
                      mainHtml = mainHtml.Replace("varQ7Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_7_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ7", Utilities.GetReportStringValue(SIVReport["Q_x002e_7"]));
                      mainHtml = mainHtml.Replace("varQ8Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "Q_x002e_8_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varQ8", Utilities.GetReportStringValue(SIVReport["Q_x002e_8"]));

                      //R SectionF
                      mainHtml = mainHtml.Replace("varR1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "R_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varR1", Utilities.GetStringValue(SIVReport["R_x002e_1"]));
                      mainHtml = mainHtml.Replace("varR2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "R_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varR2", Utilities.GetStringValue(SIVReport["R_x002e_2"]));
                      mainHtml = mainHtml.Replace("varR3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "R_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varR3", Utilities.GetStringValue(SIVReport["R_x002e_3"]));

                      //S Section
                      mainHtml = mainHtml.Replace("varS1Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "S_x002e_1_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varS1", Utilities.GetStringValue(SIVReport["S_x002e_1"]));
                      mainHtml = mainHtml.Replace("varS2Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "S_x002e_2_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varS2", Utilities.GetStringValue(SIVReport["S_x002e_2"]));
                      mainHtml = mainHtml.Replace("varS3Comments", Utilities.GetMultiLineReportTextFieldValue(SIVReport, "S_x002e_3_x0020_Comments"));
                      mainHtml = mainHtml.Replace("varS3", Utilities.GetStringValue(SIVReport["S_x002e_3"]));

                      //T Section
                      mainHtml = mainHtml.Replace("varT1Comments", Utilities.GetReportStringValue(SIVReport["T_x002e_1_x0020_Comments"]));
                      mainHtml = mainHtml.Replace("varT1", Utilities.GetStringValue(SIVReport["T_x002e_1"]));
                      mainHtml = mainHtml.Replace("varT2Comments", Utilities.GetReportStringValue(SIVReport["T_x002e_2_x0020_Comments"]));
                      mainHtml = mainHtml.Replace("varT2", Utilities.GetStringValue(SIVReport["T_x002e_2"]));

                      //Last section
                      mainHtml = mainHtml.Replace("varNextVisit", Utilities.GetShortDateValue(SIVReport["Next_x0020_Visit_x0020_Date"]));
                      mainHtml = mainHtml.Replace("varMiscComments", Utilities.GetReportStringValue(SIVReport["U_x002e__x0020_Miscellaneous_x00"]));

                      //string issueId = string.Empty;
                      //string creationDate = string.Empty;
                      //string closeDate = string.Empty;
                      //string description = string.Empty;
                      //string category = string.Empty;
                      //string action = string.Empty;
                      //string subjectId = string.Empty;

                      //string issuesList = string.Empty;


                      //Write active issues
                      //Write active issues
                      //SPListItemCollection activeIssues = Queries.GetActiveIssues(SiteNo);
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

                      //Write closed issues
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

                      //Write main html file to disk
                      File.WriteAllText(convertedFileNameMain, mainHtml);

                      //signature page section
                      string signatureHtml = File.ReadAllText(signatureTemplateFileName).Replace("\r\n", string.Empty);
                      signatureHtml = signatureHtml.Replace("varVersionNumber", Utilities.GetReportStringValue(SIVReport["VersionNumber"]));
                      signatureHtml = signatureHtml.Replace("varVersionDate", Utilities.GetShortDateValue(SIVReport["VersionDate"]));

                      //Write signature html file to disk
                      File.WriteAllText(convertedFileNameSignature, signatureHtml);

                      string pdfFile = string.Format("C:\\HFA\\temp\\{0}", pdfFileName);

                      PDFConversion.CreateFinalPDF(convertedFileNameMain, convertedFileNameSignature, pdfFile);

                      SPFolder pdfLibrary = web.Folders[PDFDocumentLibrary];

                      //Upload PDF file to document library
                      if (!Utilities.FileExists(pdfLibrary, pdfFileName))
                      {
                          if (Utilities.UplodFileToDocLibrary(web, PDFDocumentLibrary, pdfFile, SiteNo, "SIV", WorkflowName))
                          {
                              lblMessage.Text = "PDF Report successfully created";
                          }
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

        protected SPListItemCollection GetActiveIssues(int siteId)
        {
            SPList issuesList = SPContext.Current.Web.Lists["Issues List"];
            SPQuery spQuery = new SPQuery();
            string queryText = "<Where><And>";
            queryText += "<Eq><FieldRef Name='SiteNumberValue' /><Value Type='Number'>" + SiteId + "</Value></Eq>";
            queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='Choice'>Active</Value></Eq>";
            queryText += "</And></Where></Query>";

            spQuery.Query = queryText;
            SPListItemCollection issues = issuesList.GetItems(spQuery);

            return issues;
        }

        protected SPListItemCollection GetClosedIssues(int siteId)
        {
            SPList issuesList = SPContext.Current.Web.Lists["Issues List"];
            SPQuery spQuery = new SPQuery();
            string queryText = "<Where><And>";
            queryText += "<Eq><FieldRef Name='SiteNumberValue' /><Value Type='Number'>" + SiteId + "</Value></Eq>";
            queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='Choice'>Closed</Value></Eq>";
            queryText += "</And></Where></Query>";

            spQuery.Query = queryText;
            SPListItemCollection issues = issuesList.GetItems(spQuery);

            return issues;
        }

        protected SPListItem GetSIVReportData(int reportId)
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
                         SPList list = web.Lists["SIV Report"];
                         SPQuery oQuery = new SPQuery();

                         string query = "<Where>";
                         query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
                         query += "</Where>";
                         oQuery.Query = query;

                         //If is match is foud then delete list item
                         listItems = list.GetItems(oQuery);
                     }
                 }
             });

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
        }

        protected SPListItem GetSIVReportData(SPWeb web, int reportId)
        {
            if (reportId == 0) return null;

            SPListItemCollection listItems = null;

            SPList list = web.Lists["SIV Report"];
            SPQuery oQuery = new SPQuery();

            string query = "<Where>";
            query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //If is match is foud then delete list item
            listItems = list.GetItems(oQuery);

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
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
            UnselectAllMenuItems(formMenu2);

            switch (PreviousTabIndex)
            {
                case 1:
                    SaveGeneralInfo();
                    break;

                case 2:
                    SaveBrochure();
                    break;

                case 3:
                    SaveProtocol();
                    break;

                case 4:
                    SaveEnrollment();
                    break;

                case 5:
                    SaveIRBIECReq();
                    break;

                case 6:
                    SaveInformedConsent();
                    break;

                case 7:
                    SaveAdverseEvent();
                    break;

                case 8:
                    SaveInvestigatorSite();
                    break;

                case 9:
                    SaveSourceDoc();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void SaveGeneralInfo()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

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

        protected void SaveVisitAttendees()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;
            //Investigate Attendees
            //item["Investigator"] = txtInvestigatorName.Text;
            item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
            item["SitePersonnelName"] = txtSitePersonnelName.Text;
            item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;

            item["MonitorName"] = txtMonitorName.Text;
            item["MonitorTitle"] = txtMonitorTitle.Text;

            item["Intro.Clinical Site Personnel"] = ddlOtherSitePersonnel.SelectedValue;
            item["Intro.Other Personnel"] = ddlOtherPersonnel.SelectedValue;

            item["Intro_x002e_Clinical_x0020_Site_"] = txtPersonnelComments.Text;
            item["Intro_x002e_Other_x0020_Personne0"] = txtOtherSitePersonnelComments.Text;

            item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveBrochure()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //D section
            item["D_x002e_1"] = ddlD1.SelectedValue;
            item["D_x002e_2"] = ddlD2.SelectedValue;
            item["D_x002e_3"] = ddlD3.SelectedValue;

            item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
            item["D_x002e_2_x0020_Comments"] = txtD2Comments.Text;
            item["D_x002e_3_x0020_Comments"] = txtD3Comments.Text;

            item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveProtocol()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //E section
            item["E_x002e_1"] = ddlE1.SelectedValue;
            item["E_x002e_2"] = ddlE2.SelectedValue;
            item["E_x002e_3"] = ddlE3.SelectedValue;
            item["E_x002e_4"] = ddlE4.SelectedValue;
            item["E_x002e_5"] = ddlE5.SelectedValue;
            item["E_x002e_6"] = ddlE6.SelectedValue;
            item["E_x002e_7"] = ddlE7.SelectedValue;

            item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
            item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
            item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
            item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;
            item["E_x002e_5_x0020_Comments"] = txtE5Comments.Text;
            item["E_x002e_6_x0020_Comments"] = txtE6Comments.Text;
            item["E_x002e_7_x0020_Comments"] = txtE7Comments.Text;

            item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveEnrollment()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;
            //F section
            item["F_x002e_1"] = ddlF1.SelectedValue;
            item["F_x002e_2"] = ddlF2.SelectedValue;
            item["F_x002e_3"] = ddlF3.SelectedValue;
            item["F_x002e_4"] = ddlF4.SelectedValue;
            item["F_x002e_5"] = ddlF5.SelectedValue;

            item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
            item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;
            item["F_x002e_3_x0020_Comments"] = txtF3Comments.Text;
            item["F_x002e_4_x0020_Comments"] = txtF4Comments.Text;
            item["F_x002e_5_x0020_Comments"] = txtF5Comments.Text;

            item["F_x002e_Reviewer"] = txtFReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveIRBIECReq()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;
            item["G_x002e_1"] = ddlG1.SelectedValue;
            item["G_x002e_2"] = ddlG2.SelectedValue;
            item["G_x002e_3"] = ddlG3.SelectedValue;
            item["G_x002e_4"] = ddlG4.SelectedValue;

            item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
            item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
            item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
            item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;

            item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInformedConsent()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;
            //H section
            item["H_x002e_1"] = ddlH1.SelectedValue;

            item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;

            item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveAdverseEvent()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

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

            item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigatorSite()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //J section
            item["J_x002e_1"] = ddlJ1.SelectedValue;
            item["J_x002e_2"] = ddlJ2.SelectedValue;
            item["J_x002e_3"] = ddlJ3.SelectedValue;
            item["J_x002e_4"] = ddlJ4.SelectedValue;

            item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
            item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
            item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;
            item["J_x002e_4_x0020_Comments"] = txtJ4Comments.Text;

            item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSourceDoc()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //K section
            item["K_x002e_1"] = ddlK1.SelectedValue;
            item["K_x002e_2"] = ddlK2.SelectedValue;
            item["K_x002e_3"] = ddlK3.SelectedValue;
            item["K_x002e_4"] = ddlK4.SelectedValue;
            item["K_x002e_5"] = ddlK5.SelectedValue;
            item["K_x002e_6"] = ddlK6.SelectedValue;
            item["K_x002e_7"] = ddlK7.SelectedValue;

            item["K_x002e_1_x0020_Comments"] = txtK1Comments.Text;
            item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
            item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
            item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;
            item["K_x002e_5_x0020_Comments"] = txtK5Comments.Text;
            item["K_x002e_6_x0020_Comments"] = txtK6Comments.Text;
            item["K_x002e_7_x0020_Comments"] = txtK7Comments.Text;

            item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveCaseReportForms()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //L section
            item["L_x002e_1"] = ddlL1.SelectedValue;
            item["L_x002e_2"] = ddlL2.SelectedValue;
            item["L_x002e_3"] = ddlL3.SelectedValue;

            item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
            item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;
            item["L_x002e_3_x0020_Comments"] = txtL3Comments.Text;

            item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveFacilities()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //M section
            item["M_x002e_1"] = ddlM1.SelectedValue;

            item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;

            item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigationalProduct()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //N section
            item["M_x002e_1"] = ddlN1.SelectedValue;
            item["N_x002e_2"] = ddlN2.SelectedValue;
            item["N_x002e_3"] = ddlN3.SelectedValue;
            item["N_x002e_4"] = ddlN4.SelectedValue;
            item["N_x002e_5"] = ddlN5.SelectedValue;
            item["N_x002e_6"] = ddlN6.SelectedValue;
            item["N_x002e_7"] = ddlN7.SelectedValue;
            item["N_x002e_8"] = ddlN8.SelectedValue;
            item["N_x002e_9"] = ddlN9.SelectedValue;

            item["M_x002e_1_x0020_Comments"] = txtN1Comments.Text;
            item["N_x002e_2_x0020_Comments"] = txtN2Comments.Text;
            item["N_x002e_3_x0020_Comments"] = txtN3Comments.Text;
            item["N_x002e_4_x0020_Comments"] = txtN4Comments.Text;
            item["N_x002e_5_x0020_Comments"] = txtN5Comments.Text;
            item["N_x002e_6_x0020_Comments"] = txtN6Comments.Text;
            item["N_x002e_7_x0020_Comments"] = txtN7Comments.Text;
            item["N_x002e_8_x0020_Comments"] = txtN8Comments.Text;
            item["N_x002e_9_x0020_Comments"] = txtN9Comments.Text;

            item["N_x002e_Reviewer"] = txtNReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveOtherTrialMaterial()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //O section
            item["O_x002e_1"] = ddlO1.SelectedValue;
            item["O_x002e_2"] = ddlO2.SelectedValue;

            item["O_x002e_1_x0020_Comments"] = txtO1Comments.Text;
            item["O_x002e_2_x0020_Comments"] = txtO2Comments.Text;

            item["O_x002e_Reviewer"] = txtOReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveLaboratory()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //P section
            item["P_x002e_1"] = ddlP1.SelectedValue;
            item["P_x002e_2"] = ddlP2.SelectedValue;
            item["P_x002e_3"] = ddlP3.SelectedValue;
            item["P_x002e_4"] = ddlP4.SelectedValue;
            item["P_x002e_5"] = ddlP5.SelectedValue;

            item["P_x002e_1_x0020_Comments"] = txtP1Comments.Text;
            item["P_x002e_2_x0020_Comments"] = txtP2Comments.Text;
            item["P_x002e_3_x0020_Comments"] = txtP3Comments.Text;
            item["P_x002e_4_x0020_Comments"] = txtP4Comments.Text;
            item["P_x002e_5_x0020_Comments"] = txtP5Comments.Text;

            item["P_x002e_Reviewer"] = txtPReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveInvestigatorReponsibilities()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["Q_x002e_1"] = ddlQ1.SelectedValue;
            item["Q_x002e_2"] = ddlQ2.SelectedValue;
            item["Q_x002e_3"] = ddlQ3.SelectedValue;
            item["Q_x002e_4"] = ddlQ4.SelectedValue;
            item["Q_x002e_5"] = ddlQ5.SelectedValue;
            item["Q_x002e_6"] = ddlQ6.SelectedValue;
            item["Q_x002e_7"] = ddlQ7.SelectedValue;
            item["Q_x002e_8"] = ddlQ8.SelectedValue;

            item["Q_x002e_1_x0020_Comments"] = txtQ1Comments.Text;
            item["Q_x002e_2_x0020_Comments"] = txtQ2Comments.Text;
            item["Q_x002e_3_x0020_Comments"] = txtQ3Comments.Text;
            item["Q_x002e_4_x0020_Comments"] = txtQ4Comments.Text;
            item["Q_x002e_5_x0020_Comments"] = txtQ5Comments.Text;
            item["Q_x002e_6_x0020_Comments"] = txtQ6Comments.Text;
            item["Q_x002e_7_x0020_Comments"] = txtQ7Comments.Text;
            item["Q_x002e_8_x0020_Comments"] = txtQ8Comments.Text;

            item["Q_x002e_Reviewer"] = txtQReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveAncillaryStudyStaff()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //R section
            item["R_x002e_1"] = ddlR1.SelectedValue;
            item["R_x002e_2"] = ddlR2.SelectedValue;
            item["R_x002e_3"] = ddlR3.SelectedValue;

            item["R_x002e_1_x0020_Comments"] = txtR1Comments.Text;
            item["R_x002e_2_x0020_Comments"] = txtR2Comments.Text;
            item["R_x002e_3_x0020_Comments"] = txtR3Comments.Text;

            item["R_x002e_Reviewer"] = txtRReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveMonitoring()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //S section
            item["S_x002e_1"] = ddlS1.SelectedValue;
            item["S_x002e_2"] = ddlS2.SelectedValue;
            item["S_x002e_3"] = ddlS3.SelectedValue;

            item["S_x002e_1_x0020_Comments"] = txtS1Comments.Text;
            item["S_x002e_2_x0020_Comments"] = txtS2Comments.Text;
            item["S_x002e_3_x0020_Comments"] = txtS3Comments.Text;

            item["S_x002e_Reviewer"] = txtSReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSiteAcceptability()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //T section
            item["T_x002e_1"] = ddlT1.SelectedValue;
            item["T_x002e_2"] = ddlT2.SelectedValue;

            item["T_x002e_1_x0020_Comments"] = txtT1Comments.Text;
            item["T_x002e_2_x0020_Comments"] = txtT2Comments.Text;

            item["T_x002e_Reviewer"] = txtTReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void formMenu2_MenuItemClick(object sender, MenuEventArgs e)
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

            int idx = int.Parse(e.Item.Value);
            formMultiView.ActiveViewIndex = idx;
            UnselectAllMenuItems(formMenu);

            switch (PreviousTabIndex)
            {
                case 10:
                    SaveCaseReportForms();
                    break;

                case 11:
                    SaveFacilities();
                    break;

                case 12:
                    SaveInvestigationalProduct();
                    break;

                case 13:
                    SaveOtherTrialMaterial();
                    break;

                case 14:
                    SaveLaboratory();
                    break;

                case 15:
                    SaveInvestigatorReponsibilities();
                    break;

                case 16:
                    SaveAncillaryStudyStaff();
                    break;

                case 17:
                    SaveMonitoring();
                    break;

                case 18:
                    SaveSiteAcceptability();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void UnselectAllMenuItems(Menu menu)
        {
            foreach (MenuItem item in menu.Items)
            {
                item.Selected = false;
            }
        }

        //protected void BindIssuesList()
        //{
        //    SPWeb web = SPContext.Current.Web;
        //    SPList issuesList = web.Lists["Issues List"];
        //    SPListItemCollection items = issuesList.Items;

        //    //Active Issues
        //    //Create data table
        //    DataTable dtActive = new DataTable();
        //    dtActive.Columns.Add("ID", typeof(string));
        //    dtActive.Columns.Add("Title", typeof(string));
        //    dtActive.Columns.Add("Issue Status", typeof(string));
        //    dtActive.Columns.Add("Category", typeof(string));
        //    dtActive.Columns.Add("Creation Date", typeof(string));
        //    dtActive.Columns.Add("Close Date", typeof(string));
        //    dtActive.Columns.Add("Description", typeof(string));

        //    //Execute query
        //    SPListItemCollection activeIssues = Queries.GetActiveIssues(SiteNo);

        //    //Create row for each list item
        //    DataRow row;
        //    foreach (SPListItem item in activeIssues)
        //    {
        //        row = dtActive.Rows.Add();
        //        row["ID"] = item.ID;
        //        row["Title"] = item.Name;
        //        row["Issue Status"] = item["Issue Status"];
        //        row["Category"] = item["Category"];
        //        row["Creation Date"] = Convert.ToDateTime(item["Creation Date"]).ToShortDateString();
        //        row["Close Date"] = Convert.ToDateTime(item["Close Date"]).ToShortDateString();
        //        row["Description"] = item["Description"];
        //    }

        //    //Bind data to grid
        //    gdvActiveIssues.DataSource = dtActive.DefaultView;
        //    gdvActiveIssues.DataBind();

        //    //Closed Issues
        //    DataTable dtClosed = new DataTable();
        //    dtClosed.Columns.Add("ID", typeof(string));
        //    dtClosed.Columns.Add("Title", typeof(string));
        //    dtClosed.Columns.Add("Issue Status", typeof(string));
        //    dtClosed.Columns.Add("Category", typeof(string));
        //    dtClosed.Columns.Add("Creation Date", typeof(string));
        //    dtClosed.Columns.Add("Close Date", typeof(string));
        //    dtClosed.Columns.Add("Description", typeof(string));

        //    //Execute query
        //    SPListItemCollection closedIssues = Queries.GetClosedIssues(SiteNo);

        //    foreach (SPListItem item in closedIssues)
        //    {
        //        row = dtClosed.Rows.Add();
        //        row["ID"] = item.ID;
        //        row["Title"] = item.Name;
        //        row["Issue Status"] = item["Issue Status"];
        //        row["Category"] = item["Category"];
        //        row["Creation Date"] = Convert.ToDateTime(item["Creation Date"]).ToShortDateString();
        //        row["Close Date"] = Convert.ToDateTime(item["Close Date"]).ToShortDateString();
        //        row["Description"] = item["Description"];
        //    }

        //    //Bind data to grid
        //    gdvClosedIssues.DataSource = dtClosed.DefaultView;
        //    gdvClosedIssues.DataBind();
        //}

        protected void FillInvestAtt()
        {
            if (ReportId.Equals("0")) 
                return;

            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            //lblInvestigatorName.Text = Utilities.GetStringValue(item["Investigator"]);
            txtInvestigatorTitle.Text = Utilities.GetStringValue(item["InvestigatorTitle"]);
            txtSitePersonnelName.Text = Utilities.GetStringValue(item["SitePersonnelName"]);
            txtSitePersonnelTitle.Text = Utilities.GetStringValue(item["SitePersonnelTitle"]);
            txtSitePersonnelName2.Text = Utilities.GetStringValue(item["SitePersonnelName2"]);
            txtSitePersonnelTitle2.Text = Utilities.GetStringValue(item["SitePersonnelTitle2"]);
            txtSitePersonnelName3.Text = Utilities.GetStringValue(item["SitePersonnelName3"]);
            txtSitePersonnelTitle3.Text = Utilities.GetStringValue(item["SitePersonnelTitle3"]);

            txtMonitorName.Text = Utilities.GetStringValue(item["MonitorName"]);
            txtMonitorTitle.Text = Utilities.GetStringValue(item["MonitorTitle"]);

            ddlOtherSitePersonnel.SelectedValue = Utilities.GetStringValue(item["Intro.Clinical Site Personnel"]);
            ddlOtherPersonnel.SelectedValue = Utilities.GetStringValue(item["Intro.Other Personnel"]);

            txtOtherSitePersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Clinical_x0020_Site_"]);
            txtPersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Other_x0020_Personne0"]);

            txtVAReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Visit_x002e_Reviewer");
        }

        protected void FillCData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["C_x002e_1"] != null)
                ddlC1.SelectedValue = item["C_x002e_1"].ToString();

            if (item["C_x002e_2"] != null)
                ddlC2.SelectedValue = item["C_x002e_2"].ToString();

            txtC1Comments.Text = GetMultiLineTextFieldValue(item, "C_x002e_1_x0020_Comments");
            txtC2Comments.Text = GetMultiLineTextFieldValue(item, "C_x002e_2_x0020_Comments");

            txtCReviewerComments.Text = GetMultiLineTextFieldValue(item, "C_x002e_Reviewer");
            item["C_x002e_Reviewer"] = txtCReviewerComments.Text;
        }

        protected void FillDData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["D_x002e_1"] != null)
                ddlD1.SelectedValue = item["D_x002e_1"].ToString();

            if (item["D_x002e_2"] != null)
                ddlD2.SelectedValue = item["D_x002e_2"].ToString();

            if (item["D_x002e_3"] != null)
                ddlD3.SelectedValue = item["D_x002e_3"].ToString();

            txtD1Comments.Text = GetMultiLineTextFieldValue(item, "D_x002e_1_x0020_Comments");
            txtD2Comments.Text = GetMultiLineTextFieldValue(item, "D_x002e_2_x0020_Comments");
            txtD3Comments.Text = GetMultiLineTextFieldValue(item, "D_x002e_3_x0020_Comments");

            txtDReviewerComments.Text = GetMultiLineTextFieldValue(item, "D_x002e_Reviewer");
        }

        protected void FillEData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["E_x002e_1"] != null)
                ddlE1.SelectedValue = item["E_x002e_1"].ToString();

            if (item["E_x002e_2"] != null)
                ddlE2.SelectedValue = item["E_x002e_2"].ToString();

            if (item["E_x002e_3"] != null)
                ddlE3.SelectedValue = item["E_x002e_3"].ToString();

            if (item["E_x002e_4"] != null)
                ddlE4.SelectedValue = item["E_x002e_4"].ToString();

            if (item["E_x002e_5"] != null)
                ddlE5.SelectedValue = item["E_x002e_5"].ToString();

            if (item["E_x002e_6"] != null)
                ddlE6.SelectedValue = item["E_x002e_6"].ToString();

            if (item["E_x002e_7"] != null)
                ddlE7.SelectedValue = item["E_x002e_7"].ToString();

            txtE1Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_1_x0020_Comments");
            txtE2Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_2_x0020_Comments");
            txtE3Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_3_x0020_Comments");
            txtE4Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_4_x0020_Comments");
            txtE5Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_5_x0020_Comments");
            txtE6Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_6_x0020_Comments");
            txtE7Comments.Text = GetMultiLineTextFieldValue(item, "E_x002e_7_x0020_Comments");

            txtEReviewerComments.Text = GetMultiLineTextFieldValue(item, "E_x002e_Reviewer");
        }

        protected void FillFData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["F_x002e_1"] != null)
                ddlF1.SelectedValue = item["F_x002e_1"].ToString();

            if (item["F_x002e_2"] != null)
                ddlF2.SelectedValue = item["F_x002e_2"].ToString();

            if (item["F_x002e_3"] != null)
                ddlF3.SelectedValue = item["F_x002e_3"].ToString();

            if (item["F_x002e_4"] != null)
                ddlF4.SelectedValue = item["F_x002e_4"].ToString();

            if (item["F_x002e_5"] != null)
                ddlF5.SelectedValue = item["F_x002e_5"].ToString();

            txtF1Comments.Text = GetMultiLineTextFieldValue(item, "F_x002e_1_x0020_Comments");
            txtF2Comments.Text = GetMultiLineTextFieldValue(item, "F_x002e_2_x0020_Comments");
            txtF3Comments.Text = GetMultiLineTextFieldValue(item, "F_x002e_3_x0020_Comments");
            txtF4Comments.Text = GetMultiLineTextFieldValue(item, "F_x002e_4_x0020_Comments");
            txtF5Comments.Text = GetMultiLineTextFieldValue(item, "F_x002e_5_x0020_Comments");

            txtFReviewerComments.Text = GetMultiLineTextFieldValue(item, "F_x002e_Reviewer");
        }

        protected void FillGData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["G_x002e_1"] != null)
                ddlG1.SelectedValue = item["G_x002e_1"].ToString();

            if (item["G_x002e_2"] != null)
                ddlG2.SelectedValue = item["G_x002e_2"].ToString();

            if (item["G_x002e_3"] != null)
                ddlG3.SelectedValue = item["G_x002e_3"].ToString();

            if (item["G_x002e_4"] != null)
                ddlG4.SelectedValue = item["G_x002e_4"].ToString();

            txtG1Comments.Text = GetMultiLineTextFieldValue(item, "G_x002e_1_x0020_Comments");
            txtG2Comments.Text = GetMultiLineTextFieldValue(item, "G_x002e_2_x0020_Comments");
            txtG3Comments.Text = GetMultiLineTextFieldValue(item, "G_x002e_3_x0020_Comments");
            txtG4Comments.Text = GetMultiLineTextFieldValue(item, "G_x002e_4_x0020_Comments");

            txtGReviewerComments.Text = GetMultiLineTextFieldValue(item, "G_x002e_Reviewer");
        }

        protected void FillHData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            ddlH1.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_1"]);

            txtH1Comments.Text = GetMultiLineTextFieldValue(item, "H_x002e_1_x0020_Comments");

            txtHReviewerComments.Text = GetMultiLineTextFieldValue(item, "H_x002e_Reviewer");
        }

        protected void FillIData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["I_x002e_1"] != null)
                ddlI1.SelectedValue = item["I_x002e_1"].ToString();

            if (item["I_x002e_2"] != null)
                ddlI2.SelectedValue = item["I_x002e_2"].ToString();

            if (item["I_x002e_3"] != null)
                ddlI3.SelectedValue = item["I_x002e_3"].ToString();

            if (item["I_x002e_4"] != null)
                ddlI4.SelectedValue = item["I_x002e_4"].ToString();

            txtI1Comments.Text = GetMultiLineTextFieldValue(item, "I_x002e_1_x0020_Comments");
            txtI2Comments.Text = GetMultiLineTextFieldValue(item, "I_x002e_2_x0020_Comments");
            txtI3Comments.Text = GetMultiLineTextFieldValue(item, "I_x002e_3_x0020_Comments");
            txtI4Comments.Text = GetMultiLineTextFieldValue(item, "I_x002e_4_x0020_Comments");

            txtIReviewerComments.Text =GetMultiLineTextFieldValue(item, "I_x002e_Reviewer");
        }

        protected void FillJData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["J_x002e_1"] != null)
                ddlJ1.SelectedValue = item["J_x002e_1"].ToString();

            if (item["J_x002e_2"] != null)
                ddlJ2.SelectedValue = item["J_x002e_2"].ToString();

            if (item["J_x002e_3"] != null)
                ddlJ3.SelectedValue = item["J_x002e_3"].ToString();

            if (item["J_x002e_4"] != null)
                ddlJ4.SelectedValue = item["J_x002e_4"].ToString();

            txtJ1Comments.Text = GetMultiLineTextFieldValue(item, "J_x002e_1_x0020_Comments");
            txtJ2Comments.Text = GetMultiLineTextFieldValue(item, "J_x002e_2_x0020_Comments");
            txtJ3Comments.Text = GetMultiLineTextFieldValue(item, "J_x002e_3_x0020_Comments");
            txtJ4Comments.Text = GetMultiLineTextFieldValue(item, "J_x002e_4_x0020_Comments");

            txtJReviewerComments.Text =GetMultiLineTextFieldValue(item, "J_x002e_Reviewer");
        }

        protected void FillKData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["K_x002e_1"] != null)
                ddlI1.SelectedValue = item["K_x002e_1"].ToString();

            if (item["K_x002e_2"] != null)
                ddlK2.SelectedValue = item["K_x002e_2"].ToString();

            if (item["K_x002e_3"] != null)
                ddlK3.SelectedValue = item["K_x002e_3"].ToString();

            if (item["K_x002e_4"] != null)
                ddlK4.SelectedValue = item["K_x002e_4"].ToString();

            if (item["K_x002e_5"] != null)
                ddlK5.SelectedValue = item["K_x002e_5"].ToString();

            if (item["K_x002e_6"] != null)
                ddlK6.SelectedValue = item["K_x002e_6"].ToString();

            if (item["K_x002e_7"] != null)
                ddlK7.SelectedValue = item["K_x002e_7"].ToString();

            txtK1Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_1_x0020_Comments");
            txtK2Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_2_x0020_Comments");
            txtK3Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_3_x0020_Comments");
            txtK4Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_4_x0020_Comments");
            txtK5Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_5_x0020_Comments");
            txtK6Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_6_x0020_Comments");
            txtK7Comments.Text = GetMultiLineTextFieldValue(item, "K_x002e_7_x0020_Comments");

            txtKReviewerComments.Text = GetMultiLineTextFieldValue(item, "K_x002e_Reviewer");
        }

        protected void FillLData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["L_x002e_1"] != null)
                ddlL1.SelectedValue = item["L_x002e_1"].ToString();

            if (item["L_x002e_2"] != null)
                ddlL2.SelectedValue = item["L_x002e_2"].ToString();

            if (item["L_x002e_3"] != null)
                ddlL3.SelectedValue = item["L_x002e_3"].ToString();

            txtL1Comments.Text = GetMultiLineTextFieldValue(item, "L_x002e_1_x0020_Comments");
            txtL2Comments.Text = GetMultiLineTextFieldValue(item, "L_x002e_2_x0020_Comments");
            txtL3Comments.Text = GetMultiLineTextFieldValue(item, "L_x002e_3_x0020_Comments");

            txtLReviewerComments.Text =GetMultiLineTextFieldValue(item, "L_x002e_Reviewer");
        }

        protected void FillMData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["M_x002e_1"] != null)
                ddlM1.SelectedValue = item["M_x002e_1"].ToString();

            txtM1Comments.Text = GetMultiLineTextFieldValue(item, "M_x002e_1_x0020_Comments");

            txtMReviewerComments.Text = GetMultiLineTextFieldValue(item, "M_x002e_Reviewer");
        }

        protected void FillNData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["N_x002e_1"] != null)
                ddlN1.SelectedValue = item["N_x002e_1"].ToString();

            if (item["N_x002e_2"] != null)
                ddlN2.SelectedValue = item["N_x002e_2"].ToString();

            if (item["N_x002e_3"] != null)
                ddlN3.SelectedValue = item["N_x002e_3"].ToString();

            if (item["N_x002e_4"] != null)
                ddlN4.SelectedValue = item["N_x002e_4"].ToString();

            if (item["N_x002e_5"] != null)
                ddlN5.SelectedValue = item["N_x002e_5"].ToString();

            if (item["N_x002e_6"] != null)
                ddlN6.SelectedValue = item["N_x002e_6"].ToString();

            if (item["N_x002e_7"] != null)
                ddlN7.SelectedValue = item["N_x002e_7"].ToString();

            if (item["N_x002e_8"] != null)
                ddlN8.SelectedValue = item["N_x002e_8"].ToString();

            if (item["N_x002e_9"] != null)
                ddlN9.SelectedValue = item["N_x002e_9"].ToString();

            txtN1Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_1_x0020_Comments");
            txtN2Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_2_x0020_Comments");
            txtN3Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_3_x0020_Comments");
            txtN4Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_4_x0020_Comments");
            txtN5Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_5_x0020_Comments");
            txtN6Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_6_x0020_Comments");
            txtN7Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_7_x0020_Comments");
            txtN8Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_8_x0020_Comments");
            txtN9Comments.Text = GetMultiLineTextFieldValue(item, "N_x002e_9_x0020_Comments");

            txtNReviewerComments.Text = GetMultiLineTextFieldValue(item, "N_x002e_Reviewer"); ;
        }

        protected void FillOData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId)); 

            if (item["O_x002e_1"] != null)
                ddlO1.SelectedValue = item["O_x002e_1"].ToString();


            if (item["O_x002e_2"] != null)
                ddlO2.SelectedValue = item["O_x002e_2"].ToString();

            txtO1Comments.Text = GetMultiLineTextFieldValue(item, "O_x002e_1_x0020_Comments");
            txtO2Comments.Text = GetMultiLineTextFieldValue(item, "O_x002e_2_x0020_Comments");

            txtOReviewerComments.Text = GetMultiLineTextFieldValue(item, "O_x002e_Reviewer");
        }

        protected void FillPData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["P_x002e_1"] != null)
                ddlP1.SelectedValue = item["P_x002e_1"].ToString();

            if (item["P_x002e_2"] != null)
                ddlP2.SelectedValue = item["P_x002e_2"].ToString();

            if (item["P_x002e_3"] != null)
                ddlP3.SelectedValue = item["P_x002e_3"].ToString();

            if (item["P_x002e_4"] != null)
                ddlP4.SelectedValue = item["P_x002e_4"].ToString();

            if (item["P_x002e_5"] != null)
                ddlP5.SelectedValue = item["P_x002e_5"].ToString();

            txtP1Comments.Text = GetMultiLineTextFieldValue(item, "P_x002e_1_x0020_Comments");
            txtP2Comments.Text = GetMultiLineTextFieldValue(item, "P_x002e_2_x0020_Comments");
            txtP3Comments.Text = GetMultiLineTextFieldValue(item, "P_x002e_3_x0020_Comments");
            txtP4Comments.Text = GetMultiLineTextFieldValue(item, "P_x002e_4_x0020_Comments");
            txtP5Comments.Text = GetMultiLineTextFieldValue(item, "P_x002e_5_x0020_Comments");

            txtPReviewerComments.Text = GetMultiLineTextFieldValue(item, "P_x002e_Reviewer");
        }

        protected void FillQData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["Q_x002e_1"] != null)
                ddlQ1.SelectedValue = item["Q_x002e_1"].ToString();

            if (item["Q_x002e_2"] != null)
                ddlQ2.SelectedValue = item["Q_x002e_2"].ToString();

            if (item["Q_x002e_3"] != null)
                ddlQ3.SelectedValue = item["Q_x002e_3"].ToString();

            if (item["Q_x002e_4"] != null)
                ddlQ4.SelectedValue = item["Q_x002e_4"].ToString();

            if (item["Q_x002e_5"] != null)
                ddlQ5.SelectedValue = item["Q_x002e_5"].ToString();

            if (item["Q_x002e_6"] != null)
                ddlQ6.SelectedValue = item["Q_x002e_6"].ToString();

            if (item["Q_x002e_7"] != null)
                ddlQ7.SelectedValue = item["Q_x002e_7"].ToString();

            if (item["Q_x002e_8"] != null)
                ddlQ8.SelectedValue = item["Q_x002e_8"].ToString();

            txtQ1Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_1_x0020_Comments");
            txtQ2Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_2_x0020_Comments");
            txtQ3Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_3_x0020_Comments");
            txtQ4Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_4_x0020_Comments");
            txtQ5Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_5_x0020_Comments");
            txtQ6Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_6_x0020_Comments");
            txtQ7Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_7_x0020_Comments");
            txtQ8Comments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_8_x0020_Comments");

            txtQReviewerComments.Text = GetMultiLineTextFieldValue(item, "Q_x002e_Reviewer");
        }

        protected void FillRData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["R_x002e_1"] != null)
                ddlR1.SelectedValue = item["R_x002e_1"].ToString();

            if (item["R_x002e_2"] != null)
                ddlR2.SelectedValue = item["R_x002e_2"].ToString();

            if (item["R_x002e_3"] != null)
                ddlR3.SelectedValue = item["R_x002e_3"].ToString();

            txtR1Comments.Text = GetMultiLineTextFieldValue(item, "R_x002e_1_x0020_Comments");
            txtR2Comments.Text = GetMultiLineTextFieldValue(item, "R_x002e_2_x0020_Comments");
            txtR3Comments.Text = GetMultiLineTextFieldValue(item, "R_x002e_3_x0020_Comments");

            txtRReviewerComments.Text =GetMultiLineTextFieldValue(item, "R_x002e_Reviewer");
        }

        protected void FillSData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["S_x002e_1"] != null)
                ddlS1.SelectedValue = item["S_x002e_1"].ToString();

            if (item["S_x002e_2"] != null)
                ddlS2.SelectedValue = item["S_x002e_2"].ToString();

            if (item["S_x002e_3"] != null)
                ddlS3.SelectedValue = item["S_x002e_3"].ToString();

            txtS1Comments.Text = GetMultiLineTextFieldValue(item, "S_x002e_1_x0020_Comments");
            txtS2Comments.Text = GetMultiLineTextFieldValue(item, "S_x002e_2_x0020_Comments");
            txtS3Comments.Text = GetMultiLineTextFieldValue(item, "S_x002e_3_x0020_Comments");

            txtSReviewerComments.Text = GetMultiLineTextFieldValue(item, "S_x002e_Reviewer");
        }

        protected void FillTData()
        {
            SPListItem item = GetSIVReportData(int.Parse(ReportId));

            if (item["T_x002e_1"] != null)
                ddlT1.SelectedValue = item["T_x002e_1"].ToString();

            if (item["T_x002e_2"] != null)
                ddlT2.SelectedValue = item["T_x002e_2"].ToString();

            txtT1Comments.Text = GetMultiLineTextFieldValue(item, "T_x002e_1_x0020_Comments");
            txtT2Comments.Text = GetMultiLineTextFieldValue(item, "T_x002e_2_x0020_Comments");

            txtTReviewerComments.Text = GetMultiLineTextFieldValue(item, "T_x002e_Reviewer");
        }

        protected string GetMultiLineTextFieldValue(SPListItem item, string fieldName)
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

        protected string SetCommentsFieldValue(object fieldName)
        {
            string comments = string.Empty;
            string newComments = string.Empty;
            string[] commentsArray;

            if (fieldName == null)
                return string.Empty;

            comments = fieldName.ToString().Replace("<p>", "@");
            newComments = comments.Replace("</p>", "~");
            commentsArray = newComments.Split('@');

            return commentsArray[1].Split('~')[0];
        }

        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            SPListItem item = null;
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPList siteList = null;

            if (speMonitor.ResolvedEntities.Count <= 0)
                return;

            SPWeb oWeb = SPContext.Current.Web;
            SPUserToken userToken = oWeb.AllUsers[@UserToImpersonate].UserToken;

            using (SPSite site = new SPSite(siteID, userToken))
            {
                using (SPWeb web = site.AllWebs[webID])
                {

                    if (ReportId.Equals("0"))
                    {
                        if (Queries.ReportExists("SIV Report", txtTitle.Text))
                        {
                            lblMessage.Text = "Report already exists with the same name. Please enter a different name and try again.";
                            return;
                        }

                        siteList = web.Lists["SIV Report"];

                        web.AllowUnsafeUpdates = true;
                        item = siteList.Items.Add();
                        item["Title"] = txtTitle.Text;
                        item.Update();
                        web.AllowUnsafeUpdates = false;

                        //Get new ID
                        newReportId = item.ID;
                        item = GetSIVReportData(web, newReportId);
                    }
                    else
                        item = GetSIVReportData(web, int.Parse(ReportId));


                    item.Web.AllowUnsafeUpdates = true;
                    web.AllowUnsafeUpdates = true;

                    //Get Site info. by siteNo and create site lookup value field
                    SPListItem siteItem = Queries.GetSiteBySiteNo(int.Parse(ddlSites.SelectedItem.Text));
                    SiteInfo = new SiteList(int.Parse(ddlSites.SelectedItem.Text));

                    if (SiteInfo.Exists)
                        btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

                    SPFieldLookupValue siteLookupValue = new SPFieldLookupValue(siteItem.ID, siteItem.Title);
                    item["Site_x0020_Number"] = siteLookupValue.ToString();

                    item["Title"] = txtTitle.Text;
                    item["InvestigatorName"] = txtInvestigatorName.Text;
                    item["Sponsor"] = txtSponsor.Text;
                    item["Protocol_x0020__x0023_"] = txtProtocol.Text;
                    //item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    //item["Next_x0020_Visit_x0020_Date"] = calNextVisitDate.SelectedDate;
                    if (!calVisitDate.IsDateEmpty)
                        item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    else
                        item["Visit_x0020_Date"] = null;

                    if (!calNextVisitDate.IsDateEmpty)
                        item["Next_x0020_Visit_x0020_Date"] = calNextVisitDate.SelectedDate;
                    else
                        item["Next_x0020_Visit_x0020_Date"] = null;

                    item["Address"] = txtAddress.Text;
                    item["Status"] = item.Fields["Status"].GetFieldValue(ddlStatus.SelectedItem.Text);
                    item["SiteNumberText"] = SiteId;
                    item["U_x002e__x0020_Miscellaneous_x00"] = txtMiscComments.Text;
                    item["General_x0020_Reviewer_x0020_Com"] = txtGenRevComments.Text;
                    item["VersionNumber"] = ddlVersion.SelectedValue;
                    item["VersionDate"] = calVersionDate.SelectedDate;

                    //Set hyperlink values
                    SPFieldUrlValue sitePageUrl = new SPFieldUrlValue();
                    SPFieldUrlValue reportTitleUrl = new SPFieldUrlValue();

                    //Site Page hyperlink field
                    sitePageUrl.Url = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}", SPContext.Current.Web.Url, SiteNo);
                    sitePageUrl.Description = SiteNo.ToString();
                    item["Site_x0020_Page"] = sitePageUrl;

                    //Report Title Page hyperlink field
                    reportTitleUrl.Url = string.Format("{0}/SitePages/SIVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteNo, item.ID);
                    reportTitleUrl.Description = txtTitle.Text;
                    item["Report_x0020_Title"] = reportTitleUrl;

                    //People picker monitor field                
                    SPUser user = SPContext.Current.Web.SiteUsers[speMonitor.CommaSeparatedAccounts];
                    if (user != null)
                        item["Monitor"] = user;


                    //Investigate Attendees
                    item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    item["MonitorName"] = txtMonitorName.Text;
                    item["MonitorTitle"] = txtMonitorTitle.Text;

                    item["Intro.Clinical Site Personnel"] = ddlOtherSitePersonnel.SelectedValue;
                    item["Intro_x002e_Clinical_x0020_Site_"] = txtOtherSitePersonnelComments.Text;

                    item["Intro.Other Personnel"] = ddlOtherPersonnel.SelectedValue;
                    item["Intro_x002e_Other_x0020_Personne0"] = txtPersonnelComments.Text;

                    item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                    //C section
                    item["C_x002e_2"] = ddlC2.SelectedValue;
                    item["C_x002e_1"] = ddlC1.SelectedValue;

                    item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
                    item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;

                    item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

                    //D section
                    item["D_x002e_1"] = ddlD1.SelectedValue;
                    item["D_x002e_2"] = ddlD2.SelectedValue;
                    item["D_x002e_3"] = ddlD3.SelectedValue;

                    item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
                    item["D_x002e_2_x0020_Comments"] = txtD2Comments.Text;
                    item["D_x002e_3_x0020_Comments"] = txtD3Comments.Text;

                    item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

                    //E section
                    item["E_x002e_1"] = ddlE1.SelectedValue;
                    item["E_x002e_2"] = ddlE2.SelectedValue;
                    item["E_x002e_3"] = ddlE3.SelectedValue;
                    item["E_x002e_4"] = ddlE4.SelectedValue;
                    item["E_x002e_5"] = ddlE5.SelectedValue;
                    item["E_x002e_6"] = ddlE6.SelectedValue;
                    item["E_x002e_7"] = ddlE7.SelectedValue;

                    item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
                    item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
                    item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
                    item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;
                    item["E_x002e_5_x0020_Comments"] = txtE5Comments.Text;
                    item["E_x002e_6_x0020_Comments"] = txtE6Comments.Text;
                    item["E_x002e_7_x0020_Comments"] = txtE7Comments.Text;

                    item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

                    //F section
                    item["F_x002e_1"] = ddlF1.SelectedValue;
                    item["F_x002e_2"] = ddlF2.SelectedValue;
                    item["F_x002e_3"] = ddlF3.SelectedValue;
                    item["F_x002e_4"] = ddlF4.SelectedValue;
                    item["F_x002e_5"] = ddlF5.SelectedValue;

                    item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
                    item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;
                    item["F_x002e_3_x0020_Comments"] = txtF3Comments.Text;
                    item["F_x002e_4_x0020_Comments"] = txtF4Comments.Text;
                    item["F_x002e_5_x0020_Comments"] = txtF5Comments.Text;

                    item["F_x002e_Reviewer"] = txtFReviewerComments.Text;

                    //G section
                    item["G_x002e_1"] = ddlG1.SelectedValue;
                    item["G_x002e_2"] = ddlG2.SelectedValue;
                    item["G_x002e_3"] = ddlG3.SelectedValue;
                    item["G_x002e_4"] = ddlG4.SelectedValue;

                    item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
                    item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
                    item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
                    item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;

                    item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

                    //H section
                    item["H_x002e_1"] = ddlH1.SelectedValue;

                    item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;

                    item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

                    //I section
                    item["I_x002e_1"] = ddlI1.SelectedValue;
                    item["I_x002e_2"] = ddlI2.SelectedValue;
                    item["I_x002e_3"] = ddlI3.SelectedValue;
                    item["I_x002e_4"] = ddlI4.SelectedValue;

                    item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
                    item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
                    item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;
                    item["I_x002e_4_x0020_Comments"] = txtI4Comments.Text;

                    item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

                    //J section
                    item["J_x002e_1"] = ddlJ1.SelectedValue;
                    item["J_x002e_2"] = ddlJ2.SelectedValue;
                    item["J_x002e_3"] = ddlJ3.SelectedValue;
                    item["J_x002e_4"] = ddlJ4.SelectedValue;

                    item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
                    item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
                    item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;
                    item["J_x002e_4_x0020_Comments"] = txtJ4Comments.Text;

                    item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

                    //K section
                    item["K_x002e_1"] = ddlK1.SelectedValue;
                    item["K_x002e_2"] = ddlK2.SelectedValue;
                    item["K_x002e_3"] = ddlK3.SelectedValue;
                    item["K_x002e_4"] = ddlK4.SelectedValue;
                    item["K_x002e_5"] = ddlK5.SelectedValue;
                    item["K_x002e_6"] = ddlK6.SelectedValue;
                    item["K_x002e_7"] = ddlK7.SelectedValue;

                    item["K_x002e_1_x0020_Comments"] = txtK1Comments.Text;
                    item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
                    item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
                    item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;
                    item["K_x002e_5_x0020_Comments"] = txtK5Comments.Text;
                    item["K_x002e_6_x0020_Comments"] = txtK6Comments.Text;
                    item["K_x002e_7_x0020_Comments"] = txtK7Comments.Text;

                    item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

                    //L section
                    item["L_x002e_1"] = ddlL1.SelectedValue;
                    item["L_x002e_2"] = ddlL2.SelectedValue;
                    item["L_x002e_3"] = ddlL3.SelectedValue;

                    item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
                    item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;
                    item["L_x002e_3_x0020_Comments"] = txtL3Comments.Text;

                    item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

                    //M section
                    item["M_x002e_1"] = ddlM1.SelectedValue;

                    item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;

                    item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

                    //N section
                    item["N_x002e_1"] = ddlN1.SelectedValue;
                    item["N_x002e_2"] = ddlN2.SelectedValue;
                    item["N_x002e_3"] = ddlN3.SelectedValue;
                    item["N_x002e_4"] = ddlN4.SelectedValue;
                    item["N_x002e_5"] = ddlN5.SelectedValue;
                    item["N_x002e_6"] = ddlN6.SelectedValue;
                    item["N_x002e_7"] = ddlN7.SelectedValue;
                    item["N_x002e_8"] = ddlN8.SelectedValue;
                    item["N_x002e_9"] = ddlN9.SelectedValue;

                    item["N_x002e_1_x0020_Comments"] = txtN1Comments.Text;
                    item["N_x002e_2_x0020_Comments"] = txtN2Comments.Text;
                    item["N_x002e_3_x0020_Comments"] = txtN3Comments.Text;
                    item["N_x002e_4_x0020_Comments"] = txtN4Comments.Text;
                    item["N_x002e_5_x0020_Comments"] = txtN5Comments.Text;
                    item["N_x002e_6_x0020_Comments"] = txtN6Comments.Text;
                    item["N_x002e_7_x0020_Comments"] = txtN7Comments.Text;
                    item["N_x002e_8_x0020_Comments"] = txtN8Comments.Text;
                    item["N_x002e_9_x0020_Comments"] = txtN9Comments.Text;

                    item["N_x002e_Reviewer"] = txtNReviewerComments.Text;

                    //O section
                    item["O_x002e_1"] = ddlO1.SelectedValue;
                    item["O_x002e_2"] = ddlO2.SelectedValue;

                    item["O_x002e_1_x0020_Comments"] = txtO1Comments.Text;
                    item["O_x002e_2_x0020_Comments"] = txtO2Comments.Text;

                    item["O_x002e_Reviewer"] = txtOReviewerComments.Text;

                    //P section
                    item["P_x002e_1"] = ddlP1.SelectedValue;
                    item["P_x002e_2"] = ddlP2.SelectedValue;
                    item["P_x002e_3"] = ddlP3.SelectedValue;
                    item["P_x002e_4"] = ddlP4.SelectedValue;
                    item["P_x002e_5"] = ddlP5.SelectedValue;

                    item["P_x002e_1_x0020_Comments"] = txtP1Comments.Text;
                    item["P_x002e_2_x0020_Comments"] = txtP2Comments.Text;
                    item["P_x002e_3_x0020_Comments"] = txtP3Comments.Text;
                    item["P_x002e_4_x0020_Comments"] = txtP4Comments.Text;
                    item["P_x002e_5_x0020_Comments"] = txtP5Comments.Text;

                    item["P_x002e_Reviewer"] = txtPReviewerComments.Text;

                    //Q Investigatior Responsibilities
                    item["Q_x002e_1"] = ddlQ1.SelectedValue;
                    item["Q_x002e_2"] = ddlQ2.SelectedValue;
                    item["Q_x002e_3"] = ddlQ3.SelectedValue;
                    item["Q_x002e_4"] = ddlQ4.SelectedValue;
                    item["Q_x002e_5"] = ddlQ5.SelectedValue;
                    item["Q_x002e_6"] = ddlQ6.SelectedValue;
                    item["Q_x002e_7"] = ddlQ7.SelectedValue;
                    item["Q_x002e_8"] = ddlQ8.SelectedValue;

                    item["Q_x002e_1_x0020_Comments"] = txtQ1Comments.Text;
                    item["Q_x002e_2_x0020_Comments"] = txtQ2Comments.Text;
                    item["Q_x002e_3_x0020_Comments"] = txtQ3Comments.Text;
                    item["Q_x002e_4_x0020_Comments"] = txtQ4Comments.Text;
                    item["Q_x002e_5_x0020_Comments"] = txtQ5Comments.Text;
                    item["Q_x002e_6_x0020_Comments"] = txtQ6Comments.Text;
                    item["Q_x002e_7_x0020_Comments"] = txtQ7Comments.Text;
                    item["Q_x002e_8_x0020_Comments"] = txtQ8Comments.Text;

                    item["Q_x002e_Reviewer"] = txtQReviewerComments.Text;

                    //R section
                    item["R_x002e_1"] = ddlR1.SelectedValue;
                    item["R_x002e_2"] = ddlR2.SelectedValue;
                    item["R_x002e_3"] = ddlR3.SelectedValue;

                    item["R_x002e_1_x0020_Comments"] = txtR1Comments.Text;
                    item["R_x002e_2_x0020_Comments"] = txtR2Comments.Text;
                    item["R_x002e_3_x0020_Comments"] = txtR3Comments.Text;

                    item["R_x002e_Reviewer"] = txtRReviewerComments.Text;

                    //S section
                    item["S_x002e_1"] = ddlS1.SelectedValue;
                    item["S_x002e_2"] = ddlS2.SelectedValue;
                    item["S_x002e_3"] = ddlS3.SelectedValue;

                    item["S_x002e_1_x0020_Comments"] = txtS1Comments.Text;
                    item["S_x002e_2_x0020_Comments"] = txtS2Comments.Text;
                    item["S_x002e_3_x0020_Comments"] = txtS3Comments.Text;

                    item["S_x002e_Reviewer"] = txtSReviewerComments.Text;

                    //T section
                    item["T_x002e_1"] = ddlT1.SelectedValue;
                    item["T_x002e_2"] = ddlT2.SelectedValue;

                    item["T_x002e_1_x0020_Comments"] = txtT1Comments.Text;
                    item["T_x002e_2_x0020_Comments"] = txtT2Comments.Text;

                    item["T_x002e_Reviewer"] = txtTReviewerComments.Text;

                    //Save Changes
                    item.Update();

                    //If new report, redirect to current page to get updated report id in the querystring
                    if (ReportId.Equals("0"))
                    {
                        ShowMessage = true;
                        Response.Redirect(reportTitleUrl.Url);
                    }

                    web.AllowUnsafeUpdates = false;
                    item.Web.AllowUnsafeUpdates = false;
                }
            }


            lblMessage.Text = "Report successfully saved";
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
