import Vue from 'vue'
import Vuex from 'vuex'

import user from './modules/user'
import customerManage from './modules/customerManage'
import test from './modules/test'
import addOrders from './modules/addOrders'
import receipt from './modules/receipt'
import pay from './modules/pay'
import InvoiceManage from './modules/InvoiceManage'
import unbusinessManage from './modules/unbusinessManage'
import notAudit from './modules/notAudit'
import expectPayAudit from './modules/expectPayAudit'
import performanceStat from './modules/performanceStat'
import unReceiveStat from './modules/unReceiveStat'
import message from './modules/message'
import refundAudit from './modules/refundAudit'

Vue.use(Vuex)

export default new Vuex.Store({
    modules:{
        user,
        customerManage,
        receipt,
        pay,
        test,
        addOrders,
        InvoiceManage,
        unbusinessManage,
        notAudit,
        expectPayAudit,
        performanceStat,
        unReceiveStat,
        message,
        refundAudit
    }
})