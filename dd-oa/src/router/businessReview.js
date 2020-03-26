//业务审核首页
import businessReview from '../pages/businessReview/index'
//查看订单
import businessOrder from '../pages/businessReview/businessRedact'
//审批
import checkOrder from '../pages/businessReview/checkView'
const routes = [
    {
        path:'/businessReview',
        component:businessReview,
    },
    {
        path:'/businessOrder',
        component:businessOrder,
    },
    {
        path:'/checkOrder',
        component:checkOrder,
    }
]

export default routes  