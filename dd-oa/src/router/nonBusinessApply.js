//非业务支付申请
import nonBusinessApply from '../pages/nonBusinessApply/index'
//新增修改非业务支付申请
import nonBusinessDetails from '../pages/nonBusinessApply/details'

const routes = [
    {
        path:'/nonBusinessApply',
        component:nonBusinessApply,
    },
    {
        path:'/nonBusinessDetails',
        component:nonBusinessDetails,
    },
]

export default routes  