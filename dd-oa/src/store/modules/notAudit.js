import {api} from '../api'
import ajax from '../ajax'

const actions = {    
    payDetailAudit({commit},params){   //审核付款明细
        return ajax.post(api.payDetailAudit ,params)
    }
}

export default {
    actions
}