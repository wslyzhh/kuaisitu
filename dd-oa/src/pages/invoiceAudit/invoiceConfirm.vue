<!-- 开票操作 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c" @click="chosenStatus">
                <label class="title"><span>开票状态</span></label>
                <input type="text" :value="statusName" placeholder="请选择开票状态" readonly>
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex flex_a_c flex_s_b" v-if="this.addData.inv_isConfirm" @click="selectDate">
                <label class="title"><span>开票日期</span></label>
                <input type="text" v-model="addData.inv_date" placeholder="请选择开票日期" readonly>
                <div class="icon_right time"></div>
            </li>
        </ul>
        <top-nav title="开票操作" text="保存" @rightClick="submit"></top-nav>
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
                    key:'未开票',
                    value:false
                },
                {
                    key:'已开票',
                    value:true
                },
            ],
            statusName:'',
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
            inv_id:id
        }
        // 初始化数据
        this.getInvoiceDetails(params).then(res => {
            this.addData = res.data
            //this.addData.inv_payMethod=this.addData.inv_payMethod
            this.clientId = id
            if(this.addData.inv_isConfirm) {
                this.statusName = this.statusList[1].key
            }else if(!this.addData.inv_isConfirm) {
                this.statusName = this.statusList[0].key
            }
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([
            'getInvoiceDetails',
            'getInvoiceConfirm'
        ]),
        submit(item){ //提交
            if(!this.statusName){
                this.ddSet.setToast({text:'请您选择开票状态'})
                return
            }
            else if(this.addData.uba_isConfirm && !this.addData.inv_date){
                this.ddSet.setToast({text:'请您选择开票日期'})
                return
            }
            this.addData.inv_id = this.clientId
            this.addData.managerid = this.userId //userID
            //console.log(this.addData.inv_payMethod)
            this.ddSet.showLoad()
            this.getInvoiceConfirm(this.addData).then(res => {
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
                //this.$set(this.addData,'inv_isConfirm',res.value == 'true'?true:false)
                this.$set(this.addData,'inv_isConfirm',res.value)
            })
        }, 
        selectDate(){ //活动日期
			let _this = this
			_this.ddSet.setDatepicker().then(res => {
                _this.$set(this.addData,'inv_date',res.value)
            })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>