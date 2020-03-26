//业务统计分析管理首页
import reportManage from '../pages/reportManage/index'
//业务统计分析订单详情
import reportDetails from '../pages/reportManage/reportDetails'

const routes = [
    {
        path:'/reportManage',
        component:reportManage,
    },
    {
        path:'/reportDetails',
        component:reportDetails,
    }
]

export default routes