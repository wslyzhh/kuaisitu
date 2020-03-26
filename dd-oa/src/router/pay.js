//付款明细列表
import payDetailList from '../pages/pay/index'  
//付款明细详情
import PayDetailEdit from '../pages/pay/payDetailEdit'
//预付款列表
import expectPayList from '../pages/pay/expectPay'  
//预付款详情
import expectPayEdit from '../pages/pay/expectPayEdit'  

const routes = [
    {
        path:'/pay',
        component:payDetailList,
    },
    {
        path:'/payDetailEdit',
        component:PayDetailEdit,
    },
    {
        path:'/expectPay',
        component:expectPayList,
    },
    {
        path:'/expectPayEdit',
        component:expectPayEdit,
    }
]

export default routes