<!-- 确认支付业务支付申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c" @click="chosenStatus">
                <label class="title"><span>支付状态</span></label>
                <input type="text" :value="statusName" placeholder="请选择支付状态" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" v-if="this.addData.uba_isConfirm" @click="selectDate">
                <label class="title"><span>实付日期</span></label>
                <input type="text" v-model="addData.uba_date" placeholder="请选择日期" readonly>
                <div class="icon_right time"></div>
            </li>
            <li class="flex flex_a_c"  @click="getMethodList">
                <label class="title"><span class="must">付款方式</span></label>
                <input type="text" :value="this.addData.uba_payMethod" placeholder="请选择付款方式" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
        </ul>
        <top-nav title="确认支付操作" text="保存" @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
import * as dd from 'dingtalk-jsapi'
import {formatDate} from '../../assets/js/date.js'
export default {
    name:"",
    data() {
        return {
            addData:{},
            clientId:0,
            statusList:[  //类型
                {
                    key:'未选择',
                    value:''
                },
                {
                    key:'待支付',
                    value:false
                },
                {
                    key:'已支付',
                    value:true
                },
            ],
            statusName:'',
            methodName:'',
            then:0,
        };
    },
    components: {},
    computed: {
        ...mapState({
            userId:state => state.user.userInfo.id
        })
    },
    created(){
        let {id} = this.$route.query
        let params = {
            uba_id:id
        }
        // 初始化数据
        this.getUnBusinessPayDetails(params).then(res => {
            this.addData = res.data
            this.addData.uba_payMethod=this.addData.uba_payMethod
            this.clientId = id
            if(this.addData.uba_isConfirm) {
                this.statusName = this.statusList[2].key
            }else if(!this.addData.uba_isConfirm) {
                this.statusName = this.statusList[1].key
            }else{
                this.statusName = this.statusList[0].key
            }
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getMethod',
            'getUnBusinessPayDetails',
            'getUnBusinessPayConfirmPay'
        ]),
        submit(item){ //提交
            if(!this.statusName && !this.addData.uba_payMethod){
                this.ddSet.setToast({text:'请选择支付状态或支付方式'})
                return
            }
            else if(this.addData.uba_isConfirm && !this.addData.uba_date){
                this.ddSet.setToast({text:'请选择实付日期'})
                return
            }
            this.addData.uba_id = this.clientId
            this.addData.managerid = this.userId //userID
            //console.log(this.addData.uba_payMethod)
            this.ddSet.showLoad()
            this.getUnBusinessPayConfirmPay(this.addData).then(res => {
                this.ddSet.hideLoad()
                if(res.data.status){
                    this.ddSet.setToast({text:'保存信息操作成功'})
                    this.$router.go(-1)
                }else{
                    this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                this.ddSet.hideLoad()
            })           
        },
        chosenStatus(){
            let selectedKey,source
                source = this.statusList
                selectedKey = this.statusName
            this.ddSet.setChosen({source,selectedKey}).then(res => {
                this.$set(this,'statusName',res.key)
                //this.$set(this.addData,'uba_isConfirm',res.value == 'true'?true:false)
                this.$set(this.addData,'uba_isConfirm',res.value)
            })
        },               
        getMethodList(){   //付款方式
            let _this = this
            this.getMethod({managerid:this.userId}).then(res => {
                let source = []
                let selectedKey = _this.addData.uba_payMethod
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'uba_payMethod',res.value)
                    _this.$set(_this.addData,'uba_payMethod_text',res.key)
                })
            })
        },
        selectDate(){ //活动日期
			let _this = this
			_this.ddSet.setDatepicker().then(res => {
                _this.$set(this.addData,'uba_date',res.value)
            })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>