<%@ Page Title="Contacto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Galileo.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>
        <asp:Image ID="Image1" runat="server" Height="130px" ImageUrl="~/Imagen/logoaecapl.png" Width="274px" />
</h3>
    <address>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.aec.mx" Target="_blank">www.aec.mx</asp:HyperLink>
    </address>

    <address>
        <strong>Soporte:</strong>   <a href="mailto:aponcedeleon51@hotmail.com">aponcedeleon51@hotmail.com</a><br />
        <strong>Mercadotecnia:</strong> <a href="mailto:a.ponce@aec.mx">a.ponce@aec.mx</a>
    </address>

</asp:Content>
