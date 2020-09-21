<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="customer_edit.aspx.cs" Inherits="MettingSys.Web.admin.customer.customer_edit" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑业务明细</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link href="../../scripts/layer/layui.css" rel="stylesheet" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script src="../../scripts/layer/layui.js"></script>
    <script type="text/javascript">
        var element;
        layui.use('element', function () {
            element = layui.element; //Tab的切换功能，切换事件监听等，需要依赖element模块
            //触发事件
            var active = {
                tabChange: function () {
                    //切换到指定Tab项
                    element.tabChange('demo', '22'); //切换到：用户管理
                }
            };
        });
    </script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
        });
        function editContact(action, id) {
            var _title = "添加联系人";
            if (action == "Edit") {
                _title = "编辑联系人";
            }
            layer.open({
                type: 2,
                title: _title,
                shadeClose: true,
                shade: false,
                maxmin: false, //开启最大化最小化按钮
                area: ['500px', '300px'],
                content: 'contact_edit.aspx?action=' + action + '&id=' + id + '&cid=' +<%=this.id%>,
                end: function () {
                    location.reload();
                }
            });
        }
        function editBank(action, id) {
            var _title = "添加银行账号";
            if (action == "Edit") {
                _title = "编辑银行账号";
            }
            layer.open({
                type: 2,
                title: _title,
                shadeClose: true,
                shade: false,
                maxmin: false, //开启最大化最小化按钮
                area: ['600px', '400px'],
                content: 'bank_edit.aspx?action=' + action + '&id=' + id + '&cid=' +<%=this.id%>,
                end: function () {
                    location.reload();
                }
            });
        }
        function checkName(obj) {
            $.getJSON("../../tools/business_ajax.ashx?action=checkCustomerName&name=" + escape($(obj).val()) + "&id=<%=id%>", function (json) {
                if (json.status == 'y') {
                    $(obj).next().addClass("Validform_right").text(json.info);
                }
                else {
                    $(obj).next().addClass("Validform_wrong").text(json.info);
                }
            });
        }
    </script>
    <style type="text/css">
        .divContent {
            padding: 20px 15px;
            font-size: 12px;
            color: #666;
            border: 1px solid #eee;
            border-top: none;
            box-sizing: border-box;
            overflow: hidden;
        }

            .divContent dl dt {
                display: block;
                float: left;
                width: 130px;
                text-align: right;
                color: #6d7e86;
            }

            .divContent dl {
                clear: both;
                display: block;
                padding: 5px 0;
                line-height: 30px;
            }

                .divContent dl dd {
                    position: relative;
                    margin-left: 150px;
                    *position: static;
                }
    </style>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="customer_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="customer_list.aspx"><span>客户列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>客户</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->
        <div id="floatHead" runat="server" class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">客户详情</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="tab-content">
            <dl>
                <dt>客户类别</dt>
                <dd>
                    <div class="rule-single-select">
                        <asp:DropDownList ID="ddltype" runat="server">
                        </asp:DropDownList>
                    </div>
                    <asp:Label ID="labtype" runat="server"></asp:Label>
                </dd>
            </dl>
            <dl>
                <dt>客户名称</dt>
                <dd>
                    <asp:TextBox ID="txtName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" " onblur="checkName(this)"></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>业务范围</dt>
                <dd>
                    <asp:TextBox ID="txtBusinessScope" runat="server" CssClass="input normal" TextMode="MultiLine" />
                </dd>
            </dl>
            <dl>
                <dt>信用代码(税号)</dt>
                <dd>
                    <asp:TextBox ID="txtNum" runat="server" CssClass="input normal"></asp:TextBox>
                </dd>
            </dl>
            <dl>
                <dt>备注</dt>
                <dd>
                    <asp:TextBox ID="txtRemark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                </dd>
            </dl>
            <dl>
                <dt>启用状态</dt>
                <dd>
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsUse" runat="server" Checked="true" />
                    </div>
                </dd>
            </dl>
            <dl id="Mdl1" runat="server">
                <dt>主要联系人</dt>
                <dd>
                    <asp:TextBox ID="txtMContact" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" " />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl id="Mdl2" runat="server">
                <dt>主要联系号码</dt>
                <dd>
                    <asp:TextBox ID="txtMPhone" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" " />
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt></dt>
                <dd>
                    <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" OnClick="btnSubmit_Click" />
                </dd>
            </dl>

            <div class="layui-tab">
                <ul class="layui-tab-title">
                    <li class="layui-this" lay-id="11">联系人</li>
                    <li lay-id="22">银行账号</li>
                </ul>
                <div class="layui-tab-content">
                    <div class="layui-tab-item layui-show">

                        <div class="tab-content" style="padding-top: 0px;">
                            <div class="table-container">
                                <!--工具栏-->
                                <div id="contactDiv" class="toolbar-wrap" runat="server">
                                    <div class="toolbar">
                                        <div class="box-wrap">
                                            <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                                            <div class="l-list">
                                                <ul class="icon-list">
                                                    <li id="liAdd" runat="server"><a href="javascript:;" onclick="editContact('<%#DTEnums.ActionEnum.Add %>')"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                                                    <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                                                    <li>
                                                        <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','如果该联系人已被使用则无法删除，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--/工具栏-->
                                <asp:Repeater ID="rptList" runat="server">
                                    <HeaderTemplate>
                                        <table width="70%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                                            <tr style="text-align: center;">
                                                <th width="4%">选择</th>
                                                <th width="10%">主次标识</th>
                                                <th width="10%">联系人</th>
                                                <th width="10%">联系号码</th>
                                                <th>操作</th>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="text-align: center;">
                                            <td>
                                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" Visible='<%# !Convert.ToBoolean(Eval("co_flag"))%>' />
                                                <asp:HiddenField ID="hidId" Value='<%#Eval("co_id")%>' runat="server" />
                                            </td>
                                            <td><%# Convert.ToBoolean(Eval("co_flag"))?"主":"次"%></td>
                                            <td><%#Eval("co_name")%></td>
                                            <td><%#Eval("co_number")%></td>
                                            <td align="left">
                                                <%#Eval("c_flag").ToString()=="2" || Eval("c_type").ToString()=="3"?"":"<a href=\"javascript:;\" onclick=\"editContact('"+DTEnums.ActionEnum.Edit+"','"+Eval("co_id")+"')\">修改</a>"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"5\">暂无记录</td></tr>" : ""%>
  </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <!--/联系人-->
                            <div class="dRemark">
                                <p>1.主要联系人只能添加一个，次要联系人可以添加多个；必须存在主要联系人后才能添加次要联系人</p>
                            </div>
                        </div>
                    </div>
                    <div class="layui-tab-item">

                        <div class="tab-content" style="padding-top: 0px;">
                            <div id="bankDiv" runat="server" class="toolbar-wrap">
                                <div class="toolbar">
                                    <div class="box-wrap">
                                        <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                                        <div class="l-list">
                                            <ul class="icon-list">
                                                <li id="bankliAdd" runat="server"><a href="javascript:;" onclick="editBank('<%#DTEnums.ActionEnum.Add %>')"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                                                <li>
                                                <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                                                <li>
                                                    <asp:LinkButton ID="btnDelBank" runat="server" OnClientClick="return ExePostBack('btnDelBank','如果该客户银行账号已被使用则无法删除，是否继续？');" OnClick="btnDelBank_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                                                <li><asp:LinkButton ID="btnExcel" OnClick="btnExcel_Click" runat="server"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                
                            <div class="table-container">
                                <asp:Repeater ID="bankrptList" runat="server">
                                    <HeaderTemplate>
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                                            <tr>
                                                <th width="6%">选择</th>
                                                <th align="left" width="12%">银行账户名称</th>
                                                <th align="left" width="12%">客户银行账号</th>
                                                <th align="left" width="12%">开户行</th>
                                                <th align="left">开户地址</th>
                                                <th align="left" width="6%">状态</th>
                                                <th width="10%">操作</th>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td align="center">
                                                <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                                <asp:HiddenField ID="hidId" Value='<%#Eval("cb_id")%>' runat="server" />
                                            </td>
                                            <td><%#Eval("cb_bankName")%></td>
                                            <td><%#Eval("cb_bankNum")%></td>
                                            <td><%#Eval("cb_bank")%></td>
                                            <td><%#Eval("cb_bankAddress")%></td>
                                            <td><%#MettingSys.Common.BusinessDict.isUseStatus()[Convert.ToBoolean(Eval("cb_flag"))]%></td>
                                            <td align="center">
                                                <%#Eval("c_flag").ToString()=="2" || Eval("c_type").ToString()=="3"?"":"<a href=\"javascript:;\" onclick=\"editBank('"+DTEnums.ActionEnum.Edit+"','"+Eval("cb_id")+"')\">修改</a>"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%#rptList.Items.Count == 0 ? "<tr><td align=\"center\" colspan=\"7\">暂无记录</td></tr>" : ""%>
                                    </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <!--/列表-->
                            <div class="dRemark">
                                <p></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--/客户-->
            <!--工具栏-->
            <div class="page-footer">
                <div class="btn-wrap">
                    <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript: history.back(-1);" />
                </div>
            </div>
            <!--/工具栏-->

        </div>

    </form>
</body>
</html>
