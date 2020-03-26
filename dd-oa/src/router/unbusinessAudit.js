
//非业务支付审核管理首页
import unbusinessAudit from '../pages/unbusinessAudit/index'

//非业务支付详情
import unbusinessDetails from '../pages/unbusinessAudit/unbusinessDetails'
//非业务支付审批
import unBusinessPayAudit from '../pages/unbusinessAudit/unBusinessPayAudit'
//非业务支付确认
import unBusinessPayConfirmPay from '../pages/unbusinessAudit/unBusinessPayConfirmPay'

const routes = [    
    {
        path:'/unbusinessAudit',
        component:unbusinessAudit,
    },
    {
        path:'/unbusinessDetails',
        component:unbusinessDetails,
    },
    {
        path:'/unBusinessPayAudit',
        component:unBusinessPayAudit,
    },
    {
        path:'/unBusinessPayConfirmPay',
        component:unBusinessPayConfirmPay,
    }
]

export default routes