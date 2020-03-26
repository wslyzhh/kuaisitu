import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getReceiptList({commit},params){   //收款通知列表
        return ajax.post(api.receiptList ,params)
    },
    getReceiptDetails({commit},params){   //收款通知明细
        return ajax.post(api.payDetails ,params)
    },
    getMethod({commit},params){   //收款方式
        return ajax.post(api.payMethod ,params)
    }
    ,
    addReceipt({commit},params){   //添加编辑收款通知
        return ajax.post(api.receiptpay_add ,params)
    }
}

export default {
    actions
}