//业务退款审核列表
import refundAudit from '../pages/refundAudit/index'
//查看业务支付审核
import refundAuditDetails from '../pages/refundAudit/details'
const routes = [
    {
        path:'/refundAudit',
        component:refundAudit,
    },
    {
        path:'/refundAuditDetails',
        component:refundAuditDetails,
    },
]

export default routes  