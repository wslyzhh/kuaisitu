<!-- 添加客户 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="li_auto flex">
                <label class="title"><span>订单号</span></label>
                {{addData.rpdoid}}
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeClient">
                <label class="title"><span class="must">付款对象</span></label>
                <input type="text" :value="clientName" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">付款金额</span></label>
                <input type="text" v-model="addData.rpdmoney" placeholder="请输入付款金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectDate">
                <label class="title"><span class="must">预付日期</span></label>
                <input type="text" v-model="addData.rpdforedate" readonly placeholder="请选择预付日期">
                <div class="icon_right time"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>收款内容</span></label>
                <textarea v-model="addData.rpdcontent" placeholder="请输入收款内容"></textarea>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>文件</span></label>
                <textarea v-model="addData.rpcontent" ></textarea>
            </li>
        </ul>
        <top-nav :title='type == "add" ? "添加付款明细":"查看付款明细"' ></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
        return {
            addData:{},
            clientList:[],
            clientName:'请选择收款对象',
            clientId:0,         
            type:'',
            rpdid:0
        };
    },
    components: {},
    computed: {        
        ...mapState({
            selectClientArray:status => status.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {type,rpd_id} = this.$route.query
        this.type = type
        this.rpdid=rpd_id
        if(type == 'EDIT'){
            let params = {
                rpd_id
            }
            this.getPayDetailEdit(params).then(res => {
                console.log(res.data)
                this.addData = res.data
                this.clientName=this.addData.c_name
                this.clientId=this.addData.c_id
                this.addData.rpdoid = this.addData.rpd_oid
                this.addData.rpdmoney=this.addData.rpd_money
                this.addData.rpdforedate = this.addData.rpd_foredate
                this.addData.rpdcontent = this.addData.rpd_content
            })
        }
    },
    mounted() {
        this.clientCallBack(this.selectClientArray)
    },
    methods: {
        ...mapActions([
            'getAllCustomer',
            'getPayDetailEdit',
            'getMethod',
            'methodData',
            'addReceipt'
        ]),
        submit(item){ //提交
            if(!this.clientId){
                this.ddSet.setToast({text:'请选择收款对象'})
                return
            }
            if(!this.addData.rpmoney){
                this.ddSet.setToast({text:'请输入收款金额'})
                return
            }
            if(!this.addData.rpforedate){
                this.ddSet.setToast({text:'请选择预收日期'})
                return
            }
            if(!this.addData.rp_method_text){
                this.ddSet.setToast({text:'请选择收款方式'})
                return
            }
            this.addData.rpid=this.rpid
            this.addData.rptype='True'
            this.addData.rpcid=this.clientId
            this.addData.managerid = this.userInfo.id //测试ID
            this.ddSet.showLoad()
            console.log(this.addData)
            if(this.type == 'add'){
                this.addReceipt(this.addData).then(res => {
                    this.ddSet.hideLoad()
                    if(res.data.status){
                        this.ddSet.setToast({text:'新增收款通知成功'})
                        this.$router.go(-1)
                    }else{
                        this.ddSet.setToast({text:res.data.msg})
                    }
                }).catch(err => {
                    this.ddSet.hideLoad()
                })
            }else{
                this.addReceipt(this.addData).then(res => {
                    this.ddSet.hideLoad()
                    if(res.data.status){
                        this.ddSet.setToast({text:'编辑收款通知成功'})
                    }else{
                        this.ddSet.setToast({text:res.data.msg})
                    }
                }).catch(err => {
                    this.ddSet.hideLoad()
                })
            }
        },
        chosen(item){
            let selectedKey,source
            if(item == 'type'){
                source = this.typeList
                selectedKey = this.typeName
            }else if(item == 'enabled'){
                source = this.enabled
                selectedKey = this.enabledName
            }else{
                source = this.flag
                selectedKey = this.flagName
            }
            this.ddSet.setChosen({source,selectedKey}).then(res => {
                if(item == 'type'){
                    this.$set(this,'typeName',res.key)
                    this.$set(this.addData,'c_type',res.value)
                }else if(item == 'enabled'){
                    this.$set(this,'enabledName',res.key)
                    this.$set(this.addData,'c_isUse',res.value)
                }else{
                    this.$set(this,'flagName',res.key)
                    this.$set(this.addData,'c_flag',res.value)
                }
            })
        },
        changeClient(){ //选择客户
			let _this = this;
			// _this.clientName = '123123'
			// return;
			this.$router.push({ path: '/addOrders/customerSelect', query: { selected_id: _this.clientId }})
        },
		clientCallBack:function(_selectData){
			console.log(_selectData)
			if(_selectData.length){
				this.clientName = _selectData[0].name;
				this.clientId = _selectData[0].id;
			}
			else{
				this.clientName = '请选择';
				this.clientId = 0;
			}
		},
        selectDate(){ //活动日期
            this.ddSet.setDatepicker().then(res => {
                    this.$set(this.addData,'rpdforedate',res.value)
                })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>

