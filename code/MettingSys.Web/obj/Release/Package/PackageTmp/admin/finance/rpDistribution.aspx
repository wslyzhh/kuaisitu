<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rpDistribution.aspx.cs" Inherits="MettingSys.Web.admin.finance.rpDistribution" %>
<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分配</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript">
        $(function () {
           
        })
        function computeMoney(obj) {
            var _tmoney = parseFloat($("#tMoney").html());
            var _dmoney = parseFloat($("#labHasDistribute").html());
            var _lmoney = parseFloat($("#labLeftMoney").html());
            $("#tMoney").html(_tmoney - parseFloat($(obj).val()));
            $("#labHasDistribute").html(_dmoney + parseFloat($(obj).val()));
            $("#labLeftMoney").html(_lmoney - parseFloat($(obj).val()));
        }
        //确定分配
        function doDistribute() {
            var _totalMoney = 0;
            $(".disMoney").each(function () {
                if (isNaN($(this).val())) {
                    layer.msg("请正确填写分配金额");
                    return;
                }
                _totalMoney += parseFloat($(this).val()); 
            });
            if (Math.abs(_totalMoney) > <%=Math.Abs(model.rp_money.Value) %>) {
                layer.msg("分配金额的总和的绝对值不能大于本收款通知的金额的绝对值");
                return;
            }
            $(".disMoney").each(function () {
                var obj = $(this);
                var _oid = $(obj).attr("data-oid");
                var _money = $(obj).val();
                if (_money != "") {
                    var postData = { rpid:<%=rpID %>,cid:<%=model.rp_cid %>,cbid:<%=model.rp_cbid %>,oid: _oid, type: '<%=model.rp_type %>', disMoney: _money,foredate:<%=model.rp_foredate.Value.ToString("yyyy-MM-dd")%>,method:<%=model.rp_method %>,area:'<%=model.rp_area %>', chk: $("#txtChk").val()};
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=dealDistribute",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.msg == '成功') {
                                $("span[name='span" + data.oid + "']").html("<font color='green'>" + data.msg + "</font>");
                                var _tmoney = parseFloat($("#tMoney").html());                
                                $("#labHasDistribute").html(data.money);
                                $("#labLeftMoney").html((_tmoney - parseFloat(data.money)).toFixed(2));
                            } else {
                                $("span[name='span" + data.oid + "']").html("<font color='red'>" + data.msg + "</font>");
                            }
                        }
                    });
                }
            });
        }
        function computeLeftMoney() {
            var postData = { rpid:<%=rpID %>};
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=getDistributeMoney",
                data: postData,
                dataType: "json",
                success: function (data) {
                    var _tmoney = parseFloat($("#tMoney").html());                
                    $("#labHasDistribute").html(data.msg);
                    $("#labLeftMoney").html((_tmoney - parseFloat(data.msg)).toFixed(2));
                }
            });
        }
        //自动填写
        function autoDistribute() {
            var totalMoney =<%=model.rp_money %>;
            //判断剩余分配金额与可再分配金额的合计数的正负是否一致，一致可以自动分配，否则不能自动分配
            var _leftMoney = $("#labLeftMoney").text();
            var _m = 0;
            $(".disMoney").each(function () {      
                _m += parseFloat($(this).attr("data-fcmoney")) - parseFloat($(this).attr("data-tdmoney"));
            });
            if ((_leftMoney > 0 && _m < 0) || (_leftMoney < 0 && _m > 0)) {
                layer.msg("列表可再分配金额与剩余分配金额的正负不一致，不能自动分配！");
                return;
            }
            var _chk = $("#txtChk").val();
            var _money = 0;
            if (totalMoney >= 0) {
                if (_chk == "") {
                    $(".disMoney").each(function () {
                        _money = parseFloat($(this).attr("data-fcmoney")) - parseFloat($(this).attr("data-tdmoney")) + parseFloat($(this).attr("data-cdmoney"));
                        if (totalMoney != 0 && _money != 0) {
                            if (_money < totalMoney) {
                                $(this).val(_money);
                                totalMoney = totalMoney - _money;
                            }
                            else {
                                $(this).val(totalMoney);
                                totalMoney = 0;
                            }
                        }                        
                    });
                }
                else {
                    $(".disMoney").each(function () {
                        _money = parseFloat($(this).attr("data-fcmoney")) - parseFloat($(this).attr("data-tdmoney")) + parseFloat($(this).attr("data-cdmoney"));
                        if (totalMoney != 0 && _money != 0) {
                            if (_money < totalMoney) {
                                $(this).val(_money);
                                totalMoney = totalMoney - _money;
                            }
                            else {
                                $(this).val(totalMoney);
                                totalMoney = 0;
                            }
                        }
                    });
                }
            }
            else {
                if (_chk == "") {
                    $(".disMoney").each(function () {
                        _money = parseFloat($(this).attr("data-fcmoney")) - parseFloat($(this).attr("data-tdmoney")) + parseFloat($(this).attr("data-cdmoney"));
                        if (totalMoney != 0 && _money != 0) {
                            if (_money > totalMoney) {
                                $(this).val(_money);
                                totalMoney = totalMoney - _money;
                            }
                            else {
                                $(this).val(totalMoney);
                                totalMoney = 0;
                            }
                        }
                    });
                }
                else {
                    $(".disMoney").each(function () {
                        _money = parseFloat($(this).attr("data-fcmoney")) - parseFloat($(this).attr("data-tdmoney")) + parseFloat($(this).attr("data-cdmoney"));
                        if (totalMoney != 0 && _money != 0) {
                            if (_money > totalMoney) {
                                $(this).val(_money);
                                totalMoney = totalMoney - _money;
                            }
                            else {
                                $(this).val(totalMoney);
                                totalMoney = 0;
                            }
                        }
                    });
                }
            }
        }
        //重新填写
        function reWriteDistribute() {
            $(".disMoney").val("");
        }
        //取消分配
        function cancelDistribute() {
            $(".disMoney").each(function () {
                if ($(this).val() != "") {
                    $(this).val('0');
                }
            });            
        }
    </script>
    <style type="text/css">
        .date-input {
            width: 100px;
        }

        .myRuleSelect .select-tit {
            padding: 5px 5px 7px 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content-tab-wrap" id="titleDiv" runat="server">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a <%=_tag==0?"class='selected'":"" %> href="rpDistribution.aspx?tag=0&id=<%=rpID %>">未分配完</a></li>
                        <li><a <%=_tag==1?"class='selected'":"" %> href="rpDistribution.aspx?tag=1&id=<%=rpID %>">本<%=model.rp_type.Value?"收款":"付款" %>通知已分配</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content" style="padding-top: 10px;">
            <div class="searchbar">
                    订 单 号 ： 
                    <asp:TextBox ID="txtOrderId" runat="server" CssClass="input"></asp:TextBox>
                    活动开始日期：
                        <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddlMoneyType" runat="server">
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

                </div>
                <div class="searchbar">
                    对账标识：
                    <asp:TextBox ID="txtChk" runat="server" CssClass="input"></asp:TextBox>

                    活动结束日期：
                        <asp:TextBox ID="txtsDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate1\')}'})"></asp:TextBox>
                    -
                        <asp:TextBox ID="txteDate1" runat="server" CssClass="input rule-date-input" Width="100px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate1\')}'})"></asp:TextBox>
                    业务员：
                    <asp:TextBox ID="txtPerson" runat="server" CssClass="input small"></asp:TextBox>

                    <input type="hidden" name="tag" value="<%=_tag %>" />
                    <input type="hidden" name="id" value="<%=rpID %>" />
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn" Text="查询" OnClick="btnSearch_Click" />
                </div>

            <!--列表-->
            <div class="table-container" style="margin-bottom: 10px;">
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr style="text-align: center;">
                                <th width="10%">分配金额</th>
                                <th width="10%">订单号</th>
                                <th>活动名称</th>
                                <th style="text-align: right;" width="10%"><%=string.IsNullOrEmpty(_chk)?"":"<font color='green'>对账/</font>" %>应<%=model.rp_type.Value?"收":"付" %></th>
                                <th style="text-align: right;" width="10%"><%=string.IsNullOrEmpty(_chk)?"":"<font color='green'>对账/</font>" %>已<%=model.rp_type.Value?"收":"付" %></th>
                                <th style="text-align: right;" width="10%"><%=string.IsNullOrEmpty(_chk)?"":"<font color='green'>对账/</font>" %>未<%=model.rp_type.Value?"收":"付" %></th>
                                <th style="text-align: right;" width="10%"><%=string.IsNullOrEmpty(_chk)?"":"<font color='green'>对账/</font>" %>总分配</th>
                                <th style="text-align: right;" width="10%"><%=string.IsNullOrEmpty(_chk)?"":"<font color='green'>对账/</font>" %>本次分配</th>
                                <th width="8%">状态</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style="text-align: center">
                            <td>
                                <input type="text" class="input small disMoney" data-oid="<%#Eval("fin_oid") %>" data-unmoney="<%#Eval("unMoney") %>" data-fcmoney="<%#string.IsNullOrEmpty(_chk)?Eval("finMoney"):Eval("fcMoney") %>" data-tdmoney="<%#string.IsNullOrEmpty(_chk)?Eval("totalDistribute"):Eval("chktotalDistribute") %>" data-cdmoney="<%#string.IsNullOrEmpty(_chk)?Eval("currentDistribute"):Eval("chkcurrentDistribute") %>" style="text-align: right;" value="<%#string.IsNullOrEmpty(_chk)?(Utils.ObjToDecimal(Eval("currentDistribute"),0)==0?"":Eval("currentDistribute")):(Utils.ObjToDecimal(Eval("chkcurrentDistribute"),0)==0?"":Eval("chkcurrentDistribute")) %>" /></td>
                            <td><a target="_blank"  href="../order/order_edit.aspx?action=<%# DTEnums.ActionEnum.Edit.ToString() %>&oID=<%#Eval("fin_oid")%>"><%#Eval("fin_oid") %></a></td>
                            <td style="text-align: left;"><%#Eval("o_content") %></td>
                            <td style="text-align: right;"><%# string.IsNullOrEmpty(_chk)?"":"<font color='green'>"+Eval("fcMoney")+"/</font>" %><%#Eval("finMoney") %></td>
                            <td style="text-align: right;"><%# string.IsNullOrEmpty(_chk)?"":"<font color='green'>"+Eval("chkMoney")+"/</font>" %><%#Eval("rpdMoney") %></td>
                            <td style="text-align: right;"><%# string.IsNullOrEmpty(_chk)?"":"<font color='green'>"+ (Utils.ObjToDecimal(Eval("fcMoney"),0)-Utils.ObjToDecimal(Eval("chkMoney"),0))+"/</font>" %><%#Eval("unMoney") %></td>
                            <td style="text-align: right;"><%# string.IsNullOrEmpty(_chk)?"":"<font color='green'>"+Eval("chktotalDistribute")+"/</font>" %><%#Eval("totalDistribute") %></td>
                            <td style="text-align: right;"><%# string.IsNullOrEmpty(_chk)?"":"<font color='green'>"+Eval("chkcurrentDistribute")+"/</font>" %><%#Eval("currentDistribute") %></td>
                            <td><span name="span<%#Eval("fin_oid") %>"></span></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"9\">暂无记录</td></tr>" : ""%>
                </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <input type="button" onclick="doDistribute()" class="btn" value="确定分配" />
            <input type="button" onclick="autoDistribute()" class="btn" value="自动填写" />
            <input type="button" onclick="reWriteDistribute()" class="btn" value="重新填写" />
            <input type="button" onclick="cancelDistribute()" class="btn" value="取消分配" />
            <div style="float:right;font-size:14px;font-weight:bolder;margin-top:10px;">总金额：<span id="tMoney"><%=model.rp_money %></span>，已分配总计：<asp:Label ID="labHasDistribute" runat="server"></asp:Label>，剩余总计：<asp:Label ID="labLeftMoney" runat="server"></asp:Label></div>
            <!--/列表-->
            <div class="dRemark" style="margin-top: 10px;">
                <p>1.可搜索多个订单号，用“,”隔开</p>
                <p>2.分配金额：为空表示不操作某个订单，为0表示取消分配</p>
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
        </div>
    </form>
</body>
</html>
