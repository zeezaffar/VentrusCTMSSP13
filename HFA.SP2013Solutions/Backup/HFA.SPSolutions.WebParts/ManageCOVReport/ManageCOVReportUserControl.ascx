<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageCOVReportUserControl.ascx.cs" Inherits="HFA.SPSolutions.WebParts.ManageCOVReport.ManageCOVReportUserControl" %>

<SharePoint:CssRegistration ID="defaultCSS" Name="/Style Library/default.css" runat="server" After="corev4.css" />
<script type="text/javascript" src="/_layouts/DanFay/jquery-1.7.1.min.js"></script>
<script type="text/javascript" src="/_layouts/DanFay/jquery-ui-1.8.17.custom.min.js"></script>

<script type="text/javascript">
    $(document).ready(function () {

        $('[id$=btnSaveAll]').click(function () {
            var msgLabel = $("#<%=lblMessage.ClientID%>");
            var $msg = "";

            var title = $('[id$=txtTitle]');
            var visitDate = $('[id$=calVisitDate_calVisitDateDate]');
            var siteNo = $('[id$="ddlSites"]');

            var noScreened = $('[id$=txtNoSceened]');
            var noScreenedFailed = $('[id$=txtNoScreenedFailed]');
            var noRandomized = $('[id$=txtNoRandomized]');
            var noActiveTreatment = $('[id$=txtNoActiveTreatment]');
            var noCompletedTreatment = $('[id$=txtNoCompletedTreatment]');
            var NoDiscontinuation = $('[id$=txtNoDiscontinuationo]');

            var regexp = /^[0-9\\.]+$/;

            var titleRegexp = /^[A-Za-z0-9 _]+$/;
            var numberRegex = /^(0|[1-9][0-9]*)$/;

            visitDate.css('border-color', '');
            title.css('border-color', '');
            siteNo.css('border-color', '');
            noScreened.css('border-color', '');
            noScreenedFailed.css('border-color', '');
            noRandomized.css('border-color', '');
            noActiveTreatment.css('border-color', '');
            noCompletedTreatment.css('border-color', '');
            NoDiscontinuation.css('border-color', '');

            if (title[0].value.length == 0) {
                $msg = $msg + "<br/>Please enter the report title";
                title.css('border-color', 'red');
            }
            else {
                if (!titleRegexp.test(title[0].value)) {
                    $msg = $msg + "<br/>The title cannot have any special characters";
                    title.css('border-color', 'red');
                }
            }

            if (visitDate[0].value.length == 0) {
                $msg = $msg + "<br/>Please select the visit date";
                visitDate.css('border-color', 'red');
            }

            if (siteNo[0].value == "0") {
                $msg = $msg + "<br/>Please select a site number";
                siteNo.css('border-color', 'red');
            }

            if ($msg.length > 0) {
                msgLabel.html($msg).css("color", "red")
                return false;
            }

            //No screened validation
            if (noScreened.val().length > 0) {
                if (!numberRegex.test(noScreened.val())) {
                    $msg = $msg + "<br/>The number screened must be numeric";
                    noScreened.css('border-color', 'red');
                }
            }

            //No screened failed validation
            if (noScreenedFailed.val().length > 0) {
                if (!numberRegex.test(noScreenedFailed.val())) {
                    $msg = $msg + "<br/>The number screened failed must be numeric";
                    noScreenedFailed.css('border-color', 'red');
                }
            }

            //No Randomized validation
            if (noRandomized.val().length > 0) {
                if (!numberRegex.test(noRandomized.val())) {
                    $msg = $msg + "<br/>The randamized must be numeric";
                    noRandomized.css('border-color', 'red');
                }
            }

            //No Active Treament validation
            if (noActiveTreatment.val().length > 0) {
                if (!numberRegex.test(noActiveTreatment.val())) {
                    $msg = $msg + "<br/>The no active treatment must be numeric";
                    noActiveTreatment.css('border-color', 'red');
                }
            }

            //No Completed Treament validation
            if (noCompletedTreatment.val().length > 0) {
                if (!numberRegex.test(noCompletedTreatment.val())) {
                    $msg = $msg + "<br/>The completed treatment must be numeric";
                    noCompletedTreatment.css('border-color', 'red');
                }
            }

            //No Disontinuation validation
            if (NoDiscontinuation.val().length > 0) {
                if (!numberRegex.test(NoDiscontinuation.val())) {
                    $msg = $msg + "<br/>The early discontinuation must be numeric";
                    NoDiscontinuation.css('border-color', 'red');
                }
            }

            if ($msg.length > 0) {
                msgLabel.html($msg).css("color", "red")
                return false;
            }
            else
                return true;
        });
    });

