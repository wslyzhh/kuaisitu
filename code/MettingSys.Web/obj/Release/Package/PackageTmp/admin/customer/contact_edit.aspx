<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="contact_edit.aspx.cs" Inherits="MettingSys.Web.admin.customer.contact_edit" %>

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
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
        });
    </script>
    <style type="text/css">
        .tab-content dl dt {
            width: 100px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="tab-content" style="border:none;">
            <dl>
                <dt>主次标识</dt>
                <dd>
                    <asp:Label ID="labflag" runat="server" Text="次要联系人"></asp:Label>
                </dd>
            </dl>
            <dl>
                <dt>联系人</dt>
                <dd>
                    <asp:TextBox ID="txtName" runat="server" CssClass="input small" datatype="*2-100" Width="150px" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>联系号码</dt>
                <dd>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="input small" Width="150px" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt></dt>
                <dd>
                     <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn"  OnClick="btnSubmit_Click" />
                </dd>
            </dl>
        </div>
        <!--/内容-->
    </form>
</body>
</html>
