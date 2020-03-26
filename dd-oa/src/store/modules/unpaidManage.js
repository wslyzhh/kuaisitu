import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getOrderList({commit},params){   //订单列表
        return ajax.post(api.orderList ,params)
    },
}

export default {
    actions
}