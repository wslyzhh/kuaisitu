//新增订单首页
import addOrders from '../pages/addOrders/index'

import addOrdersCustomerSelect from '../pages/addOrders/customerSelect'

const routes = [
    {
        path:'/addOrders',
        component:addOrders,
    },
	{
	    path:'/addOrders/customerSelect',
	    component:addOrdersCustomerSelect,
	},
]

export default routes  