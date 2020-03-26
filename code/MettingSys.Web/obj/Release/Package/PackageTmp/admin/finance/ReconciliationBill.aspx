<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReconciliationBill.aspx.cs" Inherits="MettingSys.Web.admin.finance.ReconciliationBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="table-container">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable" style="min-width: 0;">
                <tr style="text-align: right;">
                    <td width="10%">To：</td>
                    <td width="40%" style="text-align: left;">
                        <asp:Label ID="labCustomerName" runat="server"></asp:Label></td>
                    <td width="10%">Tel：</td>
                    <td style="text-align: left;">
                        <asp:Label ID="labCustomerPhone" runat="server"></asp:Label></td>
                </tr>
                <tr style="text-align: right;">
                    <td width="10%">From：</td>
                    <td width="40%" style="text-align: left;">海南快思图商务会展有限公司-<%=manager.real_name %></td>
                    <td width="10%">Tel：</td>
                    <td style="text-align: left;"><%=manager.telephone %></td>
                </tr>
            </table>
        </div>
        <div class="table-container" style="overflow: hidden;">
            <table width="90%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                <tr style="text-align: center;">
                    <th width="10%">订单号</th>
                    <th style="text-align: left;" width="10%">活动名称</th>
                    <th width="10%">订单日期</th>
                    <%--<th width="10%">业务日期</th>--%>
                    <th width="10%" style="text-align: left;">业务性质/明细</th>
                    <th>业务说明</th>
                    <th width="10%">表达式</th>
                    <th width="7%" style="text-align: right;"><%=_type=="True"?"应收":"应付" %></th>
                    <th width="7%" style="text-align: right;"><%=_type=="True"?"已收":"已付" %></th>
                    <th width="7%" style="text-align: right;"><%=_type=="True"?"未收":"未付" %></th>
                    <th width="7%">操作人</th>
                </tr>
                <%=trHtml %>
            </table>
        </div>
        <div class="dRemark" style="float: left;">
            <p>注：以上明细核对有误，请来电告知，若无误，请确认回传！</p>
        </div>
        <div style="position:relative;float:right;line-height:25px;padding-right:20px;width: 255px;">
            <div style="position: absolute;">
                <p>海南快思图商务会展有限公司财务部</p>
                <p><%=DateTime.Now.ToString("yyyy年MM月dd日") %></p>
            </div>
            <img src="../../images/yinzhang.png" />
        </div>
        <div class="line20"></div>
        <div class="pagelist">
            <div class="l-btns">
                <span>显示</span><asp:TextBox ID="txtPageNum" runat="server" CssClass="pagenum" onkeydown="return checkNumber(event);"
                    OnTextChanged="txtPageNum_TextChanged" AutoPostBack="True"></asp:TextBox><span>条/页</span>
            </div>
            <div id="PageContent" runat="server" class="default"></div>
        </div>
    </form>
</body>
</html>
