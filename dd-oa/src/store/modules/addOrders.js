import axios from 'axios'
import {api} from '../api'
import ajax from '../ajax'

const actions = {
    getAddOrders({commit},params){  //
        return ajax.post(api.addOrder,params)
    },
    getAllcustomer({commit},params){    //所有客户信息数据绑定
        return ajax.post(api.allcustomer,params)
    },
    getContractprices({commit},params){ //合同造价数据绑定
        return ajax.post(api.contractPrices,params)
    },
    getContactsbycid({commit},params){  //根据客户ID获取主要联系人及号码
        return ajax.post(api.contactsByCid,params)
    },
    getUpLoadFile({commit},_formData){ // 上传文件
        // return _ajax.post(api.upLoadFile,params)
		return axios(api.upLoadFile,{
			headers:{'Content-Type': 'multipart/form-data'},
			method: 'post',
			data:_formData
		})
    },
	delFile({commit},params){  //删除文件
	    return ajax.post(api.delFile,params)
	},
    getFstatus({commit},params){    //订单状态数据绑定
        return ajax.post(api.fStatus,params)
    },
    getDstatus({commit},params){    //接单状态数据绑定
        return ajax.post(api.dStatus,params)
    },
    getPushstatus({commit},params){    //订单推送状态数据绑定
        return ajax.post(api.pushStatus,params)
    },
    getArea({commit},params){   //活动归属地数据绑定
        return ajax.post(api.area,params)
    },
    getEmployeebyarea({commit},params){ //根据活动归属地ID获取组织架构及人员
        return ajax.post(api.employeebyarea,params)
    },
	getLockStatus({commit},params){ //锁单状态数据绑定
	    return ajax.post(api.lockStatus,params)
	},
	/**
	 * 提交订单
	 */
	submitOrder({commit},params){
		return ajax.post(api.orderEdit,params)
	},
	/**
	 * 获取订单列表
	 */
	getOrderList({commit},params){
		return ajax.post(api.orderList,params)
	},
	/**
	 * 订单详情
	 */
	getOrderDetails({commit},params){
		return ajax.post(api.orderDetails,params)
	},
	// 业务上级审批
	checkOrder({commit},params){
		return ajax.post(api.orderCheck,params)
	},
	// 删除订单
	delOrder({commit},params){
		return ajax.post(api.orderDel,params)
	},
	/**
	 * 选择客户时使用
	 */
	changeSelectClient ({commit},clientArray) {
		commit("change_select_client_array", clientArray)
	},
	getOrderSettleMentList({commit},params){
		return ajax.post(api.orderSettleMentList,params)
	},
	getOrderInvoiceList({commit},params){
		return ajax.post(api.orderInvoiceList,params)
	},
	getOrderunBusinessList({commit},params){
		return ajax.post(api.orderunBusinessList,params)
	},
	getOrderunBusinessPic({commit},params){
		return ajax.post(api.orderunBusinessPic,params)
	}
}

const mutations = {
  change_select_client_array (state, clientArray) {
    state.selectClientArray = clientArray;
  }
};

const state = {
	selectClientArray : []
}

export default {
    actions,
	state,
	mutations
}