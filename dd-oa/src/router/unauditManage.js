//审核未通过订单管理首页
import unauditManage from '../pages/unauditManage/index'
//审核未通过订单详情
import unauditDetails from '../pages/unauditManage/unauditDetails'

const routes = [
    {
        path:'/unauditManage',
        component:unauditManage,
    },
    {
        path:'/unauditDetails',
        component:unauditDetails,
    }
]

export default routes