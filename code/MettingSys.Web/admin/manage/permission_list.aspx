﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="permission_list.aspx.cs" Inherits="MettingSys.Web.admin.manage.permission_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>权限列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../../css/pagination.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <style type="text/css">
        .tree-list .col-1 {
            width: 6%;
            text-align: center;
        }

        .tree-list .col-2 {
            width: 10%;
        }

        .tree-list .col-3 {
            width: 25%;
        }

        .tree-list .col-4 {
            width: 50%;
            white-space: nowrap;
            word-break: break-all;
            overflow: hidden;
        }

        .tree-list .col-5 {
            width: 8%;
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            //初始化分类的结构
            initCategoryHtml('.tree-list', 1);
            //初始化分类的事件
            $('.tree-list').initCategoryTree(false);
        });
        
    </script>
</head>

<body class="mainbody">
    <form id="form1" runat="server">
        <!--导航栏-->
        <div class="location">
            <a href="javascript:history.back(-1);" class="back"><i class="iconfont icon-up"></i><span>返回上一页</span></a>
            <a href="../center.aspx" class="home"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span>权限列表</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="permission_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','删除后会连同用户权限一起删除，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                        </ul>
                    </div>
                    <div class="r-list">
                        
                    </div>
                </div>
            </div>
        </div>
        <!--/工具栏-->

        <!--列表-->
        <div class="table-container">
            <div class="tree-list">
                <div class="thead">
                    <div class="col col-1">选择</div>
                    <div class="col col-2">权限代码</div>
                    <div class="col col-3">权限名称</div>
                    <div class="col col-4">备注</div>
                    <div class="col col-5">操作</div>
                </div>
                <ul>
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <li class="layer-<%#Eval("class_layer")%>">
                                <div class="tbody">
                                    <div class="col col-1">
                                        <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Visible='<%#Eval("pe_parentid").ToString()!="0"%>' Style="vertical-align: middle;" />
                                        <asp:HiddenField ID="hidId" Value='<%#Eval("pe_id")%>' runat="server" />
                                    </div>
                                    <div class="col col-2">
                                        <a href="permission_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("pe_id")%>"><%#Eval("pe_code")%></a>
                                    </div>
                                    <div class="col index col-3">
                                        <%#Eval("pe_name")%>
                                    </div>
                                    <div class="col col-4">
                                        <%#Eval("pe_remark")%>
                                    </div>
                                    <div class="col col-5">
                                        <a href="permission_edit.aspx?action=<%#DTEnums.ActionEnum.Add %>&id=<%#Eval("pe_id")%>">添子级</a>
                                        <%#Eval("pe_parentid").ToString()=="0"?"":"<a href=\"permission_edit.aspx?action="+DTEnums.ActionEnum.Edit+"&id="+Eval("pe_id")+"\">修改</a>"%>                                        
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
        <!--/列表-->
        <div class="dRemark">
            <p></p>
        </div>
        

    </form>
</body>
</html>
