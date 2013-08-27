<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ESRI_Maps.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stationery List</title>
</head>


<body>

    <h2>Stationery List:</h2>

    <form id="frmList" runat="server">
        <table border="1" cellpadding="10">
            <thead>
                <tr>
                    <th>Asset ID:</th>
                    <th>Title:</th>
                    <th>Address:</th>
                    <th>Map View:</th>
                </tr>
            </thead>
            <asp:Repeater ID="rptAssetList" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "AssetID") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Title") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Address") %></td>
                        <td><a href="/ViewAsset.aspx?AssetID=<%# DataBinder.Eval(Container.DataItem, "AssetID") %>">Map View</a></td>                
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </form>

    <br />

    <a href="ViewAllAssets.aspx">View All on the Map</a>
</body>
</html>