<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="order_edit.aspx.cs" Inherits="MettingSys.Web.admin.order.order_edit" ValidateRequest="false" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="MettingSys.Common" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=action=="Add"?"新增订单":"编辑订单("+oID+")" %></title>
    <meta name="viewport" content="width=device-width,minimum-scale=1.0,maximum-scale=1.0,initial-scale=1.0,user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <link rel="stylesheet" type="text/css" href="../../scripts/artdialog/ui-dialog.css" />
    <link rel="stylesheet" type="text/css" href="../skin/icon/iconfont.css" />
    <link rel="stylesheet" type="text/css" href="../skin/default/style.css" />
    <link href="../../scripts/layer/layui.css" rel="stylesheet" />
    <link type="text/css" href="../js/antocomplete/autocomplete.css?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>" rel="stylesheet" />
    <link href="../../scripts/tip/tip.css" rel="stylesheet" />
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/layer/layer.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/jquery.form.min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/antocomplete/jquery.autocomplete.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/jquery/Validform_v5.3.2_min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/datepicker/WdatePicker.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/artdialog/dialog-plus-min.js"></script>
    <script type="text/javascript" charset="utf-8" src="../../scripts/webuploader/webuploader.min.js"></script>
    <script src="../../scripts/tip/tip.js"></script>
    <script type="text/javascript" charset="utf-8" src="../js/common.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/newUploader.js?v=<%=DateTime.Now.ToString("yyyyMMddHHssmm") %>"></script>
    <script type="text/javascript" charset="utf-8" src="../js/laymain.js"></script>
    <script type="text/javascript">
        $(function () {
            //初始化表单验证
            $("#ajaxForm").initValidform();

            //初始化客户输入框
            $.getJSON("../../tools/business_ajax.ashx?action=getAllCustomer", function (json) {
                $('#txtCusName').devbridgeAutocomplete({
                    lookup: json,
                    minChars: 1,
                    onSelect: function (suggestion) {
                        $('#hCusId').val(suggestion.id);
                        if (suggestion.id > 0) {
                            $.getJSON("../../tools/business_ajax.ashx?action=getContactsByCid&cid=" + suggestion.id, function (json) {
                                if (json.length > 0) {
                                    //加载联系人
                                    $("#ddlcontact").html("");
                                    var _ul = $("#ddlcontact").prev().find(".select-items").first().children().first();
                                    _ul.html("");
                                    $.each(json, function (index, item) {
                                        $("#ddlcontact").append("<option value='" + item.co_id + "'>" + item.co_name + "</option>");
                                        _ul.append("<li>" + item.co_name + "</li>");
                                        if (index == 0) {
                                            $("#txtPhone").val(item.co_number);
                                        }
                                    });
                                    //需要重新初始化下拉插件
                                    $(".rule-single-select").ruleSingleSelect();
                                }
                                else {
                                    $("#ddlcontact").html("");
                                    $("#ddlcontact").append("<option value=''>暂无联系人</option>");
                                }
                            });
                        }
                    },
                    showNoSuggestionNotice: true,
                    noSuggestionNotice: '抱歉，没有匹配的选项',
                    groupBy: 'type'
                });
            });
            $("#txtCusName").change(function () {
                $("#hCusId").val("");
            });

            //一类上传控件
            $(".upload-album").InitUploader({ btntext: "批量上传", ftype: 1, multiple: true, oID: "<%=oID %>", filetypes: "<%=sysConfig.fileextension %>", filesize: "<%=sysConfig.attachsize %>", sendurl: "../../tools/upload_ajax.ashx", swf: "../../scripts/webuploader/uploader.swf" });
            $("#uploadDiv").children(".upload-btn").append("<div class='webuploader-pick1'>全部人员可查看，文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</div>");
            //二类上传控件
            $(".upload-album2").InitUploader({ btntext: "批量上传", ftype: 2, multiple: true, oID: "<%=oID %>", filetypes: "<%=sysConfig.fileextension %>", filesize: "<%=sysConfig.attachsize %>", sendurl: "../../tools/upload_ajax.ashx", swf: "../../scripts/webuploader/uploader.swf" });
            $("#uploadDiv2").children(".upload-btn").append("<div class='webuploader-pick1'>部分人员可查看，文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</div>");

            //绑定活动归属地
            $("#specAddButton").click(function () {
                var liObj = $(this).parent();
                var d = top.dialog({
                    id: 'specDialogId',
                    padding: 0,
                    title: "活动归属地",
                    url: 'admin/order/order_place.aspx'
                }).showModal();
                //将容器对象传进去
                d.data = liObj;
            });
            var ajaxFormOption = {
                dataType: "json", //数据类型  
                success: function (data) { //提交成功的回调函数  
                    if (data.status == 0) {
                        var d = top.dialog({ content: "订单保存成功,正在跳转页面..." }).show();
                        setTimeout(function () {
                            d.close().remove();
                            location.href = 'order_edit.aspx?action=Edit&oID=' + data.msg;
                        }, 2000);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () {
                                layer.closeAll();
                            }
                        }).showModal();
                    }
                }
            };
            //提交订单
            $("#btnSave").click(function () {
                printLoad();
                $("#ajaxForm").ajaxSubmit(ajaxFormOption);
            });
            //添加应收付
            $("#btnReceiptPay").click(function () {
                layer.open({
                    type: 2,
                    title: '批量添加应收付',
                        area: ['1050px', '550px'],
                        content: 'mutliFinance.aspx?oID=<%=oID%>&cid=' + $("#hCusId").val() + '&cusname='+$("#txtCusName").val(),
                        end: function () {
                            location.reload();
                        }
                });
            });
            //添加应收
            <%--$("#btnReceipt").click(function () {
                addFinance('<%=DTEnums.ActionEnum.Add.ToString()%>', '添加应收', '<%=oID%>', true, 0);
            });--%>
            //添加应付
            <%--$("#btnPay").click(function () {
                addFinance('<%=DTEnums.ActionEnum.Add.ToString()%>', '添加应付', '<%=oID%>', false, 0);
            });--%>
            //添加非业务
            $("#btnUnBusinessPay").click(function () {
                var text = $("#txtCusName").val() + "，" + $("#txtsDate").val() + "/" + $("#txteDate").val() + "，" + $("#txtAddress").val() + "，" + $("#txtContent").val();
                addUnbusinessPay('<%=DTEnums.ActionEnum.Add.ToString()%>', '添加非业务支付审', '<%=oID%>', 0, text);
            });
            //添加发票
            $("#btnInvoince").click(function () {
                addInvoice('<%=DTEnums.ActionEnum.Add.ToString()%>', '添加发票', '<%=oID%>', 0);
            });

            //接单状态
            $("#btnDstatus").click(function () {
                layer.open({
                    type: 1,
                    title: '变更接单状态',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divDstatus')
                });
            });
            //审批状态
            $("#btnFlag").click(function () {
                layer.open({
                    type: 1,
                    title: '变更审批状态',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divflag')
                });
            });
            //锁单状态
            $("#btnLockstatus").click(function () {
                layer.open({
                    type: 1,
                    title: '变更锁单状态',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divlockstatus')
                });
            });
            //税费成本
            $("#btnUpdateCost").click(function () {
                layer.open({
                    type: 1,
                    title: '变更税费成本',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divCost')
                });
            });
            //财务备注
            $("#btnFinRemark").click(function () {
                layer.open({
                    type: 1,
                    title: '编辑财务备注',
                    closeBtn: 0,
                    area: ['516px', '300px'],
                    //skin: 'layui-layer-nobg', //没有背景色
                    shadeClose: true,
                    content: $('#divFinRemark')
                });
            });
            //批量审批应收付
            $("select[name=ddlDoCheck]").change(function () {
                var idS = "";
                if ($(this).val() == "") return;
                if ($(this).parents(".ltable").children().find(".checkall input:checked").size() < 1) {
                    parent.dialog({
                        title: '提示',
                        content: '对不起，请选中您要操作的记录！',
                        okValue: '确定',
                        ok: function () { }
                    }).showModal();
                    return;
                }
                $.each($(this).parents(".ltable").children().find(".checkall input:checked"), function (index, item) {
                    idS += $(this).parent().next().val() + ",";
                });
                idS = idS.substring(0, idS.length - 1);
                var postData = { "ids": idS, "status": $(this).val(), "remark": "" };
                //发送AJAX请求
                $.ajax({
                    type: "post",
                    url: "../../tools/Business_ajax.ashx?action=checkfinanceStatus",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.status == 0) {
                            var d = top.dialog({ content: data.msg }).show();
                            setTimeout(function () {
                                d.close().remove();
                                location.reload();
                            }, 2000);
                        } else {
                            top.dialog({
                                title: '提示',
                                content: data.msg,
                                okValue: '确定',
                                ok: function () { }
                            }).showModal();
                        }
                    }
                });
            });
            //合作分成
            $("#btnSharing").click(function () {
                layer.open({
                    type: 2,
                    title: '添加合作分成',
                        area: ['1050px', '550px'],
                        content: 'mutliSharing.aspx?oID=<%=oID%>',
                        end: function () {
                            location.reload();
                        }
                });
            });

            $("input[name=cbAllCheck]").click(function () {
                if ($(this).prop("checked") == true) {
                    $(this).parents(".ltable").children().find(".checkall input[type=checkbox]").prop("checked", true);
                } else {
                    $(this).parents(".ltable").children().find(".checkall input[type=checkbox]").prop("checked", false);
                }
            });

            //弹出对账明细
            $(".cusTip").on({                
                mouseover: function () {
                    var finid = $(this).attr("data-finid");
                    var that = this;
                    var postData = { "finid": finid };
                    var _table = $("<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\"><tr><th style=\"width:50%;\">对账标识</th><th>对账金额</th></tr></table>");
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=getChkDetail",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            var jlist = eval(data);
                            var trlist = "";
                            if (jlist.length > 0) {
                                for (var i = 0; i < jlist.length; i++) {
                                    var json = eval(jlist[i]);
                                    var _tr = "<tr style=\"text-align:center;\"><td>" + json.fc_num + "</td><td>" + json.fc_money + "</td></tr>";
                                    trlist += _tr;
                                }
                            }
                            _table.append(trlist);
                            showTip(that,_table,200)
                        }
                    });
                }
            });

        });
        //获取联系人的联系号码
        function getContactPhone(obj) {
            $.get("../../tools/business_ajax.ashx?action=getPhone", { contactID: $(obj).val() },
                function (data) {
                    $("#txtPhone").val(data);
                });
        }
        //删除附件节点
        function delNode(obj) {
            $(obj).parent().remove();
        }

        //人员选择
        function chooseEmployee(obj, n, flag,isShow) {
            //业务报账员和业务执行人员必须在选择活动归属地后才能选择
            var area = "";
            if (n == 1 || n == 3) {
                if ($("input[name='hide_place']").length == 0) {
                    var d = dialog({ content: "请先选择活动归属地" }).show();
                    setTimeout(function () {
                        d.close().remove();
                    }, 1000);
                    return;
                }
                $("input[name='hide_place']").each(function (index, item) {
                    area += ',' + $(this).val();
                });
                area = area + ',';
            }
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
            d.showDstatus = isShow;//true显示接单状态，false不显示接单状态
        }

        function addFinance(action, title, oID, type, id) {
            layer.open({
                type: 2,
                title: title,
                area: ['600px', '550px'],
                content: ['../finance/finance_edit.aspx?action=' + action + '&fromOrder=true&oID=' + oID + '&type=' + type + '&id=' + id, 'no']
            });
        }
        function addUnbusinessPay(action, title, oID, id, text) {
            layer.open({
                type: 2,
                title: title,
                area: ['750px', '650px'],
                content: '../unBusiness/unBusinessPay_edit.aspx?action=' + action + '&oID=' + oID + '&fromOrder=true&id=' + id + '&functionText=' + escape(text) + '',
            });
        }
        function addInvoice(action, title, oID, id) {
            layer.open({
                type: 2,
                title: title,
                area: ['650px', '650px'],
                content: '../finance/invoice_edit.aspx?action=' + action + '&oID=' + oID + '&fromOrder=true&id=' + id + '&cid=' + $("#hCusId").val() + '&cusname='+$("#txtCusName").val()
            });
        }
        //删除非业务支付申请
        function deleteUnbusinessPay(obj, id) {
            printLoad();
            parent.dialog({
                title: '提示',
                content: "确定要删除吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { id: id };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/unBusiness_ajax.ashx?action=deleteUnbusiness",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    $(obj).parent().parent().remove();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                            closeLoad();
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }

        //单条审批应收付
        function checkFinance(obj, fid) {
            var postData = { id: fid };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=checkfinance",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        $(obj).prop("class", "check_" + data.msg);
                    } else {
                        layer.alert(data.msg);
                    }
                }
            });
        }
        //删除应收付
        function deleteFinance(obj, id) {
            parent.dialog({
                title: '提示',
                content: "确定要删除吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { id: id };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=deletefinance",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    $(obj).parent().parent().remove();
                                    location.reload();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        //批量删除应收付
        function mutliDelete(obj) {
            if ($(obj).parents(".ltable").children().find(".checkall input:checked").size() < 1) {
                parent.dialog({
                    title: '提示',
                    content: '对不起，请选中您要操作的记录！',
                    okValue: '确定',
                    ok: function () { }
                }).showModal();
                return;
            }
            parent.dialog({
                title: '提示',
                content: "确定要删除选中记录吗？",
                okValue: '确定',
                ok: function () {
                    var idS = "";
                    $.each($(obj).parents(".ltable").children().find(".checkall input:checked"), function (index, item) {
                        idS += $(this).parent().next().val() + ",";
                    });
                    idS = idS.substring(0, idS.length - 1);
                    var postData = { "ids": idS };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/Business_ajax.ashx?action=mutildeletefinance",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    location.reload();
                                }, 2000);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        //添加收付款明细
        function addReceiptPay(action, finType, oID, id, cid, cname,text,illustration) {
            var actionText = "添加";
            if (action == "Edit") {
                actionText = "编辑";
            }
            else if (action == "View") {
                actionText = "查看";
            }
            var textlist = text.split('，');
            var illtext = illustration || '';

            if (illustration != undefined && illustration.length > 200) {
                illtext = illustration.substring(0, 200) + '......';
            }
            
            var newtext = $("#txtCusName").val() + ',' + textlist[0] + "," + $("#txtAddress").val() + ',' + $("#txtContent").val();
            if (textlist[1] != undefined && textlist[1] != null) {
                newtext += ',' + textlist[1];
            }
            else {
                newtext += '：';
            }
            if (illtext != '') {
                newtext += '：' + illtext;
            }

            var title = actionText + "收款明细";
            var _url = '../finance/receiptdetail_edit.aspx?action=' + action + '&oID=' + oID + '&fromOrder=true&id=' + id + '&cid=' + cid + '&cname=' + escape(cname) + '&contentText=' + escape(newtext);
            if (finType == "False") {
                var title = actionText +"付款明细";
                _url = '../finance/paydetail_edit.aspx?action=' + action + '&oID=' + oID + '&fromOrder=true&id=' + id + '&cid=' + cid + '&cname=' + escape(cname) + '&contentText=' + escape(newtext);
            }
            layer.open({
                type: 2,
                title: title,
                area: ['650px', finType == "False"?(action!="Add"?'700px':'550px'):'550px'],
                content: [_url, 'no']
            });
        }
        //删除收付款明细
        function deleteReceiptPayDetail(obj,id) {
            parent.dialog({
                title: '提示',
                content: "确定要删除吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { id: id };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=deleteReceiptPayDetail",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    $(obj).parent().parent().remove();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        //删除发票
        function deleteInvoice(obj, id) {
            parent.dialog({
                title: '提示',
                content: "确定要删除吗？",
                okValue: '确定',
                ok: function () {
                    var postData = { id: id };
                    //发送AJAX请求
                    $.ajax({
                        type: "post",
                        url: "../../tools/business_ajax.ashx?action=deleteinvoice",
                        data: postData,
                        dataType: "json",
                        success: function (data) {
                            if (data.status == 0) {
                                var d = top.dialog({ content: data.msg }).show();
                                setTimeout(function () {
                                    d.close().remove();
                                    $(obj).parent().parent().remove();
                                }, 1500);
                            } else {
                                top.dialog({
                                    title: '提示',
                                    content: data.msg,
                                    okValue: '确定',
                                    ok: function () { }
                                }).showModal();
                            }
                        }
                    });
                },
                cancelValue: '取消',
                cancel: function () { }
            }).showModal();
        }
        //添加财务备注
        function InsertFinRemark(obj, id) {
            var postData = { id: id, remark: $(obj).val() };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=insertFinRemark",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        var d = top.dialog({ content: data.msg }).show();
                        setTimeout(function () {
                            d.close().remove();
                            $(obj).parent().parent().remove();
                        }, 1500);
                    } else {
                        top.dialog({
                            title: '提示',
                            content: data.msg,
                            okValue: '确定',
                            ok: function () { }
                        }).showModal();
                    }
                }
            });

        }
        //导出excel
        function LoadExcel(oid) {
            var postData = { oid: oid };
            //发送AJAX请求
            $.ajax({
                type: "post",
                url: "../../tools/business_ajax.ashx?action=loadOrderExcel",
                data: postData,
                dataType: "json",
                success: function (data) {
                    if (data.status == 0) {
                        window.open(data.msg);
                    } else {
                        layer.alert(data.msg);
                    }
                }
            });
        }
        function changeOrderStatus(flag) {
            if (flag == 1) {
                if ($("#ddldstatus").val() == "") {
                    layer.msg("请选择接单状态");
                    return;
                }
            }
            else if (flag == 2) {
                if ($("#ddlflag").val() == "") {
                    layer.msg("请选择审批状态");
                    return;
                }
            }
            else if (flag == 3) {
                if ($("#ddllockstatus").val() == "") {
                    layer.msg("请选择锁单状态");
                    return;
                }
            }
            else if (flag == 4) {
                if ($("#txtCost").val() == "") {
                    layer.msg("请填写税费成本");
                    return;
                }
            }
            else {
                if ($("#txtFinRemark").val() == "") {
                    layer.msg("请填写财务备注");
                    return;
                }
            }
            var i = layer.load(0, { shade: [0.7, '#fff'] });
            $.getJSON("../../tools/Order_ajax.ashx?action=changeOrderStatus", { tag: flag, oID:'<%=oID %>', status: $("#ddldstatus").val(), flag: $("#ddlflag").val(), lockstatus: $("#ddllockstatus").val(), cost: $("#txtCost").val(),finRemark:$("#txtFinRemark").val() },
                function (data) {
                    layer.close(i);
                    if (data.status == 0) {
                        layer.msg("操作成功,正在刷新页面...");
                        setTimeout(function () {
                            location.href = 'order_edit.aspx?action=Edit&oID=' + data.msg;
                        }, 1000);
                    } else {
                        layer.alert(data.msg);
                    }
                });

        }
        function loadFile(url) {
            //window.open(url);
            window.location.href = url;
        }
    </script>
    <style type="text/css">
        .tab-content dl dt {
            width: 100px;
        }

        .tab-content dl dd {
            margin-left: 110px;
        }

        .border-table tbody th {
            width: 100px;
        }

        .date-input {
            width: 100px;
        }

        .txt-item li {
            background-color: #e1e1e1;
        }

            .txt-item li.icon-btn i {
                color: #fff;
            }

            .txt-item li.icon-btn {
                background-color: #16a0d3;
            }

        .photo-list ul li {
            float: none;
            text-align: left;
        }

        .upload-box .upload-progress {
            position: relative;
        }

        .webuploader-pick1 {
            padding-left: 8px;
            display: inline-block;
            line-height: 30px;
            height: 30px;
            color: red;
            text-align: center;
            overflow: hidden;
        }

        .photo-list span {
            margin-right: 10px;
        }
        .invCollect {
            text-align:right;font-weight: bolder;font-size: 15px;
        }
        .invCollect li {
            display:inline-block;
            width:250px;
        }
    </style>
</head>

<body class="mainbody">
    <form id="ajaxForm" runat="server" action="../../tools/Order_ajax.ashx?action=saveOrder" method="post" enctype="multipart/form-data">
        <!--导航栏-->
        <div class="location">
            <a href="order_list.aspx" class="back"><i class="iconfont icon-up"></i><span>返回列表页</span></a>
            <a href="../center.aspx"><i class="iconfont icon-home"></i><span>首页</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <a href="order_list.aspx"><span>订单列表</span></a>
            <i class="arrow iconfont icon-arrow-right"></i>
            <span><%=action==DTEnums.ActionEnum.Add.ToString()?"添加":"编辑" %>订单</span>
        </div>
        <div class="line10"></div>
        <!--/导航栏-->

        <!--内容-->
        <div id="floatHead" class="content-tab-wrap">
            <div class="content-tab">
                <div class="content-tab-ul-wrap">
                    <ul>
                        <li><a class="selected" href="javascript:;">订单详细信息</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="tab-content" style="padding-top: 10px;">
            <div class="table-container">
                <table border="0" cellspacing="0" cellpadding="0" class="border-table" width="100%">
                    <tr>
                        <th>订单号</th>
                        <td><%=oID %></td>
                        <th>下单人</th>
                        <td>
                            <asp:Label ID="labOwner" runat="server"></asp:Label>
                        </td>
                        <th>税费成本</th>
                        <td>
                            <asp:Label ID="labfinanceCost" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>客户</th>
                        <td>
                            <asp:TextBox ID="txtCusName" runat="server" CssClass="input normal" datatype="*2-100" sucmsg=" "></asp:TextBox>
                            <asp:HiddenField ID="hCusId" runat="server" />
                            <span class="Validform_checktip">*</span>
                        </td>
                        <th>联系人</th>
                        <td>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcontact" runat="server" onchange="getContactPhone(this)">
                                    <asp:ListItem Value="">请先选择客户</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </td>
                        <th>联系电话</th>
                        <td>
                            <asp:TextBox ID="txtPhone" runat="server" Enabled="false" CssClass="input normal" />
                        </td>
                    </tr>
                    <tr>
                        <th>合同造价</th>
                        <td>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlcontractPrice" runat="server">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <th>活动日期</th>
                        <td>
                            <asp:TextBox ID="txtsDate" runat="server" CssClass="input rule-date-input" Width="120px" onclick="WdatePicker({maxDate:'#F{$dp.$D(\'txteDate\')}'})"></asp:TextBox>
                            -
          <asp:TextBox ID="txteDate" runat="server" CssClass="input rule-date-input" Width="120px" onclick="WdatePicker({minDate:'#F{$dp.$D(\'txtsDate\')}'})"></asp:TextBox>
                        </td>
                        <th>活动地点</th>
                        <td>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="input normal" />
                        </td>
                    </tr>
                    <tr>
                        <th>活动名称</th>
                        <td>
                            <asp:TextBox ID="txtContent" runat="server" CssClass="input normal" />
                        </td>
                        <th>合同内容</th>
                        <td>
                            <asp:TextBox ID="txtContract" runat="server" CssClass="input normal" TextMode="MultiLine" />
                        </td>
                        <th>备注</th>
                        <td>
                            <asp:TextBox ID="txtRemark" runat="server" CssClass="input normal" TextMode="MultiLine" />
                        </td>
                    </tr>
                    <tr>
                        <th>活动归属地</th>
                        <td>
                            <div class="txt-item">
                                <ul>
                                    <asp:Repeater ID="rptAreaList" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <input name="hide_place" type="hidden" value="<%#Eval("Key")%>" />
                                                <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                                <span><%#Eval("Value")%></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li class="icon-btn" id="liplace" runat="server">
                                        <a id="specAddButton"><i class="iconfont icon-close"></i></a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                        <th>订单状态</th>
                        <td>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlfStatus" runat="server">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <th>业务报账人员(一个)</th>
                        <td>
                            <div class="txt-item">
                                <ul>
                                    <asp:Repeater ID="rptEmployee1" runat="server">
                                        <ItemTemplate>
                                            <li title="<%#Eval("[\"op_number\"]")%>">
                                                <input name="hide_employee1" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                                <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                                <span><%#Eval("[\"op_name\"]")%></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li class="icon-btn" id="liemployee1" runat="server">
                                        <a href="javascript:" onclick="chooseEmployee(this,1,false,true)"><i class="iconfont icon-close"></i></a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>                        
                        <th>业务策划人员(多个)</th>
                        <td>
                            <div class="txt-item">
                                <ul>
                                    <asp:Repeater ID="rptEmployee2" runat="server">
                                        <ItemTemplate>
                                            <li title="<%#Eval("[\"op_number\"]")%>">
                                                <input name="hide_employee2" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>|<%#Eval("[\"op_dstatus\"]") %>" />
                                                <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                                <span><%#Eval("[\"op_name\"]")%>(<%#MettingSys.Common.BusinessDict.dStatus(2)[Convert.ToByte(Eval("[\"op_dstatus\"]"))]%>)</span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li class="icon-btn" id="liemployee2" runat="server">
                                        <a href="javascript:" onclick="chooseEmployee(this,2,true,true)"><i class="iconfont icon-close"></i></a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                        <th>业务设计人员(多个)</th>
                        <td>
                            <div class="txt-item">
                                <ul>
                                    <asp:Repeater ID="rptEmployee4" runat="server">
                                        <ItemTemplate>
                                            <li title="<%#Eval("[\"op_number\"]")%>">
                                                <input name="hide_employee4" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>|<%#Eval("[\"op_dstatus\"]") %>" />
                                                <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                                <span><%#Eval("[\"op_name\"]")%>(<%#MettingSys.Common.BusinessDict.dStatus(2)[Convert.ToByte(Eval("[\"op_dstatus\"]"))]%>)</span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li class="icon-btn" id="liemployee4" runat="server">
                                        <a href="javascript:" onclick="chooseEmployee(this,4,true,true)"><i class="iconfont icon-close"></i></a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                        <th>业务执行人员(多个)</th>
                        <td>
                            <div class="txt-item">
                                <ul>
                                    <asp:Repeater ID="rptEmployee3" runat="server">
                                        <ItemTemplate>
                                            <li title="<%#Eval("[\"op_number\"]")%>">
                                                <input name="hide_employee3" type="hidden" value="<%#Eval("[\"op_name\"]")%>|<%#Eval("[\"op_number\"]")%>|<%#Eval("[\"op_area\"]")%>" />
                                                <a href="javascript:;" class="del" title="删除" onclick="delNode(this);"><i class="iconfont icon-remove"></i></a>
                                                <span><%#Eval("[\"op_name\"]")%></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li class="icon-btn" id="liemployee3" runat="server">
                                        <a href="javascript:" onclick="chooseEmployee(this,3,true,true)"><i class="iconfont icon-close"></i></a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th>推送上级审批</th>
                        <td>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlpushStatus" runat="server">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <th>业务上级审批</th>
                        <td>
                            <asp:Label ID="labFlag" runat="server">待审批</asp:Label>
                        </td>
                        <th>锁单状态</th>
                        <td>
                            <asp:Label ID="labLockStatus" runat="server">未锁</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>一类活动文件</th>
                        <td colspan="3" style="padding-top: 13px;">
                            <div class="upload-box upload-album" id="uploadDiv" runat="server"></div>
                            <input type="hidden" name="hidFocusPhoto" id="hidFocusPhoto" runat="server" class="focus-photo" />
                            <%--<span style="color: red;">需生成订单号后才能上传文件，文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</span>--%>
                            <div class="photo-list">
                                <ul>
                                    <asp:Repeater ID="rptAlbumList" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <input type="hidden" name="hid_photo_name" value="" />
                                                <input type="hidden" name="hid_photo_remark" value="" />
                                                <div>
                                                    <i class="iconfont icon-attachment"></i>
                                                    <span class="remark"><i><a target="_blank" href="<%#"../../"+Eval("[\"f_filePath\"]") %>" ><%#Eval("[\"f_fileName\"]") %></a></i></span>
                                                    <span style="font-weight: bolder;"><%#Eval("[\"f_size\"]") %>KB</span>
                                                    <span><%#Eval("[\"f_addDate\"]") %></span>
                                                    <span><%#Eval("[\"f_addPerson\"]")+"("+Eval("[\"f_addName\"]")+")" %></span>
                                                    <%#Eval("[\"o_lockStatus\"]").ToString()=="1"?"":"<a href=\"javascript:;\" onclick=\"delImg(this,"+Eval("[\"f_id\"]")+");\">删除</a>" %>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </td>
                        <th>财务备注</th>
                        <td>
                            <asp:Label ID="labFinRemarks" style="width: 450px !important; display: inline-block; word-wrap: break-word; word-break: break-all;white-space: pre-wrap !important;" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trFile" runat="server">
                        <th>二类活动文件</th>
                        <td colspan="5" style="padding-top: 13px;">
                            <div class="upload-box upload-album2" id="uploadDiv2" runat="server"></div>
                            <input type="hidden" name="hidFocusPhoto2" id="hidFocusPhoto2" runat="server" class="focus-photo" />
                            <%--<span style="color: red;">需生成订单号后才能上传文件，文件类型：<%=sysConfig.fileextension %>，文件大小限制：<%=sysConfig.attachsize %>KB</span>--%>
                            <div class="photo-list">
                                <ul>
                                    <asp:Repeater ID="rptAlbumList2" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <input type="hidden" name="hid_photo_name2" value="" />
                                                <input type="hidden" name="hid_photo_remark2" value="" />
                                                <div>
                                                    <i class="iconfont icon-attachment"></i>
                                                    <span class="remark"><i><a target="_blank" href="<%#"../../"+Eval("[\"f_filePath\"]") %>"><%#Eval("[\"f_fileName\"]") %></a></i></span>
                                                    <span style="font-weight: bolder;"><%#Eval("[\"f_size\"]") %>KB</span>
                                                    <span><%#Eval("[\"f_addDate\"]") %></span>
                                                    <span><%#Eval("[\"f_addPerson\"]")+"("+Eval("[\"f_addName\"]")+")" %></span>
                                                    <%#Eval("[\"o_lockStatus\"]").ToString()=="1"?"":"<a href=\"javascript:;\" onclick=\"delImg(this,"+Eval("[\"f_id\"]")+");\">删除</a>" %>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </td>
                    </tr>
                </table>
                <div class="btn-wrap" style="padding: 10px 0;">
                    <input type="hidden" name="orderID" value="<%=oID %>" />
                    <input id="btnSave" runat="server" type="button" value="保存订单信息" class="btn" />
                    <input id="btnDstatus" runat="server" type="button" value="接单状态" class="btn" />
                    <input id="btnFlag" runat="server" type="button" value="审批状态" class="btn" />
                    <input id="btnLockstatus" runat="server" type="button" value="锁单状态" class="btn" />
                    <input id="btnUpdateCost" runat="server" type="button" value="添加修改税费成本" class="btn" />
                    <input id="btnFinRemark" runat="server" type="button" value="编辑财务备注" class="btn" />
                </div>
            </div>
            <div class="table-container">
                <!--执行备用金借款明细-->
                <asp:Repeater ID="rptunBusinessList" runat="server">
                    <HeaderTemplate>
                        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 30px;">
                            <legend>执行备用金借款明细</legend>
                        </fieldset>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr style="text-align: center;">
                                <th width="10%">申请人</th>
                                <th align="left">用途说明</th>
                                <th width="8%">收款账户名称</th>
                                <th width="8%">金额</th>
                                <th width="8%">预计借款日期</th>
                                <th width="8%">实际借款日期</th>
                                <th width="8%">出款银行</th>
                                <th width="5%">审批</th>
                                <th width="5%">支付确认</th>
                                <th width="5%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style="text-align: center;">
                            <td><span title="<%#Eval("uba_personNum")%>"><%#Eval("uba_personName") %></span></td>
                            <td style="text-align: left;"><%#Eval("uba_instruction") %></td>
                            <td><%#Eval("uba_receiveBankName") %></td>
                            <td><%#Eval("uba_money") %></td>
                            <td><%#DateTime.Parse(Eval("uba_foreDate").ToString()).ToString("yyyy-MM-dd") %></td>
                            <td><%#MettingSys.Common.ConvertHelper.toDate(Eval("uba_Date")) == null ? "" : DateTime.Parse(Eval("uba_Date").ToString()).ToString("yyyy-MM-dd") %></td>
                            <td><%#Eval("pm_name") %></td>
                            <td>
                                <span onmouseover="tip_index=layer.tips('部门审批<br/>审批人：<%#Eval("uba_checkNum1")%>-<%#Eval("uba_checkName1")%><br/>审批备注：<%#Eval("uba_checkRemark1")%><br/>审批时间：<%#Eval("uba_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag1")%>"></span>
                                <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("uba_checkNum2")%>-<%#Eval("uba_checkName2")%><br/>审批备注：<%#Eval("uba_checkRemark2")%><br/>审批时间：<%#Eval("uba_checkTime2")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag2")%>"></span>
                                <span onmouseover="tip_index=layer.tips('总经理审批<br/>审批人：<%#Eval("uba_checkNum3")%>-<%#Eval("uba_checkName3")%><br/>审批备注：<%#Eval("uba_checkRemark3")%><br/>审批时间：<%#Eval("uba_checkTime3")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("uba_flag3")%>"></span>
                            </td>
                            <td>
                                <span class="check_<%#Utils.StrToBool(Utils.ObjectToStr(Eval("uba_isConfirm")),false) ? "2" : "0"%>"></span><span title="<%#Eval("uba_confirmerNum")%>"><%#Eval("uba_confirmerName")%></span>
                            </td>
                            <td align="center">
                                <!--存在审批失败的，或者部门审批是待审批的都可以编辑，其他情况只能查看-->
                                <%#(Eval("uba_flag1").ToString() == "1" ||Eval("uba_flag2").ToString() == "1" ||Eval("uba_flag3").ToString() == "1" ) || Eval("uba_flag1").ToString() == "0" ?"<a href=\"javascript:\" onclick=\"addUnbusinessPay('"+DTEnums.ActionEnum.Edit.ToString()+"','编辑非业务支付申请','',"+Eval("uba_id")+",'')\">修改</a>":"<a href=\"javascript:\" onclick=\"addUnbusinessPay('"+DTEnums.ActionEnum.View.ToString()+"','查看非业务支付申请','',"+Eval("uba_id")+",'')\">查看</a>"%>
                                <%# Eval("uba_flag3").ToString() != "2" ?"<a href=\"javascript:\" onclick=\"deleteUnbusinessPay(this,"+Eval("uba_id")+")\">删除</a>":""%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <!--应收付-->
                <asp:Repeater ID="rptNature" runat="server" OnItemDataBound="rptNature_ItemDataBound">
                    <ItemTemplate>
                        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 30px;">
                            <legend><%#Eval("na_name")%></legend>
                        </fieldset>
                        <div style="text-align:right; font-weight:bolder;">毛利：<%#Eval("profit") %></div>
                        <asp:Repeater ID="rptFinanceList" runat="server">
                            <HeaderTemplate>
                                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                                    <tr style="text-align: center;">
                                        <th style="width:4%;">选择
                                            <input type="checkbox" name="cbAllCheck">
                                        </th>
                                        <th style="width:4%;">序号</th>
                                        <th style="width:10%;">应收付对象</th>
                                        <th style="width:6%;">收付类别</th>
                                        <th style="width:6%;">业务性质/明细</th>
                                        <%--<th style="width:6%;">业务日期</th>--%>
                                        <th>业务说明</th>
                                        <th style="width:10%;">金额表达式</th>
                                        <th style="width:10%;">审批状态
                                            <div class="rule-single-select single-select">
                                                <div class="boxwrap">
                                                    <a class="select-tit" href="javascript:;"><span>审批未通过</span><i class="iconfont icon-arrow-down"></i></a><div class="select-items" style="z-index: 1; display: none;">
                                                        <ul>
                                                            <li class="">请选择</li>
                                                            <li class="">待审批</li>
                                                            <li class="selected">审批未通过</li>
                                                            <li>审批通过</li>
                                                        </ul>
                                                    </div>
                                                </div>
                                                <select name="ddlDoCheck" style="display: none;">
                                                    <option value="">请选择</option>
                                                    <option value="0">待审批</option>
                                                    <option value="1">审批未通过</option>
                                                    <option value="2">审批通过</option>

                                                </select>
                                            </div>
                                        </th>
                                        <th style="width:6%;">添加人</th>
                                        <th style="width:6%;">对账标识</th>
                                        <th style="width:6%;">结账月份</th>
                                        <th style="width:10%;">财务备注</th>
                                        <th style="width:5%;">操作
                                            <i class="iconfont icon-delete" style="cursor: pointer;" onclick="mutliDelete(this)"></i>
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr style="text-align: center;">
                                    <td>
                                        <asp:CheckBox ID="chkId" CssClass="checkall" runat="server" Style="vertical-align: middle;" />
                                        <asp:HiddenField ID="hidId" Value='<%#Eval("fin_id")%>' runat="server" />
                                    </td>
                                    <td><%#Eval("fin_id")%></td>
                                    <td><%#Eval("c_name")%></td>
                                    <td><a href="javascript:;" onclick="addReceiptPay('<%#DTEnums.ActionEnum.Add.ToString()%>','<%#Eval("fin_type").ToString()%>','<%#Eval("fin_oid")%>',<%#Eval("fin_id") %>,<%#Eval("fin_cid") %>,'<%#Eval("c_name")%>','<%#Convert.ToDateTime(Eval("o_sdate")).ToString("yyyy-MM-dd")%>/<%#Convert.ToDateTime(Eval("o_edate")).ToString("yyyy-MM-dd")%>，<%#Eval("na_name")%>/<%#Eval("fin_detail")%>','<%#Utils.ObjectToStr(Eval("fin_illustration")).Replace("\r\n"," ").Replace("\r"," ").Replace("\n"," ")%>')"><%#Eval("fin_type").ToString()=="True"?"<font color='blue'>应收</font>":"<font color='red'>应付</font>"%></a></td>
                                    <td><%#Eval("na_name")%><br /><%#Eval("fin_detail")%></td>
                                    <%--<td><%#Convert.ToDateTime(Eval("fin_sdate")).ToString("yyyy-MM-dd")%><br /><%#Convert.ToDateTime(Eval("fin_edate")).ToString("yyyy-MM-dd")%></td>--%>
                                    <td style="text-align: left;">
                                        <span onmouseover="tip_index=layer.tips('<%#Utils.ToHtml(Utils.ObjectToStr(Eval("fin_illustration"))) %>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%#Eval("fin_illustration").ToString().Length<=30?Eval("fin_illustration").ToString():Eval("fin_illustration").ToString().Substring(0,30)+"..."%></span>

                                    </td>
                                    <td style="word-wrap:break-word;word-break:break-all;"><%#Eval("fin_expression")%>=<%#Eval("fin_money")%></td>
                                    <td>
                                        <span onmouseover="tip_index=layer.tips('审批人：<%#Eval("fin_checkNum")%>-<%#Eval("fin_checkName")%><br/>审批备注：<%#Eval("fin_checkRemark")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("fin_flag")%>"  style="cursor: pointer;" onclick="checkFinance(this,<%#Eval("fin_id")%>)"></span>
                                    </td>
                                    <td>
                                        <%#Eval("fin_personNum") %><br />
                                        <%#Eval("fin_personName") %>
                                    </td>
                                    <td>
                                        <span class="cusTip" data-finid="<%#Eval("fin_id")%>"><%#Eval("chk")%></span>
                                    </td>
                                    <td><%#Eval("fin_month")%></td>
                                    <td>
                                        <input type="text" value="<%#Eval("fin_remark")%>" class="input" onchange="InsertFinRemark(this,<%#Eval("fin_id")%>)" />
                                    </td>
                                    <td align="center">
                                        <%#Eval("fin_flag").ToString()=="2"?"<a href=\"javascript:\" onclick=\"addFinance('"+DTEnums.ActionEnum.View.ToString()+"','"+(Eval("fin_type").ToString()=="True"?"查看应收":"查看应付")+"','"+oID+"','"+Eval("fin_type").ToString()+"',"+Eval("fin_id")+")\">查看</a>":"<a href=\"javascript:\" onclick=\"addFinance('"+DTEnums.ActionEnum.Edit.ToString()+"','"+(Eval("fin_type").ToString()=="True"?"编辑应收":"编辑应付")+"','"+oID+"','"+Eval("fin_type").ToString()+"',"+Eval("fin_id")+")\">修改</a>"%>
                                        <%#Eval("fin_flag").ToString()=="2"?"":"<a href=\"javascript:\" onclick=\"deleteFinance(this,"+Eval("fin_id")+")\">删除</a>"%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
                <!--发票申请汇总-->
                <asp:Repeater ID="rptInvoiceList" runat="server" OnItemDataBound="rptInvoiceList_ItemDataBound">
                    <HeaderTemplate>
                        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 30px;">
                            <legend>发票申请汇总</legend>
                        </fieldset>
                        <div class="invCollect">
                            <ul>
                                <li>已申请:<asp:Label ID="labRequestInv" runat="server">0</asp:Label></li>
                                <li>已开具:<asp:Label ID="labConfirmInv" runat="server">0</asp:Label></li>
                                <li>剩余可申请:<asp:Label ID="labLeftInv" runat="server">0</asp:Label></li>
                            </ul>
                        </div>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr style="text-align: center;">
                                <th width="8%">客户</th>
                                <th>开票项目</th>
                                <th width="6%">专普票</th>
                                <th width="6%">开票金额</th>
                                <th width="6%">申请时超开</th>
                                <th width="6%">送票方式</th>
                                <th width="6%">开票区域</th>
                                <th width="6%">申请人</th>
                                <th width="6%">审批</th>
                                <th width="6%">开票状态</th>
                                <th width="6%">开票日期</th>
                                <th width="10%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr style="text-align: center;">
                            <td><%#Eval("c_name")%></td>
                            <td style="text-align: left;"><%#Eval("inv_serviceType")%>/<%#Eval("inv_serviceName")%></td>
                            <td><%# string.IsNullOrEmpty(Utils.ObjectToStr(Eval("inv_type")))?"":BusinessDict.invType()[Utils.StrToBool(Utils.ObjectToStr(Eval("inv_type")),false)] %></td>
                            <td><%#Eval("inv_money")%></td>
                            <td><%#Utils.StrToDecimal(Eval("inv_overmoney").ToString(),0)==0?"0":"<font color='red'>"+Eval("inv_overmoney")+"</font>"%></td>
                            <td><%#Eval("inv_sentWay")%></td>
                            <td><%#Eval("de_subname")%></td>
                            <td title="<%#Eval("inv_personNum")%>"><%#Eval("inv_personName")%></td>
                            <td>
                                <span onmouseover="tip_index=layer.tips('申请区域审批<br/>审批人：<%#Eval("inv_checkNum1")%>-<%#Eval("inv_checkName1")%><br/>审批备注：<%#Eval("inv_checkRemark1")%><br/>审批时间：<%#Eval("inv_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag1")%>"></span>
                                <span onmouseover="tip_index=layer.tips('开票区域审批<br/>审批人：<%#Eval("inv_checkNum2")%>-<%#Eval("inv_checkName2")%><br/>审批备注：<%#Eval("inv_checkRemark2")%><br/>审批时间：<%#Eval("inv_checkTime2")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag2")%>"></span>
                                <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("inv_checkNum3")%>-<%#Eval("inv_checkName3")%><br/>审批备注：<%#Eval("inv_checkRemark3")%><br/>审批时间：<%#Eval("inv_checkTime3")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("inv_flag3")%>"></span>
                            </td>
                            <td>
                                <span onmouseover="tip_index=layer.tips('开票人：<%#Eval("inv_confirmerNum")%>-<%#Eval("inv_confirmerName")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Convert.ToBoolean(Eval("inv_isConfirm").ToString())?"2":"0"%>"></span>
                            </td>
                            <td><%# ConvertHelper.toDate(Eval("inv_date"))==null?"":Convert.ToDateTime(Eval("inv_date")).ToString("yyyy-MM-dd") %></td>
                            <td align="center">
                                <%#(Eval("inv_flag1").ToString() == "1" ||Eval("inv_flag2").ToString() == "1" ||Eval("inv_flag3").ToString() == "1" ) || Eval("inv_flag1").ToString() == "0" ?"<a href=\"javascript:\" onclick=\"addInvoice('"+DTEnums.ActionEnum.Edit.ToString()+"','编辑发票','"+oID+"',"+Eval("inv_id")+")\">修改</a>":"<a href=\"javascript:\" onclick=\"addInvoice('"+DTEnums.ActionEnum.View.ToString()+"','查看发票','"+oID+"',"+Eval("inv_id")+")\">查看</a>"%>
                                <%#Eval("inv_flag3").ToString() != "2" ?"<a href=\"javascript:\" onclick=\"deleteInvoice(this,"+Eval("inv_id")+")\">删除</a>":""%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <!--已收付款汇总-->
                <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 30px;">
                            <legend>已收付款汇总</legend>
                        </fieldset>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th align="left" width="10%">收付对象</th>
                                <th align="left" width="6%">收付类别</th>
                                <th align="left">收付内容</th>
                                <th align="left" width="6%">收付金额</th>
                                <th align="left" width="8%">本单分配</th>
                                <th align="left" width="8%">预收付日期</th>
                                <th align="left" width="8%">收付方式</th>
                                <th align="left" width="8%">实收付日期</th>
                                <th align="left" width="8%">申请人</th>
                                <th align="left" width="6%">凭证号</th>
                                <th align="left" width="6%">审批</th>
                                <th align="left" width="4%">确认收付款</th>
                                <th width="6%">操作</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("c_name") %></td>
                            <td><%# Eval("rpd_type").ToString()=="True"?"<font color='blue'>收款</font>":"<font color='red'>付款</font>" %></td>
                            <td><%# Eval("rpd_content") %></td>
                            <td><%# Utils.StrToDecimal(Eval("rp_money").ToString(),0) %></td>
                            <td><%# Eval("rpd_money") %></td>
                            <td><%# Convert.ToDateTime(Eval("rpd_foredate")).ToString("yyyy-MM-dd") %></td>
                            <td><%# Eval("pm_name") %></td>
                            <td><%# ConvertHelper.toDate(Eval("rp_date"))==null?"":Convert.ToDateTime(Eval("rp_date")).ToString("yyyy-MM-dd") %></td>
                            <td><%# Eval("rpd_personNum") %>-<%# Eval("rpd_personName") %></td>
                            <td><span onmouseover="tip_index=layer.tips('凭证日期：<%#Eval("ce_date").ToString()==""?"":Convert.ToDateTime(Eval("ce_date")).ToString("yyyy-MM-dd")%><br/>备注：<%# Eval("ce_remark") %>', this, { time: 0 });" onmouseout="layer.close(tip_index);"><%# Eval("ce_num") %></span></td>
                            <td>
                                <span onmouseover="tip_index=layer.tips('部门审批<br/>审批人：<%#Eval("rpd_checkNum1")%>-<%#Eval("rpd_checkName1")%><br/>审批备注：<%#Eval("rpd_checkRemark1")%><br/>审批时间：<%#Eval("rpd_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag1")%>"></span>
                                <span onmouseover="tip_index=layer.tips('财务审批<br/>审批人：<%#Eval("rpd_checkNum2")%>-<%#Eval("rpd_checkName2")%><br/>审批备注：<%#Eval("rpd_checkRemark2")%><br/>审批时间：<%#Eval("rpd_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag2")%>"></span>
                                <span onmouseover="tip_index=layer.tips('总经理审批<br/>审批人：<%#Eval("rpd_checkNum3")%>-<%#Eval("rpd_checkName3")%><br/>审批备注：<%#Eval("rpd_checkRemark3")%><br/>审批时间：<%#Eval("rpd_checkTime1")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Eval("rpd_flag3")%>"></span>
                            </td>
                            <td>
                                <span  onmouseover="tip_index=layer.tips('确认人账号：<%#Eval("rp_confirmerNum")%>', this, { time: 0 });" onmouseout="layer.close(tip_index);" class="check_<%#Utils.StrToBool(Eval("rp_isConfirm").ToString(),false)?"2":"0"%>"><%#Eval("rp_confirmerName")%></span>
                            </td>
                            <td align="center">
                                <%# Eval("rpd_type").ToString()=="True" ? (Utils.StrToBool(Eval("rp_isConfirm").ToString(),false)?"<a href=\"javascript:;\" onclick=\"addReceiptPay('"+DTEnums.ActionEnum.View.ToString()+"', '"+Eval("rpd_type")+"', '"+Eval("rpd_oid")+"', "+Eval("rpd_id")+", 0, '','')\">查看</a>":"<a href=\"javascript:;\" onclick=\"addReceiptPay('"+DTEnums.ActionEnum.Edit.ToString()+"', '"+Eval("rpd_type")+"', '"+Eval("rpd_oid")+"', "+Eval("rpd_id")+", 0, '','')\">修改</a> <a href=\"javascript:;\" onclick=\"deleteReceiptPayDetail(this,"+Eval("rpd_id")+")\">删除</a>"):"" %>
                                <%# Eval("rpd_type").ToString()=="False"?((Eval("rpd_flag1").ToString() == "1" ||Eval("rpd_flag2").ToString() == "1" ||Eval("rpd_flag3").ToString() == "1" ) || Eval("rpd_flag1").ToString() == "0" ?"<a href=\"javascript:;\" onclick=\"addReceiptPay('"+DTEnums.ActionEnum.Edit.ToString()+"', '"+Eval("rpd_type")+"', '"+ Eval("rpd_oid")+"', "+Eval("rpd_id")+", 0, '','')\">修改</a>":"<a href=\"javascript:;\" onclick=\"addReceiptPay('"+DTEnums.ActionEnum.View.ToString()+"', '"+Eval("rpd_type")+"', '"+Eval("rpd_oid")+"', "+Eval("rpd_id")+", 0, '','')\">查看</a>"):"" %>
                                <%# Eval("rpd_type").ToString()=="False"?( Eval("rpd_flag3").ToString() != "2" ?"<a href=\"javascript:;\" onclick=\"deleteReceiptPayDetail(this,"+Eval("rpd_id")+")\">删除</a>":""):"" %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <!--结算汇总-->
                <asp:Repeater ID="rptCollect" runat="server" OnItemDataBound="rptCollect_ItemDataBound">
                    <HeaderTemplate>
                        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 30px;">
                            <legend>结算汇总</legend>
                        </fieldset>
                        <div class="invCollect">
                            <ul>
                                <li>应收总额:<asp:Label ID="labFinance1" runat="server">0</asp:Label></li>
                                <li>应付总额:<asp:Label ID="labFinance0" runat="server">0</asp:Label></li>
                                <li>利润:<asp:Label ID="labProfit" runat="server">0</asp:Label></li>
                                <li>税费成本:<asp:Label ID="labCost" runat="server">0</asp:Label></li>
                                <li>业绩利润:<asp:Label ID="labBusinessProfit" runat="server">0</asp:Label></li>
                                <li><a href="javascript:;" onclick="LoadExcel('<%=oID %>')"><i class="iconfont icon-exl"></i><span>导出Excel</span></a></li>
                                <%--<li><asp:LinkButton ID="btnExcel" runat="server" OnClick="btnExcel_Click"><i class="iconfont icon-exl"></i><span>导出Excel</span></asp:LinkButton></li>--%>
                            </ul>
                        </div>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="ltable">
                            <tr>
                                <th align="left" width="20%">收付对象</th>
                                <th align="left" width="10%">收付类别</th>
                                <th align="left" width="10%">应收付总额</th>
                                <th align="left" width="10%">已收付总额</th>
                                <th align="left">未收付总额</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("c_name") %></td>
                            <td><a href="javascript:;" onclick="addReceiptPay('<%#DTEnums.ActionEnum.Add.ToString()%>','<%#Eval("fin_type").ToString()%>','<%#Eval("fin_oid")%>',0,<%#Eval("fin_cid") %>,'<%#Eval("c_name")%>','<%#Convert.ToDateTime(Eval("o_sdate")).ToString("yyyy-MM-dd")%>/<%#Convert.ToDateTime(Eval("o_edate")).ToString("yyyy-MM-dd")%>')"><%# Eval("fin_type").ToString()=="True"?"<font color='blue'>收</font>":"<font color='red'>付</font>" %></a></td>
                            <td><%# Eval("finMoney") %></td>
                            <td><%# Eval("rpdMoney") %></td>
                            <td><%# Eval("unReceiptPay") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="table-container" id="divDstatus" style="display: none;">
                <div class="tab-content" style="border: none;">
                    <dl>
                        <dt style="width: 190px;">接单状态</dt>
                        <dd>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddldstatus" runat="server">
                                </asp:DropDownList>
                            </div>
                        </dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd></dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd>
                            <input type="button" value="提交" class="btn" onclick="changeOrderStatus(1)" />
                        </dd>
                    </dl>
                </div>
            </div>
            <div class="table-container" id="divflag" style="display: none;">
                <div class="tab-content" style="border: none;">
                    <dl>
                        <dt style="width: 190px;">审批状态</dt>
                        <dd>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddlflag" runat="server">
                                </asp:DropDownList>
                            </div>
                        </dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd></dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd>
                            <input type="button" value="提交" class="btn" onclick="changeOrderStatus(2)" />
                        </dd>
                    </dl>
                </div>
            </div>
            <div class="table-container" id="divlockstatus" style="display: none;">
                <div class="tab-content" style="border: none;">
                    <dl>
                        <dt style="width: 190px;">锁单状态</dt>
                        <dd>
                            <div class="rule-single-select">
                                <asp:DropDownList ID="ddllockstatus" runat="server">
                                </asp:DropDownList>
                            </div>
                        </dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd></dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd>
                            <input type="button" value="提交" class="btn" onclick="changeOrderStatus(3)" />
                        </dd>
                    </dl>
                </div>
            </div>
            <div class="table-container" id="divCost" style="display: none;">
                <div class="tab-content" style="border: none;">
                    <dl>
                        <dt style="width: 190px;">税费成本</dt>
                        <dd>
                            <asp:TextBox ID="txtCost" runat="server" CssClass="input" />
                        </dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd></dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd>
                            <input type="button" value="提交" class="btn" onclick="changeOrderStatus(4)" />
                        </dd>
                    </dl>
                </div>
            </div>
            <div class="table-container" id="divFinRemark" style="display: none;">
                <div class="tab-content" style="border: none;">
                    <dl>
                        <dt style="width: 110px;"></dt>
                        <dd>
                            <asp:TextBox ID="txtFinRemark" TextMode="MultiLine" Columns="150" Rows="5" runat="server" CssClass="input" />
                        </dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd></dd>
                    </dl>
                    <dl>
                        <dt style="width: 190px;">&nbsp;</dt>
                        <dd>
                            <input type="button" value="提交" class="btn" onclick="changeOrderStatus(5)" />
                        </dd>
                    </dl>
                </div>
            </div>
        </div>
        <!--/内容-->


        <!--工具栏-->
        <div class="page-footer">
            <div class="btn-wrap">
                <input id="btnUnBusinessPay" runat="server" type="button" value="添加业务备用金借款申请" class="btn red" />
                <input id="btnReceiptPay" runat="server" type="button" value="添加应收付" class="btn" />
                <%--<input id="btnPay" runat="server" type="button" value="添加应付" class="btn" />--%>
                <input id="btnInvoince" runat="server" type="button" value="发票申请" class="btn" />
                <input id="btnSharing" runat="server" type="button" value="合作分成" class="btn" />
                <input name="btnReturn" type="button" value="返回上一页" class="btn yellow" onclick="javascript: history.back(-1);" />
            </div>
        </div>
        <!--/工具栏-->

    </form>
</body>
</html>
