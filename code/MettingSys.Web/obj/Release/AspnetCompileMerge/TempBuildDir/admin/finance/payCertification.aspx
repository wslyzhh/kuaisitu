<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="payCertification.aspx.cs" Inherits="MettingSys.Web.admin.finance.payCertification" %>

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
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script language="javascript" src="../../plugins/Lodop/LodopFuncs.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <object id="LODOP" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0" height="0"></object>

    <script type="text/javascript">
        var LODOP = getLodop();
        //$(function () {
        //    CheckLodop();

        //})
        function PreviewMytable() {
            LODOP.PRINT_INIT("业务付款凭证");
            var strStyle = "<style> table,td,th {border-width: 1px;border-style: solid;border-collapse: collapse}</style>"
            LODOP.NewPageA();
            LODOP.SET_PRINT_STYLE("FontSize", 12);
            LODOP.ADD_PRINT_HTML(20, "40%", "90%", 20, "<B>业务付款凭证</B>");
            LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            //LODOP.ADD_PRINT_HTM(20, "40%", "90%", 20, document.getElementById("headDiv").innerHTML);
            //LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            //LODOP.SET_PRINT_STYLEA(0, "FontSize", 25);
            LODOP.ADD_PRINT_HTM(60,"75%", "90%", 20, document.getElementById("middleDiv").innerHTML);
            LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            LODOP.ADD_PRINT_HTM(110, 5, "100%", 20, "<B>"+document.getElementById("footDiv").innerHTML+"</B>");
            LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
            LODOP.ADD_PRINT_TABLE(128, 5, "99%", 428, strStyle + document.getElementById("printDIV").innerHTML);
            LODOP.SET_PRINT_STYLEA(0, "Vorient", 3);
            LODOP.ADD_PRINT_HTM(1, 600, 300, 100, "总页号：<font color='#0000ff' format='ChineseNum'><span tdata='pageNO'>第##页</span>/<span tdata='pageCount'>共##页</span></font>");
            LODOP.SET_PRINT_STYLEA(0, "ItemType", 1);
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
<%--        <h3 id="message1" style="display: none"><font color='#FF00FF'>打印控件未安装!点击这里<a href="../../plugins/Lodop/CLodop_Setup_for_Win32NT.zip">执行安装</a>,安装后请刷新页面。</font></h3>
        <h3 id="message2" style="display: none"><font color='#FF00FF'>（Firefox浏览器用户需先点击这里<a href="../../plugins/Lodop/npActiveX0712SFx31.xpi">安装运行环境</a>）</font></h3>
        <h3 id="message3" style="display: none"><font color='#FF00FF'>打印控件需要升级!点击这里<a href="../../plugins/Lodop/install_lodop.exe">执行升级</a>,升级后请重新进入。</font></h3>--%>
        <div style="padding: 10px;">
            <div class="table-container" style="text-align: right;">
                <a href="../../plugins/Lodop/CLodop_Setup_for_Win32NT.zip">下载插件</a>
                <input type="button" value="打印" onclick="PreviewMytable()" class="btn Noprn" />
            </div>
            <div class="table-container" id="printDIV" style="text-align: center; line-height: 24px; height: 450px; overflow-y: auto;">
                <div id="headDiv" style="font-size: 24px; margin-bottom: 9px; text-align: center;">业务付款凭证</div>
                <div id="middleDiv" style="text-align: right; padding-right: 10px;">实付日期：<%=string.IsNullOrEmpty(Utils.ObjectToStr(dr["rp_date"]))?"":ConvertHelper.toDate(dr["rp_date"]).Value.ToString("yyyy年MM月dd日") %></div>
                <div id="footDiv" style="text-align: left; padding-left: 10px; margin-top: 10px;font-weight: bolder;">
                    打印时间：<%=DateTime.Now.ToString("yyyy年MM月dd日") %>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    打印人：<%=manager.real_name %>(<%=manager.user_name %>)
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    付款人：<%=dr["rp_confirmerName"] %>
                </div>
                <table width="100%" border="1" cellspacing="0" cellpadding="0" class="ltable" style="min-width: 0;">
                    <thead>
                        <tr style="text-align: right;">
                            <td width="12%" style="font-weight: bolder;">付款对象：</td>
                            <td width="28%" style="text-align: left; padding-left: 5px;"><%=dr["c_name"] %></td>
                            <td width="7%" style="font-weight: bolder;">付款方式：</td>
                            <td width="15%" style="text-align: left; padding-left: 5px;"><%=dr["pm_name"] %></td>
                            <td width="12%" style="font-weight: bolder;">付款总额：</td>
                            <td style="text-align: left; padding-left: 5px;"><%=dr["rp_money"] %></td>
                        </tr>
                        <%if (Utils.StrToBool(Utils.ObjectToStr(dr["rp_isExpect"]), false))
                            { %>
                        <tr style="text-align: right;">
                            <td style="font-weight: bolder;">预付款审批人：</td>
                            <td style="text-align: left; padding-left: 5px;" colspan="2">
                                1:<%=dr["rp_checkName"]%>(<%=dr["rp_checkNum"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(dr["rp_flag"])] %>，<%=dr["rp_checkRemark"] %>，<%=dr["rp_checkTime"]%>
                                <br />
                                2:<%=dr["rp_checkName1"]%>(<%=dr["rp_checkNum1"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(dr["rp_flag1"])] %>，<%=dr["rp_checkRemark1"] %>，<%=dr["rp_checkTime1"]%>
                            </td>
                            <td style="font-weight: bolder;">付款内容：</td>
                            <td style="text-align: left; padding-left: 5px;" colspan="2">
                                <%=dr["rp_content"] %>
                            </td>
                        </tr>
                        <%} %>
                        <%if (detailDT != null && detailDT.Rows.Count > 0)
                            { %>
                        <tr style="text-align: right; text-align: center;font-weight: bolder;">
                            <td>订单号</td>
                            <td>付款内容</td>
                            <td>付款金额</td>
                            <td>付款申请人</td>
                            <td colspan="2">审批人</td>
                        </tr>
                    </thead>
                    <% for (int i = 0; i < detailDT.Rows.Count; i++)
                        { %>
                    <tbody>
                        <tr style="text-align: right; text-align: center;">
                            <td><%=detailDT.Rows[i]["rpd_oid"] %></td>
                            <td><%=detailDT.Rows[i]["rpd_content"] %></td>
                            <td><%=detailDT.Rows[i]["rpd_money"] %></td>
                            <td><%=detailDT.Rows[i]["rpd_personName"] %></td>
                            <td colspan="2" style="text-align: left; padding-left: 5px;">
                                <%if (Utils.StrToBool(Utils.ObjectToStr(dr["rp_isExpect"]), false))
                                    { %>
                            1:自动审批
                            <%}
                                else
                                { %>
                            1:<%=detailDT.Rows[i]["rpd_checkName1"] %>(<%=detailDT.Rows[i]["rpd_checkNum1"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(detailDT.Rows[i]["rpd_flag1"])] %>，<%=detailDT.Rows[i]["rpd_checkRemark1"] %>，<%=detailDT.Rows[i]["rpd_checkTime1"]%>
                                <%} %>
                                <br />
                                <%if (Utils.StrToBool(Utils.ObjectToStr(dr["rp_isExpect"]), false))
                                    { %>
                            2:自动审批
                            <%}
                                else
                                { %>
                            2:<%=detailDT.Rows[i]["rpd_checkName2"] %>(<%=detailDT.Rows[i]["rpd_checkNum2"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(detailDT.Rows[i]["rpd_flag2"])] %>，<%=detailDT.Rows[i]["rpd_checkRemark2"] %>，<%=detailDT.Rows[i]["rpd_checkTime2"]%>
                                <%} %>
                                <br />
                                <%if (Utils.StrToBool(Utils.ObjectToStr(dr["rp_isExpect"]), false))
                                    { %>
                            3:自动审批
                            <%}
                                else
                                { %>
                            3:<%=detailDT.Rows[i]["rpd_checkName3"] %>(<%=detailDT.Rows[i]["rpd_checkNum3"] %>)，<%=BusinessDict.checkStatus()[Convert.ToByte(detailDT.Rows[i]["rpd_flag3"])] %>，<%=detailDT.Rows[i]["rpd_checkRemark3"] %>，<%=detailDT.Rows[i]["rpd_checkTime3"]%>
                                <%} %>
                            </td>
                        </tr>
                    </tbody>
                    <%}
                        }
                        else
                        { %>
                     </thead>
                    <%} %>
                    <%if (detailDT != null && detailDT.Rows.Count > 0)
                            { %>
                    <tfoot>
                      <tr>
                        <TD ></TD>
                        <TD align="right">小计：</TD>
	                    <TD width="19%" tdata="subSum" format="#,##0.00" align="right"><font color="#0000FF">###</font></TD>   
                        <TD ><b>&nbsp;</b></TD>
	                    <%--<TD tdata="pageNO" format="#" align="left"></TD>
	                    <TD tdata="pageCount" format="#" align="left"></TD>     --%>
                        <td colspan="2"></td>
                     </tr>
                    </tfoot>
                    <%} %>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
