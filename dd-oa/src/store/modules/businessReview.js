import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getBillList({commit},params){   //开票通知分页列表
        return ajax.post(api.billList,params)
    },
    getinvoiceDetails({commit},params){    //开票通知详情
        return ajax.post(api.invoiceDetails,params)
    }
}

export default {
    actions
}