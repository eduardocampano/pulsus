<%@ Assembly Name="Pulsus.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=80e8440141debbd5" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages.Administration, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="AdminControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint.ApplicationPages.Administration" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" Src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" Src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" Src="~/_controltemplates/ButtonSection.ascx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PulsusSettings.aspx.cs" Inherits="Pulsus.SharePoint.ApplicationPages.PulsusSettings" MasterPageFile="~/_admin/admin.master" %>

<asp:Content ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server" Text="Pulsus Settings" EncodeMethod='HtmlEncode' />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	<SharePoint:EncodedLiteral runat="server" Text="Pulsus Settings" EncodeMethod='HtmlEncode' />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageDescription" runat="server">
	<SharePoint:EncodedLiteral Text="Use this page to configure Pulsus Error Logging" runat="server" EncodeMethod='HtmlEncodeAllowSimpleTextFormatting' />
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<table width="100%" class="propertysheet" cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td class="ms-descriptionText">
				<asp:Label ID="LabelMessage" runat="server" EnableViewState="False" class="ms-descriptionText" />
			</td>
		</tr>
		<tr>
			<td class="ms-error">
				<asp:Label ID="LabelErrorMessage" runat="server" EnableViewState="False" /></td>
		</tr>
		<tr>
			<td class="ms-descriptionText">
				<asp:ValidationSummary ID="ValSummary" HeaderText="<%$SPHtmlEncodedResources:spadmin, ValidationSummaryHeaderText%>" DisplayMode="BulletList" ShowSummary="True" runat="server"></asp:ValidationSummary>
			</td>
		</tr>
	</table>
	<input type="hidden" id="HidVirtualServerUrl" runat="server">
	<table border="0" cellspacing="0" cellpadding="0" width="100%">
		<wssuc:InputFormSection runat="server" Id="idWebApplicationSelectorSection"
			Title="<%$Resources:spadmin, multipages_webapplication_title%>"
			Description="<%$Resources:spadmin, multipages_webapplication_desc%>">
			<template_inputformcontrols>
			<tr><td>
			<SharePoint:WebApplicationSelector id="Selector" runat="server" OnContextChange="Selector_ContextChange" />
			</td></tr>
		</template_inputformcontrols>
		</wssuc:InputFormSection>
		<wssuc:InputFormSection runat="server" Id="InputFormSection1"
			Title="ULS Target Settings"
			Description="">
			<template_inputformcontrols>
				<tr>
					<td>
						<asp:CheckBox ID="enableULSLogging" runat="server" Text="Enable ULS Logging" />
					</td>
				</tr>
			</template_inputformcontrols>
		</wssuc:InputFormSection>
		<wssuc:InputFormSection runat="server" Id="emailSettingsSection"
			Title="Email Target Settings"
			Description="">
			<template_inputformcontrols>
				<tr>
					<td>
						<asp:Label ID="emailTargetFromAddressLabel" runat="server" Text="From:" AssociatedControlID="emailTargetFromAddress"></asp:Label><br />
						<asp:TextBox ID="emailTargetFromAddress" runat="server"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="emailTargetToAddressLabel" runat="server" Text="To:" AssociatedControlID="emailTargetToAddress"></asp:Label><br />
						<asp:TextBox ID="emailTargetToAddress" runat="server"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="emailTargetServerLabel" runat="server" Text="SMTP Server:" AssociatedControlID="emailTargetServer"></asp:Label><br />
						<asp:TextBox ID="emailTargetServer" runat="server"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="emailTargetServerPortLabel" runat="server" Text="Port:" AssociatedControlID="emailTargetServerPort"></asp:Label><br />
						<asp:TextBox ID="emailTargetServerPort" runat="server"></asp:TextBox>
					</td>
				</tr>
			</template_inputformcontrols>
		</wssuc:InputFormSection>
		<wssuc:InputFormSection runat="server" Id="secureStoreDatabaseTargetSettings"
			Title="Secure Store Database Target Settings"
			Description="">
			<template_inputformcontrols>
				<tr>
					<td>
						<asp:Label ID="targetApplicationIdLabel" runat="server" Text="Target Application:" AssociatedControlID="targetApplicationId"></asp:Label><br />
						<asp:TextBox ID="targetApplicationId" runat="server"></asp:TextBox>
					</td>
				</tr>
			</template_inputformcontrols>
		</wssuc:InputFormSection>
		<tr>
			<td height="10px" class="ms-descriptiontext" colspan="2">
				<img src="/_layouts/images/blank.gif" width='1' height='10' alt="" /></td>
		</tr>
		<tr>
			<td colspan="2">
				<table cellpadding="0" cellspacing="0" width="100%">
					<colgroup>
						<col width="99%" />
						<col width="1%" />
					</colgroup>
					<tr>
						<td>&#160;</td>
						<td nowrap="nowrap">
							<asp:Button ID="okButton" runat="server" Text="<%$Resources:spadmin, multipages_okbutton_text%>" OnClick="okButton_Click" RequireValidation="false" class="ms-ButtonHeightWidth" CausesValidation="False" /></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Content>
