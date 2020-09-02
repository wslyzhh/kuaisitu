import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getPayAudit({commit},params){   //审批预付款
        return ajax.post(api.payAudit ,params)
    }
}

export default {
    actions
}