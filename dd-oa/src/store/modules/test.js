import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getInitFStatus({commit},params){
        return ajax.post(api.initFStatus,params)
    }
}

export default {
    actions
}