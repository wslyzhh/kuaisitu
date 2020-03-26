//未收款订单管理首页
import unpaidManage from '../pages/unpaidManage/index'
//未收款订单详情
import unpaidDetails from '../pages/unpaidManage/unpaidDetails'

const routes = [
    {
        path:'/unpaidManage',
        component:unpaidManage,
    },
    {
        path:'/unpaidDetails',
        component:unpaidDetails,
    }
]

export default routes