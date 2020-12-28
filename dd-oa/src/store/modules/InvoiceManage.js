import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getBillList({commit},params){   //开票通知分页列表
        return ajax.post(api.billList,params)
    },
    getInvoiceList({commit},params){    //发票审核分页列表
        return ajax.post(api.invoiceList,params)
    },
    getInvoiceDetails({commit},params){    //查看发票信息
        return ajax.post(api.invoiceDetails,params)
    },
    getInvoiceAdd({commit},params){    //新增发票申请
        return ajax.post(api.invoiceAdd,params)
    },
    getInvoiceAuditObtain({commit},params){    //初始化发票审批数据绑定
        return ajax.post(api.invoiceAuditObtain,params)
    },
    getInvoiceAudit({commit},params){    //审批发票申请
        return ajax.post(api.invoiceAudit,params)
    },
    getInvoiceConfirm({commit},params){    //确认发票申请是否已开票
        return ajax.post(api.invoiceConfirm,params)
    },
    getServiceType({commit},params){    //获取应税劳务数据
        return ajax.post(api.serviceType,params)
    },
    getServiceName({commit},params){    //获取服务名称数据
        return ajax.post(api.serviceName,params)
    },
    getSentMethod({commit},params){    //获取送票方式数据
        return ajax.post(api.sentMethod,params)
    },
    getInvoiceArea({commit},params){    //获取开票区域数据
        return ajax.post(api.invoiceArea,params)
    },
    getInvoiceUnit({commit},params){    //获取开票单位数据
        return ajax.post(api.invoiceUnit,params)
    }   
}

export default {
    actions
}