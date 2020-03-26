//业务支付审核列表
import expectPayAudit from '../pages/expectPayAudit/index'
//查看业务支付审核
import expectPayAuditDetails from '../pages/expectPayAudit/details'
//审核页面
import checkDetail from '../pages/expectPayAudit/checkDetail'

const routes = [
    {
        path:'/expectPayAudit',
        component:expectPayAudit,
    },
    {
        path:'/expectPayAuditDetails',
        component:expectPayAuditDetails,
    },
    {
        path:'/checkDetail',
        component:checkDetail,
    },
]

export default routes  