<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="department_list.aspx.cs" Inherits="MettingSys.Web.admin.manage.department_list" %>

<%@ Import Namespace="MettingSys.Common" %>
<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <title>部门岗位列表</title>
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?<%=DateTime.Now.ToString("yyyyMMddhhmmss") %>"></script>
    <style type="text/css">
        .tree-list .col-1 {
            width: 6%;
            text-align: center;
        }

        .tree-list .col-2 {
            width: 14%;
            white-space: nowrap;
            word-break: break-all;
            overflow: hidden;
        }

        .tree-list .col-3 {
            width: 30%;
            white-space: nowrap;
            word-break: break-all;
            overflow: hidden;
        }

        .tree-list .col-4 {
            width: 6%;
            text-align: center;
        }

        .tree-list .col-5 {
            width: 8%;
            text-align: center;
        }

        .tree-list .col-6 {
            width: 8%;
        }

        .tree-list .col-7 {
            width: 8%;
            text-align: center;
        }

        .tree-list .col-8 {
            width: 6%;
            text-align: center;
        }

        .tree-list .col-9 {
            width: 12%;
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
            <span>部门岗位</span>
        </div>
        <!--/导航栏-->

        <!--工具栏-->
        <div id="floatHead" class="toolbar-wrap">
            <div class="toolbar">
                <div class="box-wrap">
                    <a class="menu-btn"><i class="iconfont icon-more"></i></a>
                    <div class="l-list">
                        <ul class="icon-list">
                            <li><a href="department_edit.aspx?action=<%=DTEnums.ActionEnum.Add %>"><i class="iconfont icon-close"></i><span>新增</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click"><i class="iconfont icon-save"></i><span>保存</span></asp:LinkButton></li>
                            <li><a href="javascript:;" onclick="checkAll(this);"><i class="iconfont icon-check"></i><span>全选</span></a></li>
                            <li><a href="javascript:;" onclick="reverseCheckAll();"><i class="iconfont icon-check-mark"></i><span>反选</span></a></li>
                            <li>
                                <asp:LinkButton ID="btnDelete" runat="server" OnClientClick="return ExePostBack('btnDelete','本操作会删除本机构及下属机构，是否继续？');" OnClick="btnDelete_Click"><i class="iconfont icon-delete"></i><span>删除</span></asp:LinkButton></li>
                        </ul>
                    </div>
                    <%--<div class="r-list">
                        <asp:TextBox ID="txtKeywords" runat="server" CssClass="keyword" />
                        <asp:LinkButton ID="lbtnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click"><i class="iconfont icon-search"></i></asp:LinkButton>
                    </div>--%>
                </div>
            </div>
        </div>
        <!--/工具栏-->

        <!--列表-->
        <div class="table-container">
            <div class="tree-list">
                <div class="thead">
                    <div class="col col-1">选择</div>
                    <div class="col col-2">类别</div>
                    <div class="col col-3">机构名称</div>
                    <div class="col col-4">机构简称</div>
                    <div class="col col-5">总部</div>
                    <div class="col col-6">区域</div>
                    <div class="col col-7">排序</div>
                    <div class="col col-8">状态</div>
                    <div class="col col-9">操作</div>
                </div>
                <ul>
                    <asp:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <li class="layer-<%#Eval("class_layer")%>">
                                <div class="tbody">
                                    <div class="col col-1">
                                        <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                        <asp:HiddenField ID="hidId" Value='<%#Eval("de_id")%>' runat="server" />
                                    </div>
                                    <div class="col col-2">
                                        <%#MettingSys.Common.BusinessDict.departType()[Utils.ObjToByte(Eval("de_type"))]%>
                                    </div>
                                    <div class="col index col-3">
                                        <a href="department_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("de_id")%>"><%#Eval("de_name")%></a>
                                    </div>
                                    <div class="col col-4">
                                        <%#Eval("de_subname")%>
                                    </div>
                                    <div class="col col-5">
                                        <%#Convert.ToBoolean(Eval("de_isGroup"))?"是":"否"%>
                                    </div>
                                    <div class="col col-6">
                                        <%#Eval("de_area")%>
                                    </div>
                                    <div class="col col-7">
                                        <asp:TextBox ID="txtSortId" runat="server" Text='<%#Eval("de_sort")%>' CssClass="sort" onkeydown="return checkNumber(event);" />
                                    </div>
                                    <div class="col col-8">
                                        <%#MettingSys.Common.BusinessDict.isUseStatus()[Convert.ToBoolean(Eval("de_isUse"))]%>
                                    </div>
                                    <div class="col col-9">
                                        <a href="department_edit.aspx?action=<%#DTEnums.ActionEnum.Add %>&id=<%#Eval("de_id")%>">添子级</a>
                                        <a href="department_edit.aspx?action=<%#DTEnums.ActionEnum.Edit %>&id=<%#Eval("de_id")%>">修改</a>
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