</script>
 
 <asp:HiddenField ID="hidSelectedId" runat="server" />
 <asp:HiddenField ID="hidFormMode" runat="server" />
 <asp:HiddenField ID="hidSiteId" runat="server" />

 <div style="padding-left:2px">
 
     <!-- Heading -->
    <table id="tblMain" cellpadding="2" cellspacing="2" width="100%">
    <tr>
         <td class="columnHeader">Report Title:</td>
         <td colspan="3"><asp:TextBox ID="txtTitle" runat="server" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr>
        <td class="columnHeader">Sponsor:</td>
        <td style="width:40%"><asp:TextBox ID="txtSponsor" runat="server" Width="98%" CssClass="columnText" /></td>
        <td class=""><label class="columnHeader">Protocol #:</label></td>
        <td><asp:TextBox ID="txtProtocol" runat="server" Width="98%" CssClass="columnText"  /></td>
    </tr>
    <tr>
        <td class=""><label class="columnHeader">Visit Date:</label></td>
        <td ><SharePoint:DateTimeControl ID="calVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
        <td class="columnHeader">Monitor:</td>
        <td><SharePoint:PeopleEditor ID="speMonitor" runat="server" MultiSelect="false" AllowEmpty="false" ValidatorEnabled="true" CssClass="columnText"
                                    BorderWidth="1" PlaceButtonsUnderEntityEditor="false" SelectionSet="User" BorderColor="#BEBEBE" Width="99%" /></td>
    </tr>
   <%-- <tr>
        <td><label class="columnHeader">Date of Next Visit:</label></td>
        <td colspan="3"><SharePoint:DateTimeControl ID="calNextVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
    </tr>--%>
    <tr>
        <td class=""><label class="columnHeader">Address:</label></td>
        <td><SharePoint:InputFormTextBox ID="txtAddress" runat="server" RichText="false" TextMode="MultiLine" Rows="3" Width="98%" CssClass="columnText"  /></td>
