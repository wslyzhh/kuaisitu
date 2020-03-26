//订单查询首页
import shoppingCart from '../pages/shoppingCart/index'
//修改订单
import modifyOrder from '../pages/shoppingCart/modifyOrder'
//查看订单
import lookOrder from '../pages/shoppingCart/lookOrder'
//新增非业务申请
import notBusiness from '../pages/shoppingCart/notBusiness'
//新增收款通知
import adviceOfReceipt from '../pages/shoppingCart/adviceOfReceipt'
//新增付款通知
import adviceOfPayment from '../pages/shoppingCart/adviceOfPayment'
//新增发票申请
import addInvoice from '../pages/shoppingCart/addInvoice'


const routes = [
    {
        path:'/shoppingCart',
        component:shoppingCart,
    },
    {
        path:'/modifyOrder',
        component:modifyOrder,
    },
    {
        path:'/lookOrder',
        component:lookOrder,
    },
    {
        path:'/notBusiness',
        component:notBusiness,
    },
    {
        path:'/adviceOfReceipt',
        component:adviceOfReceipt,
    },
    {
        path:'/adviceOfPayment',
        component:adviceOfPayment,
    },
    {
        path:'/addInvoice',
        component:addInvoice,
    },
]

export default routes  