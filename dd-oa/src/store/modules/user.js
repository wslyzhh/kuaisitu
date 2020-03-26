import {api} from '../api'
import ajax from '../ajax'
const state = {
    userInfo:{},
    isBinding:null,
    corpid:''
}
const mutations = {
    setUser: (state,res) =>{
        state.userInfo = res
    },
    setBinding:(state,res) => {
        state.isBinding = res
    },
    setCorpid:(state,res) => {
        state.corpid = res
    }
}
const actions = {
    getCorpid({commit},params){ //获取企业ID
        return ajax.post(api.corpid,params)
    },
    getUserId({commit},params){ //用户绑定钉钉userid并授权免登--获取用户信息
        return ajax.post(api.dingtalkUseridValidate,params)
    },
    getUserName({commit},params){   //验证用户名是否可绑定
        return ajax.post(api.userNameValidate,params)
    },
    getBinding({commit},params){    //用户绑定
        return ajax.post(api.managerOauthBind,params)
    },
    removeBinding({commit},params){    //用户解除绑定
        return ajax.post(api.managerOauthRemove,params)
    }
}

export default {
    state,
    mutations,
    actions
}