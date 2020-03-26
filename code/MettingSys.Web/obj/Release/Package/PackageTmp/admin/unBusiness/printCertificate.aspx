<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="printCertificate.aspx.cs" Inherits="MettingSys.Web.admin.unBusiness.printCertificate" %>

<%@ Import Namespace="MettingSys.Common" %>
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
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script language="javascript" src="../../plugins/Lodop/LodopFuncs.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <object id="LODOP" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0" height="0"></object>
    <script type="text/javascript">
        var LODOP = getLodop();
        //$(function () {
        //    if (document.readyState == "complete") {
        //        CheckLodop();
        //    }
        //})
        function PreviewMytable() {
            LODOP.PRINT_INIT("非业务付款凭证");
            //var strStyle = "<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse}</style>"
            //LODOP.NewPageA();
            LODOP.SET_PRINT_STYLE("FontSize", 18);
            //LODOP.ADD_PRINT_HTML(20, "40%", "90%", 20, "<B>非业务付款凭证</B>");
            //LODOP.ADD_PRINT_HTM(20, "40%", "90%", 20, document.getElementById("headDiv").innerHTML);
            //LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            //LODOP.SET_PRINT_STYLEA(0, "FontSize", 25);
            //LODOP.ADD_PRINT_HTM(60, "75%", "90%", 20, document.getElementById("middleDiv").innerHTML);
            //LODOP.SET_PRINT_STYLEA(0, "ItemType", 0);
            //LODOP.ADD_PRINT_HTM(110, 5, "100%", 20, "<B>"+document.getElementById("footDiv").innerHTML+"</B>");
            //LODOP.SET_PRINT_STYLEA(0, "ItemType", 0);
            LODOP.ADD_PRINT_HTM(10, 5, "99%", 328, document.getElementById("printDIV").innerHTML);
            //LODOP.SET_PRINT_STYLEA(0, "Vorient", 3);
            //LODOP.ADD_PRINT_HTM(1, 600, 300, 100, "总页号：<font color='#0000ff' format='ChineseNum'><span tdata='pageNO'>第##页</span>/<span tdata='pageCount'>共##页</span></font>");
            //LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            LODOP.PREVIEW();
        };
        //function CheckLodop() {
        //    var oldVersion = LODOP.Version;
        //    newVerion = "4.0.0.2";
        //    if (oldVersion == null) {
        //        $("#message1").show();
        //        //document.write("<h3><font color='#FF00FF'>打印控件未安装!点击这里<a href='install_lodop.exe'>执行安装</a>,安装后请刷新页面。</font></h3>");
        //        if (navigator.appName == "Netscape") {
        //            $("#message2").show();
        //            //document.write("<h3><font color='#FF00FF'>（Firefox浏览器用户需先点击这里<a href='npActiveX0712SFx31.xpi'>安装运行环境</a>）</font></h3>");
        //        }
        //    } else if (oldVersion < newVerion) {
        //        $("#message3").show();
        //        //document.write("<h3><font color='#FF00FF'>打印控件需要升级!点击这里<a href='install_lodop.exe'>执行升级</a>,升级后请重新进入。</font></h3>");
        //    }
        //}
    </script>
    <style>
        @media print {
            .Noprn {
                display: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <%--<h3 id="message1" style="display: none"><font color='#FF00FF'>打印控件未安装!点击这里<a href="../../plugins/Lodop/CLodop_Setup_for_Win32NT.zip">执行安装</a>,安装后请刷新页面。</font></h3>
        <h3 id="message2" style="display: none"><font color='#FF00FF'>（Firefox浏览器用户需先点击这里<a href="../../plugins/Lodop/npActiveX0712SFx31.xpi">安装运行环境</a>）</font></h3>
        <h3 id="message3" style="display: none"><font color='#FF00FF'>打印控件需要升级!点击这里<a href="../../plugins/Lodop/install_lodop.exe">执行升级</a>,升级后请重新进入。</font></h3>--%>
        <div style="padding: 10px;">
            <div class="table-container" style="text-align: right;">
                <a href="../../plugins/Lodop/CLodop_Setup_for_Win32NT.zip">下载插件</a>
                <input type="button" value="打印" onclick="PreviewMytable()" class="btn Noprn" />
            </div>
            <div class="table-container" id="printDIV" style="text-align: center; line-height: 24px;height:450px;overflow-y:auto;">
                <div id="headDiv" style="font-size: 24px; margin-bottom: 9px; text-align:center;">非业务付款凭证</div>
                <div id="middleDiv" style="text-align: right; padding-right: 10px;">申请日期：<%=ConvertHelper.toDate(dr["uba_addDate"]).Value.ToString("yyyy年MM月dd日") %></div>
                <div id="footDiv" style="text-align: left; padding-left: 10px;font-weight:bolder; margin-top:20px;">
                    打印时间：<%=DateTime.Now.ToString("yyyy年MM月dd日") %>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    打印人：<%=manager.real_name %>(<%=manager.user_name %>)
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    付款人：<%=dr["uba_confirmerName"] %>
                </div>
                <table width="100%" border="1" cellspacing="0" cellpadding="0" class="ltable" style="min-width: 0;">
                    <tr style="text-align: right;">
                        <td width="12%" style="font-weight: bolder;">申请编号：</td>
                        <td width="20%" style="text-align: left; padding-left: 5px;"><%=dr["uba_id"] %></td>
                        <td width="12%" style="font-weight: bolder;">申请人：</td>
                        <td width="20%" style="text-align: left; padding-left: 5px;"><%=dr["uba_personName"] %></td>
                        <td width="15%" style="font-weight: bolder;">申请人区域：</td>
                        <td style="text-align: left; padding-left: 5px;"><%=new MettingSys.BLL.department().getAreaText(dr["uba_area"].ToString()) %></td>
                    </tr>
                    <tr style="text-align: right;">
                        <td width="10%" style="font-weight: bolder;">申请事由：</td>
                        <td style="text-align: left; padding-left: 5px;" colspan="5">
                            <%=BusinessDict.unBusinessNature()[Convert.ToByte(dr["uba_type"])]%>-<%=dr["uba_function"] %>：<%=dr["uba_oid"]%>，<%=dr["uba_instruction"]%>
                        </td>
                    </tr>
                    <tr style="text-align: right;">
                        <td width="10%" style="font-weight: bolder;">付款总额：</td>
                        <td width="20%" style="text-align: left; padding-left: 5px;"><%=dr["uba_money"] %></td>
                        <td width="10%" style="font-weight: bolder;">付款方式：</td>
                        <td width="20%" style="text-align: left; padding-left: 5px;"><%=dr["pm_name"] %></td>
                        <td width="10%" style="font-weight: bolder;">付款日期：</td>
                        <td style="text-align: left; padding-left: 5px;"><%=ConvertHelper.toDate(dr["uba_date"]).Value.ToString("yyyy-MM-dd") %></td>
                    </tr>
                    <tr style="text-align: right;">
                        <td width="10%" style="font-weight: bolder;">收款账户：</td>
                        <td width="40%" colspan="2" style="text-align: left; padding-left: 5px;"><%=dr["uba_receiveBankName"]%></td>
                        <td width="10%" style="font-weight: bolder;">收款银行：</td>
                        <td style="text-align: left; padding-left: 5px;" colspan="2"><%=dr["uba_receiveBank"] %></td>
                    </tr>
                    <tr style="text-align: right;">
                        <td width="10%" style="font-weight: bolder;">收款账号：</td>
                        <td style="text-align: left; padding-left: 5px;" colspan="5"><%=dr["uba_receiveBankNum"] %></td>
                    </tr>
                    <tr style="text-align: right;">
                        <td width="10%" rowspan="3" style="font-weight: bolder;">审批人：</td>
                        <td style="text-align: left; height: 17px; padding-left: 5px;" colspan="5">
                            1:<%=dr["uba_checkName1"] %>(<%=dr["uba_checkNum1"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(dr["uba_flag1"])] %>，<%=dr["uba_checkRemark1"] %>，<%=dr["uba_checkTime1"]%>
                        </td>
                    </tr>
                    <tr style="text-align: right;">
                        <td style="text-align: left; height: 17px; padding-left: 5px;" colspan="5">
                            2:<%=dr["uba_checkName2"] %>(<%=dr["uba_checkNum2"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(dr["uba_flag2"])] %>，<%=dr["uba_checkRemark2"] %>，<%=dr["uba_checkTime2"] %>
                        </td>
                    </tr>
                    <tr style="text-align: right;">
                        <td style="text-align: left; height: 17px; padding-left: 5px;" colspan="5">
                            3:<%=dr["uba_checkName3"] %>(<%=dr["uba_checkNum3"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(dr["uba_flag3"])] %>，<%=dr["uba_checkRemark3"] %>，<%=dr["uba_checkTime3"] %>
                        </td>
                    </tr>
                </table>                
            </div>
        </div>
    </form>
</body>
</html>
