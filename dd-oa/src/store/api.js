let baseUrl =  process.env.NODE_ENV === 'production' ? '' : '/api'
const api = {
    get corpid(){
        return baseUrl+'/tools/dingtalk_login.ashx?action=get_dingtalk_corpid'
    },
    ///钉钉授权绑定 start--------------------------------------------------------
    get dingtalkUseridValidate(){  //验证是否已绑定钉钉userid用户
        return baseUrl+'/tools/dingtalk_login.ashx?action=dingtalk_userid_validate'
    },
    get dingtalkUseridValidateTest(){  //验证是否已绑定钉钉userid用户---test
        return baseUrl+'/tools/dingtalk_login.ashx?action=dingtalk_userid_validate_Test'
    },
    get userNameValidate(){  //验证用户名是否可绑定
        return baseUrl+'/tools/dingtalk_login.ashx?action=username_validate'
    },
    get managerOauthBind(){  //用户绑定钉钉userid并授权免登
        return baseUrl+'/tools/dingtalk_login.ashx?action=manager_oauth_bind'
    },
    get managerOauthRemove(){  //用户绑定钉钉userid并授权免登
        return baseUrl+'/tools/dingtalk_login.ashx?action=Manager_oauth_remove'
    },
    ///钉钉授权绑定 end--------------------------------------------------------

    ///数据初始化 start--------------------------------------------------------
    get contractPrices(){   //合同造价数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_contractprice'
    },
    get fStatus(){   //订单状态数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_fstatus'
    },
    get dStatus(){   //接单状态数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_dstatus'
    },
    get allcustomer(){  //所有客户信息数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_allcustomer'
    },
    get checkStatus(){   //审批状态数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_checkstatus'
    },
    get lockStatus(){   //锁单状态数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_lockstatus'
    },
    get invoiceConfirmStatus(){   //开票状态数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_invoiceconfirmstatus'
    },
    get pushStatus(){   //推送上级审批数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_pushstatus'
    },
    get area(){ //活动归属地数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_area'
    },
    get allCustomer(){  //所有客户信息数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_allcustomer'
    },
    get contactsByCid(){    //根据客户ID获取主要联系人及号码
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_contactsbycid'
    },
    get employeebyarea(){   //根据活动归属地ID获取组织架构及人员
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_employeebyarea'
    },
    get unBusinessNature(){   //非业务支付申请支付类别
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_unbusinessnature'
    },
    get unBusinessPayFunction(){   //非业务支付员工往来支付用途
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_unbusinesspayfunction'
    },
    get nature(){   //业务性质数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_nature'
    },
    get natureDetail(){   //业务明细数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_naturedetail'
    },
    get payMethod(){   //支付方式数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_paymethod'
    },
    get methodData(){   //获取收付方式数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=method_data'
    },
    get serviceType(){   //获取应税劳务数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_servicetype'
    },
    get serviceName(){   //获取服务名称数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_servicename'
    },
    get sentMethod(){   //获取送票方式数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_sentmethod'
    },
    get invoiceArea(){   //获取开票区域数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_invoicearea'
    },
    get invoiceUnit(){   //获取开票单位数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getInvoiceUnit'
    },
    ///数据初始化 end--------------------------------------------------------


    ///客户管理模块 start--------------------------------------------------------
    get customerList(){    //查看客户分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=customer_list'
    },
    get customerDetails(){  //查看客户详细信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=customer_show'
    },
    get customerAdd(){  //新增客户详情
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=customer_add'
    },
    get customerObtain(){   //获取客户详情
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=get_customerById'
    },
    get customerEdit(){ //编辑客户详情
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=customer_edit'
    },
    get customerDel(){ //删除客户
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=customer_del'
    },
    get contactAdd(){   //新增次要联系人
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=contact_add'
    },
    get contactEdit(){   //编辑主、次要联系人
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=contact_edit'
    },
    get contactDel(){   //删除次要联系人
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=contact_del'
    },
    get bankEdit(){   //添加编辑银行账号
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=bank_Edit'
    },
    get bankDel(){   //删除银行账号
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=bank_Del'
    },
    
    ///客户管理模块 end--------------------------------------------------------


    ///订单管理模块 start--------------------------------------------------------
    get orderList(){ //订单分页数据列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_list'
    },
    get orderAdd(){ //新增订单信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_edit'
    },
    get orderEdit(){ //修改订单信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_edit'
    },
    get orderDetails(){ //查看订单详细信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_show'
    },
    get orderCheck(){ //业务上级审批
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_check'
    },
	get orderDel(){ //删除订单
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=order_delete'
	},


    get unBusinessPayList(){ //非业务支付申请分页数据列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_list'
    },
    get unBusinessPayDetails(){ //查看非业务支付申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_show'
    },
    get unBusinessPayAdd(){ //新增非业务支付申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_add'
    },  
    get unBusinessPayEdit(){ //修改业务支付申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_edit'
    }, 
    get unBusinessPayDel(){ //删除业务支付申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_del'
    }, 
    get unBusinessPayAuditBind(){ //绑定非业务支付审批类型
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_auditBind'
    },  
    get unBusinessPayAudit(){ //审批业务支付申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_audit'
    },  
    get unBusinessPayConfirmPay(){ //业务支付申请支付确认
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unbusinesspay_confirm_pay'
    },


    get financeAdd(){ //新增收款通知\付款通知
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=finance_add'
    },
    get financeAudit(){ //审批业务信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=finance_audit'
    },
    

    get invoiceList(){ //发票申请分页数据列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_list'
    },
    get invoiceDetails(){ //查看发票申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_show'
    },
    get invoiceAdd(){ //新增发票申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_add'
    },
    get invoiceAuditObtain(){ //初始化发票审批数据绑定
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_auditBind'
    },
    get invoiceAudit(){ //审批发票申请
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_audit'
    },
    get invoiceConfirm(){ //确认发票申请是否已开票
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=invoice_confirm'
    },


    get payList(){ //预付款审核分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=pay_list'
    },
    get payDetails(){ //查看预付款信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=pay_show'
    },
    get payAudit(){ //审批预付款信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=pay_audit'
    },
    get payConfirm(){ //预付款确认支付
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=pay_confirm'
    },
    get addReceiptPayDetail(){ //添加收付款明细
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=add_receiptpayDetail'
    },
    get bankList(){//获取客户银行账号
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getBank'
    },
    get orderSettleMentList(){//获取订单汇总数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getSettlementlist'
    },
    get orderInvoiceList(){//获取订单发票申请数据
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getInvoiceList'
    },
    get orderunBusinessList(){//获取执行备用金借款明细
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getunBusinessList'
    },
    get orderunBusinessPic(){//获取执行备用金借款明细附件
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=getunBusinessPic'
    },
    ///订单管理模块 end--------------------------------------------------------

    ///财务管理模块 start------------------------------------------------------
    get payDetailAudit(){ //业务审批
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=paydetail_audit'
    },
    ///财务管理模块 end--------------------------------------------------------

    ///通知管理模块 start------------------------------------------------------
    get receiptList(){ //收款通知分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=receipt_list'
    },
    get paymentList(){ //付款通知分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=payment_list'
    },
    get receiptpay_add(){ //添加编辑收付款通知
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=receiptpay_add'
    },
    get paydetailList(){ //付款明细分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=paydetail_list'
    },
    get paydetailDetails(){ //查看付款明细详细信息
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=paydetail_show'
    },
    get billList(){ //开票通知分页列表
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=bill_list'
    },
    ///通知管理模块 end--------------------------------------------------------

    ///业务统计分析 start--------------------------------------------------------
    get achievementStatistic(){ //业绩统计
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=AchievementStatistic'
    },
    get unReceiveStatistic(){ //员工未收款
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=unReceiveStatistic'
    },
    ///业务统计分析 end--------------------------------------------------------

    ///公共方法管理模块 start--------------------------------------------------------    
    get upLoadFile(){
        return baseUrl+'/tools/dingtalk_ajax.ashx?action=UpLoadFile'
    },
	get delFile(){
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=File_delete'
    },
	get selfmessage(){//个人消息列表
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=self_message'
    },
	get selfmessageDetail(){//消息详情
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=self_messageDetail'
    },
	get selfmessageDel(){//删除消息
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=self_messageDel'
    },
	get selfmessageRead(){//读取消息
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=self_messageRead'
    },
	get AduitCount(){//读取消息
	    return baseUrl+'/tools/dingtalk_ajax.ashx?action=init_AduitCount'
    }
    
    ///公共方法管理模块 end-------------------------------------------------------- 
}
export {
    api
}