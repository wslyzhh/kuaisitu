<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InnerCustomer_Add.aspx.cs" Inherits="MettingSys.Web.admin.customer.InnerCustomer_Add" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加内部客户</title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link type="text/css" rel="stylesheet" href="../skin/default/style.css" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        //人员选择
        function chooseEmployee(obj, n, flag) {
            //业务报账员和业务执行人员必须在选择活动归属地后才能选择
            var area = "";
            var liObj = $(obj).parent();
            var d = top.dialog({
                id: 'specDialogId',
                padding: 0,
                title: "选择员工",
                url: 'admin/selectEmployee.aspx?area=' + area + ''
            }).showModal();
            //将容器对象传进去
            d.data = liObj;
            d.n = n;//容器序号，一个页面上有多个容器时要传
            d.multi = flag;//true可以选择多个人,false只能选一个人
        }
        function confirmSubmit() {
            var v1 = $("input[name=hide_employee1]").val();
            if (v1 == "") {
                jsdialog('提示', '请选择来源用户', '');
                return false;
            }
            var list = "";
            $("input[name=hide_employee1]").each(function () {
                list += $(this).val() + ",";
            });

            var _index = parent.layer.load(0, { shade: [0.7, '#fff'] });
            var postData = { "list": list};
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/Business_ajax.ashx?action=addInnerCustomer",
                data: postData,
                dataType: "json",
                success: function (data) {
                    parent.layer.close(_index);
                    parent.layer.alert(data.msg);
                }
            });
        }
        //删除附件节点
        function delNode(obj) {
            $(obj).parent().remove();
        }
    </script>
    <style type="text/css">
        .tab-content dl dt {
            width: 60px;
        }

        .tab-content dl dd {
            margin-left: 80px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="tab-content" style="border: none;">
            <dl>
                <dt>来源用户</dt>
                <dd>
                    <div class="txt-item">
                        <ul>
                            <li class="icon-btn" id="liemployee1" runat="server">
                                <a href="javascript:" onclick="chooseEmployee(this,1,true)"><i class="iconfont icon-close"></i></a>
                            </li>
                        </ul>
                    </div>
                </dd>
            </dl>
            <dl style="text-align: center;">
                <input type="button" class="btn" onclick="confirmSubmit()" value="提交" />
            </dl>
        </div>
    </form>
</body>
</html>
