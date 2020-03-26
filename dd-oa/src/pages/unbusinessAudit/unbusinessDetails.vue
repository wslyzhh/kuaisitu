<!-- 查看非业务支付申请详细信息 -->
<template>
    <div class="body_gery" v-if="isDatails">
        <ul class="form_list">
            <li class="li_auto flex" v-show="datails.uba_oid">
                <label class="title"><span>订单号</span></label>
                <router-link tag="h3" class="hint_1" :to="{path:'/modifyOrder', query: { id: datails.uba_oid }}">{{datails.uba_oid}}</router-link>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>支付类别</span></label>
                <h3 class="hint_1">{{datails.uba_typeText}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>支付用途</span></label>
                <h3 class="hint_1">{{datails.uba_function}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>用途说明</span></label>
                <h3 class="hint_1">{{datails.uba_instruction}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>收款银行</span></label>
                <h3 class="hint_1">{{datails.uba_receiveBank}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>收款账户</span></label>
                <h3 class="hint_1">{{datails.uba_receiveBankName}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>收款账号</span></label>
                <h3 class="hint_1">{{datails.uba_receiveBankNum}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>金额</span></label>
                <h3 class="hint_1">{{datails.uba_money}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>预付日期</span></label>
                <h3 class="hint_1">{{datails.uba_foreDate| formatDate}}</h3>
            </li>
           <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <h3 class="hint_1">{{datails.uba_remark}}</h3>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>附件</span></label>
            </li>
    		<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files" :key="index">
    			<a :href="'/' + f.pp_filePath">{{f.pp_fileName}}</a>
    			<span>{{f.pp_size}}K</span>
    		</li>
            <li class="flex flex_a_c flex_s_b">
			    <label class="title newTitle"><span>审批类型</span></label>
			    <input type="text" readonly :value="datails.type_text">
			</li>
            <li class="flex flex_a_c flex_s_b" @click="changeFlag">
                <label class="title newTitle"><span>审批状态</span></label>
                <input type="text" readonly :value="status_text">
			    <div class="icon_right arrows_right"></div>
            </li>
            <li class="li_auto flex">
                <label class="title newTitle"><span>审批备注</span></label>
                <textarea v-model="datails.remark"></textarea>
            </li>
        </ul>       
       <!--  <ul class="looK_button_list c_flex">
            <router-link tag="li" :to="{path:'/unBusinessPayAudit',query: {id:datails.uba_id,type:'check'}}" style="background-color:#3395fa;">审批</router-link>
            <router-link tag="li" :to="{path:'/unBusinessPayConfirmPay',query: {id:datails.uba_id}}" style="background-color:#47a21f;">确认支付</router-link>
        </ul> -->
        <top-nav title="审批非业务支付申请" :text='"保存"' @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
import {formatDate} from '../../assets/js/date.js'

export default {
    name:"",
    data() {
       return {
           datails:{},
           isDatails:false,
           status:0,
           status_text:'待审批',
           files:[],
           typeList:[  //审批类型
                {
                    key:'部门审批',
                    value:'1'
                },
                {
                    key:'财务审批',
                    value:'2'
                },
                {
                    key:'总经理审批',
                    value:'3'
                },
            ],
            flagList:[  //审批状态
                {
                    key:'待审批',
                    value:'0'
                },
                {
                    key:'审批未通过',
                    value:'1'
                },
                {
                    key:'审批成功',
                    value:'2'
                },
            ]
       };
    },
    filters:{
        formatDate(time){
            let date = new Date(time)
            return formatDate(date,'yyyy-MM-dd')
        }
    },
    components: {},
    computed: {
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){
        let _this = this
        let {id,type} = _this.$route.query
        _this.ddSet.showLoad()
        _this.getUnBusinessPayDetails({uba_id:id,type:type,managerid:this.userInfo.id}).then(res => {
            _this.ddSet.hideLoad()
            if (res.data.status==undefined) {
                _this.datails = res.data
                _this.datails.type_text = this.typeList[_this.datails.type-1].key
                _this.status_text = this.flagList[_this.datails.flag].key
                _this.isDatails = true
                _this.files = _this.datails.picList
            }
            else{
                _this.ddSet.setToast({text:res.data.msg}).then(res => {
                    //this.$router.go(-1)
                    //成功后跳转订单列表
                    _this.$router.go(-1)
                })
            }
            
        }).catch(err => {
            _this.ddSet.hideLoad()
        })
    },
    mounted() {

    },
    methods: {
        ...mapActions([            
            'getUnBusinessPayDetails',
            'getUnBusinessPayAudit'
        ]),
        submit(item){ //提交
            let _this=this
            if(!_this.status){
                //console.log(this.flagName)
                _this.ddSet.setToast({text:'请您选择审批状态'})
                return
            }
            _this.datails.ctype = _this.datails.type
            //console.log(this.addData.ctype)
            _this.datails.cstatus = _this.status
            //console.log(this.flagId)
            _this.datails.managerid = _this.userInfo.id //测试ID
            _this.ddSet.showLoad()
            _this.getUnBusinessPayAudit(_this.datails).then(res => {
                _this.ddSet.hideLoad()
                if(res.data.status){                    
                    _this.ddSet.setToast({text:'审批信息成功'}).then(res => {
                        _this.$router.go(-1)
                    })
                }else{
                    _this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })           
        },
        changeFlag(){
            let _this = this
                let source =_this.flagList,selectedKey = _this.status_text          
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'status',res.value)
                    _this.$set(_this,'status_text',res.key)
                })
        }
    },
}
</script>

<style scoped lang='scss'>
    .hint{
        padding: .3rem;
        font-size: .24rem;
        color: #fc0214;
        line-height: .4rem;
    }
    .looK_button_list{
        li{
            outline: none;
            background: none;
            border: none;
            width: 6.9rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
        }
    }
    .newTitle{
        font-family: SimHei;
        font-weight: bolder;
    }
</style>

