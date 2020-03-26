//开票通知管理首页
import billManage from '../pages/billManage/index'
//开票通知详情
import billDetails from '../pages/billManage/billDetails'

const routes = [
    {
        path:'/billManage',
        component:billManage,
    },
    {
        path:'/billDetails',
        component:billDetails,
    }
]

export default routes