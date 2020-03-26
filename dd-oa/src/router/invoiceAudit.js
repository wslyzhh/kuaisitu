
//发票审核管理首页
import invoiceAudit from '../pages/invoiceAudit/index'
//发票详情
import invoiceDetails from '../pages/invoiceAudit/invoiceDetails'
//发票审批
import invoiceCheck from '../pages/invoiceAudit/invoiceCheck'
//发票确认支付
import invoiceConfirm from '../pages/invoiceAudit/invoiceConfirm'

const routes = [    
    {
        path:'/invoiceAudit',
        component:invoiceAudit,
    },
    {
        path:'/invoiceDetails',
        component:invoiceDetails,
    },
    {
        path:'/invoiceCheck',
        component:invoiceCheck,
    },
    {
        path:'/invoiceConfirm',
        component:invoiceConfirm,
    }
]

export default routes