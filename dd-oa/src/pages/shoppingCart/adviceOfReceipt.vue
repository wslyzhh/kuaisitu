<!-- 新增收款明细 -->
<template>
    <div class="body_gery">
        <ul class="form_list">
            <li class="flex flex_a_c">
                <label class="title"><span>订单号</span></label>
                <h3 class="hint_1">{{addData.rpd_oid}}</h3>
            </li>
            <li class="flex flex_a_c flex_s_b" @click="changeClient">
                <label class="title"><span class="must">收款对象</span></label>
                <input type="text" :value="clientName" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c">
                <label class="title"><span class="must">收款金额</span></label>
                <input type="text" v-model="addData.rpd_money" placeholder="请输入收款金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectDate">
                <label class="title"><span class="must">预收日期</span></label>
                <input type="text" v-model="addData.rpd_foredate" readonly placeholder="请选择预收日期">
                <div class="icon_right time"></div>
            </li>
			<li class="flex flex_a_c flex_s_b" @click="getMethodList">
			    <label class="title"><span class="must">收款方式</span></label>
			    <input type="text" readonly v-model="addData.rpd_method_text">
			    <div class="icon_right arrows_right"></div>
			</li>
            <li class="li_auto flex">
                <label class="title"><span>收款内容</span></label>
                <textarea class="rpContent" v-model="addData.rpd_content" placeholder="请输入收款内容"></textarea>
            </li>
        </ul>
        <top-nav :title='type == "add" ? "添加收款明细":"查看收款明细"' :text='"保存"' @rightClick="submit"  ></top-nav>
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
           type:''
       };
    },
    components: {},
    computed: {
        ...mapState(            
            {
            selectClientArray:state => state.addOrders.selectClientArray,
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let {type,oID} = this.$route.query
        this.addData.rpd_oid=oID
        this.type=type
    },
    mounted() {
        this.clientCallBack(this.selectClientArray)
    },
    methods: {
        ...mapActions([
            'getAllCustomer',
            'getMethod',
            'getAddReceiptPayDetail'
        ]),
        submit(item){ //提交
            if(!this.clientId){
                this.ddSet.setToast({text:'请选择收款对象'})
                return
            }
            if(!this.addData.rpd_money){
                this.ddSet.setToast({text:'请填写收款金额'})
                return
            }
            if(!this.addData.rpd_foredate){
                this.ddSet.setToast({text:'请填写预收日期'})
                return
            }
            if(!this.addData.rpd_method){
                this.ddSet.setToast({text:'请选择收款方式'})
                return
            }

            this.addData.rpd_type=true
            this.addData.rpd_cid=this.clientId
            this.addData.managerid = this.userInfo.id //测试ID
            
            console.log(this.addData)
            this.ddSet.showLoad()
            if(this.type == 'add'){
                this.getAddReceiptPayDetail(this.addData).then(res => {
                    this.ddSet.hideLoad()
                    if(res.data.status){
                        this.ddSet.setToast({text:'添加成功'}).then(res => {
                            this.$router.go(-1)
                        })
                    }else{
                        this.ddSet.setToast({text:res.data.msg})
                    }
                }).catch(err => {
                    this.ddSet.hideLoad()
                })
            }
        },
        changeClient(){ //选择客户
			let _this = this;
			// _this.clientName = '123123'
			// return;
			this.$router.push({ path: '/addOrders/customerSelect', query: { selected_id: _this.clientId }})
        },
		clientCallBack:function(_selectData){
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
                    this.$set(this.addData,'rpd_foredate',res.value)
                })
        },
        getMethodList(){//收款方式
            let _this = this
            this.getMethod({managerid:14}).then(res => {
                let source = []
                let selectedKey = _this.addData.rpdmethod
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'rpd_method',res.value)
                    _this.$set(_this.addData,'rpd_method_text',res.key)
                })
            })
        }
    },
}
</script>

<style scoped lang='scss'>

    .rpContent{
        height: 1.75rem;
    }
</style>
