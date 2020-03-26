//非业务支付申请管理首页
import unbusinessManage from '../pages/unbusinessManage/index'

//新增非业务支付申请
import UnBusinessPayAdd from '../pages/unbusinessManage/UnBusinessPayAdd'
//修改非业务支付申请
import unBusinessPayEdit from '../pages/unbusinessManage/unBusinessPayEdit'


const routes = [
    {
        path:'/unbusinessManage',
        component:unbusinessManage,
    },
    {
        path:'/UnBusinessPayAdd',
        component:UnBusinessPayAdd,
    },
    {
        path:'/unBusinessPayEdit',
        component:unBusinessPayEdit,
    }
]

export default routes