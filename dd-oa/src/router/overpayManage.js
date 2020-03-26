//多付款订单管理首页
import overpayManage from '../pages/overpayManage/index'
//多付款订单详情
import overpayDetails from '../pages/overpayManage/overpayDetails'

const routes = [
    {
        path:'/overpayManage',
        component:overpayManage,
    },
    {
        path:'/overpayDetails',
        component:overpayDetails,
    }
]

export default routes