<%@ Assembly Name="HFA.SPSolutions.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8c2017e68e4445f2" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageIMVReportUserControl.ascx.cs" Inherits="HFA.SPSolutions.WebParts.ManageIMVReport.ManageIMVReportUserControl"  %>

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

        $('[id$=btnSaveSAT]').click(function () {
            var msgLabelSAT = $("#<%=lblMessageSAT.ClientID%>");
            var $msg = "";
            var subjectID = $('[id$="ddlSubjectID"]');

            subjectID.css('border-color', '');

            if (subjectID[0].value == "0") {
                $msg = $msg + "<br/>Please select a Subject ID";
                subjectID.css('border-color', 'red');
            }

            if ($msg.length > 0) {
                msgLabelSAT.html($msg).css("color", "red")
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
 <asp:HiddenField ID="hidSiteVisitLogDate" runat="server" />

 <div style="padding-left:2px">
 
    <!-- Heading -->
    <table id="tblMain" cellpadding="5" cellspacing="5" width="100%">
    <tr>
         <td class="columnHeader">Report Title:</td>
         <td colspan="3"><asp:TextBox ID="txtTitle" runat="server" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr>
        <td class="columnHeader">Sponsor:</td>
        <td style="width:40%"><asp:TextBox ID="txtSponsor" runat="server" Width="98%" CssClass="columnText" /></td>
        <td class=""><label class="columnHeader">Protocol #:</label></td>
        <td><asp:TextBox ID="txtProtocol" runat="server" Width="98%" CssClass="columnText" /></td>
    </tr>
    <tr>
        <td class=""><label class="columnHeader">Visit Date:</label></td>
        <td ><SharePoint:DateTimeControl ID="calVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
        <td class="columnHeader">Monitor:</td>
        <td><SharePoint:PeopleEditor ID="speMonitor" runat="server" MultiSelect="false" AllowEmpty="false" ValidatorEnabled="true" CssClassTextBox="columnText"
                                    BorderWidth="1" PlaceButtonsUnderEntityEditor="false" SelectionSet="User" BorderColor="#BEBEBE" Width="99%" /></td>
    </tr>
     <tr>
        <td><label class="columnHeader">Last Date of Visit:</label></td>
        <td colspan="3"><SharePoint:DateTimeControl ID="calLastDayOfVisit" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
    </tr>
    <tr>
        <td><label class="columnHeader">Date of Next Visit:</label></td>
        <td colspan="3"><SharePoint:DateTimeControl ID="calNextVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
    </tr>
    <tr>
        <td class=""><label class="columnHeader">Address:</label></td>
        <td colspan="3"><SharePoint:InputFormTextBox ID="txtAddress" runat="server" RichText="false" TextMode="MultiLine" Rows="3" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr>
        <td class="columnHeader">Status</td>
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
          <td><asp:DropDownList id="ddlVersion" runat="server"  CssClass="columnText" /></td>
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
            <asp:LinkButton ID="btnSiteOverview" runat="server" Text="Site Overview" onclick="btnSiteOverview_Click"  />&nbsp;
         <%--   <asp:HyperLink ID="hplSiteOverView" runat="server" Text="Site Overview"  />&nbsp;--%>
            <asp:LinkButton ID="btnPDFGenerator" runat="server" Text="Generate PDF" 
                 onclick="btnPDFGenerator_Click"></asp:LinkButton>
        </td>
    </tr>
    <tr>
    <td colspan="4" style="text-align:right">
         <asp:Button id="btnSaveAll" runat="server" Text="Save" onclick="btnSaveAll_Click"  />
    </td>
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
         <asp:MenuItem Text="Subject Recruitment" Value="1"></asp:MenuItem>
         <asp:MenuItem Text="Informed Consent" Value="2"></asp:MenuItem>
         <asp:MenuItem Text="Documentation" Value="3"></asp:MenuItem>
         <asp:MenuItem Text="Adverse Events" Value="4"></asp:MenuItem>
         <asp:MenuItem Text="Site File" Value="5"></asp:MenuItem>
         <asp:MenuItem Text="Supplies" Value="6"></asp:MenuItem>
         <asp:MenuItem Text="Laboratory" Value="7"></asp:MenuItem>
         <asp:MenuItem Text="Site Staff Changes" Value="8"></asp:MenuItem>
         <asp:MenuItem Text="Site Acceptability" Value="9"></asp:MenuItem>
         <asp:MenuItem Text="Discussion" Value="10"></asp:MenuItem>
        </Items>  
        <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 
    </asp:Menu>
    </div>
    <br />

    <asp:MultiView ID="formMultiView" runat="server" ActiveViewIndex="0">
   
         <!-- Visit Attendees -->
        <asp:View ID="viewVisitAttendees" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%" border="0">
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
             <td><asp:TextBox ID="txtSitePersonnelName" runat="server" Width="100%" CssClass="columnText" /></td>
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
                <asp:DropDownList id="ddlSitePersonnel" runat="server">
                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
       <tr>
	        <td colspan="3"><asp:TextBox ID="txtSitePersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3"  CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">OTHER INC RESEARCH AND/OR SPONSOR PERSONNEL:</td>
        </tr>
        <tr>
	        <td colspan="2" class="question">
	        Were any other INC or Sponsor Personnel present at this visit? If so, provide their name, company affiliation and purpose for attending this visit in comments below:
	        </td>
	        <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlPersonnel" runat="server">
                     <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                      <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
          <tr>
	        <td colspan="3"><asp:TextBox ID="txtPersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="3" class="columnText"><asp:TextBox ID="txtVAReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>
    </asp:View>

        <!-- C - Subect /Status Recruitment -->
        <asp:View ID="viewCSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	            <td class="columnText">Were subject recruitment methods / strategies (e.g., advertisements, etc) and recruitment period discussed? </td>
                <td class="columnText">
                        <asp:DropDownList id="ddlC1" runat="server">
                                <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC1Comments" TextMode="MultiLine" Rows="3" runat="server" Width="99.5%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText" style="width:97%">Is the Site on target to meet the enrollment plan for this study?</td>
	        <td class="columnText">
                        <asp:DropDownList id="ddlC2" runat="server">
                           <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC2Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText"/></td>
        </tr>
        <tr>
	            <td class="columnText" style="width:97%">
                    Are there any additional details regarding subject recruitment?
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
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC3Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtCReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
            <tr>
                <td class="columnHeading">Confirmed</td><td class="columnHeading">Number of Subjects</td>
            </tr>
            <tr>
                <td style="width:50%">Screened</td><td><asp:TextBox ID="txtNoSceened" runat="server" CssClass="columnText" /></td>
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

        <!-- D - Informed Consent -->
        <asp:View ID="viewDSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:99%">Have all subjects been properly consented with the correct ICF version?</td>
	        <td class="columnText" style="text-align:right;"><asp:DropDownList id="ddlD1" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="gdvIC" runat="server" Width="1000px" CellPadding="5" CellSpacing="5" AutoGenerateColumns="true" 
                        HeaderStyle-CssClass="gridViewHeaderStyle"  GridLines="None"  CssClass="gridView"  /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtDReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
        </asp:View>

        <!-- E - Documentation -->
        <asp:View ID="viewESection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:100%">Are all required source documents present, up-to-date and complete for all subjects including past medical history to verify inclusion/exclusion criteria?</td>
	        <td class="columnText"><asp:DropDownList id="ddlE1" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Are there any subject related issues that were identified during this visit that should be noted here? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE2" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Have any changes occurred with respect to any Site electronic/computer systems from the previous visit? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	           If so, has an updated Site Assessment for Electronic/Computer Systems (SAES) form been completed and retrieved for filing in the TMF? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE4" runat="server">
                               <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="subcategory" style="line-height:20px;padding-left:5px;width:100%">SUBJECT ACTIVITY TRACKER:</div><br />
                <div id="divGrid" runat="server">
                    <asp:GridView ID="gdvSAT" runat="server" CellPadding="5" CellSpacing="5" AutoGenerateColumns="false" 
                                HeaderStyle-CssClass="gridViewHeaderStyle" GridLines="Both" CssClass="gridView"
                                OnRowDataBound="gdvSAT_RowDataBound" AllowPaging="true" PageSize="20"
                                OnPageIndexChanging="gdvSAT_PageIndexChanging" Width="100%"
                                OnRowEditing= "gdvSAT_RowEditing"
                                OnRowDeleting= "gdvSAT_RowDeleting"
                                DataKeyNames="ID" OnRowCommand="gdvSAT_OnRowCommand">
                        <Columns>
                        <asp:BoundField DataField="ID" Visible="false"  />
                        <asp:BoundField DataField="SubjectID" ConvertEmptyStringToNull="true" HeaderText="Subject ID" />
                        <asp:BoundField DataField="ActivityThisVisit" ConvertEmptyStringToNull="true" HeaderText="Activity This Visit" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" />
                        <%--<asp:BoundField DataField="DroppedFromStudy" ConvertEmptyStringToNull="true"  HeaderText="Dropped From Study" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top" />--%>
                        <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" EditText="Edit" ShowEditButton="true" HeaderText=" " />
                        <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" DeleteText="Delete" ShowDeleteButton="true" HeaderText=" " />
                    </Columns>
                    
                    </asp:GridView>
                    <br />
                    <img src="_layouts/images/HFA/plus.png" alt="" /> 
                    <asp:LinkButton ID="btnAddNew" runat="server" Text="Add new item" onclick="btnAddNew_Click" />
                    <br /><br />
                </div>

                <div id="divDetail" runat="server" visible="false">
                    <table ID="tblDetail" runat="server" cellPadding="3" cellSpacing="3" width="50%" border="0">
                    <tr>
                        <td colspan="4"><label><asp:Label ID="lblHeading" runat="server" Font-Bold="true" /></label></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align:center;width:100%;"><label><asp:Label ID="lblMessageSAT" runat="server" Font-Bold="true" /></label></td>
                    </tr>
                    <tr><td style="height:5px" colspan="4"></td></tr>
                    <tr>
                        <td class="labelColumn" style="width:140px;">Subject ID:</td>
                        <td><asp:DropDownList ID="ddlSubjectID" runat="server" /></td>
                        <td class="labelColumn" style="width:140px;">Report Type:</td>
                        <td><asp:Label ID="lblReprtType" runat="server" Text="IMV" CssClass="columnText" /></td>
                    </tr>
                    <tr>
                        <td class="labelColumn">Activity This Visit:</td>
                        <td colspan="3"><asp:TextBox ID="txtActivityThisVisit" runat="server" TextMode="MultiLine" Rows="3" Width="98%" CssClass="columnText" /></td>
<%--                        <td class="labelColumn">Dropped From Study:</td><td><asp:TextBox ID="txtDroppedFromStudy" TextMode="MultiLine" Rows="3" CssClass="columnText" Width="98%" runat="server"/></td>
--%>                    </tr>
                    <tr style="height:10px;">
                        <td colspan="4"></td>
                    </tr>
                    <tr>
                        <td style="text-align:right" colspan="4">
                            <asp:LinkButton ID="btnCancelSAT" Text="Cancel" runat="server" onclick="btnCancelSAT_Click" />&nbsp;
                            <asp:LinkButton ID="btnSaveSAT" Text="Save" runat="server" onclick="btnSaveSAT_Click" />
                        </td>
                    </tr>
                </table>
            </div>     
            </td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtEReviewerComments" Width="100%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- G - Serious Adverse Events -->
        <asp:View ID="viewGSection" runat="server">
           <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Have all SAEs been resolved and documented appropriately in the source, on the SAE forms and CRFs? </td>
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
	            Have all SAEs been reported to the Sponsor, the IRB/IEC and to the Regulatory Authority in accordance with local requirements and copies filed in the TMF?
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
            <td colspan="2"><asp:GridView ID="gdvSAE" Width="1000px"  HeaderStyle-CssClass="gridViewHeaderStyle" CellPadding="5" CellSpacing="5"  CssClass="gridView"  
                GridLines="None" runat="server"></asp:GridView></td>
        </tr>
        <Br />
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtGReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- H - Investigator Site File -->
        <asp:View ID="viewHSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the ISF reviewed at this visit? If not, explain in comments. </td>
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
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtHReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table> 
    <table cellpadding="5" cellspacing="5" width="100%">
    <tr>
	    <td style="width:40%;text-align:center;"><b>Document/Section</b></td>
	    <td style="width:20%;text-align:center;"><b>Date last Checked</b></td>
	    <td style="width:40%;text-align:center;"><b>Comments</b></td>
    </tr>
    <tr>
	    <td>Site Visit / Site Delegation Logs</td>
	    <td><SharePoint:DateTimeControl ID="calSiteVisitLogDate" runat="server" DateOnly="true" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
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

        <!-- I -IP and Other Supplies -->
        <asp:View ID="viewISection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was overall drug accountability completed at this visit? </td>
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
	         Was individual drug accountability completed at this visit? 
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
	         Are IP accountability records complete and accurate? 
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
	         Is IP storage still acceptable and according to Protocol specifications? 
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
	        <td class="columnText">
	        If required, are temperature/humidity logs available and current? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI5" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Are all Staff responsible for IP inventory, dispensing and drug accountability appropriately trained and listed on the Site Delegation of Authority Log? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI6" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Has the randomization process been followed according to Protocol since the last visit? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI7" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Have any blinded randomization codes been broken since the last visit? If so, provide details in the Comments. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI8" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI8Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Is there a sufficient supply of IP? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI9" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI9Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Are expiration dates (if present on the label/shipping invoice) current? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI10" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI10Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Is inventory of other supplies adequate? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI11" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI11Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Have any IP and/or other supplies been returned to INC Research and/or Sponsor? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI12" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI12Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
             <tr>
	        <td class="columnText">
	         Have copies of appropriate logs been submitted in-house for filing in the TMF? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI13" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI13Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtIReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- J -Laboratory -->
        <asp:View ID="viewJSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Were laboratory supplies checked and found to be sufficient?</td>
	        <td class="columnText"><asp:DropDownList id="ddlJ1" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Are expiration dates on laboratory supplies current? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ2" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
                Is the Site compliant with sample shipments and are copies of laboratory requisitions appropriately filed? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlJ3" runat="server">
                           <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- K -Site Staff Changes -->
        <asp:View ID="viewKSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Have there been any changes to Study Site Personnel? </td>
	        <td class="columnText"><asp:DropDownList id="ddlK1" runat="server">
                               <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText">New Site Personnnel:</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtNSF" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td colspan="2" class="columnText">Discontinued Personnnel:</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtDSP" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           If so, have all new personnel been adequately trained on study-related procedures and is appropriate documentation of the training on file in the ISF? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK2" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Have all new personnel signed the Study Delegation of Authority Log? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlK3" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Has an updated 1572 with CVs, licenses and financial disclosure forms been retrieved and sent inhouse as appropriate? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK4" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtKReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- L -Site Acceptability -->
        <asp:View ID="viewLSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Have there been any changes to facilities or equipment at the Site? If so, provide details in Comments.</td>
	        <td class="columnText"><asp:DropDownList id="ddlL1" runat="server">
                              <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           Do the Investigator, Staff and Site continue to be suitable for the conduct of this Protocol? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlL2" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtLReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- M -Discussion -->
        <asp:View ID="viewMSection" runat="server">
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Were issues identified during this visit reviewed and discussed with the Investigator?</td>
	        <td class="columnText"><asp:DropDownList id="ddlM1" runat="server">
                             <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
      
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtMReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

    </asp:MultiView>
    <br /><br />
    <div style="width:800px;text-align:center">
       
    </div>
 </div>
