//业务支付审核列表
import notAudit from '../pages/notAudit/index'
//查看业务支付审核
import notAuditDetails from '../pages/notAudit/details'

const routes = [
    {
        path:'/notAudit',
        component:notAudit,
    },
    {
        path:'/notAuditDetails',
        component:notAuditDetails,
    },
]

export default routes  