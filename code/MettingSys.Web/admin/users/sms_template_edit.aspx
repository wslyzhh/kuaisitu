﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sms_template_edit.aspx.cs" Inherits="MettingSys.Web.admin.users.sms_template_edit" ValidateRequest="false" %>
<%@ Import namespace="MettingSys.Common" %>

<!DOCTYPE html>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>编辑短信模板</title>
<meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
<meta name="apple-mobile-web-app-capable" content="yes" />
<link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
<link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
<link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
<script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
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
</head>

<body class="mainbody">
<form id="form1" runat="server">
<!--导航栏-->
<div class="location">
  <a href="sms_template_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
  <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
  <i class="arrow iconfont icon-arrow-right"></i>
  <span>会员设置</span>
  <i class="arrow iconfont icon-arrow-right"></i>
  <span>编辑短信模板</span>
</div>
<div class="line10"></div>
<!--/导航栏-->

<!--内容-->
<div id="floatHead" class="content-tab-wrap">
  <div class="content-tab">
    <div class="content-tab-ul-wrap">
      <ul>
        <li><a class="selected" href="javascript:;">编辑模板内容</a></li>
      </ul>
    </div>
  </div>
</div>

<div class="tab-content">
  <dl>
    <dt>模板标题</dt>
    <dd>
      <asp:TextBox ID="txtTitle" runat="server" CssClass="input normal" datatype="*1-100" sucmsg=" "></asp:TextBox>
      <span class="Validform_checktip">*标题名称，100字符内</span>
    </dd>
  </dl>
  <dl>
    <dt>调用别名</dt>
    <dd>
      <asp:TextBox ID="txtCallIndex" runat="server" CssClass="input normal" datatype="s0-50" sucmsg=" "></asp:TextBox>
      <span class="Validform_checktip">程序调用的别名，50字符内</span>
    </dd>
  </dl>
  <dl>
    <dt>短信内容</dt>
    <dd>
      <asp:TextBox ID="txtContent" runat="server" CssClass="input normal" TextMode="MultiLine" />
      <span class="Validform_checktip">*发送短信的内容，不支持HTML</span>
    </dd>
  </dl>
</div>
<!--/内容-->

<!--工具栏-->
<div class="page-footer">
  <div class="btn-wrap">
    <asp:Button ID="btnSubmit" runat="server" Text="提交保存" CssClass="btn" onclick="btnSubmit_Click" />
    <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript:history.back(-1);" />
  </div>
</div>
<!--/工具栏-->

</form>
</body>
</html>