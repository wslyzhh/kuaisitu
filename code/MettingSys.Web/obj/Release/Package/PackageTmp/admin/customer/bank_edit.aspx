<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="bank_edit.aspx.cs" Inherits="MettingSys.Web.admin.customer.bank_edit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>编辑客户银行账号</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#form1").initValidform();
            var ajaxFormOption = {
                dataType: "json", //数据类型  
                success: function (data) { //提交成功的回调函数 
                    closeLoad();
                    if (data.status == 0) {
                        if (data.fromPay == "true") {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                parent.layer.closeAll();
                                parent.showBank(data.cid);
                            }, 1500);
                        } else {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                parent.location.reload();
                            }, 1500);
                        }
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () {

                            }
                        }).showModal();
                    }
                }
            };
            //提交
            $("#btnSave").click(function () {
                printLoad();
                $("#form1").ajaxSubmit(ajaxFormOption);
            });
        });
    </script>
    <style type="text/css">
        .tab-content dl dt {
            width: 100px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" action="../../tools/business_ajax.ashx?action=saveBank" method="post" enctype="multipart/form-data">
        <div class="tab-content">
            <dl>
                <dt>银行账户名称</dt>
                <dd>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>客户银行账号</dt>
                <dd>
                    <asp:TextBox ID="txtBankNum" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>开户行</dt>
                <dd>
                    <asp:TextBox ID="txtBank" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>开户地址</dt>
                <dd>
                    <asp:TextBox ID="txtBankAddress" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                    <span class="Validform_checktip">*</span>
                </dd>
            </dl>
            <dl>
                <dt>启用状态</dt>
                <dd>
                    <div class="rule-single-checkbox">
                        <asp:CheckBox ID="cbIsUse" runat="server" Checked="true" />
                    </div>
                    <span class="Validform_checktip"></span>
                </dd>
            </dl>
            <dl>
                <dt></dt>
                <dd>
                    <input type="hidden" name="bID" value="<%=id %>" />
                    <input type="hidden" name="actionType" value="<%=action %>" />
                    <input type="hidden" name="cID" value="<%=cid %>" />
                    <input type="hidden" name="fromPay" value="<%=fromPay %>" />
                    <input id="btnSave" runat="server" type="button" value="提交保存" class="btn" />
                </dd>
            </dl>
        </div>
    </form>
</body>
</html>
