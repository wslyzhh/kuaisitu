import {api} from '../api'
import ajax from '../ajax'

let state = {
    paymentModes:[]
}

const actions = {
    getUnBusinessPayList({commit},params){   //非业务支付申请分页数据列表
        return ajax.post(api.unBusinessPayList ,params)
    },
    getUnBusinessPayDetails({commit},params){   //查看非业务支付申请
        return ajax.post(api.unBusinessPayDetails ,params)
    },
    getUnBusinessPayAdd({commit},params){   //新增非业务支付申请
        return ajax.post(api.unBusinessPayAdd ,params)
    },
    getUnBusinessPayEdit({commit},params){   //修改业务支付申请
        return ajax.post(api.unBusinessPayEdit ,params)
    },
    getUnBusinessPayDel({commit},params){   //删除业务支付申请
        return ajax.post(api.unBusinessPayDel ,params)
    },
    getUnBusinessPayAuditObtain({commit},params){ //初始化非业务支付审批数据
        return ajax.post(api.unBusinessPayAuditBind ,params)
    },
    getUnBusinessPayAudit({commit},params){   //审批业务支付申请
        return ajax.post(api.unBusinessPayAudit ,params)
    },
    getUnBusinessPayConfirmPay({commit},params){   //业务支付申请支付确认
        return ajax.post(api.unBusinessPayConfirmPay ,params)
    },
    getUnBusinessNature({commit},params){   //支付类别数据绑定
        return ajax.post(api.unBusinessNature ,params)
    },
    getUnBusinessPayFunction({commit},params){   //业务用途数据绑定
        return ajax.post(api.unBusinessPayFunction ,params)
    },
}

const mutations = {
    //付款方式列表
    setPaymentModes:(state,res) => {
        state.paymentModes = res
    }
}

export default {
    actions,
    mutations
}