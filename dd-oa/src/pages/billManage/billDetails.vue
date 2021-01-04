<!-- 查看发票通知详细信息 -->
<template>
    <div class="body_gery" v-if="isDatails">
        <ul class="form_list">
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>订单号</span></label>
                 {{datails.inv_oid}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>客户</span></label>
                {{datails.c_name}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>购买方名称</span></label>
                {{datails.inv_purchaserName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>纳税人编号</span></label>
                {{datails.inv_purchaserNum}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>地址</span></label>
                {{datails.inv_purchaserAddress}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>电话</span></label>
                {{datails.inv_purchaserPhone}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开户行</span></label>
                {{datails.inv_purchaserBank}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>银行账号</span></label>
                {{datails.inv_purchaserBankNum}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>应税劳务、服务名称</span></label>
                {{datails.inv_serviceType}} - {{datails.inv_serviceName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>发票类型</span></label>
                {{datails.inv_type}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开票金额</span></label>
                {{datails.inv_money}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>申请时超开</span></label>
                {{datails.inv_overMoney}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>送票方式</span></label>
                {{datails.inv_sentWay}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开票区域</span></label>
                {{datails.de_subname}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>开票单位</span></label>
                {{datails.invU_name}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人名称</span></label>
                {{datails.inv_receiveName}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人电话</span></label>
                {{datails.inv_receivePhone}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收票人地址</span></label>
                {{datails.inv_receiveAddress}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>备注</span></label>
                {{datails.inv_remark}}
            </li>
        </ul>
        <top-nav :title='"发票详情"' ></top-nav>
    </div>
</template>

<script>
import {mapActions} from 'vuex'

export default {
    name:"",
    data() {
       return {
           isIocn:[],
           datails:{},
           isDatails:false,
           clientType:['未开票','已开票'],
           invType:['专票','普票','电子发票']
       };
    },
    components: {},
    computed: {},
    created(){
        let {id} = this.$route.query
        this.ddSet.showLoad()
        this.getInvoiceDetails({inv_id:id}).then(res => {
            this.ddSet.hideLoad()
            this.datails = res.data
            this.isDatails = true
        }).catch(err => {
            this.ddSet.hideLoad()
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getInvoiceDetails'
        ])
    },
}
</script>

<style scoped lang='scss'>
    
</style>