<%--        <td><asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="3" Width="98%" CssClass="columnText" /></td>
--%>         <td class="columnHeader">Status</td>
        <td>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="columnText" Width="99%">
                <asp:ListItem Text="Incomplete" Value="Incomplete" />
                <asp:ListItem Text="Ready for Review" Value="Ready for Review" />
                <asp:ListItem Text="Ready for Signatures" Value="Ready for Signatures" />
                <asp:ListItem Text="Complete / Verified" Value="Complete / Verified" />
            </asp:DropDownList>
         </td>
    </tr>
    <tr>
        <td class="columnHeader">Version</td>
        <td><asp:DropDownList id="ddlVersion" runat="server" CssClass="columnText" /></td>
        <td class="columnHeader">Version Date:</td>
           <td><SharePoint:DateTimeControl ID="calVersionDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
     </tr>
    <tr>
        <td><label class="columnHeader">Miscellaneous Comments:</label></td>
        <td><asp:TextBox ID="txtMiscComments" runat="server" TextMode="MultiLine" Rows="2" Width="99%" CssClass="columnText" /></td>
        <td class="columnHeader">Site Number:</td>
        <td><asp:DropDownList ID="ddlSites" runat="server" /></td>
    </tr>
    <tr>
        <td><label class="columnHeader">General Reviewer Comments:</label></td>
        <td colspan="3"><asp:TextBox ID="txtGenRevComments" runat="server" TextMode="MultiLine" Rows="4" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr>
         <td style="text-align:right" class=""></td>
         <td colspan="3" style="text-align:right" class="">
          <%--  <asp:HyperLink ID="hplSiteOverView" runat="server" Text="Site Overview" />&nbsp;--%>
            <asp:LinkButton ID="btnSiteOverview" runat="server" Text="Site Overview" onclick="btnSiteOverview_Click"  />&nbsp;
            <asp:LinkButton ID="btnPDFGenerator" runat="server" Text="Generate PDF" onclick="btnPDFGenerator_Click"></asp:LinkButton>
        </td>
    </tr>
     <tr>
        <td colspan="4"style="text-align:right"><asp:Button id="btnSaveAll" runat="server" Text="Save" onclick="btnSaveAll_Click"  /></td>
    </tr>
    <tr>
        <td colspan="4" class="messageLabel"><asp:Label ID="lblMessage" runat="server" ForeColor="Red" /></td>
    </tr>
    </table>

     <br />
     <div style="width:100%">
        <asp:Menu ID="formMenu" OnMenuItemClick="formMenu_MenuItemClick" 
             Orientation="Horizontal" runat="server" Width="100%"
             StaticMenuItemStyle-cssClass="StaticMenuItemStyle"
             StaticMenuStyle-CssClass="StaticMenuStyle"
             DynamicHorizontalOffset="2" > 
           
         <Items>
         <asp:MenuItem Text="Visit Attendees" Value="0" Selected="true"></asp:MenuItem>
         <asp:MenuItem Text="Subject Recruitment" Value="1" ></asp:MenuItem>
         <asp:MenuItem Text="Investigator Site File" Value="2"></asp:MenuItem>
         <asp:MenuItem Text="Informed Consent" Value="3"></asp:MenuItem>
         <asp:MenuItem Text="Adverse Events" Value="4"></asp:MenuItem>
         <asp:MenuItem Text="Investigational Product" Value="5"></asp:MenuItem>
         <asp:MenuItem Text="Trial Material" Value="6"></asp:MenuItem>
         <asp:MenuItem Text="Investigator Discussion" Value="7"></asp:MenuItem>
        </Items>  
        <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 
    </asp:Menu>
    </div>
    <br />

    <asp:MultiView ID="formMultiView" runat="server" ActiveViewIndex="0">
         <!-- Visit Attendees -->
        <asp:View ID="viewVisitAttendees" runat="server">
        <br />
        <table cellpadding="2" cellspacing="2" width="100%" border="0">
        <tr>
	        <td class="subcategory" colspan="3">CLINICAL SITE PERSONNEL:</td>
        </tr>
        <tr>
	        <td class="columnHeading">Investigator Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
	        <td style="width:50%"><asp:TextBox ID="txtInvestigatorName" runat="server" Width="100%" CssClass="columnText" /></td>
	        <td colspan="2"><asp:TextBox ID="txtInvestigatorTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnHeading">Site Participant Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName" runat="server" Width="100%"  CssClass="columnText"/></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName2" runat="server" Width="100%" CssClass="columnText" /></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle2" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName3" runat="server" Width="100%" CssClass="columnText" /></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle3" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">INC RESEARCH MONITORS:</td>
        </tr>
        <tr>
	        <td class="columnHeading">Monitor Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
	        <td><asp:TextBox ID="txtMonitorName" runat="server" Width="100%" CssClass="columnText" /></td>
	        <td colspan="2"><asp:TextBox ID="txtMonitorTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td colspan="2">Were any additional Site Personnel met with at this visit? If so, provide name and role in comments below.</td>
	         <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlOtherSitePersonnel" runat="server">
                      <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                      <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                      <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
	        <td colspan="3" class="comments"><asp:TextBox ID="txtOtherSitePersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">OTHER INC RESEARCH AND/OR SPONSOR PERSONNEL:</td>
        </tr>
        <tr>
	        <td colspan="2" class="question">
	        Were any other INC or Sponsor Personnel present at this visit? If so, provide their name, company affiliation and purpose for attending this visit in comments below:
	        </td>
	        <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlOtherPersonnel" runat="server">
                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
	        <td colspan="3" class="comments"><asp:TextBox ID="txtPersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
          <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="3" class="columnText"><asp:TextBox ID="txtVAReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>
    </asp:View>

         <!-- Subect /Status Recruitment -->
        <asp:View ID="viewSubjectRecruitment" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
            <td class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td class="columnText"><asp:TextBox ID="txtSRReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
        <br />
        <table cellpadding="2" cellspacing="2" width="100%">
            <tr>
                <td class="columnHeading">Confirmed</td><td class="columnHeading">Number of Subjects</td>
            </tr>
            <tr>
                <td style="width:20%">Screened</td><td><asp:TextBox ID="txtNoSceened" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
                <td>Screened Failed</td><td><asp:TextBox ID="txtNoScreenedFailed" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
                <td>Randomized</td><td><asp:TextBox ID="txtNoRandomized" runat="server" CssClass="columnText" /></td>
           </tr>
            <tr>
                <td>Active Treatment</td><td><asp:TextBox ID="txtNoActiveTreatment" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
                <td>Completed Treatment per Protocol</td><td><asp:TextBox ID="txtNoCompletedTreatment" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
                <td>Early Discontinuation</td><td><asp:TextBox ID="txtNoDiscontinuationo" runat="server" CssClass="columnText" /></td>
            </tr>
         </table>

        </asp:View>

        <!-- Investigator Site File -->
        <asp:View ID="viewCSection" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	            <td class="columnText">Is the ISF complete according to the tracking tool use for the study?</td>
                <td class="columnText">
                        <asp:DropDownList id="ddlC1" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC1Comments" TextMode="MultiLine" Rows="3" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>

        <tr>
	            <td class="columnText" style="width:97%">Are all study specific logs completed?</td>
	            <td class="columnText">
                        <asp:DropDownList id="ddlC2" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>

        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC2Comments" Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>

        <tr>
	            <td class="columnText" style="width:97%">
                    Have all required documents been signed and is a copy/ original available in the Investigator Site File or INC Research TMF respectively?
                </td>
	            <td class="columnText">
                        <asp:DropDownList id="ddlC3" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>

        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC3Comments" Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>

        <tr>
            <td class="columnText" style="width:97%">
                        Have copies of appropriate completed and signed Site logs been collected and sent to INC Research for inclusion in the TMF?
            </td>
	        <td class="columnText">
                <asp:DropDownList id="ddlC4" runat="server">
                     <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                      <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                      <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC4Comments"  Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>

         <tr>
            <td class="columnText" style="width:97%">
                        Has the Investigator Correspondence been verified for the duration of the trial? If yes, please provide the end date of verfification process.
            </td>
	        <td class="columnText">
                <asp:DropDownList id="ddlC5" runat="server">
                     <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                     <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                     <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC5Comments" Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>

        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtCReviewerComments" Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>

        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	        <td style="width:40%;text-align:center;"><b>Document/Section</b></td>
	        <td style="width:20%;text-align:center;"><b>Date last Checked</b></td>
	     <td style="width:40%;text-align:center;"><b>Comments</b></td>
        </tr>
        <tr>
	        <td>Site Visit / Site Delegation Logs</td>
	        <td><SharePoint:DateTimeControl ID="calSiteVisitLogDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText"  /></td>
	        <td><asp:TextBox ID="txtSiteVisitLogComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Protocol, Amendments, Signature Pages</td>
            <td><SharePoint:DateTimeControl ID="calProtocolDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtProtocolComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Investigator Brochure / Package Insert</td>
            <td><SharePoint:DateTimeControl ID="calBrochureDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtBrochureComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Statement of Investigator (1572)/Financial Disclosure Forms</td>
            <td><SharePoint:DateTimeControl ID="calDisclosureDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtDisclosureComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>CV / License</td>
            <td><SharePoint:DateTimeControl ID="calLicenseDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtLicenseComments" TextMode="MultiLine" Rows="2"  Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>IRB/IEC, Regulatory Documents</td>
            <td><SharePoint:DateTimeControl id="calRegulatoryDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtRegulatoryComments" TextMode="MultiLine" Rows="2"  Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Approved Informed Consent Versions</td>
            <td><SharePoint:DateTimeControl ID="calVersionsDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtVersionsComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Lab Certification / Reference Ranges</td>
            <td><SharePoint:DateTimeControl ID="calCertificationDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtCertificationComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Correspondence</td>
            <td><SharePoint:DateTimeControl ID="calCorrespondenceDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtCorrespondenceComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
        <tr>
	        <td>Miscellaneous</td>
            <td><SharePoint:DateTimeControl ID="calMiscellaneousDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
            <td><asp:TextBox ID="txtMiscellaneousComments" TextMode="MultiLine" Rows="2" Width="99.5%" runat="server" /></td>
        </tr>
    </table>
 </asp:View>

        <!-- D - Informed Consent -->
        <asp:View ID="viewInvBrochure" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Are signed and dated informed consents present for all Subjects screened/enrolled in the study?</td>
	        <td class="columnText"><asp:DropDownList id="ddlD1" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
       
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtDReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
        </asp:View>

        <!-- F - Adverse Events -->
        <asp:View ID="viewEnrollment" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	        <td class="columnText" style="width:97%"> Have all SAEs been resolved and documented appropriately in the source, on SAE Forms, CRFs and filed as directed by the Sponsor?</td>
	        <td class="columnText"><asp:DropDownList id="ddlF1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>

        <tr>
	        <td class="columnText">
	           Have all SAEs been reported to the Sponsor, the IRB / IEC and to the Regulatory Authority in accordance with any local requirements?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlF2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtFReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- G Investigational Product -->
        <asp:View ID="viewIRB" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Are IP accountability logs complete?</td>
	        <td class="columnText"><asp:DropDownList id="ddlG1" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Has the quantity of IP received on Site been accounted for and documented on the Return Product Form? Discrepancies to be documented in the Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlG2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Have copies of all IP shipping and receipt documents been retained in the ISF and sent to INC Research for inclusion in the TMF?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlG3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Has all IP been returned?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlG4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         If locally destroyed, verify Site has documentation of the process and that it is in compliance with Sponsor requirements. Provide details in the Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlG5" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         If applicable, have all randomization code breaks been returned?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlG6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3"  CssClass="columnText"/></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtGReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- H - Other Trial Material -->
        <asp:View ID="viewInformedConsent" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">

        <tr>
	        <td class="columnText" style="width:97%">Have all unused laboratory supplies been returned or disposed of as appropriate?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
          <tr>
	        <td class="columnText" style="width:97%">Have all biological specimens been shipped according to protocol and regulatory requirements?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
          <tr>
	        <td class="columnText" style="width:97%">Have all unused Case Report Forms been returned or disposed of as appropriate?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH3" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
          <tr>
	        <td class="columnText" style="width:97%">Have all unused miscellaneous supplies been returned or disposed of as appropriate?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtHReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- I -Investigator Discussion -->
        <asp:View ID="viewAdverseEvent" runat="server">
        <table cellpadding="2" cellspacing="2" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Has the Investigator been informed of Regulatory and Sponsor requirements for record retention, publication of study data and the potential for regulatory inspections?</td>
	        <td class="columnText"><asp:DropDownList id="ddlI1" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           Was the Investigator advised to notify the Sponsor in the event of relocation, retirement or any changes in off-Site document storage?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI2" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Has the final study report to the IRB / IEC been submitted, is a copy on file in the ISF and has a copy been submitted in-house for filing in the TMF?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlI3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was the Investigator advised of the need to complete and update Financial Disclosure information if it changes up to 1 year following study completion?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtIReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

    </asp:MultiView>
 </div>