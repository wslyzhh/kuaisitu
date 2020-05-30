<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="receipt_list.aspx.cs" Inherits="MettingSys.Web.admin.finance.receipt_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>收款通知列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCustomer", function (json) {
                $('#txtCusName').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });
            $("#ddlisConfirm1").change(function () {
                if ($(this).val() == "True") {
                    $(".spdate").show();
                }
                else {
                    $(".spdate").hide();
                }
            });
            bingCertificate();

            $(".checkall input").change(function () {
                computeSelect();
            });
        })
        function computeSelect() {
            $("#sCount").text($(".checkall input:checked").size());
            var _smoney = 0, _sumoney = 0;
            $(".checkall input:checked").each(function () {
                _smoney += parseFloat($(this).parent().parent().parent().children(".moneyTd").html());
                _sumoney += parseFloat($(this).parent().parent().parent().children(".umoneyTd").children().html());
            });
            $("#sMoney").text(_smoney.toFixed(2));
            $("#suMoney").text(_sumoney.toFixed(2));
        }
        //绑定凭证
        function bingCertificate() {
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCertificate", function (json) {
                $('#txtCenum').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 0,
                    onSelect: function (suggestion) {
                        $('#txtCedate').val(suggestion.id);
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项'
                });
            });
        }
        //展开明细
        function showDetail(rpid) {
            if ($(".Detail" + rpid).length == 0) {
                $.getJSON("../../tools/business_ajax.ashx?action=getDetails&rpid=" + rpid, function (json) {
                    $("#tr" + rpid).siblings(".Detail" + rpid + "").hide();
                    if (json.length > 0) {
                        var _trhtml = "<tr class='Detail" + rpid + "'><td></td><td>订单号</td><td>付款对象</td><td>付款内容</td><td>付款金额</td><td>预付日期</td><td>付款方式</td><td>申请人</td><td>区域</td><td colspan=\"2\">状态</td></tr>";
                        $.each(json, function (index, item) {
                            _trhtml += "<tr class='Detail" + rpid + "'><td></td><td><a href=\"../order/order_edit.aspx?action=Edit&oID=" + item.rpd_oid + "\"><span class='orderstatus_"+item.o_status+"'>" + item.rpd_oid + "</span></a></td>"
                                + "<td>" + item.c_name + "</td>"
                                + "<td>" + item.rpd_content + "</td>"
                                + "<td>" + item.rpd_money + "</td>"
                                + "<td>" + item.rpd_foredate.substring(0, 10) + "</td>"
                                + "<td>" + item.pm_name + "</td>"
                                + "<td title=\"" + item.rpd_personNum + "\">" + item.rpd_personName + "</td>"
                                + "<td>" + item.rpd_area + "</td>"
                                + "<td colspan=\"2\"><span class=\"check_" + item.rpd_flag1 + "\"></span></td></tr>";
                        });
                        $("#tr" + rpid).after(_trhtml);
                    }
                });
            }
            else {
                $(".Detail" + rpid).remove();
            }
        }
        function toggleconfirmDiv() {
            $("#certificateDiv").hide();
            $("#confirmDiv").toggle();
            $(".spdate").show();
        }
        function togglecertificateDiv() {
            $("#confirmDiv").hide();
            $("#certificateDiv").toggle();
        }
        /*确认收款 */
        function submitConfirm() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#ddlisConfirm1").val() == "") {
                jsprint("请选择收款状态");
                return;
            }
            if ($("#ddlisConfirm1").val() == "True") {
                if ($("#txtdate1").val() == "") {
                    jsprint("请选择实付日期");
                    return;
                }
            }
            $(".checkall input:checked").each(function () {                
                var id = $(this).parent().next().val();
                var postData = { "id": id, "status": $("#ddlisConfirm1").val(), "date": $("#txtdate1").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=confirmReceiptPay&type=1",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {                            
                            if ($("#ddlisConfirm1").val() == "True") {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_2");
                                $("#tr" + id).find(".dateTd").text($("#txtdate1").val());
                            }
                            else {
                                $("#tr" + id).find(".confirmTd").children().eq(0).removeClass().addClass("check_0"); 
                                $("#tr" + id).find(".dateTd").html("");
                            }
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });            
        }
        /*确认收款 */
        /*凭证 */
        function submitCertificate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            if ($("#txtCenum").val() == "") {
                jsprint("请选择凭证号");
                return;
            }
            if ($("#txtCedate").val() == "") {
                jsprint("请选择凭证日期");
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id, "num": $("#txtCenum").val(), "date": $("#txtCedate").val() };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=signCertificate",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {   
                                $("#tr" + id).find(".numTd").html($("#txtCenum").val());
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });

            
        }
        /*凭证 */
        //取消凭证
        function cancelCertificate() {
            if ($(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            $(".checkall input:checked").each(function () {
                var id = $(this).parent().next().val();
                var postData = { "id": id };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=unsignCertificate",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        $("#tr" + id).children().last().children().last().html("");
                        if (data.status == 0) {   
                                $("#tr" + id).find(".numTd").html("");
                        } else {
                            $("#tr" + id).children().last().children().last().append(data.msg);
                        }
                    }
                });
            });
        }        
        //分配
        function dealDistribution(id) {
            layer.open({
                type: 2,
                title: '分配收款金额',
                area: ['950px', '650px'],
                content: 'rpDistribution.aspx?id=' + id
            });
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 110px;
        }
        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
        }
    </style>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>收款通知列表</span>
        </div>
        <!--/导航栏-->
        <div class="content-tab-wrap" id="titleDiv" runat="server">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_flag=="0"?"class=\"selected\"":"" %> href="receipt_list.aspx?flag=0">全部列表</a></li>
                        <li><a <%=_flag=="1"?"class=\"selected\"":"" %> href="receipt_list.aspx?flag=1">未到账列表</a></li>
                        <li><a <%=_flag=="2"?"class=\"selected\"":"" %> href="receipt_list.aspx?flag=2">已到账列表</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="receipt_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);computeSelect();"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li><a href="javascript:;" onclick="reverseCheckAll();computeSelect();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后无法恢复，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                            <li><a href="javascript:;" onclick="toggleconfirmDiv()"><span>确认收款</span></a></li>
                            <li><a href="javascript:;" onclick="togglecertificateDiv()"><span>标记凭证</span></a></li>
                        </ul>
                        <div id="confirmDiv" style="display: none;">
                            收款状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm1" runat="server">
                                </asp:DropDownList>
                            </div>
                            <span class="spdate" style="display: none;">实收日期：<asp:TextBox ID="txtdate1" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                            </span>
                            <input type="button" class="btn" value="提交" onclick="submitConfirm()" />
                        </div>
                        <div id="certificateDiv" style="display: none;">
                            凭证号：<asp:TextBox ID="txtCenum" runat="server" CssClass="input " Width="150" />
                            凭证日期：<asp:TextBox ID="txtCedate" runat="server" CssClass="input rule-date-input" Width="120" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
                            <input type="button" class="btn" value="标记凭证" onclick="submitCertificate()" />
                            <%--<input type="button" class="btn" value="添加凭证" onclick="addCertificate()" />--%>
                            <input type="button" class="btn" value="取消凭证" onclick="cancelCertificate()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->
        <div class="searchbar">
            收款对象：
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input"></asp:TextBox>
            <asp:HiddenField ID="hCusId" runat="server" />
            收款方式：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlmethod" runat="server"></asp:DropDownList>
                            </div>
            收款状态：
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlisConfirm" runat="server"></asp:DropDownList>
                            </div>
            预收日期：
                            <asp:TextBox ID="txtsforedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txteforedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsforedate\')}'})" />
            实收日期：
                            <asp:TextBox ID="txtsdate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            -
                            <asp:TextBox ID="txtedate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtsdate\')}'})" />
            
            <input type="hidden" name="self" value="<%=_self %>" />
            <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                <asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton>
        </div>
        <div class="searchbar" style="margin-top:10px;">
            凭证号：
                            <asp:TextBox ID="txtNum" runat="server" Width="100px" CssClass="input" />
            凭证日期：
            <asp:TextBox ID="txtNumDate" runat="server" CssClass="input rule-date-input" Width="110" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            对账凭证：
                            <asp:TextBox ID="txtChk" runat="server" Width="100px" CssClass="input" />
            <div class="rule-single-select">
                            <asp:DropDownList ID="ddlmoneyType" runat="server">
                                <asp:ListItem Value="1">金额</asp:ListItem>
                                <asp:ListItem Value="2">未分配金额</asp:ListItem>
                            </asp:DropDownList>
                        </div>
            <div class="rule-single-select myRuleSelect">
                            <asp:DropDownList ID="ddlsign" runat="server" Width="50">
                                <asp:ListItem Value=">">></asp:ListItem>
                                <asp:ListItem Value=">=">>=</asp:ListItem>
                                <asp:ListItem Value="=">=</asp:ListItem>
                                <asp:ListItem Value="<>"><></asp:ListItem>
                                <asp:ListItem Value="<"><</asp:ListItem>
                                <asp:ListItem Value="<="><=</asp:ListItem>
                            </asp:DropDownList>
                        </div>
            <asp:TextBox ID="txtMoney" runat="server" CssClass="input small"></asp:TextBox>   
            预收款：
            <div class="rule-single-select">
                            <asp:DropDownList ID="ddlType" runat="server">
                                <asp:ListItem Value="">不限</asp:ListItem>
                                <asp:ListItem Value="True">预收款</asp:ListItem>
                                <asp:ListItem Value="False">非预收款</asp:ListItem>
                            </asp:DropDownList>
                        </div>
            申请人：
                            <asp:TextBox ID="txtAddPerson" runat="server" Width="100px" CssClass="input" />
        </div>
        <!--列表-->
        <div class="table-container">
            <asp:Repeater ID="rptList" runat="server">
                <HeaderTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                        <tr>
                            <th width="6%">选择</th>
                            <th align="left" width="10%">收款对象</th>
                            <th align="left" width="6%">凭证</th>
                            <th align="left">收款内容</th>
                            <th align="left" width="6%">收款金额</th>
                            <th align="left" width="6%">未分配金额</th>
                            <th align="left" width="6%">预收日期</th>
                            <th align="left" width="6%">收款方式</th>
                            <th align="left" width="6%">实收日期</th>
                            <th align="left" width="6%">申请人</th>
                            <th align="left" width="4%">确认收款</th>
                            <th width="8%">操作</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr id="tr<%#Eval("rp_id")%>">
                        <td align="center">
                            <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                            <asp:HiddenField ID="hidId" Value='<%#Eval("rp_id")%>' runat="server" />
                        </td>
                        <td><%# Eval("c_name") %><%# bool.Parse(Eval("rp_isExpect").ToString())?"<font color='green'>[预]</font>":"" %></td>
                        <td class="numTd"><span onmouseover="tip_index=layer.tips('凭证日期：<%#Eval("ce_date").ToString()==""?"":Convert.ToDateTime(Eval("ce_date")).ToString("yyyy-MM-dd")%><br/>备注：<%# Eval("ce_remark") %>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%# Eval("ce_num") %></span></td>
                        <td><%# Eval("rp_content") %></td>
                        <td class="moneyTd"><%# Eval("rp_money") %></td>
                        <td class="umoneyTd"><a href="javascript:;" onclick="showDetail(<%#Eval("rp_id")%>)"><%# Eval("undistribute") %></a></td>
                        <td><%# Convert.ToDateTime(Eval("rp_foredate")).ToString("yyyy-MM-dd") %></td>
                        <td><%# Eval("pm_name") %></td>
                        <td class="dateTd"><%# ConvertHelper.toDate(Eval("rp_date"))==null?"":Convert.ToDateTime(Eval("rp_date")).ToString("yyyy-MM-dd") %></td>
                        <td><span title="<%# Eval("rp_personNum") %>"><%# Eval("rp_personName") %></span></td>
                        <td class="confirmTd"><span onmouseover="tip_index=layer.tips('收款确认人：<%#Eval("rp_confirmerNum")%>-<%#Eval("rp_confirmerName")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Convert.ToBoolean(Eval("rp_isConfirm"))?"2":"0"%>"></span></td>
                        <td align="center">
                            <a href="receipt_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("rp_id")%>">修改</a>
                            <a href="javascript:;" onclick="dealDistribution(<%#Eval("rp_id")%>)">分配</a>
                            <span style="color:red;"></span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"12\">暂无记录</td></tr>" : ""%>
  </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="font-size: 12px;color:darkblue;">
            <span style="float: left;">选中：<asp:Label ID="sCount" runat="server">0</asp:Label>条记录，收款金额：<asp:Label ID="sMoney" runat="server">0</asp:Label>，未分配金额：<asp:Label ID="suMoney" runat="server">0</asp:Label></span>
        </div>
        <div style="font-size: 12px;margin-top: 25px;">
            <span style="float: left;">本页：<asp:Label ID="pCount" runat="server">0</asp:Label>条记录，总计收款金额：<asp:Label ID="pMoney" runat="server">0</asp:Label>，总计未分配金额：<asp:Label ID="pUnMoney" runat="server">0</asp:Label></span>
            <span style="float: right;">总计：<asp:Label ID="tCount" runat="server">0</asp:Label>条记录，总计收款金额：<asp:Label ID="tMoney" runat="server">0</asp:Label>，总计未分配金额：<asp:Label ID="tUnMoney" runat="server">0</asp:Label></span>
        </div>
        <!--/列表-->
        <div class="dRemark" style="margin-top: 47px;">
            <p>1.收款：<span class="check_0"></span>待收款，<span class="check_2"></span>已收款</p>
            <p>2.删除收款通知会连同收款明细一起删除</p>
            <p>3.付款对象后面带“<font color='green'>[预]</font>”，表示预收款</p>            
            <p>4.搜索凭证号：填“0”表示搜索凭证号为空的记录；填“1”表示搜索凭证号不为空的记录</p>
        </div>
        <!--内容底部-->
        <div class="line20"></div>
        <div class="pagelist">
            <div class="l-btns">
                <span>显示</span><asp:TextBox ID="txtPageNum" runat="server" CssClass="pagenum" onkeydown="return checkNumber(event);"
                    OnTextChanged="txtPageNum_TextChanged" AutoPostBack="True"></asp:TextBox><span>条/页</span>
            </div>
            <div id="PageContent" runat="server" class="default"></div>
        </div>
        <!--/内容底部-->

    </form>
</body>
</html>
