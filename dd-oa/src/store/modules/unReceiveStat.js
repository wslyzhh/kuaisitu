import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getUnReceiveStatistic({commit},params){  
        return ajax.post(api.unReceiveStatistic,params)
    },
}

export default {
    actions
}