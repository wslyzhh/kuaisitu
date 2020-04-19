import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getPaytList({commit},params){   //预付款列表
        return ajax.post(api.paymentList ,params)
    },
    getPayDetailList({commit},params){   //付款明细列表
        return ajax.post(api.paydetailList ,params)
    },
    getPayDetailEdit({commit},params){   //付款明细详情
        return ajax.post(api.paydetailDetails ,params)
    },
    getAddReceiptPayDetail({commit},params){   //添加收付款明细
        return ajax.post(api.addReceiptPayDetail ,params)
    },
    getBankList({commit},params){//获取客户银行账号
        return ajax.post(api.bankList ,params)
    }
}

export default {
    actions
}