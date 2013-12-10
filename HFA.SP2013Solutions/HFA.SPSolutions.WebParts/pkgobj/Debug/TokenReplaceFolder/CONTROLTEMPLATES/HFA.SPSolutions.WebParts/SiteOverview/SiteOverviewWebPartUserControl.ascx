<%@ Assembly Name="HFA.SPSolutions.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8c2017e68e4445f2" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SiteOverviewWebPartUserControl.ascx.cs" Inherits="HFA.SPSolutions.WebParts.SiteOverview.SiteOverviewWebPartUserControl" %>

<SharePoint:CssRegistration ID="defaultCSS" Name="/Style Library/default.css" runat="server" After="corev4.css" />

<br /><br />
<div style="padding-left:5px">
    <table class="ms-WPBody">
        <tr>
            <td style="vertical-align:top;">
                <br />
                <table id="tblMain" cellpadding="5" cellspacing="5" width="620px">
                    <tr>
                        <td class="columnHeader" style="width:140px;">Site Number:</td>
                        <td style="white-space:normal;"><asp:Label ID="lblSiteNo" runat="server" CssClass="columnText" /></td>
                    </tr>
                    <tr>
                        <td class="columnHeader" style="width:140px;">Site Title:</td>
                        <td style="white-space:normal;"><asp:Label ID="lblSiteTitle" runat="server" CssClass="columnText" /></td>
                    </tr>
                    <tr>
                        <td class="columnHeader">Investigator Name:</td>
                        <td><asp:Label ID="lblInvestigatorName" runat="server" CssClass="columnText" /></td>
                    </tr>
                    <tr>
                        <td class="columnHeader">Phone Number:</td>
                        <td><asp:Label ID="lblPhoneNo" runat="server" CssClass="columnText" /></td>
                    </tr>
                    <tr>
                        <td class="columnHeader" style="vertical-align:top;">Address:</td>
                        <td colspan="3">
                            <SharePoint:InputFormTextBox ID="lblAddress" TextMode="MultiLine" RichText="false" Rows="5" runat="server" Width="80%" CssClass="addressField" />
                        </td>
                    </tr>
               </table>
           </td>
            <td style="vertical-align:top;">
                <asp:Menu
                    ID="formMenu" runat="server"  OnMenuItemClick="formMenu_MenuItemClick" Width="600px"
                    StaticEnableDefaultPopOutImage="False" DynamicMenuItemStyle-VerticalPadding="0px" 
                    Orientation="Horizontal" StaticMenuItemStyle-cssClass="StaticMenuItemStyle"
                    StaticMenuStyle-CssClass="StaticMenuStyle" DynamicHorizontalOffset="2" > 
                    <Items>
                        <asp:MenuItem Text="IMV Report" Value="0" Selected="true"></asp:MenuItem>
                        <asp:MenuItem Text="SSV Report" Value="1"></asp:MenuItem>
                        <asp:MenuItem Text="SIV Report" Value="2"></asp:MenuItem>
                        <asp:MenuItem Text="COV Report" Value="3"></asp:MenuItem>
                    </Items>
                    <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 
                </asp:Menu>
                <br />
                <asp:MultiView ID="formMultiView" runat="server" ActiveViewIndex="0">
                <asp:View ID="viewIMV" runat="server">
            <!-- IMV Report -->
            <div>
                <asp:GridView ID="gdvIMV" runat="server" CellPadding="5" CellSpacing="5" AutoGenerateColumns="false" 
                                HeaderStyle-CssClass="gridViewHeaderStyle"  GridLines="None"  CssClass="gridView"
                                OnRowDataBound="gdvIMV_RowDataBound" AllowPaging="true" PageSize="5"
                                OnPageIndexChanging="gdvIMV_PageIndexChanging" Width="600px"
                                DataKeyNames="ID" OnRowCommand="gdvIMV_OnRowCommand" OnRowDeleting="gdvIMV_RowDeleting">
                  <Columns>
                        <asp:BoundField DataField="ID" Visible="false" />
                        <asp:BoundField DataField="Title" ConvertEmptyStringToNull="true" HeaderText="Title" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Status" ConvertEmptyStringToNull="true"  HeaderText="Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Visit Date" ConvertEmptyStringToNull="true" HeaderText="Visit Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Monitor" ConvertEmptyStringToNull="true" HeaderText="Monitor" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" DeleteText="Delete" ShowDeleteButton="true" HeaderText=" " />
                    </Columns>
               </asp:GridView>
               <br />
               <img src="_layouts/images/HFA/plus.png" alt="" /> <asp:HyperLink ID="hplNewIMV" runat="server" Text="Add new item" ></asp:HyperLink>
               <br />               
            </div>
        </asp:View>

        <asp:View ID="viewSSV" runat="server">
                    <!-- SSV Report  -->
                    <div>
                        <asp:GridView ID="gdvSSV" runat="server" CellPadding="5" CellSpacing="5" AutoGenerateColumns="false" 
                                HeaderStyle-CssClass="gridViewHeaderStyle"  GridLines="None"  CssClass="gridView"
                                OnRowDataBound="gdvSSV_RowDataBound" AllowPaging="true" PageSize="5"
                                OnPageIndexChanging="gdvSSV_PageIndexChanging" Width="600px"
                                DataKeyNames="ID" OnRowCommand="gdvSSV_OnRowCommand" OnRowDeleting="gdvSSV_RowDeleting">
                  <Columns>
                        <asp:BoundField DataField="ID" Visible="false" />
                        <asp:BoundField DataField="Title" ConvertEmptyStringToNull="true" HeaderText="Title" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Status" ConvertEmptyStringToNull="true"  HeaderText="Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Visit Date" ConvertEmptyStringToNull="true" HeaderText="Visit Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Monitor" ConvertEmptyStringToNull="true" HeaderText="Monitor" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" DeleteText="Delete" ShowDeleteButton="true" HeaderText=" " />
                    </Columns>
               </asp:GridView>
                      <br />
                         <img src="_layouts/images/HFA/plus.png" alt="" /> <asp:HyperLink ID="hplNewSSV" runat="server" Text="Add new item" ></asp:HyperLink>
                    </div>
                </asp:View>

        <asp:View ID="viewSIV" runat="server">
            <!-- SIV Report -->
            <div>
                <asp:GridView ID="gdvSIV" runat="server" CellPadding="5" CellSpacing="5" AutoGenerateColumns="false" 
                                HeaderStyle-CssClass="gridViewHeaderStyle"  GridLines="None"  CssClass="gridView"
                                OnRowDataBound="gdvSIV_RowDataBound" AllowPaging="true" PageSize="5" 
                                OnPageIndexChanging="gdvSIV_PageIndexChanging" Width="600px"
                                DataKeyNames="ID" OnRowCommand="gdvSIV_OnRowCommand" OnRowDeleting="gdvSIV_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="ID" Visible="false" />
                        <asp:BoundField DataField="Title" ConvertEmptyStringToNull="true" HeaderText="Title" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Status" ConvertEmptyStringToNull="true"  HeaderText="Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Visit Date" ConvertEmptyStringToNull="true" HeaderText="Visit Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="Monitor" ConvertEmptyStringToNull="true" HeaderText="Monitor" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                        <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" DeleteText="Delete" ShowDeleteButton="true" HeaderText=" " />
                    </Columns>
                </asp:GridView>
                <br />
                <img src="_layouts/images/HFA/plus.png" alt="" /> <asp:HyperLink ID="hplNewSIV" runat="server" Text="Add new item" ></asp:HyperLink>
            </div>
        </asp:View>

        <asp:View ID="viewCOV" runat="server">
         <!-- COV Report-->
            <div>
                <asp:GridView ID="gdvCOV" runat="server" CellPadding="5" CellSpacing="5" AutoGenerateColumns="false"  
                               HeaderStyle-CssClass="gridViewHeaderStyle"  GridLines="None"  CssClass="gridView"
                               OnRowDataBound="gdvCOV_RowDataBound" Width="600px" DataKeyNames="ID" OnRowCommand="gdvCOV_OnRowCommand"
                               AllowPaging="true" PageSize="5" OnPageIndexChanging="gdvCOV_PageIndexChanging"
                               OnRowDeleting = "gdvCOV_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="ID" Visible="false" />
                    <asp:BoundField DataField="Title" ConvertEmptyStringToNull="true" HeaderText="Title" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                    <asp:BoundField DataField="Status" ConvertEmptyStringToNull="true"  HeaderText="Status" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="Visit Date" ConvertEmptyStringToNull="true" HeaderText="Visit Date" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px" />
                    <asp:BoundField DataField="Monitor" ConvertEmptyStringToNull="true" HeaderText="Monitor" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" />
                   <asp:CommandField ButtonType="Link" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" DeleteText="Delete" ShowDeleteButton="true" HeaderText=" " />
                </Columns>
                </asp:GridView>
                               
             <br />
             <img src="_layouts/images/HFA/plus.png" alt="" /> <asp:HyperLink ID="hplNewCOV" runat="server" Text="Add new item" ></asp:HyperLink>
            </div>
        </asp:View>
</asp:MultiView>
            </td>
        </tr>
    </table>
</div>
