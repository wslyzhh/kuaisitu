import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getAchievementStatistic({commit},params){  
        return ajax.post(api.achievementStatistic,params)
    },
}

export default {
    actions
}